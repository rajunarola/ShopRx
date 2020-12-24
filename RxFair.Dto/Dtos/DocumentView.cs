using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace RxFair.Dto.Dtos
{
    public class DocumentView : BaseModel
    {
        public int DocumentType { get; set; }

        public string TypeName { get; set; }

       //[Remote("UserAlreadyExistsAsync", "Account", ErrorMessage = "User with this Email already exists")]
        public string DocumentName { get; set; }

        public string DocumentFile { get; set; }

        public IFormFile Document { get; set; }
    }

    public class GroupDocumentView : BaseModel
    {
        public int DocumentType { get; set; }
        public string TypeName { get; set; }
        public List<DocumentView> Document { get; set; }
    }

    public class DistributorDocumentMasterView : BaseModel
    {
        public IFormFile ReturnPolicy { get; set; }
        public string ReturnPolicyFile { get; set; }
        public IFormFile LicenseCertificate { get; set; }
        public string LicenseCertificateFile { get; set; }
        public IFormFile Waiver { get; set; }
        public string WaiverFile { get; set; }
        public long DistributorId { get; set; }
    }
}

