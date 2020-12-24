using System.ComponentModel.DataAnnotations;

namespace RxFair.Dto.Dtos
{
    public class TermsAndConditionDto : BaseModel
    {
        [Required(ErrorMessage = "Please enter Terms & Condition")]
        public string TermsCondition { get; set; }
    }
}
