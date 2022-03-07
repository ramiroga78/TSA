using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

using System.Security.Cryptography.Pkcs;
using System.Diagnostics;
using System.Security.Permissions;
using Security.Cryptography.X509Certificates;
using Security.Cryptography;
using System.Windows.Forms;

namespace CryptoActiveX
{
    [ComVisible(true)]
    [Guid("89423b84-50a9-4152-915d-52ef60051fc8")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Signer : IObjectSafetyImpl
    {

        private string _certSerialNumber;
        private string _issuer;
        private string _certIssuer;
        private bool _isHardware=false;

        [ComVisible(true)]
        public string CertSerialNumber
        {
            get { return _certSerialNumber; }
            set { _certSerialNumber = value; }
        }

        [ComVisible(true)]
        public string CertIssuer
        {
            get { return _certIssuer; }
        }


        [ComVisible(true)]
        public string Issuer
        {
            get { return _issuer; }
        
            // CMP 2017-04-20 SE PUEDE DECLARAR MÁS DE UN ISSUER SEPARADO POR ;
            //Issuer Name con mas de un atributo se leen 
            //Firefox = "CN=ISSUER,DC=XX"
            //Explorer = "CN=ISSUER, DC=XX"
            set { _issuer = value.ToLower().Replace(",", ", "); }
        }

        //no expuesta por COM
        [ComVisible(false)]
        public bool isHardware
        {
            get { return _isHardware; }
        }


        [ComVisible(true)]
        public string GetComponentVersion()
        {
            return "1.0";
        }

        [ComVisible(true)]
        public byte[] SignText(byte[] bytes)
        {
            //X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            //store.Open(OpenFlags.ReadOnly);
            //var certificates = X509Certificate2UI.SelectFromCollection(FilterValidCertificates(store.Certificates), "Seleccionar Certificado", "Certificado para firmar", X509SelectionFlag.SingleSelection);
            System.Security.Cryptography.X509Certificates.X509Store store = new System.Security.Cryptography.X509Certificates.X509Store(System.Security.Cryptography.X509Certificates.StoreName.My, System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser);
            store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadOnly);
            var certificates = System.Security.Cryptography.X509Certificates.X509Certificate2UI.SelectFromCollection(store.Certificates, "Seleccionar Certificado", "Certificado para firmar", System.Security.Cryptography.X509Certificates.X509SelectionFlag.SingleSelection);
            

            _isHardware = false;
            if (certificates.Count > 0)
            {
                var cert = certificates[0] as X509Certificate2;

                _certSerialNumber = cert.GetSerialNumberString();
                _certIssuer = cert.Issuer;

                byte[] cipherbytes = bytes;
                byte[] signedData;
                if (cert.HasCngKey())
                {
                    var rsaCng = new RSACng(cert.GetCngPrivateKey());
                    rsaCng.SignatureHashAlgorithm = System.Security.Cryptography.CngAlgorithm.MD5;
                    signedData = rsaCng.SignData(cipherbytes);
                }
                else
                {

                    var pk = cert.PrivateKey as System.Security.Cryptography.RSACryptoServiceProvider;

                    if (pk.CspKeyContainerInfo.HardwareDevice)
                    {
                        _isHardware = true;
                        string pass = GetPasswordFromUserInput();

                        if (string.IsNullOrEmpty(pass))
                            throw new Exception("Debe ingresar una contraseña");

                        var securePass = GenerateSecureString(pass);

                        System.Security.Cryptography.CspParameters cspParams = new System.Security.Cryptography.CspParameters(
                            pk.CspKeyContainerInfo.ProviderType,
                            pk.CspKeyContainerInfo.ProviderName,
                            pk.CspKeyContainerInfo.KeyContainerName,
                            new System.Security.AccessControl.CryptoKeySecurity(),
                            securePass);

                        //cspParams.KeyNumber = (int)System.Security.Cryptography.KeyNumber.Signature;
                        //cspParams.KeyNumber = (int)System.Security.Cryptography.KeyNumber.Exchange;
                        cspParams.KeyNumber = (int)pk.CspKeyContainerInfo.KeyNumber;

                        //Si se puede crear bien, la pass debe quedar en cache
                        try
                        {
                            var hardwareCsp = new System.Security.Cryptography.RSACryptoServiceProvider(cspParams);
                        }
                        catch
                        {
                            //MessageBox.Show("No se pudo acceder al dispositivo criptográfico. Verifique la contraseña ingresada", "Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            throw new Exception("No se pudo acceder al dispositivo criptográfico. Verifique la contraseña ingresada");
                        }

                    }

                    signedData = pk.SignData(cipherbytes, System.Security.Cryptography.MD5.Create());
                }

                return signedData;
            }
            else
            {
                _certSerialNumber = null;
                return null;
            }
            //return null;
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
                if (cert.HasCngKey())
                {
                    var rsaCng = new RSACng(cert.GetCngPrivateKey());
                    rsaCng.SignatureHashAlgorithm =  new  System.Security.Cryptography.CngAlgorithm(hashAlgorithmName);
                    signedData = rsaCng.SignData(cipherbytes);
                }
                else
                {
                    var hashAlgorithm = System.Security.Cryptography.HashAlgorithm.Create(hashAlgorithmName);
                    var pk = cert.PrivateKey as System.Security.Cryptography.RSACryptoServiceProvider;

                    if (pk.CspKeyContainerInfo.HardwareDevice)
                    {
                        _isHardware = true;                       
                        //GetPasswordFromUserInput();

                        if (string.IsNullOrEmpty(password))
                            throw new Exception("No password configured");

                        var securePass = GenerateSecureString(password);

                        System.Security.Cryptography.CspParameters cspParams = new System.Security.Cryptography.CspParameters(
                            pk.CspKeyContainerInfo.ProviderType,
                            pk.CspKeyContainerInfo.ProviderName,
                            pk.CspKeyContainerInfo.KeyContainerName,
                            new System.Security.AccessControl.CryptoKeySecurity(),
                            securePass);

                        //cspParams.KeyNumber = (int)System.Security.Cryptography.KeyNumber.Signature;
                        //cspParams.KeyNumber = (int)System.Security.Cryptography.KeyNumber.Exchange;
                        cspParams.KeyNumber = (int)pk.CspKeyContainerInfo.KeyNumber;

                        //Si se puede crear bien, la pass debe quedar en cache
                        try
                        {
                            var hardwareCsp = new System.Security.Cryptography.RSACryptoServiceProvider(cspParams);
                        }
                        catch
                        {
                            //MessageBox.Show("No se pudo acceder al dispositivo criptográfico. Verifique la contraseña ingresada", "Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            throw new Exception("No se pudo acceder al dispositivo criptográfico. Verifique la contraseña ingresada");
                        }

                    }

                    signedData = pk.SignData(cipherbytes, hashAlgorithm);
                    //signedData = null;
                }

                return signedData;
            }
            else
            {
                _certSerialNumber = null;
                return null;
            }
            //return null;
        }

        private string GetPasswordFromUserInput()
        {
            using (var dialog = new PasswordDialog())
            {
                return dialog.GetPassword();
            }
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

        private X509Certificate2Collection FilterValidCertificates(X509Certificate2Collection certificates)
        {
            //TEST - 20141027 Borrar
            //EventLog.WriteEntry("Application", "Application Config Issuer:" + _issuer);

            X509Certificate2Collection filteredCollection = new X509Certificate2Collection();

            for (int i = 0; i < certificates.Count; i++)
            {
                //TEST - 20141027 Borrar
                //EventLog.WriteEntry("Application", "Certificate Issuer:" + certificates[i].Issuer.ToLower());
                if (certificates[i].HasPrivateKey)
                {
                    //Soporte para más de una CA de confianza
                    var IssuerArray = _issuer.Split( char.Parse(";"));
                    foreach (string _iss in IssuerArray)
                    {
                        if (string.IsNullOrEmpty(_iss) || string.IsNullOrEmpty(_iss.Trim()) || certificates[i].Issuer.ToLower() == _iss)
                        {
                            if ((String.IsNullOrEmpty(_certSerialNumber) || string.IsNullOrEmpty(_certSerialNumber.Trim())) || (certificates[i].SerialNumber == _certSerialNumber))
                            {
                                filteredCollection.Add(certificates[i]);
                            }
                        }
                    }
                }
            }
            return filteredCollection;
        }

        [ComVisible(true)]
        public bool VerifySignature(string originalText, string signature, X509Certificate2 certificate)
        {
            var signatureBytes = Convert.FromBase64String(signature);
            var rsa = certificate.PublicKey.Key as System.Security.Cryptography.RSACryptoServiceProvider;
            byte[] cipherbytes = StrToByteArray(originalText);
            return rsa.VerifyData(cipherbytes, System.Security.Cryptography.MD5.Create(), signatureBytes);
        }

        [ComVisible(true)]
        public byte[] StrToByteArray(string str)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        [ComVisible(true)]
        public string ByteArrayToStr(byte[] bytes)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetString(bytes);
        }



        /* UNDONE: 20120625 GDG
            Prueba de Renovación         
         */

        #region RENEWAL POC

        //[ComVisible(true)]
        //public bool Renewal()
        //{
        //    X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        //    store.Open(OpenFlags.ReadOnly);
        //    var certificates = X509Certificate2UI.SelectFromCollection(FilterValidCertificates(store.Certificates), "Seleccionar Certificado", "Certificado para renovar", X509SelectionFlag.SingleSelection);

        //    if (certificates.Count > 0)
        //    {
        //        var cert = certificates[0] as X509Certificate2;
        //        var rawData = cert.RawData;

                

        //        /*
        //        _certSerialNumber = cert.GetSerialNumberString();
        //        _certIssuer = cert.Issuer;
                
        //        byte[] cipherbytes = StrToByteArray(text);
        //        byte[] signedData;
        //        if (cert.HasCngKey())
        //        {
        //            var rsaCng = new RSACng(cert.GetCngPrivateKey());
        //            rsaCng.SignatureHashAlgorithm = CngAlgorithm.MD5;
        //            signedData = rsaCng.SignData(cipherbytes);
        //        }
        //        else
        //        {

        //            var pk = cert.PrivateKey as RSACryptoServiceProvider;

        //            if (pk.CspKeyContainerInfo.HardwareDevice)
        //            {
        //                string pass = GetPasswordFromUserInput();

        //                if (string.IsNullOrEmpty(pass))
        //                    throw new Exception("Debe ingresar una contraseña");

        //                var securePass = GenerateSecureString(pass);

        //                CspParameters cspParams = new CspParameters(
        //                    pk.CspKeyContainerInfo.ProviderType,
        //                    pk.CspKeyContainerInfo.ProviderName,
        //                    pk.CspKeyContainerInfo.KeyContainerName,
        //                    new System.Security.AccessControl.CryptoKeySecurity(),
        //                    securePass);

        //                //cspParams.KeyNumber = (int)System.Security.Cryptography.KeyNumber.Signature;
        //                //cspParams.KeyNumber = (int)System.Security.Cryptography.KeyNumber.Exchange;
        //                cspParams.KeyNumber = (int)pk.CspKeyContainerInfo.KeyNumber;

        //                //Si se puede crear bien, la pass debe quedar en cache
        //                try
        //                {
        //                    var hardwareCsp = new RSACryptoServiceProvider(cspParams);
        //                }
        //                catch
        //                {
        //                    //MessageBox.Show("No se pudo acceder al dispositivo criptográfico. Verifique la contraseña ingresada", "Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
        //                    throw new Exception("No se pudo acceder al dispositivo criptográfico. Verifique la contraseña ingresada");
        //                }

        //            }

        //            signedData = pk.SignData(cipherbytes, MD5.Create());
        //        }

        //        return Convert.ToBase64String(signedData);*/
        //    }
        //    else
        //    {/*
        //        _certSerialNumber = null;
        //        return null;*/

        //        return false;
        //    }
        //}

        #endregion
    }
}
