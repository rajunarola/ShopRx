using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RxFair.Service.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RxFair.Utility.Common
{
    public class S3Manager
    {
        private IAmazonS3 _client;
        //public static string BucketName = ConfigurationManager.AppSettings["S3Bucket"];
        //public static readonly string AwsAccessKeyId = ConfigurationManager.AppSettings["AWSAccessKey"];
        //public static readonly string AwsSecretAccessKey = ConfigurationManager.AppSettings["AWSSecretKey"];
        //static readonly string AwsUrlLink = ConfigurationManager.AppSettings["S3ServiceURL"];
        //readonly string TinyPNGKey = ConfigurationManager.AppSettings["TinyPNGKey"];
        private readonly string BucketName;
        private readonly string AwsAccessKeyId;
        private readonly string AwsSecretAccessKey;
        private readonly string AwsUrlLink;
        private readonly RegionEndpoint EndPoint = RegionEndpoint.USEast2;
        string delimiter = "/";
        private readonly S3Service _s3Service;


        public S3Manager(IOptions<AwsS3Storage> s3Service)
        {
            _s3Service = new S3Service(s3Service);
            BucketName = _s3Service._awsS3Storage.S3Bucket;
            AwsAccessKeyId = _s3Service._awsS3Storage.AWSAccessKey;
            AwsSecretAccessKey = _s3Service._awsS3Storage.AWSSecretKey;
            AwsUrlLink = _s3Service._awsS3Storage.S3ServiceURL;

        }

        public  bool UploadImageToParticularFolder(string mainFolder, IFormFile file, string newfileName)
        {
            using (_client = new AmazonS3Client(AwsAccessKeyId, AwsSecretAccessKey, EndPoint))
            {
                try
                {
                    var transfarUtility = new TransferUtility(_client);

                    using (var filetoUpload = file.OpenReadStream())
                    {
                       var result= UploadFile(BucketName, newfileName, filetoUpload);
                        return result;
                    }
                  
                }
                catch (AmazonS3Exception e)
                {
                    return false;
                }
            }
        }
        public string GetUrl(string filePath, string fileName)
        {
            return string.Concat(delimiter, filePath, delimiter, fileName);
        }
        public string GetUrl(string filePath)
        {
            return string.Concat(delimiter, filePath);
        }
        public string GetUrlFromBackground(string filePath, string host)
        {
            return string.Concat(delimiter, filePath);
        }
        public bool FileExist(string filePath, string fileName)
        {
            string filepath = string.Concat(filePath, delimiter, fileName);
            using (_client = new AmazonS3Client(AwsAccessKeyId, AwsSecretAccessKey, EndPoint))
            {
                // do stuff
                //S3FileInfo s3FileInfo = new S3FileInfo(_client, BucketName, filepath);
                //return s3FileInfo.Exists;
                return true;
            }
        }
        public async Task<bool> CreateNewSubFolder(string mainFolder)
        {
            using (_client = new AmazonS3Client(AwsAccessKeyId, AwsSecretAccessKey, EndPoint))
            {
                try
                {
                    PutObjectRequest putObjectRequest = new PutObjectRequest
                    {
                        CannedACL = S3CannedACL.PublicRead,
                        BucketName = string.Concat(BucketName, delimiter, mainFolder.Trim())
                    };
                    PutObjectResponse putObjectResponse = await _client.PutObjectAsync(putObjectRequest);
                    return putObjectResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
                }
                catch (AmazonS3Exception)
                {
                    return false;
                }
            }
        }
        public async Task<bool> CopyFile(string origin, string destination)
        {
            try
            {
                AmazonS3Client copyClient = new AmazonS3Client(EndPoint);
                CopyObjectRequest request = new CopyObjectRequest
                {
                    SourceBucket = BucketName,
                    SourceKey = origin.Trim(),
                    DestinationBucket = BucketName,
                    DestinationKey = destination.Trim(),
                    CannedACL = S3CannedACL.PublicRead
                };

                await copyClient.CopyObjectAsync(request);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
        public async Task DeleteObjectFromS3(string mainFolder, string fileName)
        {
            using (_client = new AmazonS3Client(AwsAccessKeyId, AwsSecretAccessKey, EndPoint))
            {
                try
                {
                    DeleteObjectRequest request = new DeleteObjectRequest
                    {
                        BucketName = string.Concat(BucketName, delimiter, mainFolder.Trim())
                    };
                    ;
                    request.Key = fileName.Trim();
                    await _client.DeleteObjectAsync(request);
                }
                catch (AmazonS3Exception ex)
                {

                }
            }
        }
        public async Task GiveReadPermission(string filepath)
        {
            using (_client = new AmazonS3Client(AwsAccessKeyId, AwsSecretAccessKey, EndPoint))
            {
                try
                {
                    await _client.PutACLAsync(new PutACLRequest
                    {
                        BucketName = BucketName,
                        Key = filepath.Trim(),
                        CannedACL = S3CannedACL.PublicRead
                    });
                }
                catch (AmazonS3Exception ex)
                {

                }
            }

        }
        public void RemoveAllFiles(string folderName)
        {
            using (_client = new AmazonS3Client(AwsAccessKeyId, AwsSecretAccessKey, EndPoint))
            {
                try
                {
                    //S3DirectoryInfo directoryToDelete = new S3DirectoryInfo(_client, BucketName, folderName);
                    //directoryToDelete.Delete(true); // true will delete recursively in folder inside                    
                }
                catch (AmazonS3Exception ex)
                {

                }
            }
        }

        public void UploadAllFilesFromFolder(string searchFolder)
        {
            //var filters = new[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp", "svg" };
            //var imageFiles = GetFilesFrom(searchFolder, filters, false);

            using (_client = new AmazonS3Client(AwsAccessKeyId, AwsSecretAccessKey, EndPoint))
            {
                try
                {
                    using (var fileTransferUtility = new TransferUtility(_client))
                    {
                        fileTransferUtility.UploadDirectory(searchFolder, BucketName, "*.jpg", SearchOption.TopDirectoryOnly);
                    }
                }
                catch (AmazonS3Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public string[] GetFilesFrom(string searchFolder, string[] filters, bool isRecursive)
        {
            if (searchFolder == null) throw new ArgumentNullException(nameof(searchFolder));
            List<String> filesFound = new List<string>();
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var filter in filters)
            {
                filesFound.AddRange(Directory.GetFiles(searchFolder, $"*.{filter}", searchOption));
            }
            return filesFound.ToArray();
        }


        public bool UploadFile(string awsBucketName, string key, Stream stream)
        {
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                BucketName = awsBucketName,
                CannedACL = S3CannedACL.AuthenticatedRead,
                Key = key
            };

            TransferUtility fileTransferUtility = new TransferUtility(this._client);
            fileTransferUtility.Upload(uploadRequest);
            return true;
        }
    }



}
