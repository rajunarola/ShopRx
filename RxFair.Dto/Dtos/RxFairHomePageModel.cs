using System.Collections.Generic;

namespace RxFair.Dto.Dtos
{
    public class RxFairHomePageModel
    {
        public List<TestimonialDto> TestimonialDto { get; set; }
        public EmailUnSubscribe EmailUnSubscribe { get; set; }
    }

    public class EmailUnSubscribe
    {
        public string Email { get; set; }
        public string ReturnUrl { get; set; }
    }
}
