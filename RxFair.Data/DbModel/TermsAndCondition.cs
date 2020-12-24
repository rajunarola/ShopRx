using System.ComponentModel.DataAnnotations;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class TermsAndCondition : EntityWithAudit
    {
        [Required]
        public string TermsCondition { get; set; }
    }
}
