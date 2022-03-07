using CryptoActiveX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelperProject
{
    public class SignerBridge
    {

        public byte[] Sign(byte[] bytes)
        {
            var signer = new Signer();
            return signer.SignText2(bytes);
        }
    }
}
