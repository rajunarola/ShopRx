namespace RxFair.Dto.Dtos
{
    public class DashboardDto
    {
        public DashboardUserDto UserDto { get; set; }
        public DashboardOrderDto OrderDto { get; set; }
    }
    public class DashboardUserDto
    {
        public int ActiveDistributors { get; set; }
        public int NewDistributorRequest { get; set; }
        public int ActivePharmacies { get; set; }
        public int NewPharmacyRequests { get; set; }
    }
    public class DashboardOrderDto
    {
        public int TotalOrder { get; set; }
        public int Pending { get; set; }
        public int Confirmed { get; set; }
        public int Shipped { get; set; }
        public int Delivered { get; set; }
        public int Cancelled { get; set; }
        public int Returned { get; set; }
        public int PartialReturned { get; set; }
    }

    public class PharmacyDistributorDashboardDto
    {
        public PharmacyDistributorUserDashboardDto UserDto { get; set; }
        public DashboardOrderDto OrderDto { get; set; }
    }
    public class PharmacyDistributorUserDashboardDto
    {
        public int TotalUsers { get; set; }
        public int AdminActive { get; set; }
        public int AdminInactive { get; set; }
        public int StaffActive { get; set; }
        public int StaffInactive { get; set; }
    }
}
