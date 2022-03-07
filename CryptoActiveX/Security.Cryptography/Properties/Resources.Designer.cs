﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3521
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Security.Cryptography.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Security.Cryptography.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This transform has already transformed its final block and can no longer transform additional data..
        /// </summary>
        internal static string AlreadyTransformedFinalBlock {
            get {
                return ResourceManager.GetString("AlreadyTransformedFinalBlock", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot decrypt a partial block..
        /// </summary>
        internal static string CannotDecryptPartialBlock {
            get {
                return ResourceManager.GetString("CannotDecryptPartialBlock", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Alias &apos;{0}&apos; already exists in the CryptoConfig2 map..
        /// </summary>
        internal static string DuplicateCryptoConfigAlias {
            get {
                return ResourceManager.GetString("DuplicateCryptoConfigAlias", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Null or empty CryptoConfig aliases are invalid..
        /// </summary>
        internal static string EmptyCryptoConfigAlias {
            get {
                return ResourceManager.GetString("EmptyCryptoConfigAlias", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CNG chaining modes cannot have empty names..
        /// </summary>
        internal static string InvalidChainingModeName {
            get {
                return ResourceManager.GetString("InvalidChainingModeName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified IV was not long enough. IVs must be the same length as the block size..
        /// </summary>
        internal static string InvalidIVSize {
            get {
                return ResourceManager.GetString("InvalidIVSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The padding on the block is invalid and cannot be removed..
        /// </summary>
        internal static string InvalidPadding {
            get {
                return ResourceManager.GetString("InvalidPadding", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified RSA parameters are not valid; both Exponent and Modulus are required fields..
        /// </summary>
        internal static string InvalidRsaParameters {
            get {
                return ResourceManager.GetString("InvalidRsaParameters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The hash algorithm is not supported for signatures. Only MD5, SHA1, SHA256,SHA384, and SHA512 are supported at this time..
        /// </summary>
        internal static string InvalidSignatureHashAlgorithm {
            get {
                return ResourceManager.GetString("InvalidSignatureHashAlgorithm", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified tag is not a valid size for this implementation..
        /// </summary>
        internal static string InvalidTagSize {
            get {
                return ResourceManager.GetString("InvalidTagSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified key must be an RSA key.
        /// </summary>
        internal static string KeyMustBeRsa {
            get {
                return ResourceManager.GetString("KeyMustBeRsa", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No IV was given, and the specified CipherMode requires the use of an IV..
        /// </summary>
        internal static string MissingIV {
            get {
                return ResourceManager.GetString("MissingIV", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Authentication tags are only available after the final block has been transformed..
        /// </summary>
        internal static string TagIsOnlyGeneratedAfterFinalBlock {
            get {
                return ResourceManager.GetString("TagIsOnlyGeneratedAfterFinalBlock", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Authentication tags are only generated during encryption operations, and cannot be retrieved from a decryption transform..
        /// </summary>
        internal static string TagIsOnlyGeneratedDuringEncryption {
            get {
                return ResourceManager.GetString("TagIsOnlyGeneratedDuringEncryption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified CipherMode is not supported with BCrypt symmetric algorithms..
        /// </summary>
        internal static string UnsupportedCipherMode {
            get {
                return ResourceManager.GetString("UnsupportedCipherMode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified PaddingMode is not supported.
        /// </summary>
        internal static string UnsupportedPaddingMode {
            get {
                return ResourceManager.GetString("UnsupportedPaddingMode", resourceCulture);
            }
        }
    }
}
