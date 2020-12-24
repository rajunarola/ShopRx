using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using static RxFair.Dto.Enum.GlobalEnums;

namespace RxFair.Dto.Dtos
{
    public class MedicineList
    {
        public int Quantity { get; set; }
        public long MedicineId { get; set; }
        public string MedicineName { get; set; }
        public decimal MedicinePrice { get; set; }

    }

    public class DistributorOrderDto : BaseModel
    {
        public string CompanyLogo { get; set; }
        public long DistributorId { get; set; }
        public string DistributorName { get; set; }
        public decimal OverNightCharge { get; set; }
        public string CuttOffTime { get; set; }
        public decimal GTotal { get; set; }
        public decimal ShippingCharges { get; set; }
        public List<MedicineList> MedicineList { get; set; }
       
    }



    public class OrderInfoDto : DistributorOrderDto
    {
        public string UniqueOrder { get; set; }
        public string InvoiceFile { get; set; }
        public IFormFile Invoice { get; set; }
        public string Distributor { get; set; }
        public string OrderStatus { get; set; }
        public string ShippingDateCreated { get; set; }
        public string Track { get; set; }
        public string TrackingLink { get; set; }
    }

    public class PharmacyOrderList : OrderInfoDto
    {
        public string PaymentStatus { get; set; }
        public string ShippingStatus { get; set; }
        public string CompanyName { get; set; }
        public string TrackingNo { get; set; }

    }

    public class OrderDto : BaseModel
    {
        public long BillingAddressId { get; set; }
        public long ShippingAddressId { get; set; }
        public List<DistributorOrderDto> DistributorOrders { get; set; }
    }

    public class ViewOrderDetailDto : MedicineDto
    {
        public string OrderBy { get; set; }

        public string Pharmacy { get; set; }

        public string Distributor { get; set; }

        public string OrderNo { get; set; }

        public decimal GrandTotal { get; set; }

        public string OrderStatus { get; set; }

        public string PaymentDate { get; set; }

        public string Category { get; set; }

        public decimal? PaymentAmount { get; set; }

        public decimal TotalPrice { get; set; }

        public string ShippingDate { get; set; }

        public decimal ShippingCost { get; set; }

        public decimal OvernightCost { get; set; }

        public string TrackingNumber { get; set; }

        public string UniqueOrder { get; set; }
        
        public string OrderDate { get; set; }

        public string TrackingLink { get; set; }

        public string PaymentNote { get; set; }
    }

    public class GetOrderDetailList : ViewOrderDetailDto
    {
        public long?  PharmacyId { get; set; }

        public short orderStatus { get; set; }
   
    }

    public class InvoiceDto : ViewOrderDetailDto
    {
        public string OrderDueDate { get; set; }
        public int MedicineCount { get; set; }
        public List<MedicineList> MedicineList { get; set; }
    }

    public class InvoicecommissionDto : BaseModel {

        public long InvoiceId { get; set; }
        public string Distributor { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string InvoiceDate { get; set; }
        public string PaymentStatus { get; set; }   
    }


}
