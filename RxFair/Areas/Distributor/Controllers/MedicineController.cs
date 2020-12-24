using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Models;
using RxFair.Service.Exceptions;
using RxFair.Service.Interface;
using RxFair.Service.Utility;
using RxFair.Utility;
using RxFair.Utility.Common;
using RxFair.Utility.Extension;

namespace RxFair.Areas.Distributor.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Distributor), Area("Distributor")]
    public class MedicineController : BaseController<MedicineController>
    {
        #region Fields
        private readonly IDistributorService _distributor;
        private readonly IDistributorMedicineService _distributorMedicine;
        private readonly IMedicinePriceMasterService _medicinePriceMaster;
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
            IUploadedMedicineService uploadedMedicine, IDistributorMedicineService distributorMedicine, IMedicinePriceMasterService medicinePriceMaster,
            IOptions<EmailSettingsGmail> emailSettingsGmail, IOptions<AwsS3Storage> awsS3Storage)
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
            _distributorMedicine = distributorMedicine;
            _medicinePriceMaster = medicinePriceMaster;
            _s3Service = new S3Manager(awsS3Storage);
        }
        #endregion

        #region Methods
        public IActionResult BulkMedicine()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, DisableRequestSizeLimit]
        public async Task<IActionResult> BulkMedicine(UploadMedicinePriceData model)
        {
            var transactionOptions = new TransactionOptions { Timeout = TransactionManager.MaximumTimeout };
            //var transactionOptions = new TransactionOptions();
            using (var txscope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (model.File == null || model.File.Length < 0) return JsonResponse.GenerateJsonResult(0, "No file selected !");
                    var extension = Path.GetExtension(model.File.FileName);
                    if (!extension.Equals(".xls") && !extension.Equals(".xlsx") && !extension.Equals(".csv"))
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, "Invalid file !");
                    }

                    using (var stream = model.File.OpenReadStream())
                    {
                        var rv = _medicineHelper.GetBulkMedicineData(stream, (extension != ".csv"));

                        // Checking if  any  Mendatory field Data is missing .
                        //foreach (var item in rv.UploadMedicines)
                        //{
                        //    if (item.Ndc == "" || item.MedicineName == "" || item.Manufacturer == "" || item.Strength == "" || item.DosageForm=="" || item.PackageSize.ToString()=="" || item.Unit=="")
                        //    {
                        //        txscope.Dispose();
                        //        return JsonResponse.GenerateJsonResult(0, "Bulk medicine data is missing.");
                        //    }
                        //}

                        if (!string.IsNullOrEmpty(rv.ErrorMessage))
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, rv.ErrorMessage);
                        }

                        if (!rv.UploadMedicines.Any())
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Bulk medicine data is missing.");
                        }

                        var result = await _medicineHelper.ProcessDistributorBulkMedicineData(rv, User.GetUserId(), model.DistributorId);

                        if (result)
                        {
                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, "Bulk medicine uploaded.");
                        }

                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                    }
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

                        if (rv.UploadMedicinePrices == null)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Bulk medicine data is missing.");
                        }

                        // Checking if  any  Mendatory field Data is missing .
                        foreach (var item in rv.UploadMedicinePrices)
                        {
                            if (item.Ndc == "" || item.ShortDated.ToString() == "" || item.InStock.ToString() == "" || item.Contracted.ToString() == "" || item.Price.ToString() == "" || item.Stock == null)
                            {
                                txscope.Dispose();
                                return JsonResponse.GenerateJsonResult(0, "Bulk medicine data is missing.");
                            }
                            if (item.Price <= 0)
                            {
                                txscope.Dispose();
                                return JsonResponse.GenerateJsonResult(0, "Please Provide Valid Price To  Medicine " + item.Ndc);
                            }
                        }
                        if (!string.IsNullOrEmpty(rv.ErrorMessage))
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, rv.ErrorMessage);
                        }
                        var result = await _medicineHelper.ProcessBulkMedicinePriceData(rv, User.GetUserId(), model.DistributorId);
                        if (result.IsSuccess)
                        {
                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, "Bulk medicine price uploaded.", result);
                        }
                        else
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, result.ErrorMessage, result);
                        }

                        //return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong, result);
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

        public IActionResult MedicineRequest()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetMedicineRequestList(JQueryDataTableParamModel param)
        {
            try
            {
                var distributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));

                parameters.Parameters.Insert(0, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = distributorId });
                parameters.Parameters.Insert(1, new SqlParameter("@IsApproved", SqlDbType.Bit) { Value = DBNull.Value });

                var allList = await _uploadedMedicine.GetUploadedMedicineList(parameters.Parameters.ToArray());
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

        public IActionResult AddEditMedicine(long id = 0)
        {
            var distributorId = CommonMethod.GetUserGroupName(User.GetClaimValue(UserClaims.UserRole)).Contains(UserRoleGroup.Distributor) ? Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId)) : (long?)null;

            BindDropdownList();
            var model = new AddEditMedicine { Id = id };
            if (id == 0) return View(model);
            var result = _medicine.GetSingle(x => x.Id == id);
            if (result == null) return View(model);
            model.Ndc = result.NdcUpcHri;
            model.MedicineName = result.DrugName;
            model.BrandId = result.BrandId;
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
                    if (model.MedicineFile != null)
                    {
                        model.MedicineImage = CommonMethod.GetFileName(model.MedicineFile.FileName);
                    }
                    var distributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                    var result = await _medicineHelper.AddUpdateMedicine(model, User.GetUserId(), true, distributorId);
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

        public IActionResult UploadMedicine(long id = 0)
        {
            var distributorId = CommonMethod.GetUserGroupName(User.GetClaimValue(UserClaims.UserRole)).Contains(UserRoleGroup.Distributor)
                ? Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId))
                : (long?)null;

            BindDropdownList();
            var model = new AddEditMedicine { Id = id, IsNdc = true };
            if (id == 0) return View(model);
            model = Mapper.Map<AddEditMedicine>(_uploadedMedicine.GetSingle(x => x.Id == id && x.DistributorId == distributorId));

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadMedicine(AddEditMedicine model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var distributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));

                    var result = await _medicineHelper.ProcessDistributorMedicineData(model, User.GetUserId(), distributorId);

                    if (result.IsSuccess)
                    {
                        txscope.Complete();
                        result.ErrorMessage = $@"Medicine {(model.Id == 0 ? "added" : "updated")} successfully. Please wait for admin approval.";
                        return JsonResponse.GenerateJsonResult(1, result.ErrorMessage);
                    }
                    else
                    {
                        result.ErrorMessage = string.IsNullOrEmpty(result.ErrorMessage)
                            ? GlobalConstant.SomethingWrong
                            : result.ErrorMessage;
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, result.ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddEditMedicine");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        #region SystemMedicine

        public IActionResult SystemMedicines()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSystemMedicineList(JQueryDataTableParamModel param)
        {
            var distributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));
                parameters.Parameters.Insert(0, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = distributorId });
                var allList = await _medicine.GetDistributorSystemMedicineList(parameters.Parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetSystemMedicineList");
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
        public async Task<IActionResult> AddtoDistributorMedicine(List<long> medicineList)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var medicineId in medicineList)
                    {

                        var currentDistributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                        var result = _distributorMedicine.GetSingle(x => x.MedicineId == medicineId && x.DistributorId == currentDistributorId);
                        if (result != null)
                        {
                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, GlobalConstant.MedicineAlreadyExist);
                        }

                        var distributorMedicineObj = new DistributorMedicine
                        {
                            MedicineId = medicineId,
                            DistributorId = currentDistributorId
                        };
                        await _distributorMedicine.InsertAsync(distributorMedicineObj, Accessor, User.GetUserId());

                        //
                        var distributorPriceObj = _medicinePriceMaster.GetSingle(x => x.DistributorId == currentDistributorId && x.MedicineId == medicineId);
                        if (distributorPriceObj != null)
                        {
                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, GlobalConstant.MedicineAlreadyExist);
                        }

                        var oldeMedicinePriceObj = _medicinePriceMaster.GetAll().FirstOrDefault(x => x.MedicineId == medicineId);
                        var medicinePriceMaster = Mapper.Map<MedicinePriceMaster>(oldeMedicinePriceObj);
                        medicinePriceMaster.DistributorId = currentDistributorId;

                        await _medicinePriceMaster.InsertAsync(medicinePriceMaster, Accessor, User.GetUserId());
                    }
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, GlobalConstant.AddtoMySellMedicine);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddtoDistributorMedicine");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        #endregion

        #region MySellMedicines
        [Route("~/Distributor/Medicine/MyCatalog/")]
        public IActionResult MySellMedicines()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetDistributorMySellMedicineList(JQueryDataTableParamModel param)
        {
            var distributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));
                parameters.Parameters.Insert(0, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = distributorId });
                var allList = await _medicine.GetDistributorMySellMedicineList(parameters.Parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetDistributorMySellMedicineList");
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
        public async Task<IActionResult> ManageIsActive(long id)
        {
            try
            {
                var distributorMedicine = _medicinePriceMaster.GetSingle(x => x.Id == id);
                distributorMedicine.IsActive = !distributorMedicine.IsActive;
                await _medicinePriceMaster.UpdateAsync(distributorMedicine, Accessor, User.GetUserId());
                return JsonResponse.GenerateJsonResult(1, $@"Medicine {(distributorMedicine.IsActive ? "activated" : "deactivated")} successfully.");
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Post-Medicine/ManageIsActive");
                return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
            }
        }

        [HttpPost]
        [Route("/Distributor/Medicine/ShortDated")]
        [Route("/Distributor/Medicine/InStock")]
        [Route("/Distributor/Medicine/Contracted")]
        public async Task<IActionResult> ManageDistributorMedicines(long id)
        {
            try
            {
                var route = HttpContext.Request.Path.Value.Split("/");
                var temp = route[3];
                var medicinePriceMaster = _medicinePriceMaster.GetSingle(x => x.Id == id);

                switch (temp)
                {
                    case MedicineInfo.InStock:
                        medicinePriceMaster.InStock = !medicinePriceMaster.InStock;
                        break;
                    case MedicineInfo.Contracted:
                        medicinePriceMaster.IsContracted = !medicinePriceMaster.IsContracted;
                        break;
                    case MedicineInfo.ShortDated:
                        medicinePriceMaster.IsShortDated = !medicinePriceMaster.IsShortDated;
                        break;
                }

                await _medicinePriceMaster.UpdateAsync(medicinePriceMaster, Accessor, User.GetUserId());
                return JsonResponse.GenerateJsonResult(1, GlobalConstant.UpdateMedicine);
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Post-Medicine/ManageDistributorMedicines");
                return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePrice(long id, string price)
        {
            try
            {
                var medicinePriceObj = _medicinePriceMaster.GetSingle(x => x.Id == id);
                medicinePriceObj.WacpackagePrice = float.Parse(price);
                await _medicinePriceMaster.UpdateAsync(medicinePriceObj, Accessor, User.GetUserId());
                return JsonResponse.GenerateJsonResult(1, GlobalConstant.UpdateMedicine);
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Post-Medicine/UpdatePrice");
                return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
            }

        }

        [HttpPost]
        public async Task<IActionResult> UpdateStock(long id, string stock)
        {
            try
            {
                var medicinePriceObj = _medicinePriceMaster.GetSingle(x => x.Id == id);
                medicinePriceObj.Stock = long.Parse(stock);
                await _medicinePriceMaster.UpdateAsync(medicinePriceObj, Accessor, User.GetUserId());
                return JsonResponse.GenerateJsonResult(1, GlobalConstant.UpdateMedicine);
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Post-Medicine/UpdateStock");
                return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
            }

        }
        #endregion

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
        #endregion
    }

}