using System;
using System.Collections.Generic;
using RxFair.Models;
using RxFair.Service.Interface;
using RxFair.Utility.Common;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Utility.Extension;

namespace RxFair.Utility.ExcelUtility
{
    public interface IExcelReports
    {
        Task<byte[]> AdminPharmacyPurchaseOrder(long timeZoneId, long pharmacyId, string startDate, string endDate, string search);
        Task<byte[]> AdminDistributorSalesOrder(long timeZoneId, long distributorId, string startDate, string endDate, string search);
        Task<byte[]> PharmacyOrderSummary(long timeZoneId, string orderId, string startDate, string endDate, string search,long PharmacyId);
        Task<byte[]> UnshippedOrders(long timeZoneId, string startDate, string endDate, string search);
        Task<byte[]> SalesOrderReport(long timeZoneId, long distributorId, string startDate, string endDate, string search);
        Task<byte[]> ReviewMedicinePrice(long timeZoneId, List<string> distributorList, string search);
    }

    public class ExcelReports : IExcelReports
    {
        private readonly IReportService _report;
        public ExcelReports(IReportService report)
        {
            _report = report;
        }
        
        public async Task<byte[]> AdminPharmacyPurchaseOrder(long timeZoneId, long pharmacyId, string startDate, string endDate, string search)
        {
            var jQueryDataTableParam = new JQueryDataTableParamModel
            {
                sSearch = search ?? ""
            };
            var ms = new MemoryStream();
            if (!string.IsNullOrEmpty(startDate))
                startDate = Convert.ToDateTime(startDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(endDate))
                endDate = Convert.ToDateTime(endDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            var parameters = CommonMethod.GetJQueryDatatableParamList(jQueryDataTableParam, timeZoneId, "id").Parameters;
            parameters.Insert(1, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = true });
            parameters.Insert(2, new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = pharmacyId });
            parameters.Insert(3, new SqlParameter("@StartDate", SqlDbType.VarChar) { Value = startDate ?? "" });
            parameters.Insert(4, new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = endDate ?? "" });

            var reportData = await _report.GetPharmacyPurchaseOrder(parameters.ToArray());

            using (ExcelPackage excelData = new ExcelPackage(ms))
            {
                ExcelWorksheet worksheet = excelData.Workbook.Worksheets.Add("Pharmacy Purchase Order");
                worksheet.Cells[1, 4, 2, 7].Merge = true;
                worksheet.Cells[1, 4].Value = "Pharmacy Purchase Order";
                worksheet.Cells[1, 4].Style.Font.SetFromFont(new Font("Arial", 20));
                worksheet.Cells[1, 4].Style.Font.Bold = true; //Font should be bold
                worksheet.Cells[1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Alignment is center

                //Here we have need to highlight 3 row with 10 colums
                var headerCells = worksheet.Cells[3, 1, 3, 10];
                var headerFont = headerCells.Style.Font;
                headerFont.SetFromFont(new Font("Arial", 12));
                headerFont.Bold = true;
                headerFont.Color.SetColor(Color.White);
                var headerFill = headerCells.Style.Fill;
                headerFill.PatternType = ExcelFillStyle.Solid;
                headerFill.BackgroundColor.SetColor(Color.Gray);

                worksheet.Cells[3, 1].Value = "#";
                worksheet.Cells[3, 2].Value = "Order ID";
                worksheet.Cells[3, 3].Value = "Date Created";
                worksheet.Cells[3, 4].Value = "Pharmacy Name";
                worksheet.Cells[3, 5].Value = "Distributor";
                worksheet.Cells[3, 6].Value = "Order Status";
                worksheet.Cells[3, 7].Value = "Delivery Status";
                worksheet.Cells[3, 8].Value = "Shipping Total";
                worksheet.Cells[3, 9].Value = "Sub Total";
                worksheet.Cells[3, 10].Value = "Grand Total";

                int row = 3, i = 0;                foreach (var item in reportData)                {                    row++; i++;
                    worksheet.Cells[row, 1].Value = i;
                    worksheet.Cells[row, 2].Value = item.UniqueOrder;
                    worksheet.Cells[row, 3].Value = item.DateCreated;
                    worksheet.Cells[row, 4].Value = item.PharmacyName;
                    worksheet.Cells[row, 5].Value = item.Distributor;
                    worksheet.Cells[row, 6].Value = item.OrderStatus;
                    worksheet.Cells[row, 7].Value = item.DeliveryStatus ? "Yes" : "No";
                    worksheet.Cells[row, 8].Value = item.ShippingTotal;
                    worksheet.Cells[row, 9].Value = item.SubTotal;
                    worksheet.Cells[row, 10].Value = item.GrandTotal;

                    var innerQueryparameters = CommonMethod.GetJQueryDatatableParamList(jQueryDataTableParam, timeZoneId, "Id").Parameters;
                    innerQueryparameters.Insert(1, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = true });
                    innerQueryparameters.Insert(2, new SqlParameter("@OrderChargeId", SqlDbType.BigInt) { Value = item.Id });
                    innerQueryparameters.Insert(3, new SqlParameter("@StartDate", SqlDbType.VarChar) { Value = startDate ?? "" });
                    innerQueryparameters.Insert(4, new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = endDate ?? "" });

                    var innderList = await _report.GetPharmacyPurchaseDistributorOrder(innerQueryparameters.ToArray());

                    if (!innderList.Any()) continue;
                    var innerRow = row + 1;

                    #region Inner Header
                    var innerHeaderCells = worksheet.Cells[innerRow, 3, innerRow, 10];
                    var innerHeaderFont = innerHeaderCells.Style.Font;
                    innerHeaderFont.SetFromFont(new Font("Arial", 12));
                    innerHeaderFont.Bold = true;
                    innerHeaderFont.Color.SetColor(Color.White);
                    var innerHeaderFill = innerHeaderCells.Style.Fill;
                    innerHeaderFill.PatternType = ExcelFillStyle.Solid;

                    innerHeaderFill.BackgroundColor.SetColor(Color.Gray);

                    worksheet.Cells[innerRow, 3].Value = "Ndc";
                    worksheet.Cells[innerRow, 4].Value = "Medicine Name";
                    worksheet.Cells[innerRow, 5].Value = "Category";
                    worksheet.Cells[innerRow, 6].Value = "Package Size";
                    worksheet.Cells[innerRow, 7].Value = "Package Quantity";
                    worksheet.Cells[innerRow, 8].Value = "Quantity";
                    worksheet.Cells[innerRow, 9].Value = "Price";
                    worksheet.Cells[innerRow, 10].Value = "Total Price";
                    #endregion
                    foreach (var innerItem in innderList)
                    {
                        innerRow++;
                        worksheet.Cells[innerRow, 3].Value = innerItem.Ndc;
                        worksheet.Cells[innerRow, 4].Value = innerItem.MedicineName;
                        worksheet.Cells[innerRow, 5].Value = innerItem.Category;
                        worksheet.Cells[innerRow, 6].Value = innerItem.PackageSize;
                        worksheet.Cells[innerRow, 7].Value = innerItem.PackageQuantity;
                        worksheet.Cells[innerRow, 8].Value = innerItem.Quantity;
                        worksheet.Cells[innerRow, 9].Value = innerItem.Price;
                        worksheet.Cells[innerRow, 10].Value = innerItem.TotalPrice;
                    }
                    row = innerRow;
                }
                #region Autofit
                //Make all text fit the cells
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                #endregion
                excelData.Save();
            }
            return ms.ToArray();
        }

        public async Task<byte[]> AdminDistributorSalesOrder(long timeZoneId, long distributorId, string startDate, string endDate, string search)
        {
            var jQueryDataTableParam = new JQueryDataTableParamModel
            {
                sSearch = search ?? ""
            };
            var ms = new MemoryStream();
            if (!string.IsNullOrEmpty(startDate))
                startDate = Convert.ToDateTime(startDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(endDate))
                endDate = Convert.ToDateTime(endDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            var parameters = CommonMethod.GetJQueryDatatableParamList(jQueryDataTableParam, timeZoneId, "id").Parameters;
            parameters.Insert(1, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = true });
            parameters.Insert(2, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = distributorId });
            parameters.Insert(3, new SqlParameter("@StartDate", SqlDbType.VarChar) { Value = startDate ?? "" });
            parameters.Insert(4, new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = endDate ?? "" });

            var reportData = await _report.GetDistributorSalesOrder(parameters.ToArray());

            using (ExcelPackage excelData = new ExcelPackage(ms))
            {
                ExcelWorksheet worksheet = excelData.Workbook.Worksheets.Add("Distributor Sales Order");
                worksheet.Cells[1, 4, 2, 7].Merge = true;
                worksheet.Cells[1, 4].Value = "Distributor Sales Order";
                worksheet.Cells[1, 4].Style.Font.SetFromFont(new Font("Arial", 20));
                worksheet.Cells[1, 4].Style.Font.Bold = true; //Font should be bold
                worksheet.Cells[1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Alignment is center

                //Here we have need to highlight 3 row with 10 colums
                var headerCells = worksheet.Cells[3, 1, 3, 10];
                var headerFont = headerCells.Style.Font;
                headerFont.SetFromFont(new Font("Arial", 12));
                headerFont.Bold = true;
                headerFont.Color.SetColor(Color.White);
                var headerFill = headerCells.Style.Fill;
                headerFill.PatternType = ExcelFillStyle.Solid;
                headerFill.BackgroundColor.SetColor(Color.Gray);

                worksheet.Cells[3, 1].Value = "#";
                worksheet.Cells[3, 2].Value = "Order ID";
                worksheet.Cells[3, 3].Value = "Date Created";
                worksheet.Cells[3, 4].Value = "Pharmacy Name";
                worksheet.Cells[3, 5].Value = "Distributor";
                worksheet.Cells[3, 6].Value = "Order Status";
                worksheet.Cells[3, 7].Value = "Delivery Status";
                worksheet.Cells[3, 8].Value = "Shipping Total";
                worksheet.Cells[3, 9].Value = "Sub Total";
                worksheet.Cells[3, 10].Value = "Grand Total";

                int row = 3, i = 0;                foreach (var item in reportData)                {                    row++; i++;
                    worksheet.Cells[row, 1].Value = i;
                    worksheet.Cells[row, 2].Value = item.UniqueOrder;
                    worksheet.Cells[row, 3].Value = item.DateCreated;
                    worksheet.Cells[row, 4].Value = item.PharmacyName;
                    worksheet.Cells[row, 5].Value = item.Distributor;
                    worksheet.Cells[row, 6].Value = item.OrderStatus;
                    worksheet.Cells[row, 7].Value = item.DeliveryStatus ? "Yes" : "No";
                    worksheet.Cells[row, 8].Value = item.ShippingTotal;
                    worksheet.Cells[row, 9].Value = item.SubTotal;
                    worksheet.Cells[row, 10].Value = item.GrandTotal;

                    var innerQueryparameters = CommonMethod.GetJQueryDatatableParamList(jQueryDataTableParam, timeZoneId, "Id").Parameters;
                    innerQueryparameters.Insert(1, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = true });
                    innerQueryparameters.Insert(2, new SqlParameter("@OrderChargeId", SqlDbType.BigInt) { Value = item.Id });
                    innerQueryparameters.Insert(3, new SqlParameter("@StartDate", SqlDbType.VarChar) { Value = startDate ?? "" });
                    innerQueryparameters.Insert(4, new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = endDate ?? "" });

                    var innderList = await _report.GetPharmacyPurchaseDistributorOrder(innerQueryparameters.ToArray());

                    if (!innderList.Any()) continue;
                    var innerRow = row + 1;

                    #region Inner Header
                    var innerHeaderCells = worksheet.Cells[innerRow, 3, innerRow, 10];
                    var innerHeaderFont = innerHeaderCells.Style.Font;
                    innerHeaderFont.SetFromFont(new Font("Arial", 12));
                    innerHeaderFont.Bold = true;
                    innerHeaderFont.Color.SetColor(Color.White);
                    var innerHeaderFill = innerHeaderCells.Style.Fill;
                    innerHeaderFill.PatternType = ExcelFillStyle.Solid;

                    innerHeaderFill.BackgroundColor.SetColor(Color.Gray);

                    worksheet.Cells[innerRow, 3].Value = "Ndc";
                    worksheet.Cells[innerRow, 4].Value = "Medicine Name";
                    worksheet.Cells[innerRow, 5].Value = "Category";
                    worksheet.Cells[innerRow, 6].Value = "Package Size";
                    worksheet.Cells[innerRow, 7].Value = "Package Quantity";
                    worksheet.Cells[innerRow, 8].Value = "Quantity";
                    worksheet.Cells[innerRow, 9].Value = "Price";
                    worksheet.Cells[innerRow, 10].Value = "Total Price";
                    #endregion
                    foreach (var innerItem in innderList)
                    {
                        innerRow++;
                        worksheet.Cells[innerRow, 3].Value = innerItem.Ndc;
                        worksheet.Cells[innerRow, 4].Value = innerItem.MedicineName;
                        worksheet.Cells[innerRow, 5].Value = innerItem.Category;
                        worksheet.Cells[innerRow, 6].Value = innerItem.PackageSize;
                        worksheet.Cells[innerRow, 7].Value = innerItem.PackageQuantity;
                        worksheet.Cells[innerRow, 8].Value = innerItem.Quantity;
                        worksheet.Cells[innerRow, 9].Value = innerItem.Price;
                        worksheet.Cells[innerRow, 10].Value = innerItem.TotalPrice;
                    }
                    row = innerRow;
                }
                #region Autofit
                //Make all text fit the cells
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                #endregion
                excelData.Save();
            }
            return ms.ToArray();
        }

        public async Task<byte[]> PharmacyOrderSummary(long timeZoneId, string orderId, string startDate, string endDate,string search,long PharmacyId)
        {

            
            var jQueryDataTableParam = new JQueryDataTableParamModel
            {
                sSearch = search ?? ""
            };
            var ms = new MemoryStream();
            if (!string.IsNullOrEmpty(startDate))
                startDate = Convert.ToDateTime(startDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(endDate))
                endDate = Convert.ToDateTime(endDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            var parameters = CommonMethod.GetJQueryDatatableParamList(jQueryDataTableParam, timeZoneId, "id").Parameters;
            parameters.Insert(1, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = true });
            parameters.Insert(2, new SqlParameter("@OrderId", SqlDbType.VarChar) { Value = orderId ?? ""});
            parameters.Insert(3, new SqlParameter("@StartDate", SqlDbType.VarChar) { Value = startDate ?? "" });
            parameters.Insert(4, new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = endDate ?? "" });
            parameters.Insert(5, new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = PharmacyId });

            var reportData = await _report.GetPharmacyOrderSummary(parameters.ToArray());

            using (ExcelPackage excelData = new ExcelPackage(ms))
            {
                ExcelWorksheet worksheet = excelData.Workbook.Worksheets.Add("Pharmacy Order Summary");
                worksheet.Cells[1, 4, 2, 7].Merge = true;
                worksheet.Cells[1, 4].Value = "Pharmacy Order Summary";
                worksheet.Cells[1, 4].Style.Font.SetFromFont(new Font("Arial", 20));
                worksheet.Cells[1, 4].Style.Font.Bold = true; //Font should be bold
                worksheet.Cells[1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Alignment is center

                //Here we have need to highlight 3 row with 10 colums
                var headerCells = worksheet.Cells[3, 1, 3, 10];
                var headerFont = headerCells.Style.Font;
                headerFont.SetFromFont(new Font("Arial", 12));
                headerFont.Bold = true;
                headerFont.Color.SetColor(Color.White);
                var headerFill = headerCells.Style.Fill;
                headerFill.PatternType = ExcelFillStyle.Solid;
                headerFill.BackgroundColor.SetColor(Color.Gray);

                worksheet.Cells[3, 1].Value = "#";
                worksheet.Cells[3, 2].Value = "Order ID";
                worksheet.Cells[3, 3].Value = "Date Created";
                worksheet.Cells[3, 4].Value = "Pharmacy Name";
                worksheet.Cells[3, 5].Value = "Distributor";
                worksheet.Cells[3, 6].Value = "Order Status";
                worksheet.Cells[3, 7].Value = "Delivery Status";
                worksheet.Cells[3, 8].Value = "Shipping Total";
                worksheet.Cells[3, 9].Value = "Sub Total";
                worksheet.Cells[3, 10].Value = "Grand Total";
                
                int row = 3, i = 0;                foreach (var item in reportData)                {                    row++; i++;
                    worksheet.Cells[row, 1].Value = i;
                    worksheet.Cells[row, 2].Value = item.UniqueOrder;
                    worksheet.Cells[row, 3].Value = item.DateCreated;
                    worksheet.Cells[row, 4].Value = item.PharmacyName;
                    worksheet.Cells[row, 5].Value = item.Distributor;
                    worksheet.Cells[row, 6].Value = item.OrderStatus;
                    worksheet.Cells[row, 7].Value = item.DeliveryStatus ? "Yes" : "No";
                    worksheet.Cells[row, 8].Value = item.ShippingTotal;
                    worksheet.Cells[row, 9].Value = item.SubTotal;
                    worksheet.Cells[row, 10].Value = item.GrandTotal;

                    var innerQueryparameters = CommonMethod.GetJQueryDatatableParamList(jQueryDataTableParam, timeZoneId, "Id").Parameters;
                    innerQueryparameters.Insert(1, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = true });
                    innerQueryparameters.Insert(2, new SqlParameter("@OrderChargeId", SqlDbType.BigInt) { Value = item.Id });
                    innerQueryparameters.Insert(3, new SqlParameter("@StartDate", SqlDbType.VarChar) { Value = startDate ?? "" });
                    innerQueryparameters.Insert(4, new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = endDate ?? "" });

                    var innderList = await _report.GetPharmacyPurchaseDistributorOrder(innerQueryparameters.ToArray());

                    if (!innderList.Any()) continue;
                    var innerRow = row + 1;

                    #region Inner Header
                    var innerHeaderCells = worksheet.Cells[innerRow, 3, innerRow, 10];
                    var innerHeaderFont = innerHeaderCells.Style.Font;
                    innerHeaderFont.SetFromFont(new Font("Arial", 12));
                    innerHeaderFont.Bold = true;
                    innerHeaderFont.Color.SetColor(Color.White);
                    var innerHeaderFill = innerHeaderCells.Style.Fill;
                    innerHeaderFill.PatternType = ExcelFillStyle.Solid;

                    innerHeaderFill.BackgroundColor.SetColor(Color.Gray);

                    worksheet.Cells[innerRow, 3].Value = "Ndc";
                    worksheet.Cells[innerRow, 4].Value = "Medicine Name";
                    worksheet.Cells[innerRow, 5].Value = "Category";
                    worksheet.Cells[innerRow, 6].Value = "Package Size";
                    worksheet.Cells[innerRow, 7].Value = "Package Quantity";
                    worksheet.Cells[innerRow, 8].Value = "Quantity";
                    worksheet.Cells[innerRow, 9].Value = "Price";
                    worksheet.Cells[innerRow, 10].Value = "Total Price";
                    #endregion
                    foreach (var innerItem in innderList)
                    {
                        innerRow++;
                        worksheet.Cells[innerRow, 3].Value = innerItem.Ndc;
                        worksheet.Cells[innerRow, 4].Value = innerItem.MedicineName;
                        worksheet.Cells[innerRow, 5].Value = innerItem.Category;
                        worksheet.Cells[innerRow, 6].Value = innerItem.PackageSize;
                        worksheet.Cells[innerRow, 7].Value = innerItem.PackageQuantity;
                        worksheet.Cells[innerRow, 8].Value = innerItem.Quantity;
                        worksheet.Cells[innerRow, 9].Value = innerItem.Price;
                        worksheet.Cells[innerRow, 10].Value = innerItem.TotalPrice;
                    }
                    row = innerRow;
                }

                #region Autofit
                //Make all text fit the cells
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                #endregion
                excelData.Save();
            }
            return ms.ToArray();
        }

        public async Task<byte[]> UnshippedOrders(long timeZoneId, string startDate, string endDate, string search)
        {
            var jQueryDataTableParam = new JQueryDataTableParamModel
            {
                sSearch = search ?? ""
            };
            var ms = new MemoryStream();
            if (!string.IsNullOrEmpty(startDate))
                startDate = Convert.ToDateTime(startDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(endDate))
                endDate = Convert.ToDateTime(endDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            var parameters = CommonMethod.GetJQueryDatatableParamList(jQueryDataTableParam, timeZoneId, "id").Parameters;
            parameters.Insert(1, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = true });
            parameters.Insert(2, new SqlParameter("@StartDate", SqlDbType.VarChar) { Value = startDate ?? "" });
            parameters.Insert(3, new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = endDate ?? "" });

            var reportData = await _report.GetUnshippedOrders(parameters.ToArray());

            using (ExcelPackage excelData = new ExcelPackage(ms))
            {
                ExcelWorksheet worksheet = excelData.Workbook.Worksheets.Add("Unshipped Orders");
                worksheet.Cells[1, 4, 2, 7].Merge = true;
                worksheet.Cells[1, 4].Value = "Unshipped Orders";
                worksheet.Cells[1, 4].Style.Font.SetFromFont(new Font("Arial", 20));
                worksheet.Cells[1, 4].Style.Font.Bold = true; //Font should be bold
                worksheet.Cells[1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Alignment is center

                //Here we have need to highlight 3 row with 10 colums
                var headerCells = worksheet.Cells[3, 1, 3, 10];
                var headerFont = headerCells.Style.Font;
                headerFont.SetFromFont(new Font("Arial", 12));
                headerFont.Bold = true;
                headerFont.Color.SetColor(Color.White);
                var headerFill = headerCells.Style.Fill;
                headerFill.PatternType = ExcelFillStyle.Solid;
                headerFill.BackgroundColor.SetColor(Color.Gray);

                worksheet.Cells[3, 1].Value = "#";
                worksheet.Cells[3, 2].Value = "Order ID";
                worksheet.Cells[3, 3].Value = "Date Created";
                worksheet.Cells[3, 4].Value = "Pharmacy Name";
                worksheet.Cells[3, 5].Value = "Distributor";
                worksheet.Cells[3, 6].Value = "Order Status";
                worksheet.Cells[3, 7].Value = "Delivery Status";
                worksheet.Cells[3, 8].Value = "Shipping Total";
                worksheet.Cells[3, 9].Value = "Sub Total";
                worksheet.Cells[3, 10].Value = "Grand Total";

                int row = 3, i = 0;                foreach (var item in reportData)                {                    row++; i++;
                    worksheet.Cells[row, 1].Value = i;
                    worksheet.Cells[row, 2].Value = item.UniqueOrder;
                    worksheet.Cells[row, 3].Value = item.DateCreated;
                    worksheet.Cells[row, 4].Value = item.PharmacyName;
                    worksheet.Cells[row, 5].Value = item.Distributor;
                    worksheet.Cells[row, 6].Value = item.OrderStatus;
                    worksheet.Cells[row, 7].Value = item.DeliveryStatus ? "Yes" : "No";
                    worksheet.Cells[row, 8].Value = item.ShippingTotal;
                    worksheet.Cells[row, 9].Value = item.SubTotal;
                    worksheet.Cells[row, 10].Value = item.GrandTotal;

                    var innerQueryparameters = CommonMethod.GetJQueryDatatableParamList(jQueryDataTableParam, timeZoneId, "Id").Parameters;
                    innerQueryparameters.Insert(1, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = true });
                    innerQueryparameters.Insert(2, new SqlParameter("@OrderChargeId", SqlDbType.BigInt) { Value = item.Id });
                    innerQueryparameters.Insert(3, new SqlParameter("@StartDate", SqlDbType.VarChar) { Value = startDate ?? "" });
                    innerQueryparameters.Insert(4, new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = endDate ?? "" });

                    var innderList = await _report.GetPharmacyPurchaseDistributorOrder(innerQueryparameters.ToArray());

                    if (!innderList.Any()) continue;
                    var innerRow = row + 1;

                    #region Inner Header
                    var innerHeaderCells = worksheet.Cells[innerRow, 3, innerRow, 10];
                    var innerHeaderFont = innerHeaderCells.Style.Font;
                    innerHeaderFont.SetFromFont(new Font("Arial", 12));
                    innerHeaderFont.Bold = true;
                    innerHeaderFont.Color.SetColor(Color.White);
                    var innerHeaderFill = innerHeaderCells.Style.Fill;
                    innerHeaderFill.PatternType = ExcelFillStyle.Solid;

                    innerHeaderFill.BackgroundColor.SetColor(Color.Gray);

                    worksheet.Cells[innerRow, 3].Value = "Ndc";
                    worksheet.Cells[innerRow, 4].Value = "Medicine Name";
                    worksheet.Cells[innerRow, 5].Value = "Category";
                    worksheet.Cells[innerRow, 6].Value = "Package Size";
                    worksheet.Cells[innerRow, 7].Value = "Package Quantity";
                    worksheet.Cells[innerRow, 8].Value = "Quantity";
                    worksheet.Cells[innerRow, 9].Value = "Price";
                    worksheet.Cells[innerRow, 10].Value = "Total Price";
                    #endregion
                    foreach (var innerItem in innderList)
                    {
                        innerRow++;
                        worksheet.Cells[innerRow, 3].Value = innerItem.Ndc;
                        worksheet.Cells[innerRow, 4].Value = innerItem.MedicineName;
                        worksheet.Cells[innerRow, 5].Value = innerItem.Category;
                        worksheet.Cells[innerRow, 6].Value = innerItem.PackageSize;
                        worksheet.Cells[innerRow, 7].Value = innerItem.PackageQuantity;
                        worksheet.Cells[innerRow, 8].Value = innerItem.Quantity;
                        worksheet.Cells[innerRow, 9].Value = innerItem.Price;
                        worksheet.Cells[innerRow, 10].Value = innerItem.TotalPrice;
                    }
                    row = innerRow;
                }
                #region Autofit
                //Make all text fit the cells
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                #endregion
                excelData.Save();
            }
            return ms.ToArray();
        }

        public async Task<byte[]> SalesOrderReport(long timeZoneId, long distributorId, string startDate, string endDate, string search)
        {
            var jQueryDataTableParam = new JQueryDataTableParamModel
            {
                sSearch = search ?? ""
            };
            var ms = new MemoryStream();
            if (!string.IsNullOrEmpty(startDate))
                startDate = Convert.ToDateTime(startDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(endDate))
                endDate = Convert.ToDateTime(endDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            var parameters = CommonMethod.GetJQueryDatatableParamList(jQueryDataTableParam, timeZoneId, "id").Parameters;
            parameters.Insert(1, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = true });
            parameters.Insert(2, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = distributorId });
            parameters.Insert(3, new SqlParameter("@StartDate", SqlDbType.VarChar) { Value = startDate ?? "" });
            parameters.Insert(4, new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = endDate ?? "" });

            var reportData = await _report.GetSalesOrderReport(parameters.ToArray());

            using (ExcelPackage excelData = new ExcelPackage(ms))
            {
                ExcelWorksheet worksheet = excelData.Workbook.Worksheets.Add("Sales Order Report");
                worksheet.Cells[1, 4, 2, 7].Merge = true;
                worksheet.Cells[1, 4].Value = "Sales Order Report";
                worksheet.Cells[1, 4].Style.Font.SetFromFont(new Font("Arial", 20));
                worksheet.Cells[1, 4].Style.Font.Bold = true; //Font should be bold
                worksheet.Cells[1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Alignment is center

                //Here we have need to highlight 3 row with 10 colums
                var headerCells = worksheet.Cells[3, 1, 3, 10];
                var headerFont = headerCells.Style.Font;
                headerFont.SetFromFont(new Font("Arial", 12));
                headerFont.Bold = true;
                headerFont.Color.SetColor(Color.White);
                var headerFill = headerCells.Style.Fill;
                headerFill.PatternType = ExcelFillStyle.Solid;
                headerFill.BackgroundColor.SetColor(Color.Gray);

                worksheet.Cells[3, 1].Value = "#";
                worksheet.Cells[3, 2].Value = "Order ID";
                worksheet.Cells[3, 3].Value = "Date Created";
                worksheet.Cells[3, 4].Value = "Pharmacy Name";
                worksheet.Cells[3, 5].Value = "Distributor";
                worksheet.Cells[3, 6].Value = "Order Status";
                worksheet.Cells[3, 7].Value = "Delivery Status";
                worksheet.Cells[3, 8].Value = "Shipping Total";
                worksheet.Cells[3, 9].Value = "Sub Total";
                worksheet.Cells[3, 10].Value = "Grand Total";

                int row = 3, i = 0;                foreach (var item in reportData)                {                    row++; i++;
                    worksheet.Cells[row, 1].Value = i;
                    worksheet.Cells[row, 2].Value = item.UniqueOrder;
                    worksheet.Cells[row, 3].Value = item.DateCreated;
                    worksheet.Cells[row, 4].Value = item.PharmacyName;
                    worksheet.Cells[row, 5].Value = item.Distributor;
                    worksheet.Cells[row, 6].Value = item.OrderStatus;
                    worksheet.Cells[row, 7].Value = item.DeliveryStatus ? "Yes" : "No";
                    worksheet.Cells[row, 8].Value = item.ShippingTotal;
                    worksheet.Cells[row, 9].Value = item.SubTotal;
                    worksheet.Cells[row, 10].Value = item.GrandTotal;

                    var innerQueryparameters = CommonMethod.GetJQueryDatatableParamList(jQueryDataTableParam, timeZoneId, "Id").Parameters;
                    innerQueryparameters.Insert(1, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = true });
                    innerQueryparameters.Insert(2, new SqlParameter("@OrderChargeId", SqlDbType.BigInt) { Value = item.Id });
                    innerQueryparameters.Insert(3, new SqlParameter("@StartDate", SqlDbType.VarChar) { Value = startDate ?? "" });
                    innerQueryparameters.Insert(4, new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = endDate ?? "" });

                    var innderList = await _report.GetPharmacyPurchaseDistributorOrder(innerQueryparameters.ToArray());

                    if (!innderList.Any()) continue;
                    var innerRow = row + 1;

                    #region Inner Header
                    var innerHeaderCells = worksheet.Cells[innerRow, 3, innerRow, 10];
                    var innerHeaderFont = innerHeaderCells.Style.Font;
                    innerHeaderFont.SetFromFont(new Font("Arial", 12));
                    innerHeaderFont.Bold = true;
                    innerHeaderFont.Color.SetColor(Color.White);
                    var innerHeaderFill = innerHeaderCells.Style.Fill;
                    innerHeaderFill.PatternType = ExcelFillStyle.Solid;

                    innerHeaderFill.BackgroundColor.SetColor(Color.Gray);

                    worksheet.Cells[innerRow, 3].Value = "Ndc";
                    worksheet.Cells[innerRow, 4].Value = "Medicine Name";
                    worksheet.Cells[innerRow, 5].Value = "Category";
                    worksheet.Cells[innerRow, 6].Value = "Package Size";
                    worksheet.Cells[innerRow, 7].Value = "Package Quantity";
                    worksheet.Cells[innerRow, 8].Value = "Quantity";
                    worksheet.Cells[innerRow, 9].Value = "Price";
                    worksheet.Cells[innerRow, 10].Value = "Total Price";
                    #endregion
                    foreach (var innerItem in innderList)
                    {
                        innerRow++;
                        worksheet.Cells[innerRow, 3].Value = innerItem.Ndc;
                        worksheet.Cells[innerRow, 4].Value = innerItem.MedicineName;
                        worksheet.Cells[innerRow, 5].Value = innerItem.Category;
                        worksheet.Cells[innerRow, 6].Value = innerItem.PackageSize;
                        worksheet.Cells[innerRow, 7].Value = innerItem.PackageQuantity;
                        worksheet.Cells[innerRow, 8].Value = innerItem.Quantity;
                        worksheet.Cells[innerRow, 9].Value = innerItem.Price;
                        worksheet.Cells[innerRow, 10].Value = innerItem.TotalPrice;
                    }
                    row = innerRow;
                }
                #region Autofit
                //Make all text fit the cells
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                #endregion
                excelData.Save();
            }
            return ms.ToArray();
        }

        public async Task<byte[]> ReviewMedicinePrice(long timeZoneId, List<string> distributorList, string search)
        {
            ReportDto model = new ReportDto();
            distributorList = distributorList ?? new List<string>();
            model.Distributor = string.Join(",", distributorList ?? new List<string>());
            var jQueryDataTableParam = new JQueryDataTableParamModel
            {
                sSearch = search ?? ""
            };
            var ms = new MemoryStream();
            var parameters = CommonMethod.GetJQueryDatatableParamList(jQueryDataTableParam, timeZoneId, "id").Parameters;
            parameters.Insert(1, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = true });
            parameters.Insert(2, new SqlParameter("@DistributorId", SqlDbType.VarChar) { Value = model.Distributor ?? "" });

            var reportData = await _report.GetReviewMedicinePrice(parameters.ToArray());

            using (ExcelPackage excelData = new ExcelPackage(ms))
            {
                ExcelWorksheet worksheet = excelData.Workbook.Worksheets.Add("Review Medicine Price");
                worksheet.Cells[1, 3, 2, 5].Merge = true;
                worksheet.Cells[1, 3].Value = "Review Medicine Price";
                worksheet.Cells[1, 3].Style.Font.SetFromFont(new Font("Arial", 20));
                worksheet.Cells[1, 3].Style.Font.Bold = true; //Font should be bold
                worksheet.Cells[1, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Alignment is center

                //Here we have need to highlight 3 row with 10 colums
                var headerCells = worksheet.Cells[3, 1, 3, 7];
                var headerFont = headerCells.Style.Font;
                headerFont.SetFromFont(new Font("Arial", 12));
                headerFont.Bold = true;
                headerFont.Color.SetColor(Color.White);
                var headerFill = headerCells.Style.Fill;
                headerFill.PatternType = ExcelFillStyle.Solid;
                headerFill.BackgroundColor.SetColor(Color.Gray);

                worksheet.Cells[3, 1].Value = "#";
                worksheet.Cells[3, 2].Value = "NDC";
                worksheet.Cells[3, 3].Value = "Medicine Name";
                worksheet.Cells[3, 4].Value = "Strength";
                worksheet.Cells[3, 5].Value = "Dosage";
                worksheet.Cells[3, 6].Value = "Package Size";
                worksheet.Cells[3, 7].Value = "Manufacturer";
                //worksheet.Cells[3, 8].Value = "Shipping Total";
                //worksheet.Cells[3, 9].Value = "Sub Total";
                //worksheet.Cells[3, 10].Value = "Grand Total";

                int row = 3, i = 0;                foreach (var item in reportData)                {                    row++; i++;
                    worksheet.Cells[row, 1].Value = i;
                    worksheet.Cells[row, 2].Value = item.Ndc;
                    worksheet.Cells[row, 3].Value = item.MedicineName;
                    worksheet.Cells[row, 4].Value = item.Strength;
                    worksheet.Cells[row, 5].Value = item.Dosage;
                    worksheet.Cells[row, 6].Value = item.PackageSize;
                    worksheet.Cells[row, 7].Value = item.Manufacturer;
                    //worksheet.Cells[row, 8].Value = item.ShippingTotal;
                    //worksheet.Cells[row, 9].Value = item.SubTotal;
                    //worksheet.Cells[row, 10].Value = item.GrandTotal;

                    jQueryDataTableParam.sSearch = "";

                    var innerQueryparameters = CommonMethod.GetJQueryDatatableParamList(jQueryDataTableParam, timeZoneId, "Id").Parameters;
                    innerQueryparameters.Insert(1, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = true });
                    innerQueryparameters.Insert(2, new SqlParameter("@MedicineId", SqlDbType.BigInt) { Value = item.Id });

                    var innderList = await _report.GetReviewMedicineDistributorWise(innerQueryparameters.ToArray());

                    if (!innderList.Any()) continue;
                    var innerRow = row + 1;

                    #region Inner Header
                    var innerHeaderCells = worksheet.Cells[innerRow, 3, innerRow, 4];
                    var innerHeaderFont = innerHeaderCells.Style.Font;
                    innerHeaderFont.SetFromFont(new Font("Arial", 12));
                    innerHeaderFont.Bold = true;
                    innerHeaderFont.Color.SetColor(Color.White);
                    var innerHeaderFill = innerHeaderCells.Style.Fill;
                    innerHeaderFill.PatternType = ExcelFillStyle.Solid;

                    innerHeaderFill.BackgroundColor.SetColor(Color.Gray);

                    worksheet.Cells[innerRow, 3].Value = "Distributor";
                    worksheet.Cells[innerRow, 4].Value = "Distributor Price";
                    //worksheet.Cells[innerRow, 5].Value = "Distributor Price";
                    //worksheet.Cells[innerRow, 6].Value = "Package Size";
                    //worksheet.Cells[innerRow, 7].Value = "Package Quantity";
                    //worksheet.Cells[innerRow, 8].Value = "Quantity";
                    //worksheet.Cells[innerRow, 9].Value = "Price";
                    //worksheet.Cells[innerRow, 10].Value = "Total Price";
                    #endregion
                    foreach (var innerItem in innderList)
                    {
                        innerRow++;
                        worksheet.Cells[innerRow, 3].Value = innerItem.Distributor;
                        worksheet.Cells[innerRow, 4].Value = innerItem.DistributorPrice;
                        worksheet.Cells[innerRow, 4].Style.Numberformat.Format = "#,##0.00";
                        //worksheet.Cells[innerRow, 5].Value = innerItem.Category;
                        //worksheet.Cells[innerRow, 6].Value = innerItem.PackageSize;
                        //worksheet.Cells[innerRow, 7].Value = innerItem.PackageQuantity;
                        //worksheet.Cells[innerRow, 8].Value = innerItem.Quantity;
                        //worksheet.Cells[innerRow, 9].Value = innerItem.Price;
                        //worksheet.Cells[innerRow, 10].Value = innerItem.TotalPrice;
                    }
                    row = innerRow;
                }
                #region Autofit
                //Make all text fit the cells
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                #endregion
                excelData.Save();
            }
            return ms.ToArray();
        }
    }
}
