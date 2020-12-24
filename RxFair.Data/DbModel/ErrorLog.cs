using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RxFair.Data.DbModel
{
    public class ErrorLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LogId { get; set; }
        public string Source { get; set; }
        public string TargetSite { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string Stack { get; set; }
        public DateTime LogDate { get; set; }
        public string Path { get; set; }

    }
}
