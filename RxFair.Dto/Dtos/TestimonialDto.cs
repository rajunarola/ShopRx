using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RxFair.Dto.Dtos
{
    public class TestimonialDto : BaseModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Feedback { get; set; }

        public string Image { get; set; }

        public IFormFile TestimonialFile { get; set; }
    }
}
