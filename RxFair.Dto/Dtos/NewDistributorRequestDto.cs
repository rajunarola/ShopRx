using System;

namespace RxFair.Dto.Dtos
{
    public class NewDistributorRequestDto : BaseModel
    {
        public string CompanyName { get; set; }

        public string Mobile { get; set; }
        public string Email { get; set; }

        public string Phone { get; set; }

        public string ZipCode { get; set; }

        public string Address { get; set; }

        public long StateId { get; set; }

        public string StateName { get; set; }

        public string City { get; set; }

        //ContactPersonInfo

        public string ContactName { get; set; }

        public string ContactEmail { get; set; }

        public string ContactMobile { get; set; }

        public string ContactAddress { get; set; }

        public string ContactCity { get; set; }

        public long ContactStateId { get; set; }

        public string ContactStateName { get; set; }

        public string ContactZipCode { get; set; }

    }
}
