using Microsoft.AspNetCore.Http;

namespace RxFair.Dto.Dtos
{
    public class UserProfileDto : BaseModel
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string City { get; set; }

        public long? StateId { get; set; }

        public string StateName { get; set; }

        public string ZipCode { get; set; }

        public string JobTitle { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public long? DistributorId { get; set; }

        public long? PharmacyId { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }

        public IFormFile ProfileImage { get; set; }

        public string UserProfileImage { get; set; }


    }
}
