using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Models;
using RxFair.Service.Exceptions;
using RxFair.Service.Interface;
using RxFair.Service.Utility;
using RxFair.Utility;
using RxFair.Utility.Common;
using RxFair.Utility.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace RxFair.Areas.Admin.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Admin), Area("Admin")]
    public class MedicineController : BaseController<MedicineController>
    {
        #region Fields
        private readonly IDistributorService _distributor;
        private readonly IDosageFormService _dosageForm;
        private readonly EmailService _emailService;
        private readonly IManufacturerService _manufacturer;
        private readonly IMeasurementService _measurement;
        private readonly IMedicineCategoryService _category;
        private readonly IMedicineHelper _medicineHelper;
        private readonly IMedicineMasterService _medicine;
        private readonly IUploadedMedicineService _uploadedMedicine;
        private readonly S3Manager _s3Service;
        #endregion

        #region Ctor
        public MedicineController(IDistributorService distributor, IDosageFormService dosageForm, IManufacturerService manufacturer,
            IMeasurementService measurement, IMedicineCategoryService category, IMedicineHelper medicineHelper, IMedicineMasterService medicine,
            IUploadedMedicineService uploadedMedicine, IOptions<EmailSettingsGmail> emailSettingsGmail, IOptions<AwsS3Storage> awsS3Storage)
        {
            _category = category;
            _distributor = distributor;
            _dosageForm = dosageForm;
            _emailService = new EmailService(emailSettingsGmail);
            _manufacturer = manufacturer;
            _measurement = measurement;
            _medicineHelper = medicineHelper;
            _medicine = medicine;
            _uploadedMedicine = uploadedMedicine;
            _s3Service = new S3Manager(awsS3Storage);
        }
        #endregion

        #region Methods

        public IActionResult ManageDuplicate()
        {
            //var model = new List<UploadMedicine>();
            //try
            //{
            //    model = await _medicine.GetDuplicateMedicineList();
            //    model.ForEach(x => { x.MedicineImage = $@"{FilePathList.MedicineImage}\{x.MedicineImage}"; });
            //}
            //catch (Exception ex)
            //{
            //    ErrorLog.AddErrorLog(ex, "ManageDuplicate");
            //}
            //return View(model);
            return View();
        }

        public IActionResult BulkMedicine()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, DisableRequestSizeLimit]
        public async Task<IActionResult> BulkMedicine(IFormFile file)
        {
            var transactionOptions = new TransactionOptions { Timeout = TransactionManager.MaximumTimeout };
            using (var txscope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (file == null || file.Length < 0) return JsonResponse.GenerateJsonResult(0, "No file selected !");
                    var extension = Path.GetExtension(file.FileName);
                    if (!extension.Equals(".xls") && !extension.Equals(".xlsx") && !extension.Equals(".csv"))
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, "Invalid File !");
                    }

                    using (var stream = file.OpenReadStream())
                    {
                        var rv = _medicineHelper.GetBulkMedicineData(stream, (extension != ".csv"));

                        if (!string.IsNullOrEmpty(rv.ErrorMessage))
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, rv.ErrorMessage);
                        }

                        if (!rv.UploadMedicines.Any())
                            return JsonResponse.GenerateJsonResult(0, "Bulk medicine data is missing.");

                        await _medicineHelper.ProcessBulkMedicineData(rv, User.GetUserId());
                        txscope.Complete();
                    }
                    return JsonResponse.GenerateJsonResult(1, "Bulk medicine uploaded.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post-BulkMedicine");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.UnhandledError);
                }
            }
        }

        public IActionResult BulkMedicinePrice()
        {
            ViewBag.DistributorList = _distributor.GetDistributorAdminList().Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).OrderBy(x => x.Text).ToList();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, DisableRequestSizeLimit]
        public async Task<IActionResult> BulkMedicinePrice(UploadMedicinePriceData model)
        {
            var transactionOptions = new TransactionOptions { Timeout = TransactionManager.MaximumTimeout };
            using (var txscope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (model.File == null || model.File.Length < 0) return JsonResponse.GenerateJsonResult(0, "No file Selected !");
                    var extension = Path.GetExtension(model.File.FileName);
                    if (!extension.Equals(".xls") && !extension.Equals(".xlsx") && !extension.Equals(".csv")) return JsonResponse.GenerateJsonResult(0, "Invalid File");

                    using (var stream = model.File.OpenReadStream())
                    {
                        var rv = _medicineHelper.GetBulkMedicinePriceData(stream, (extension != ".csv"));

                        if (!string.IsNullOrEmpty(rv.ErrorMessage))
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, rv.ErrorMessage);
                        }
                        var result = await _medicineHelper.ProcessBulkMedicinePriceData(rv, User.GetUserId(), model.DistributorId);
                        if (result.IsSuccess)
                        {
                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, result.ErrorMessage, result);
                        }

                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0,
                            result.ErrorMessage != GlobalConstant.SomethingWrong ? result.ErrorMessage : GlobalConstant.SomethingWrong,
                            result);
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post-BulkMedicinePrice");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.UnhandledError);
                }
            }
        }

        public IActionResult ManageMedicines()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetMedicineList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = 0 });
                var allList = await _medicine.GetSystemMedicineList(parameters.ToArray());
                allList.ForEach(x => { x.MedicineImage = GetS3ServiceUrl(BucketName.MedicineImage, x.MedicineImage); });
                var total = allList.FirstOrDefault()?.TotalRecords ?? 0;
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = allList
                });
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "GetMedicineList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public IActionResult AddEditMedicine(long id = 0)
        {
            var distributorId = CommonMethod.GetUserGroupName(User.GetClaimValue(UserClaims.UserRole)).Contains(UserRoleGroup.Distributor) ? Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId)) : (long?)null;

            BindDropdownList();
            var model = new AddEditMedicine { Id = id, IsNdc = true };
            if (id == 0) return View(model);
            var result = _medicine.GetSingle(x => x.Id == id);
            if (result == null) return View(model);
            model.Ndc = result.NdcUpcHri;
            model.IsNdc = result.IsNdc;
            model.MedicineName = result.DrugName;
            model.BrandId = result.BrandId;
            model.Flavour = result.Flavour;
            model.Strength = result.Strength;
            model.StrengthId = result.StrengthId;
            model.DosageFormId = result.DosageFormId;
            model.PackageSize = result.PackageSize;
            model.PackagingSizeId = result.PackageSizeId;
            model.UnitId = result.UnitSizeId;
            model.PackageDescriptionCodeId = result.PackageDescriptionCodeId;
            model.AwpPrice = result.MedicinePriceMasters.FirstOrDefault(x => x.DistributorId == distributorId)?.AwppackagePrice;
            model.CategoryId = result.CategoryId;
            model.ManufacturerId = result.ManufacturerId;
            model.MedicineImage = result.MedicineImage;
            model.Description = result.Description;
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditMedicine(AddEditMedicine model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = await _medicineHelper.AddUpdateMedicine(model, User.GetUserId(), false);
                    if (result.IsSuccess)
                        txscope.Complete();
                    else
                        txscope.Dispose();
                    return JsonResponse.GenerateJsonResult(result.IsSuccess ? 1 : 0, result.ErrorMessage);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddEditMedicine");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMedicine(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _medicine.GetById(id);
                    result.IsDelete = true;
                    await _medicine.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Medicine deleted successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, $@"Post/DeleteMedicine");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageMedicineStatus(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _medicine.GetById(id);
                    result.IsActive = !result.IsActive;
                    await _medicine.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"Medicine {(result.IsActive ? "activated" : "deactivated")} successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, $@"Post/DeleteMedicine");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> DiscontinueMedicine(long id, string reason)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _medicine.GetById(id);
                    result.IsDiscontinue = true;
                    result.Reason = reason;
                    await _medicine.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Medicine discontinued successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/DiscontinueMedicine");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        public IActionResult MedicineRequest()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetMedicineRequestList(bool? isApproved, JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));

                parameters.Parameters.Insert(0, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = 0 });
                parameters.Parameters.Insert(1, new SqlParameter("@IsApproved", SqlDbType.Bit) { Value = isApproved ?? (object)DBNull.Value });

                var allList = await _uploadedMedicine.GetUploadedMedicineList(parameters.Parameters.ToArray());

                allList.ForEach(x => { x.MedicineImage = GetS3ServiceUrl(BucketName.MedicineImage, x.MedicineImage); });
                var total = 0;
                //if (isApproved == null || isApproved == false)
                total = Convert.ToInt32(allList.FirstOrDefault()?.TotalRecords ?? 0);
                //else
                //  total = allList.FirstOrDefault()?.TotalRecords ?? 0;
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = allList
                });
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "GetMedicineRequestList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageMedicineRequestStatus(long id, bool status, string reason)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var medicine = _uploadedMedicine.GetSingle(x => x.Id == id);
                    var dUser = medicine.DistributorMedicine.DistributorAdminUser;
                    if (!status)
                    {
                        medicine.IsApproved = false;
                        medicine.Reason = reason;
                        await _uploadedMedicine.UpdateAsync(medicine, Accessor, User.GetUserId());
                        txscope.Complete();
                        await MedicineApproveOrRejectNotification(dUser.UserName, dUser.FullName, false, reason);
                        return JsonResponse.GenerateJsonResult(2, @"Medicine rejected successfully.", medicine.Id);
                    }
                    var result = await _medicineHelper.ProcessBulkMedicineData(new ImportMedicineData
                    {
                        UploadMedicines = new List<UploadMedicine>
                            {
                                new UploadMedicine
                                {
                                    Ndc = medicine.Ndc, MedicineName = medicine.MedicineName,
                                    Strength = medicine.Strength, DosageForm = medicine.DosageForm,
                                    Brand = medicine.Brand, PackageSize = medicine.PackageSize,
                                    Unit = medicine.Unit, Manufacturer = medicine.Manufacturer,
                                    Category = medicine.Category, Description = medicine.Description,
                                    DistributorId = medicine.DistributorId, IsActive = true, IsNdc = (medicine.Ndc != medicine.Upc)
                                }
                            }
                    }, User.GetUserId(), medicine.DistributorId);
                    if (result)
                    {
                        await MedicineApproveOrRejectNotification(dUser.UserName, dUser.FullName, true, reason);
                        _uploadedMedicine.Delete(medicine);
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, @"Medicine approved successfully.", medicine.Id);
                    }
                    txscope.Dispose();
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ManageMedicineRequestStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }
        #endregion

        #region MedicineHistory
        public IActionResult MedicineHistory()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetMedicineHistory(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0)).Parameters;
                var allList = await _medicine.GetMedicineHistory(parameters.ToArray());
                //var result = allList.GroupBy(x => x.Ndc)
                //    .Select(x => new MedicineHistoryDto
                //    {
                //        Ndc = x.Key,
                //        MedicineImage = x.FirstOrDefault(y => y.Ndc == x.Key)?.MedicineImage,
                //        MedicineName = x.FirstOrDefault(y => y.Ndc == x.Key)?.MedicineName,
                //        DistributorName = x.FirstOrDefault(y => y.Ndc == x.Key)?.DistributorName,
                //        NewPrice = x.FirstOrDefault(y => y.Ndc == x.Key).NewPrice,
                //        Createdby = x.FirstOrDefault(y => y.Ndc == x.Key)?.Createdby,
                //        Createddate = x.FirstOrDefault(y => y.Ndc == x.Key)?.Createddate,
                //        OldPriceList = x.Where(y => y.Ndc == x.Key).ToList(),
                //    }).ToList();

                //allList.ForEach(x => { x.MedicineImage = $@"{FilePathList.MedicineImage}\{x.MedicineImage}"; });
                var total = allList.FirstOrDefault()?.TotalRecords;
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = allList
                });
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "GetMedicineList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMedicineHistoryById(long id)
        {
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("@MedicineId",SqlDbType.BigInt){Value = id},
                    new SqlParameter("@TimeZoneId",SqlDbType.Int){Value = GlobalRxFair.Value.CurrentTimeZoneId},
                };
                var allList = await _medicine.GetMedicineHistoryById(param.ToArray());
                return JsonResponse.GenerateJsonResult(1, "", allList);
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "GetMedicineList");
                return JsonResponse.GenerateJsonResult(0);
            }
        }

        #endregion

        #region Controller Common
        private void BindDropdownList()
        {
            var measurements = _measurement.GetAll().ToList();
            ViewBag.BrandList = measurements.Where(x => x.IsActive && x.MeasurementType == (short)GlobalEnums.MeasurementType.Brand).Select(x => new SelectListItem { Text = x.MeasurementUnit, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            ViewBag.StrengthList = measurements.Where(x => x.IsActive && x.MeasurementType == (short)GlobalEnums.MeasurementType.Strength).Select(x => new SelectListItem { Text = x.MeasurementUnit, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            ViewBag.PackageSizeList = measurements.Where(x => x.IsActive && x.MeasurementType == (short)GlobalEnums.MeasurementType.PackageSize).Select(x => new SelectListItem { Text = x.MeasurementUnit, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            ViewBag.DoseUnitList = measurements.Where(x => x.IsActive && x.MeasurementType == (short)GlobalEnums.MeasurementType.DoseUnit).Select(x => new SelectListItem { Text = x.MeasurementUnit, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            ViewBag.PackageDesList = measurements.Where(x => x.IsActive && x.MeasurementType == (short)GlobalEnums.MeasurementType.PackageDescriptionCode).Select(x => new SelectListItem { Text = x.MeasurementUnit, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();

            ViewBag.CategoryList = _category.GetAll(x => x.IsActive).Select(x => new SelectListItem { Text = x.MedicineCategory, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            ViewBag.ManufacturerList = _manufacturer.GetAll(x => x.IsActive).Select(x => new SelectListItem { Text = x.ManufacturerName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            ViewBag.DosageFormList = _dosageForm.GetAll(x => x.IsActive).Select(x => new SelectListItem { Text = x.DosageForm, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
        }

        private async Task MedicineApproveOrRejectNotification(string email, string name, bool status, string reason)
        {
            var message = status
                ? "NDC medicine is approved. You can add price and sell medicine to the pharmacies."
                : $@"NDC medicine is not approved due to {reason} by RxFair Admin.";
            var emailTemplate = CommonMethod.ReadEmailTemplate(ErrorLog, HostingEnvironment.WebRootPath, EmailTemplateFileList.MedicineRequestStatus, GetPhysicalUrl());
            emailTemplate = emailTemplate.Replace("{UserName}", name);
            emailTemplate = emailTemplate.Replace("{message}", message);
            await _emailService.SendEmailAsyncByGmail(new SendEmailModel
            {
                ToAddress = email,
                ToDisplayName = name,
                Subject = "Medicine Request Status",
                BodyText = emailTemplate
            });
        }
        #endregion
    }
}