using System;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using Security.Cryptography.X509Certificates;
using Security.Cryptography;

namespace Org.BouncyCastle.ONTI
{
    public class Signer
    {

        public Signer()
        {

        }

        private string _certSerialNumber;
        private string _issuer;
        private string _certIssuer;
        private bool _isHardware = false;

        public string CertSerialNumber
        {
            get { return _certSerialNumber; }
            set { _certSerialNumber = value; }
        }

        public string CertIssuer
        {
            get { return _certIssuer; }
        }


        public string Issuer
        {
            get { return _issuer; }

            // CMP 2017-04-20 SE PUEDE DECLARAR MÁS DE UN ISSUER SEPARADO POR ;
            //Issuer Name con mas de un atributo se leen 
            //Firefox = "CN=ISSUER,DC=XX"
            //Explorer = "CN=ISSUER, DC=XX"
            set { _issuer = value.ToLower().Replace(",", ", "); }
        }

        public bool isHardware
        {
            get { return _isHardware; }
        }

        public byte[] SignBytes(byte[] bytes, string thumbprint, string hashAlgorithmName, string password)
        {
            //X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            //store.Open(OpenFlags.ReadOnly);
            //var certificates = X509Certificate2UI.SelectFromCollection(FilterValidCertificates(store.Certificates), "Seleccionar Certificado", "Certificado para firmar", X509SelectionFlag.SingleSelection);
            
            System.Security.Cryptography.X509Certificates.X509Store store = new System.Security.Cryptography.X509Certificates.X509Store(System.Security.Cryptography.X509Certificates.StoreName.My, System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser);
            store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadOnly);
            var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

            _isHardware = false;
            if (certificates.Count > 0)
            {
                var cert = certificates[0] as X509Certificate2;

                _certSerialNumber = cert.GetSerialNumberString();
                _certIssuer = cert.Issuer;

                byte[] cipherbytes = bytes;
                byte[] signedData;

                //var hashAlgorithm = System.Security.Cryptography.HashAlgorithm.Create(hashAlgorithmName);
                var hashAlgorithm = new System.Security.Cryptography.HashAlgorithmName(hashAlgorithmName);
                System.Security.Cryptography.RSA pk;

                //if (cert.HasCngKey())
                //{
                //    var rsaCng = new RSACng(cert.GetCngPrivateKey());
                //    rsaCng.SignatureHashAlgorithm = System.Security.Cryptography.CngAlgorithm.Sha256;
                //    signedData = rsaCng.SignData(cipherbytes);
                //}
                //else
                //{
                    pk = cert.GetRSAPrivateKey(); //as System.Security.Cryptography.RSACryptoServiceProvider;

                    //if (pk.CspKeyContainerInfo.HardwareDevice)
                    //{
                    //    _isHardware = true;
                    //    //GetPasswordFromUserInput();

                    //    if (string.IsNullOrEmpty(password))
                    //        throw new Exception("No password configured");

                    //    var securePass = GenerateSecureString(password);

                    //    System.Security.Cryptography.CspParameters cspParams = new System.Security.Cryptography.CspParameters(
                    //        pk.CspKeyContainerInfo.ProviderType,
                    //        pk.CspKeyContainerInfo.ProviderName,
                    //        pk.CspKeyContainerInfo.KeyContainerName);
                    //    //,
                    //    //new System.Security.AccessControl.CryptoKeySecurity(),
                    //    //securePass);

                    //    //cspParams.KeyNumber = (int)System.Security.Cryptography.KeyNumber.Signature;
                    //    //cspParams.KeyNumber = (int)System.Security.Cryptography.KeyNumber.Exchange;
                    //    cspParams.KeyNumber = (int)pk.CspKeyContainerInfo.KeyNumber;

                    //    //Si se puede crear bien, la pass debe quedar en cache
                    //    try
                    //    {
                    //        var hardwareCsp = new System.Security.Cryptography.RSACryptoServiceProvider(cspParams);
                    //    }
                    //    catch
                    //    {
                    //        //MessageBox.Show("No se pudo acceder al dispositivo criptográfico. Verifique la contraseña ingresada", "Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    //        throw new Exception("No se pudo acceder al dispositivo criptográfico. Verifique la contraseña ingresada");
                    //    }

                    //}

                    signedData = pk.SignData(cipherbytes, hashAlgorithm, System.Security.Cryptography.RSASignaturePadding.Pss);
                    //signedData = null;

                //}
                return signedData;
            }
            else
            {
                _certSerialNumber = null;
                return null;
            }
            //return null;
        }


        private static System.Security.SecureString GenerateSecureString(string pass)
        {
            var securePass = new System.Security.SecureString();
            foreach (var c in pass)
            {
                securePass.AppendChar(c);
            }
            return securePass;
        }

    }
}
