using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace RxFair.Service.Utility
{
    public class S3Service
    {
        public readonly AwsS3Storage _awsS3Storage;
        public S3Service(IOptions<AwsS3Storage> awsS3Storage)
        {
            _awsS3Storage = awsS3Storage.Value;
        }   

    }

    public class AwsS3Storage
    {
        public string S3Bucket { get; set; }
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public string S3ServiceURL { get; set; }
    }
}
