using CryptoActiveX;
using Org.BouncyCastle.Cms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace SignAPI.Controllers
{
    
    public class SignerController : ApiController
    {
        public byte[] Post()
        {
            var password = ConfigurationManager.AppSettings["SignerPassword"];            
            // Decrypt the bytes to a string.
            password = DecryptStringFromBytes_Aes(Convert.FromBase64String(password), Encoding.ASCII.GetBytes("OntiSecretTSA!!!"));
            
            SignRequest signReq = null;
           
            using (var memoryStream = new MemoryStream())
            {
                Request.Content.CopyToAsync(memoryStream);
                BinaryFormatter formatter = new BinaryFormatter();
                memoryStream.Seek(0, SeekOrigin.Begin);
                signReq = (SignRequest)formatter.Deserialize(memoryStream);

            }

            var signer = new Signer();          
          
            return signer.SignBytes(signReq.Data, signReq.Thumbprint, signReq.HashAlgorithmName, password);
        }       

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
           

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.Mode = CipherMode.ECB;


                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, null);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}

