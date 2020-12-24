using System;
using System.Security.Cryptography;
using System.Text;

namespace RxFair.Utility.Common
{
    public class CryptoGraphyService
    {
        private string _securityKey = "RxFair";
        public string EncryptPlainTextToCipherText(string plainText)
        {
            //Getting the bytes of Input String.

            byte[] toEncryptedArray = Encoding.UTF8.GetBytes(plainText);

            SHA512 objSha512CryptoService = SHA512.Create();

            //Gettting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
            byte[] securityKeyArray = objSha512CryptoService.ComputeHash(Encoding.UTF8.GetBytes(_securityKey));
            byte[] trimmedBytes = new byte[24];
            Buffer.BlockCopy(securityKeyArray, 0, trimmedBytes, 0, 24);
            securityKeyArray = trimmedBytes;

            //De-allocatinng the memory after doing the Job.
            objSha512CryptoService.Dispose();

            TripleDES objTripleDesCryptoService = TripleDES.Create();

            //Assigning the Security key to the TripleDES Service Provider.
            objTripleDesCryptoService.Key = securityKeyArray;

            //Mode of the Crypto service is Electronic Code Book.
            objTripleDesCryptoService.Mode = CipherMode.ECB;

            //Padding Mode is PKCS7 if there is any extra byte is added.
            objTripleDesCryptoService.Padding = PaddingMode.PKCS7;

            var objCrytpoTransform = objTripleDesCryptoService.CreateEncryptor();

            //Transform the bytes array to resultArray
            byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);

            //Releasing the Memory Occupied by TripleDES Service Provider for Encryption.
            objTripleDesCryptoService.Dispose();

            //Convert and return the encrypted data/byte into string format.
            return ConvertStringToHex(Convert.ToBase64String(resultArray, 0, resultArray.Length), System.Text.Encoding.Unicode);
        }

        public string DecryptCipherTextToPlainText(string cipherText)
        {

            byte[] toEncryptArray = Convert.FromBase64String(ConvertHexToString(cipherText, System.Text.Encoding.Unicode));



            SHA512 objSha512CryptoService = SHA512.Create();

            //Gettting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.

            byte[] securityKeyArray = objSha512CryptoService.ComputeHash(Encoding.UTF8.GetBytes(_securityKey));

            byte[] trimmedBytes = new byte[24];
            Buffer.BlockCopy(securityKeyArray, 0, trimmedBytes, 0, 24);
            securityKeyArray = trimmedBytes;

            //De-allocatinng the memory after doing the Job.

            objSha512CryptoService.Dispose();



            TripleDES objTripleDesCryptoService = TripleDES.Create();



            //Assigning the Security key to the TripleDES Service Provider.

            objTripleDesCryptoService.Key = securityKeyArray;



            //Mode of the Crypto service is Electronic Code Book.

            objTripleDesCryptoService.Mode = CipherMode.ECB;



            //Padding Mode is PKCS7 if there is any extra byte is added.

            objTripleDesCryptoService.Padding = PaddingMode.PKCS7;



            var objCrytpoTransform = objTripleDesCryptoService.CreateDecryptor();



            //Transform the bytes array to resultArray

            byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);



            //Releasing the Memory Occupied by TripleDES Service Provider for Decryption.          

            objTripleDesCryptoService.Dispose();



            //Convert and return the decrypted data/byte into string format.

            return Encoding.UTF8.GetString(resultArray);

        }
        public string ConvertStringToHex(string input, Encoding encoding)
        {
            Byte[] stringBytes = encoding.GetBytes(input);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }

        public string ConvertHexToString(string hexInput, Encoding encoding)
        {
            int numberChars = hexInput.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
            }
            return encoding.GetString(bytes);
        }
    }
}