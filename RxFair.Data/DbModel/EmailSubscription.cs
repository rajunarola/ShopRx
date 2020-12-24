using System.ComponentModel.DataAnnotations;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class EmailSubscription : EntityWithAudit
    {
        [Required, StringLength(255)]
        public string Email { get; set; }
    }
}
