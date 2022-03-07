using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;



namespace TSA.Infrastructure.Helpers
{
    public class Signer
    {
        public byte[] Sign(byte[] dataToSign)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            var certificates = X509Certificate2UI.SelectFromCollection(store.Certificates, "Seleccionar Certificado", "Certificado para firmar", X509SelectionFlag.SingleSelection);

            if (certificates.Count > 0)
            {
                var cert = certificates[0] as X509Certificate2;

                var _certSerialNumber = cert.GetSerialNumberString();
                var _certIssuer = cert.Issuer;

                byte[] signedData;

                //var rsaCng = new RSACng(CngKey.Create(cert.PrivateKey as CngAlgorithm));
                //rsaCng.SignatureHashAlgorithm = CngAlgorithm.MD5;
                //signedData = rsaCng.SignData(cipherbytes);
                //var pk = cert.PrivateKey as RSACryptoServiceProvider;

                //if (pk.CspKeyContainerInfo.HardwareDevice)
                //{
                //    _isHardware = true;
                //    string pass = GetPasswordFromUserInput();

                //    if (string.IsNullOrEmpty(pass))
                //        throw new Exception("Debe ingresar una contraseña");

                //    var securePass = GenerateSecureString(pass);

                //    CspParameters cspParams = new CspParameters(
                //        pk.CspKeyContainerInfo.ProviderType,
                //        pk.CspKeyContainerInfo.ProviderName,
                //        pk.CspKeyContainerInfo.KeyContainerName,
                //        new System.Security.AccessControl.CryptoKeySecurity(),
                //        securePass);

                //    //cspParams.KeyNumber = (int)System.Security.Cryptography.KeyNumber.Signature;
                //    //cspParams.KeyNumber = (int)System.Security.Cryptography.KeyNumber.Exchange;
                //    cspParams.KeyNumber = (int)pk.CspKeyContainerInfo.KeyNumber;

                //    //Si se puede crear bien, la pass debe quedar en cache
                //    try
                //    {
                //        var hardwareCsp = new RSACryptoServiceProvider(cspParams);
                //    }
                //    catch
                //    {
                //        //MessageBox.Show("No se pudo acceder al dispositivo criptográfico. Verifique la contraseña ingresada", "Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                //        throw new Exception("No se pudo acceder al dispositivo criptográfico. Verifique la contraseña ingresada");
                //    }

                //}

                signedData = cert.GetRSAPrivateKey().SignData(dataToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return signedData;
            }
            else
                return null;
        }
    }
}
