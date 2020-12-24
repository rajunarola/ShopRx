using System;
using System.Collections.Generic;
using System.Text;

namespace RxFair.Dto.Dtos
{
    public class SendEmailModel
    {
        public string ToDisplayName { get; set; }

        public string ToAddress { get; set; }

        public string CcAddress { get; set; }

        public string Subject { get; set; }
        public string BodyText { get; set; }

        public byte[] Attachment { get; set; }

        public string AttachmentName { get; set; }
    }
}
