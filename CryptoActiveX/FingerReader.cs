using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Diagnostics;
using System.Security.Permissions;
using Security.Cryptography.X509Certificates;
using Security.Cryptography;
using System.Windows.Forms;

namespace CryptoActiveX
{
    /// <summary>Finger Reader Activex Control</summary>
    [ComVisible(true)]
    [Guid("2156D96B-E257-4864-9773-B52E7A3F3224")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class FingerReader : IObjectSafetyImpl
    {
        private ReaderModels _readerModel;

        /// <summary>
        ///   Readers Models Supported 
        /// </summary>
        [ComVisible(true)]
        public enum ReaderModels
        {
            UareU,
            SecuGen,
            Morpho
        }

        /// <summary>Gets or sets the reader model.</summary>
        /// <value>The reader model.</value>
        /// <remarks>Valid values: UareU</remarks>
        [ComVisible(true)]
        public ReaderModels ReaderModel
        {
            get { return _readerModel; }
            set { _readerModel = value; }
        }

        /// <summary>Gets the finger image.</summary>
        /// <value>The finger image.</value>
        [ComVisible(true)] 
        public string FingerImage { get; private set; }


        public string GetFingerPrint()
        {

            return "";
        }

    }
}
