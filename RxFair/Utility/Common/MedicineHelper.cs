using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Service.Interface;
using RxFair.Service.Utility;
using RxFair.Utility.Helpers;

namespace RxFair.Utility.Common
{
    public interface IMedicineHelper
    {
        ImportMedicineData GetBulkMedicineData(Stream fileStream, bool isExcel = true);
        Task<bool> ProcessBulkMedicineData(ImportMedicineData model, long userId, long? distributorId = null);
        Task<UploadMedicineResponse> ProcessDistributorMedicineData(AddEditMedicine model, long userId, long distributorId);
        Task<bool> ProcessDistributorBulkMedicineData(ImportMedicineData model, long userId, long distributorId);

        ImportMedicinePriceData GetBulkMedicinePriceData(Stream fileStream, bool isExcel = true);
        Task<MedicinePriceResponse> ProcessBulkMedicinePriceData(ImportMedicinePriceData model, long userId, long? distributorId = null);

        Task<MedicineResponse> AddUpdateMedicine(AddEditMedicine model, long userId, bool isDistributor, long? distributorId = null);
    }
    public class MedicineHelper : IMedicineHelper
    {
        #region Fields
        private readonly IHttpContextAccessor _accessor;
        private readonly IDosageFormService _dosageForm;
        private readonly IManufacturerService _manufacturer;
        private readonly IMeasurementService _measurement;
        private readonly IMedicineMasterService _medicine;
        private readonly IMedicineCategoryService _medicineCategory;
        private readonly IMedicineImageService _medicineImage;
        private readonly IMedicinePriceMasterService _medicinePrice;
        private readonly IUploadedMedicineService _uploadedMedicine;
        private readonly IErrorLogService _errorLog;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly S3Manager _s3Service;
        #endregion

        #region Ctor
        public MedicineHelper(IHttpContextAccessor accessor, IDosageFormService dosageForm, IManufacturerService manufacturer, IMeasurementService measurement, IErrorLogService errorLog,
            IMedicineMasterService medicine, IMedicineCategoryService medicineCategory, IMedicineImageService medicineImage, IMedicinePriceMasterService medicinePrice, IUploadedMedicineService uploadedMedicine,
            IHostingEnvironment hostingEnvironment, IOptions<AwsS3Storage> awsS3Storage)
        {
            _accessor = accessor;
            _dosageForm = dosageForm;
            _manufacturer = manufacturer;
            _measurement = measurement;
            _medicine = medicine;
            _medicineCategory = medicineCategory;
            _medicineImage = medicineImage;
            _medicinePrice = medicinePrice;
            _uploadedMedicine = uploadedMedicine;
            _hostingEnvironment = hostingEnvironment;
            _errorLog = errorLog;
            _s3Service = new S3Manager(awsS3Storage);
        }
        #endregion

        #region Bulk Medicine File Upload

        public ImportMedicineData GetBulkMedicineData(Stream fileStream, bool isExcel = true)
        {
            try
            {
                if (isExcel)
                {
                    using (var reader = ExcelReaderFactory.CreateReader(fileStream))
                    {
                        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = false, // To set First Row As Column Names
                            }
                        });

                        if (dataSet.Tables.Count <= 0)
                            return new ImportMedicineData { ErrorMessage = "No data found!", UploadMedicines = null };
                        IList<UploadMedicine> uploadMedicines = new List<UploadMedicine>();
                        var datatable = dataSet.Tables[0];

                        foreach (var column in datatable.Columns.Cast<DataColumn>().ToArray())
                        {
                            if (datatable.AsEnumerable().All(dr => dr.IsNull(column)))
                                datatable.Columns.Remove(column);
                        }

                        var columnList = new List<string> { "NDC", "UPC", "Medicine name", "Strength", "Dosage form", "Flavour", "Packaging size", "Unit", "Manufacturer", "Category", "Description" };
                        var firstRowData = datatable.Rows[0].ItemArray.Select(x => x.ToString()).ToList();
                        var newColumnList = firstRowData.Except(columnList).ToList();

                        if (newColumnList.Any()) return new ImportMedicineData { ErrorMessage = "Uploaded file does not match with sample file.", UploadMedicines = null };

                        if (datatable.Rows.Count <= 1) return new ImportMedicineData { ErrorMessage = "No data found!", UploadMedicines = null };
                        {
                            for (var i = 1; i < datatable.Rows.Count; i++)
                            {
                                if (datatable.Rows[i].ItemArray.All(x => string.IsNullOrEmpty(x?.ToString()))) continue;
                                uploadMedicines.Add(SetRowItemToModel(datatable.Rows[i]));
                            }
                            return new ImportMedicineData { ErrorMessage = "", UploadMedicines = uploadMedicines };
                        }
                    }
                }
                using (var reader = ExcelReaderFactory.CreateCsvReader(fileStream))
                {
                    var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = false, // To set First Row As Column Names
                        }
                    });

                    if (dataSet.Tables.Count <= 0)
                        return new ImportMedicineData { ErrorMessage = "No data found!", UploadMedicines = null };
                    IList<UploadMedicine> uploadMedicines = new List<UploadMedicine>();
                    var datatable = dataSet.Tables[0];

                    foreach (var column in datatable.Columns.Cast<DataColumn>().ToArray())
                    {
                        if (datatable.AsEnumerable().All(dr => dr.IsNull(column)))
                            datatable.Columns.Remove(column);
                    }

                    var columnList = new List<string> { "NDC", "UPC", "Medicine name", "Strength", "Dosage form", "Flavour", "Packaging size", "Unit", "Manufacturer", "Category", "Description" };
                    var firstRowData = datatable.Rows[0].ItemArray.Select(x => x.ToString()).ToList();
                    var newColumnList = firstRowData.Except(columnList).ToList();

                    if (newColumnList.Any()) return new ImportMedicineData { ErrorMessage = "Uploaded file does not match with sample file.", UploadMedicines = null };

                    if (datatable.Rows.Count <= 1) return new ImportMedicineData { ErrorMessage = "No data found!", UploadMedicines = null };
                    {
                        for (var i = 1; i < datatable.Rows.Count; i++)
                        {
                            if (datatable.Rows[i].ItemArray.All(x => string.IsNullOrEmpty(x?.ToString()))) continue;
                            uploadMedicines.Add(SetRowItemToModel(datatable.Rows[i]));
                        }
                        return new ImportMedicineData { ErrorMessage = "", UploadMedicines = uploadMedicines };
                    }
                }
            }
            catch (Exception e)
            {
                return new ImportMedicineData { ErrorMessage = e.Message, UploadMedicines = null };
            }
        }

        private static UploadMedicine SetRowItemToModel(DataRow objRow)
        {
            var packageSize = !string.IsNullOrEmpty(Convert.ToString(objRow[6]).Trim()) ?
                Convert.ToSingle(Convert.ToString(objRow[6]).Trim()) : (float?)null;
            return new UploadMedicine
            {
                Ndc = Convert.ToString(objRow[0]).Trim(),
                Upc = Convert.ToString(objRow[1]).Trim(),
                MedicineName = Convert.ToString(objRow[2]).Trim(),
                Strength = Convert.ToString(objRow[3]).Trim(),
                DosageForm = Convert.ToString(objRow[4]).Trim(),
                Flavour = Convert.ToString(objRow[5]).Trim(),
                PackageSize = packageSize,
                PackagingSize = Convert.ToString(objRow[6]).Trim(),
                Unit = Convert.ToString(objRow[7]).Trim(),
                Manufacturer = Convert.ToString(objRow[8]).Trim(),
                Category = Convert.ToString(objRow[9]).Trim(),
                Description = Convert.ToString(objRow[10]).Trim(),
                IsNdc = (Convert.ToString(objRow[0]).Trim() != Convert.ToString(objRow[1]).Trim())
            };
        }

        public async Task<bool> ProcessBulkMedicineData(ImportMedicineData model, long userId, long? distributorId)
        {
            try
            {
                // Upload file duplicate ndc
                var modelNdcDuplicate = model.UploadMedicines.GroupBy(x => x.Ndc).Where(x => x.Count() > 1)
                    .Select(x => x.Key).ToList();

                // Remove duplicate medicine from upload file model
                model.UploadMedicines = model.UploadMedicines.Where(x => !modelNdcDuplicate.Contains(x.Ndc)).ToList();

                // Find unique ndc from upload file
                var modelNdc = model.UploadMedicines.Select(x => x.Ndc).ToList();

                #region Upload File Data
                var dosageFormList = model.UploadMedicines.GroupBy(x => x.DosageForm).Select(x => x.Key).ToList();
                var manufacturerList = model.UploadMedicines.GroupBy(x => x.Manufacturer).Select(x => x.Key).ToList();
                var strengthList = model.UploadMedicines.GroupBy(x => x.Strength).Select(x => x.Key).ToList();
                //var brandList = model.UploadMedicines.GroupBy(x => x.Brand).Select(x => x.Key).ToList();
                //var packagingSizeList = model.UploadMedicines.GroupBy(x => x.PackagingSize).Select(x => x.Key).ToList();
                var doseUnitList = model.UploadMedicines.GroupBy(x => x.Unit).Select(x => x.Key).ToList();
                var categoryList = model.UploadMedicines.GroupBy(x => x.Category).Select(x => x.Key).ToList();
                #endregion

                #region Find Exist and New Master Entry

                #region Add New DosageForm
                var existDosageForm = _dosageForm.GetAll().ToList();
                var newDosageForm = dosageFormList.Except(existDosageForm.Select(x => x.DosageForm.Trim())).ToList();
                var addDosageForm = new List<DosageFormMaster>();
                foreach (var item in newDosageForm)
                {
                    addDosageForm.Add(new DosageFormMaster { DosageForm = item, IsActive = true });
                }

                if (addDosageForm.Any())
                {
                    await _dosageForm.InsertRangeAsync(addDosageForm, _accessor, userId);
                    existDosageForm.AddRange(addDosageForm);
                }
                #endregion

                #region Add New Manufacturer
                var existManufacturer = _manufacturer.GetAll().ToList();
                var newManufacturer = manufacturerList.Except(existManufacturer.Select(x => x.ManufacturerName.Trim())).ToList();
                var addManufacturer = new List<ManufacturerMaster>();
                foreach (var item in newManufacturer)
                {
                    addManufacturer.Add(new ManufacturerMaster { ManufacturerName = item, IsActive = true });
                }

                if (addManufacturer.Any())
                {
                    await _manufacturer.InsertRangeAsync(addManufacturer, _accessor, userId);
                    existManufacturer.AddRange(addManufacturer);
                }
                #endregion

                #region Add New Category
                var existCategory = _medicineCategory.GetAll().ToList();
                var newCategory = categoryList.Except(existCategory.Select(x => x.MedicineCategory.Trim())).ToList();
                var addCategory = new List<MedicineCategoryMaster>();
                foreach (var item in newCategory)
                {
                    if (string.IsNullOrEmpty(item)) continue;
                    addCategory.Add(new MedicineCategoryMaster { MedicineCategory = item, IsActive = true });
                }

                if (addCategory.Any())
                {
                    await _medicineCategory.InsertRangeAsync(addCategory, _accessor, userId);
                    existCategory.AddRange(addCategory);
                }
                #endregion

                #region Add New Measurement Types
                List<Measurement> addMeasurements = new List<Measurement>();
                var existStrength = _measurement.GetAll(x => x.MeasurementType == (short)GlobalEnums.MeasurementType.Strength && strengthList.Contains(x.MeasurementUnit)).ToList();
                var newStrength = strengthList.Except(existStrength.Select(x => x.MeasurementUnit.Trim())).ToList();

                //var existBrand = _measurement.GetAll(x => x.MeasurementType == (short)GlobalEnums.MeasurementType.Brand && brandList.Contains(x.MeasurementUnit)).ToList();
                //var newBrand = brandList.Except(existBrand.Select(x => x.MeasurementUnit.Trim())).ToList();

                //var existPackagingSize = _measurement.GetAll(x => x.MeasurementType == (short)GlobalEnums.MeasurementType.PackageSize && packagingSizeList.Contains(x.MeasurementUnit)).ToList();
                //var newPackagingSize = packagingSizeList.Except(existPackagingSize.Select(x => x.MeasurementUnit.Trim())).ToList();

                var existDoseUnit = _measurement.GetAll(x => x.MeasurementType == (short)GlobalEnums.MeasurementType.DoseUnit && doseUnitList.Contains(x.MeasurementUnit)).ToList();
                var newDoseUnit = doseUnitList.Except(existDoseUnit.Select(x => x.MeasurementUnit.Trim())).ToList();

                foreach (var item in newStrength)
                {
                    addMeasurements.Add(new Measurement { MeasurementUnit = item, MeasurementType = (short)GlobalEnums.MeasurementType.Strength, IsActive = true });
                }
                //foreach (var item in newBrand)
                //{
                //    addMeasurements.Add(new Measurement { MeasurementUnit = item, MeasurementType = (short)GlobalEnums.MeasurementType.Brand, IsActive = true });
                //}
                //foreach (var item in newPackagingSize)
                //{
                //    addMeasurements.Add(new Measurement { MeasurementUnit = item, MeasurementType = (short)GlobalEnums.MeasurementType.PackageSize, IsActive = true });
                //}
                foreach (var item in newDoseUnit)
                {
                    addMeasurements.Add(new Measurement { MeasurementUnit = item, MeasurementType = (short)GlobalEnums.MeasurementType.DoseUnit, IsActive = true });
                }
                if (addMeasurements.Any())
                {
                    await _measurement.InsertRangeAsync(addMeasurements, _accessor, userId);
                    existStrength.AddRange(addMeasurements.Where(x => x.MeasurementType == (short)GlobalEnums.MeasurementType.Strength));
                    //existBrand.AddRange(addMeasurements.Where(x => x.MeasurementType == (short)GlobalEnums.MeasurementType.Brand));
                    //existPackagingSize.AddRange(addMeasurements.Where(x => x.MeasurementType == (short)GlobalEnums.MeasurementType.PackageSize));
                    existDoseUnit.AddRange(addMeasurements.Where(x => x.MeasurementType == (short)GlobalEnums.MeasurementType.DoseUnit));
                }
                #endregion

                #endregion

                #region Database Data
                var dbNdc = _medicine.GetAll(x => x.DistributorId == distributorId).Select(x => x.NdcUpcHri).ToList();
                var dbExistNdc = dbNdc.Where(x => modelNdc.Contains(x)).ToList();
                var newNdcList = modelNdc.Except(dbExistNdc).ToList();
                #endregion

                #region Update Exist Medicine
                if (dbExistNdc.Any())
                {
                    var editExistMedicines = model.UploadMedicines.Where(x => dbExistNdc.Contains(x.Ndc)).ToList();
                    foreach (var item in editExistMedicines)
                    {
                        var single = _medicine.GetSingle(x => x.NdcUpcHri == item.Ndc && x.DistributorId == distributorId);
                        if (single == null) continue;
                        {
                            // UPC
                            single.DrugName = item.MedicineName;
                            single.StrengthId = existStrength.FirstOrDefault(x => x.MeasurementUnit == item.Strength)?.Id;
                            single.DosageFormId = existDosageForm.FirstOrDefault(x => x.DosageForm == item.DosageForm)?.Id;
                            //single.BrandId = existBrand.FirstOrDefault(x => x.MeasurementUnit == item.Brand)?.Id;
                            single.Flavour = item.Flavour;
                            single.PackageSize = item.PackageSize;
                            //single.PackageSizeId = existPackagingSize.FirstOrDefault(x => x.MeasurementUnit == item.PackagingSize)?.Id;
                            single.UnitSizeId = existDoseUnit.FirstOrDefault(x => x.MeasurementUnit == item.Unit)?.Id;
                            single.ManufacturerId = existManufacturer.FirstOrDefault(x => x.ManufacturerName == item.Manufacturer)?.Id;
                            single.CategoryId = existCategory.FirstOrDefault(x => x.MedicineCategory == item.Category)?.Id;
                            single.Description = item.Description;
                            
                            await _medicine.UpdateAsync(single, _accessor, userId);
                        }
                    }
                }
                #endregion

                #region Add New Medicine

                if (!newNdcList.Any()) return true;
                {
                    var addMedicines = new List<MedicineMaster>();
                    var newMedicines = model.UploadMedicines.Where(x => newNdcList.Contains(x.Ndc)).ToList();
                    foreach (var item in newMedicines)
                    {
                        addMedicines.Add(new MedicineMaster
                        {
                            NdcUpcHri = item.Ndc,
                            IsNdc = item.IsNdc,
                            DrugName = item.MedicineName,
                            StrengthId = existStrength.FirstOrDefault(x => x.MeasurementUnit == item.Strength)?.Id,
                            DosageFormId = existDosageForm.FirstOrDefault(x => x.DosageForm == item.DosageForm)?.Id,
                            //BrandId = existBrand.FirstOrDefault(x => x.MeasurementUnit == item.Brand)?.Id,
                            Flavour = item.Flavour,
                            PackageSize = item.PackageSize,
                            //PackageSizeId = existPackagingSize.FirstOrDefault(x => x.MeasurementUnit == item.PackagingSize)?.Id,
                            UnitSizeId = existDoseUnit.FirstOrDefault(x => x.MeasurementUnit == item.Unit)?.Id,
                            ManufacturerId = existManufacturer.FirstOrDefault(x => x.ManufacturerName == item.Manufacturer)?.Id,
                            CategoryId = existCategory.FirstOrDefault(x => x.MedicineCategory == item.Category)?.Id,
                            Description = item.Description,
                            DistributorId = distributorId,
                            IsActive = true,
                            IsDelete = false,
                            IsDiscontinue = false
                        });
                    }

                    if (addMedicines.Any())
                    {
                        await _medicine.InsertRangeAsync(addMedicines, _accessor, userId);
                    }
                }
                #endregion
                return true;
            }
            catch (Exception ex)
            {
                _errorLog.AddErrorLog(ex, "ProcessBulkMedicineData");
                return false;
            }
        }

        public async Task<UploadMedicineResponse> ProcessDistributorMedicineData(AddEditMedicine model, long userId, long distributorId)
        {
            if (model.Id == 0)
            {
                var dbNdc = _medicine.GetSingle(x => x.DistributorId == distributorId && x.NdcUpcHri.Equals(model.Ndc) && x.IsNdc == model.IsNdc); // ALL NDC base on distributorId
                if (dbNdc != null)
                {
                    return new UploadMedicineResponse
                    {
                        Id = model.Id,
                        ErrorMessage = "You are trying to upload existing medicine again.",
                        IsSuccess = false
                    };
                }

                var dbUploaded = _uploadedMedicine.GetSingle(x => x.DistributorId == distributorId && x.Ndc.Equals(model.Ndc));
                if (dbUploaded != null)
                {
                    return new UploadMedicineResponse
                    {
                        Id = model.Id,
                        ErrorMessage = "You are trying to upload existing medicine again.",
                        IsSuccess = false
                    };
                }

                var dbModel = new UploadedMedicine
                {
                    Ndc = model.Ndc,
                    Upc = (model.IsNdc ? null : model.Upc),
                    MedicineName = model.MedicineName,
                    Strength = model.Strength,
                    DosageForm = model.DosageForm,
                    Brand = model.Brand,
                    Flavour = model.Flavour,
                    PackageSize = model.PackageSize,
                    Unit = model.Unit,
                    Manufacturer = model.Manufacturer,
                    Category = model.Category,
                    Description = model.Description,
                    DistributorId = distributorId,
                    IsActive = true
                };
                if (model.MedicineFile != null)
                {
                    var newFileName = CommonMethod.GetFileName(model.MedicineFile.FileName);
                    _s3Service.UploadImageToParticularFolder(BucketName.MedicineImage, model.MedicineFile, newFileName);

                    dbModel.MedicineImage = newFileName;
                }
                await _uploadedMedicine.InsertAsync(dbModel, _accessor, userId);

                return new UploadMedicineResponse { Id = dbModel.Id, IsSuccess = true };
            }

            var editUploadedMedicine = _uploadedMedicine.GetSingle(x => x.Id == model.Id && x.Ndc.Equals(model.Ndc));
            if (editUploadedMedicine == null) return new UploadMedicineResponse { IsSuccess = false, Id = 0, ErrorMessage = "" };
            var oldMedicineImage = editUploadedMedicine.MedicineImage;
            editUploadedMedicine.Upc = (model.IsNdc ? null : model.Upc);
            editUploadedMedicine.MedicineName = model.MedicineName;
            editUploadedMedicine.MedicineImage = model.MedicineImage;
            editUploadedMedicine.Strength = model.Strength;
            editUploadedMedicine.DosageForm = model.DosageForm;
            editUploadedMedicine.Brand = model.Brand;
            editUploadedMedicine.Flavour = model.Flavour;
            editUploadedMedicine.PackageSize = model.PackageSize;
            editUploadedMedicine.Unit = model.Unit;
            editUploadedMedicine.Manufacturer = model.Manufacturer;
            editUploadedMedicine.Category = model.Category;
            editUploadedMedicine.Description = model.Description;
            if (model.MedicineFile != null)
            {
                var newFileName = CommonMethod.GetFileName(model.MedicineFile.FileName);
                _s3Service.UploadImageToParticularFolder(BucketName.MedicineImage, model.MedicineFile, newFileName);
                editUploadedMedicine.MedicineImage = newFileName;
            }
            if (!string.IsNullOrEmpty(oldMedicineImage))
            {
                CommonMethod.DeleteFile(CommonMethod.CheckServerPath(_hostingEnvironment.WebRootPath, FilePathList.UserProfile, oldMedicineImage), true);
            }
            return new UploadMedicineResponse { IsSuccess = true, Id = editUploadedMedicine.Id };
        }

        public async Task<bool> ProcessDistributorBulkMedicineData(ImportMedicineData model, long userId, long distributorId)
        {
            try
            {
                // Upload file duplicate ndc
                var modelNdcDuplicate = model.UploadMedicines.GroupBy(x => x.Ndc).Where(x => x.Count() > 1)
                    .Select(x => x.Key).ToList();

                // Remove duplicate medicine from upload file model
                model.UploadMedicines = model.UploadMedicines.Where(x => !modelNdcDuplicate.Contains(x.Ndc)).ToList();

                // Find unique ndc from upload file
                var modelNdc = model.UploadMedicines.Select(x => x.Ndc).ToList();

                #region Database Data
                var dbNdc = _medicine.GetAll(x => x.DistributorId == distributorId).Select(x => x.NdcUpcHri).ToList(); // ALL NDC base on distributorId
                var dbExistNdc = dbNdc.Where(x => modelNdc.Contains(x)).ToList(); // Find exist NDC list from Medicine Master
                                                                                  //update medicine
                var updateExist = _medicine.GetAll().Where(x => dbExistNdc.Contains(x.NdcUpcHri) && x.DistributorId == distributorId);
                int i = 0;
                foreach (var item in dbExistNdc)
                {
                    i++;
                    var modelData = model.UploadMedicines.FirstOrDefault(x => x.Ndc == item);
                    var single = updateExist.FirstOrDefault(x => x.NdcUpcHri == item);
                    if (single == null) continue;
                    single.DrugName = modelData.MedicineName;
                    single.Strength = modelData.Strength;
                    single.DosageFormId = _dosageForm.GetSingle(x => x.DosageForm.Equals(modelData.DosageForm))?.Id ?? 0;
                    single.Flavour = modelData.Flavour;
                    single.PackageSize = modelData.PackageSize;
                    single.UnitSizeId = _measurement.GetSingle(x => x.MeasurementUnit.Equals(modelData.Unit) && x.MeasurementType == (short)GlobalEnums.MeasurementType.DoseUnit)?.Id ?? 0;

                    single.ManufacturerId = _manufacturer.GetSingle(x => x.ManufacturerName.Equals(modelData.Manufacturer))?.Id ?? 0;
                    single.CategoryId = _medicineCategory.GetSingle(x => x.MedicineCategory.Equals(modelData.Category))?.Id ?? null;
                    single.Description = modelData.Description;

                    await _medicine.UpdateAsync(single, _accessor, userId);
                }

                #region Update UploadMedicine table
                //check for uplaod medicine 
                var newUploadedNdcList = modelNdc.Except(dbExistNdc).ToList(); // Find new upload ndc list from upload file

                var dbUploadedNdc = _uploadedMedicine.GetAll(x => x.DistributorId == distributorId).Select(x => x.Ndc).ToList();

                var existUploadedNdcList = dbUploadedNdc.Where(x => newUploadedNdcList.Contains(x)).ToList();

                var notExistUploadedNdcList = existUploadedNdcList.Except(dbUploadedNdc).ToList();


                if (notExistUploadedNdcList.Count == 0 && existUploadedNdcList.Count == 0)
                {
                    notExistUploadedNdcList = newUploadedNdcList;
                }
                // Select Exist Medicine From UploadedNdcList

                var exist = model.UploadMedicines.Where(x => existUploadedNdcList.Contains(x.Ndc)).ToList();
                var editUpMed = _uploadedMedicine.GetAll(x => x.DistributorId == distributorId && exist.Select(y => y.Ndc).Contains(x.Ndc))
                    .ToList();
                foreach (var item in exist)
                {
                    var single = editUpMed.FirstOrDefault(x => x.Ndc == item.Ndc);
                    if (single == null) continue;
                    single.Upc = item.Upc;
                    single.MedicineName = item.MedicineName;
                    single.MedicineImage = item.MedicineImage;
                    single.Strength = item.Strength;
                    single.DosageForm = item.DosageForm;
                    //single.Brand = item.Brand;
                    single.Flavour = item.Flavour;
                    single.PackageSize = item.PackageSize;
                    single.Unit = item.Unit;
                    single.Manufacturer = item.Manufacturer;
                    single.Category = item.Category;
                    single.Description = item.Description;
                    await _uploadedMedicine.UpdateAsync(single, _accessor, userId);
                }
                #endregion


                // Add Medicine From UploadedNdcList
                //var finalUploadNdc = newUploadedNdcList.Except(existUploadedNdcList).ToList();
                var newUploadNdc = model.UploadMedicines.Where(x => notExistUploadedNdcList.Contains(x.Ndc))
                    .Select(item => new UploadedMedicine
                    {
                        Ndc = item.Ndc,
                        Upc = item.Upc,
                        MedicineName = item.MedicineName,
                        MedicineImage = item.MedicineImage,
                        Strength = item.Strength,
                        DosageForm = item.DosageForm,
                        //Brand = item.Brand,
                        Flavour = item.Flavour,
                        PackageSize = item.PackageSize,
                        Unit = item.Unit,
                        Manufacturer = item.Manufacturer,
                        Category = item.Category,
                        Description = item.Description,
                        IsActive = true,
                        DistributorId = distributorId
                    }).ToList();
                // New From Uploaded Ndc List
                if (newUploadNdc.Any())
                {
                    await _uploadedMedicine.InsertRangeAsync(newUploadNdc, _accessor, userId);
                }

                #endregion

                return true;
            }
            catch (Exception ex)
            {
                _errorLog.AddErrorLog(ex, "ProcessDistributorBulkMedicineData");
                return false;
            }
        }

        #endregion

        #region Bulk Medicine Price Upload

        public ImportMedicinePriceData GetBulkMedicinePriceData(Stream fileStream, bool isExcel = true)
        {
            try
            {
                if (isExcel)
                {
                    using (var reader = ExcelReaderFactory.CreateReader(fileStream))
                    {
                        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = false, // To set First Row As Column Names
                            }
                        });

                        if (dataSet.Tables.Count <= 0)
                            return new ImportMedicinePriceData { ErrorMessage = "No data found!", UploadMedicinePrices = null };
                        IList<UploadMedicinePrice> uploadMedicinesPrice = new List<UploadMedicinePrice>();
                        var datatable = dataSet.Tables[0];

                        foreach (var column in datatable.Columns.Cast<DataColumn>().ToArray())
                        {
                            if (datatable.AsEnumerable().All(dr => dr.IsNull(column)))
                                datatable.Columns.Remove(column);
                        }

                        var columnList = new List<string> { "NDC/UPC", "Price", "Short Dated (Yes / No)", "Contracted (Yes / No)", "In Stock (Yes / No)", "Stock" };
                        var firstRowData = datatable.Rows[0].ItemArray.Select(x => x.ToString()).ToList();
                        var newColumnList = firstRowData.Except(columnList).ToList();

                        if (newColumnList.Any()) return new ImportMedicinePriceData { ErrorMessage = "Uploaded Prices does not match with sample file.", UploadMedicinePrices = null };

                        if (datatable.Rows.Count <= 1) return new ImportMedicinePriceData { ErrorMessage = "No data found!", UploadMedicinePrices = null };
                        {
                            for (var i = 1; i < datatable.Rows.Count; i++)
                            {
                                if (datatable.Rows[i].ItemArray.All(x => string.IsNullOrEmpty(x?.ToString()))) continue;
                                uploadMedicinesPrice.Add(SetRowItemToModelPrice(datatable.Rows[i]));
                            }
                            return new ImportMedicinePriceData { ErrorMessage = "", UploadMedicinePrices = uploadMedicinesPrice };
                        }
                    }
                }
                using (var reader = ExcelReaderFactory.CreateCsvReader(fileStream))
                {
                    var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = false, // To set First Row As Column Names
                        }
                    });

                    if (dataSet.Tables.Count <= 0)
                        return new ImportMedicinePriceData { ErrorMessage = "No data found!", UploadMedicinePrices = null };
                    IList<UploadMedicinePrice> uploadMedicinesPrice = new List<UploadMedicinePrice>();
                    var datatable = dataSet.Tables[0];

                    foreach (var column in datatable.Columns.Cast<DataColumn>().ToArray())
                    {
                        if (datatable.AsEnumerable().All(dr => dr.IsNull(column)))
                            datatable.Columns.Remove(column);
                    }

                    var columnList = new List<string> { "NDC/UPC", "Price", "Short Dated (Yes / No)", "Contracted (Yes / No)", "In Stock (Yes / No)", "Stock" };
                    var firstRowData = datatable.Rows[0].ItemArray.Select(x => x.ToString()).ToList();
                    var newColumnList = firstRowData.Except(columnList).ToList();

                    if (newColumnList.Any()) return new ImportMedicinePriceData { ErrorMessage = "Uploaded Prices does not match with sample file.", UploadMedicinePrices = null };

                    if (datatable.Rows.Count <= 1) return new ImportMedicinePriceData { ErrorMessage = "No data found!", UploadMedicinePrices = null };
                    {
                        for (var i = 1; i < datatable.Rows.Count; i++)
                        {
                            if (datatable.Rows[i].ItemArray.All(x => string.IsNullOrEmpty(x?.ToString()))) continue;
                            uploadMedicinesPrice.Add(SetRowItemToModelPrice(datatable.Rows[i]));
                        }
                        return new ImportMedicinePriceData { ErrorMessage = "", UploadMedicinePrices = uploadMedicinesPrice };
                    }
                }
            }
            catch (Exception e)
            {
                return new ImportMedicinePriceData { ErrorMessage = GlobalConstant.UnhandledError, UploadMedicinePrices = null };
            }

        }

        private static UploadMedicinePrice SetRowItemToModelPrice(DataRow objRow)
        {
            return new UploadMedicinePrice
            {
                Ndc = Convert.ToString(objRow[0]).Trim(),
                Price = (float)Convert.ToDouble(objRow[1]),
                ShortDated = Convert.ToString(objRow[2]).ToLower().Trim() == "yes" ? true : false,
                Contracted = Convert.ToString(objRow[3]).ToLower().Trim() == "yes" ? true : false,
                InStock = Convert.ToString(objRow[4]).ToLower().Trim() == "yes" ? true : false,
                Stock = Convert.ToInt64(objRow[5])
            };
        }

        public async Task<MedicinePriceResponse> ProcessBulkMedicinePriceData(ImportMedicinePriceData model, long userId, long? distributorId = null)
        {
            try
            {
                // Upload file duplicate ndc
                var modelNdcDuplicate = model.UploadMedicinePrices.GroupBy(x => x.Ndc).Where(x => x.Count() > 1)
                    .Select(x => x.Key).ToList();

                // Remove duplicate medicine from upload price model
                model.UploadMedicinePrices = model.UploadMedicinePrices.Where(x => !modelNdcDuplicate.Contains(x.Ndc)).ToList();

                // Find unique ndc from upload price
                var modelNdc = model.UploadMedicinePrices.Select(x => x.Ndc).ToList();

                // Find Exist entry
                var dbNdc = _medicine.GetAll().Select(x => new MedicineWithId { Id = x.Id, Ndc = x.NdcUpcHri }).ToList();
                var dbNotExist = modelNdc.Except(dbNdc.Select(x => x.Ndc)).ToList();
                var newNdcList = modelNdc.Except(dbNotExist).ToList();

                #region Insert Or Update Medicine Price

                bool isDistributor = distributorId == null;
                var result = _medicinePrice.GetAll(x => modelNdc.Contains(x.NdcUpcHri) && x.DistributorId == distributorId);
                if (!result.Any())
                {
                    var addMedicinePrices = new List<MedicinePriceMaster>();
                    var newMedicinePrices = model.UploadMedicinePrices.Where(x => newNdcList.Contains(x.Ndc)).ToList();
                    foreach (var item in newMedicinePrices)
                    {
                        addMedicinePrices.Add(new MedicinePriceMaster
                        {
                            MedicineId = dbNdc.FirstOrDefault(x => x.Ndc == item.Ndc)?.Id ?? 0,
                            NdcUpcHri = item.Ndc,
                            DistributorId = distributorId,

                            // If uploaded by Distributor
                            WacpackagePrice = isDistributor ? null : item.Price,
                            WacunitPrice = isDistributor ? null : item.Price,
                            WacunitPriceExtended = isDistributor ? null : item.Price,

                            // If uploaded by Admin
                            AwppackagePrice = isDistributor ? item.Price : null,
                            AwpunitPrice = isDistributor ? item.Price : null,
                            AwpunitPriceExtended = isDistributor ? item.Price : null,

                            IsShortDated = item.ShortDated,
                            IsContracted = item.Contracted,
                            InStock = item.InStock,

                            Stock = item.Stock,
                            IsActive = true,
                            ModifiedBy = userId,
                            ModifiedDate = DateTime.UtcNow
                        });
                    }

                    if (!addMedicinePrices.Any())
                        return new MedicinePriceResponse
                        {
                            IsSuccess = false,
                            NotExistMedicine = dbNotExist,
                            ErrorMessage = "Uploaded Medicine Price couldn't find medicine in the system database."
                        };
                    await _medicinePrice.InsertRangeAsync(addMedicinePrices, _accessor, userId);
                    return new MedicinePriceResponse
                    {
                        IsSuccess = true,
                        InsertedMedicine = addMedicinePrices.Select(x => x.NdcUpcHri).ToList(),
                        ErrorMessage = "Bulk medicine price uploaded."
                    };
                }

                var dbExistNdc = dbNdc.Where(x => modelNdc.Contains(x.Ndc)).ToList();

                //Update
                var editExistMedicinePrices = model.UploadMedicinePrices.Where(x => dbExistNdc.Select(y => y.Ndc).Contains(x.Ndc)).ToList();
                var notDistrubutorMedicine = new List<string>();
                var uploadedMedicine = new List<string>();

                foreach (var item in editExistMedicinePrices)
                {
                    var single = _medicinePrice.GetSingle(x => x.NdcUpcHri == item.Ndc && x.DistributorId == distributorId);
                    if (single == null)
                        notDistrubutorMedicine.Add(item.Ndc);
                    else
                    {
                        // If uploaded by Admin
                        single.WacpackagePrice = isDistributor ? null : item.Price;
                        single.WacunitPrice = isDistributor ? null : item.Price;
                        single.WacunitPriceExtended = isDistributor ? null : item.Price;

                        // If uploaded by Distributor
                        single.AwppackagePrice = isDistributor ? item.Price : null;
                        single.AwpunitPrice = isDistributor ? item.Price : null;
                        single.AwpunitPriceExtended = isDistributor ? item.Price : null;

                        single.IsShortDated = item.ShortDated;
                        single.IsContracted = item.Contracted;
                        single.InStock = item.InStock;

                        single.Stock = item.Stock;
                        single.ModifiedBy = userId;
                        single.CreatedDate = DateTime.UtcNow;
                        _medicinePrice.Update(single);

                        uploadedMedicine.Add(single.NdcUpcHri);
                    }
                }
                return new MedicinePriceResponse
                {
                    IsSuccess = true,
                    NotExistMedicine = notDistrubutorMedicine,
                    UploadedMedicine = uploadedMedicine,
                    ErrorMessage = "Bulk medicine price uploaded."
                };
                #endregion
            }
            catch (Exception ex)
            {
                _errorLog.AddErrorLog(ex, "ProcessBulkMedicinePriceData");
                return new MedicinePriceResponse
                {
                    IsSuccess = false,
                    ErrorMessage = GlobalConstant.SomethingWrong
                };
            }
        }

        public async Task<MedicineResponse> AddUpdateMedicine(AddEditMedicine model, long userId, bool isDistributor, long? distributorId = null)
        {
            string newFileName;
            if (model.Id == 0)
            {
                if (_medicine.GetCount(x => x.NdcUpcHri == model.Ndc) > 0)
                    return new MedicineResponse { IsSuccess = false, ErrorMessage = "Medicine NDC No. is duplicate." };
                var newMedicine = new MedicineMaster
                {
                    NdcUpcHri = model.Ndc,
                    DrugName = model.MedicineName,
                    BrandId = model.BrandId,
                    Flavour = model.Flavour,
                    Strength = model.Strength,
                    StrengthId = model.StrengthId,
                    DosageFormId = model.DosageFormId,
                    PackageSize = model.PackageSize,
                    PackageSizeId = model.PackagingSizeId,
                    UnitSizeId = model.UnitId,
                    PackageDescriptionCodeId = model.PackageDescriptionCodeId,
                    CategoryId = model.CategoryId,
                    ManufacturerId = model.ManufacturerId,
                    Description = model.Description,
                    DistributorId = distributorId,
                    IsNdc = model.IsNdc,
                    IsActive = true,
                    IsDelete = false,
                    IsDiscontinue = false
                };
                if (model.MedicineFile != null)
                {
                    newFileName = CommonMethod.GetFileName(model.MedicineFile.FileName);
                    _s3Service.UploadImageToParticularFolder(BucketName.MedicineImage, model.MedicineFile, newFileName);
                    //await CommonMethod.UploadFileAsync(_hostingEnvironment.WebRootPath, FilePathList.MedicineImage, newFileName, model.MedicineFile);
                    newMedicine.MedicineImage = newFileName;
                }
                await _medicine.InsertAsync(newMedicine, _accessor, userId);
                var newMediPrice = new MedicinePriceMaster
                {
                    MedicineId = newMedicine.Id,
                    NdcUpcHri = newMedicine.NdcUpcHri,
                    DistributorId = distributorId,
                    IsContracted = false,
                    IsShortDated = false,
                    InStock = false,
                    IsActive = true
                };
                if (isDistributor)    // If uploaded by Distributor
                {
                    newMediPrice.WacpackagePrice = model.AwpPrice;
                    //newMediPrice.WacunitPrice = model.AwpPrice;
                    //newMediPrice.WacunitPriceExtended = model.AwpPrice;
                }
                else // If uploaded by Admin
                {
                    newMediPrice.AwppackagePrice = model.AwpPrice;
                    //newMediPrice.AwpunitPrice = model.AwpPrice;
                    //newMediPrice.AwpunitPriceExtended = model.AwpPrice;
                }
                await _medicinePrice.InsertAsync(newMediPrice, _accessor, userId);
                return new MedicineResponse { IsSuccess = true, ErrorMessage = "Medicine added successfully." };
            }

            var editMedicine = _medicine.GetSingle(x => x.Id == model.Id && x.NdcUpcHri.Equals(model.Ndc));
            if (editMedicine == null) return new MedicineResponse { IsSuccess = false, ErrorMessage = "Medicine could not found." };

            editMedicine.NdcUpcHri = model.Ndc;
            editMedicine.IsNdc = model.IsNdc;
            editMedicine.DrugName = model.MedicineName;
            editMedicine.BrandId = model.BrandId;
            editMedicine.Flavour = model.Flavour;
            editMedicine.Strength = model.Strength;
            editMedicine.StrengthId = model.StrengthId;
            editMedicine.DosageFormId = model.DosageFormId;
            editMedicine.PackageSize = model.PackageSize;
            editMedicine.PackageSizeId = model.PackagingSizeId;
            editMedicine.UnitSizeId = model.UnitId;
            editMedicine.PackageDescriptionCodeId = model.PackageDescriptionCodeId;
            editMedicine.CategoryId = model.CategoryId;
            editMedicine.ManufacturerId = model.ManufacturerId;
            editMedicine.Description = model.Description;
            if (model.MedicineFile != null)
            {
                newFileName = model.MedicineFile.FileName.Trim();
                _s3Service.UploadImageToParticularFolder(BucketName.MedicineImage, model.MedicineFile, newFileName);
                //await CommonMethod.UploadFileAsync(_hostingEnvironment.WebRootPath, FilePathList.MedicineImage, newFileName, model.MedicineFile);
                if (editMedicine.MedicineImages.Count != 0)
                {
                    await _medicineImage.InsertAsync(new MedicineImage
                    {
                        MedicineId = editMedicine.Id,
                        ImageName = editMedicine.MedicineImage
                    }, _accessor, userId);
                }
                editMedicine.MedicineImage = newFileName;
            }
            await _medicine.UpdateAsync(editMedicine, _accessor, userId);
            var editMediPrice = editMedicine.MedicinePriceMasters.FirstOrDefault(x => x.MedicineId == editMedicine.Id && x.DistributorId == distributorId);
            if (editMedicine.Id != 0)
            {
                if (editMediPrice != null)
                {
                    if (isDistributor)
                        editMediPrice.WacpackagePrice = model.AwpPrice; // If uploaded by Distributor
                    else
                        editMediPrice.AwppackagePrice = model.AwpPrice; // If uploaded by Admin
                    await _medicinePrice.UpdateAsync(editMediPrice, _accessor, userId);
                }
            }
            else
            {
                var newMediPrice = new MedicinePriceMaster
                {
                    MedicineId = editMedicine.Id,
                    DistributorId = distributorId,
                    IsContracted = false,
                    IsShortDated = false,
                    InStock = false,
                    IsActive = true
                };
                if (isDistributor)    // If uploaded by Distributor
                {
                    newMediPrice.WacpackagePrice = model.AwpPrice;
                    //newMediPrice.WacunitPrice = model.AwpPrice;
                    //newMediPrice.WacunitPriceExtended = model.AwpPrice;
                }
                else // If uploaded by Admin
                {
                    newMediPrice.AwppackagePrice = model.AwpPrice;
                    //newMediPrice.AwpunitPrice = model.AwpPrice;
                    //newMediPrice.AwpunitPriceExtended = model.AwpPrice;
                }
                await _medicinePrice.InsertAsync(newMediPrice, _accessor, userId);
            }
            return new MedicineResponse { IsSuccess = true, ErrorMessage = "Medicine updated successfully." };
        }
        #endregion
    }
    public class MedicineWithId
    {
        public long Id { get; set; }
        public string Ndc { get; set; }
    }
    public class MedicineResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public MedicineMaster MedicineMaster { get; set; }
    }
    public class MedicinePriceResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> NotExistMedicine { get; set; }
        public List<string> UploadedMedicine { get; set; }
        public List<string> InsertedMedicine { get; set; }
    }
    public class UploadMedicineResponse
    {
        public bool IsSuccess { get; set; }
        public long Id { get; set; }
        public string ErrorMessage { get; set; }
    }
}
