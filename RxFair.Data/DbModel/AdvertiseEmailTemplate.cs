using System.ComponentModel.DataAnnotations;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class AdvertiseEmailTemplate : EntityWithAudit
    {
        [Required]
        public string TemplateName { get; set; }

        [Required]
        public string Template { get; set; }
    }
}
