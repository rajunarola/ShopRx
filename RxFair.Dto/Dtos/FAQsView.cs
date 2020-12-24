using System.ComponentModel.DataAnnotations;

namespace RxFair.Dto.Dtos
{
    public class FaQsView : BaseModel
    {
        [Required(ErrorMessage = "Please enter question")]
        public string Question { get; set; }

        [Required(ErrorMessage = "Please enter answer")]
        public string Answer { get; set; }

    }
}
