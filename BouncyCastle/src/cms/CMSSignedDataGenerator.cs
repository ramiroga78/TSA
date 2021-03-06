
using System.Collections;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Crypto.Operators;
using System.Net;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;


namespace Org.BouncyCastle.Cms
{
	/**
     * general class for generating a pkcs7-signature message.
     * <p>
     * A simple example of usage.
     *
     * <pre>
     *      IX509Store certs...
     *      IX509Store crls...
     *      CmsSignedDataGenerator gen = new CmsSignedDataGenerator();
     *
     *      gen.AddSigner(privKey, cert, CmsSignedGenerator.DigestSha1);
     *      gen.AddCertificates(certs);
     *      gen.AddCrls(crls);
     *
     *      CmsSignedData data = gen.Generate(content);
     * </pre>
	 * </p>
     */
	[Serializable()]
	public class SignRequest
	{
		public string Thumbprint { get; set; }
		public string HashAlgorithmName { get; set; }
		public byte[] Data { get; set; }
	}
	
	public class CmsSignedDataGenerator
        : CmsSignedGenerator
    {
		private static readonly CmsSignedHelper Helper = CmsSignedHelper.Instance;

		private readonly IList signerInfs = Platform.CreateArrayList();

		private class SignerInf
        {
            private readonly CmsSignedGenerator outer;

			private readonly ISignatureFactory		sigCalc;
			private readonly SignerIdentifier			signerIdentifier;
			private readonly string						digestOID;
			private readonly string						encOID;
			private readonly CmsAttributeTableGenerator	sAttr;
			private readonly CmsAttributeTableGenerator	unsAttr;
			private readonly Asn1.Cms.AttributeTable	baseSignedTable;

			internal SignerInf(
                CmsSignedGenerator			outer,
	            AsymmetricKeyParameter		key,
	            SignerIdentifier			signerIdentifier,
	            string						digestOID,
	            string						encOID,
	            CmsAttributeTableGenerator	sAttr,
	            CmsAttributeTableGenerator	unsAttr,
	            Asn1.Cms.AttributeTable		baseSignedTable)
	        {
                string digestName = Helper.GetDigestAlgName(digestOID);

                string signatureName = digestName + "with" + Helper.GetEncryptionAlgName(encOID);

                this.outer = outer;
                this.sigCalc = new Asn1SignatureFactory(signatureName, key);
                this.signerIdentifier = signerIdentifier;
                this.digestOID = digestOID;
                this.encOID = encOID;
	            this.sAttr = sAttr;
	            this.unsAttr = unsAttr;
	            this.baseSignedTable = baseSignedTable;
            }

            internal SignerInf(
                CmsSignedGenerator outer,
                ISignatureFactory sigCalc,
                SignerIdentifier signerIdentifier,
                CmsAttributeTableGenerator sAttr,
                CmsAttributeTableGenerator unsAttr,
                Asn1.Cms.AttributeTable baseSignedTable)
            {
                this.outer = outer;
                this.sigCalc = sigCalc;
                this.signerIdentifier = signerIdentifier;
                this.digestOID = new DefaultDigestAlgorithmIdentifierFinder().find((AlgorithmIdentifier)sigCalc.AlgorithmDetails).Algorithm.Id;
                this.encOID = ((AlgorithmIdentifier)sigCalc.AlgorithmDetails).Algorithm.Id;
                this.sAttr = sAttr;
                this.unsAttr = unsAttr;
                this.baseSignedTable = baseSignedTable;
            }

            internal AlgorithmIdentifier DigestAlgorithmID
			{
				get { return new AlgorithmIdentifier(new DerObjectIdentifier(digestOID), DerNull.Instance); }
			}

			internal CmsAttributeTableGenerator SignedAttributes
            {
				get { return sAttr; }
            }

            internal CmsAttributeTableGenerator UnsignedAttributes
            {
				get { return unsAttr; }
            }

			internal SignerInfo ToSignerInfo(
                DerObjectIdentifier	contentType,
                CmsProcessable		content,
				SecureRandom		random, string certThumbprint)//, string signerUrl )
            {
                AlgorithmIdentifier digAlgId = DigestAlgorithmID;
				string digestName = Helper.GetDigestAlgName(digestOID);

				string signatureName = digestName + "with" + Helper.GetEncryptionAlgName(encOID);
				
                byte[] hash;
                if (outer._digests.Contains(digestOID))
                {
                    hash = (byte[])outer._digests[digestOID];
                }
                else
                {
                    IDigest dig = Helper.GetDigestInstance(digestName);
                    if (content != null)
                    {
                        content.Write(new DigestSink(dig));
                    }
                    hash = DigestUtilities.DoFinal(dig);
                    outer._digests.Add(digestOID, hash.Clone());
                }

                IStreamCalculator calculator = sigCalc.CreateCalculator();

#if NETCF_1_0 || NETCF_2_0 || SILVERLIGHT || PORTABLE
				Stream sigStr = calculator.Stream;
#else
				Stream sigStr = new BufferedStream(calculator.Stream);
#endif

				Asn1Set signedAttr = null;
				if (sAttr != null)
				{
					IDictionary parameters = outer.GetBaseParameters(contentType, digAlgId, hash);

//					Asn1.Cms.AttributeTable signed = sAttr.GetAttributes(Collections.unmodifiableMap(parameters));
					Asn1.Cms.AttributeTable signed = sAttr.GetAttributes(parameters);

                    if (contentType == null) //counter signature
                    {
                        if (signed != null && signed[CmsAttributes.ContentType] != null)
                        {
                            IDictionary tmpSigned = signed.ToDictionary();
                            tmpSigned.Remove(CmsAttributes.ContentType);
                            signed = new Asn1.Cms.AttributeTable(tmpSigned);
                        }
                    }

					// TODO Validate proposed signed attributes

					signedAttr = outer.GetAttributeSet(signed);

					// sig must be composed from the DER encoding.
					new DerOutputStream(sigStr).WriteObject(signedAttr);
				}
                else if (content != null)
                {
					// TODO Use raw signature of the hash value instead
					content.Write(sigStr);
                }

                Platform.Dispose(sigStr);

				/////////////////////rewrited not using exported parameters////////////////////////////
				/////byte[] sigBytes2 = ((IBlockResult)calculator.GetResult()).Collect();// old signing method
				///
				Org.BouncyCastle.ONTI.Signer s = new ONTI.Signer();
				//byte[] sigBytes = GetSignerResponse(signerUrl, hash, certThumbprint, digestName);			
				byte[] sigBytes = s.SignBytes(hash, certThumbprint, digestName, "");
				Asn1Set unsignedAttr = null;
				if (unsAttr != null)
				{
					IDictionary baseParameters = outer.GetBaseParameters(contentType, digAlgId, hash);
					baseParameters[CmsAttributeTableParameter.Signature] = sigBytes.Clone();

					//Asn1.Cms.AttributeTable unsigned = unsAttr.GetAttributes(Collections.unmodifiableMap(baseParameters));
					Asn1.Cms.AttributeTable unsigned = unsAttr.GetAttributes(baseParameters);

					// TODO Validate proposed unsigned attributes

					unsignedAttr = outer.GetAttributeSet(unsigned);
				}

				// TODO[RSAPSS] Need the ability to specify non-default parameters
				Asn1Encodable sigX509Parameters = SignerUtilities.GetDefaultX509Parameters(signatureName);
				AlgorithmIdentifier encAlgId = Helper.GetEncAlgorithmIdentifier(
					new DerObjectIdentifier(encOID), sigX509Parameters);
				
                return new SignerInfo(signerIdentifier, digAlgId,
                    signedAttr, encAlgId, new DerOctetString(sigBytes), unsignedAttr);
            }
        }

		
		//private static byte[] GetSignerResponse(string signerUrl, byte[] bytes, string thumbPrint, string hashAlgName)
		//{
		//	HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(signerUrl);
		//	httpReq.Method = "POST";			
		//	Stream reqStream = httpReq.GetRequestStream();

		//	var reqdata = new SignRequest();

		//	reqdata.Thumbprint = thumbPrint;
		//	reqdata.Data = bytes;
		//	reqdata.HashAlgorithmName = hashAlgName;

		//	var reqbytes = ObjectToByteArray(reqdata);
		//	reqStream.Write(reqbytes, 0, reqbytes.Length);
		//	reqStream.Close();
		//	HttpWebResponse httpResp = (HttpWebResponse)httpReq.GetResponse();
		//	Stream respStream = httpResp.GetResponseStream();
		//    StreamReader readStream = new StreamReader(respStream, Encoding.UTF8);			
		//	var resul = readStream.ReadToEnd();
		//	// Convert Base64 enconded trimed bytes
		//	return System.Convert.FromBase64String(resul.Substring(1).Remove(resul.Length - 2)); 


		//}
		public static byte[] ObjectToByteArray(object obj)
		{
			if (obj == null)
				return null;
			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				return ms.ToArray();
			}
		}



		public CmsSignedDataGenerator()
        {
        }

		/// <summary>Constructor allowing specific source of randomness</summary>
		/// <param name="rand">Instance of <c>SecureRandom</c> to use.</param>
		public CmsSignedDataGenerator(
			SecureRandom rand)
			: base(rand)
		{
		}

		/**
        * add a signer - no attributes other than the default ones will be
        * provided here.
		*
		* @param key signing key to use
		* @param cert certificate containing corresponding public key
		* @param digestOID digest algorithm OID
        */
        public void AddSigner(
            AsymmetricKeyParameter	privateKey,
            X509Certificate			cert,
            string					digestOID)
        {
        	AddSigner(privateKey, cert, Helper.GetEncOid(privateKey, digestOID), digestOID);
		}

		/**
		 * add a signer, specifying the digest encryption algorithm to use - no attributes other than the default ones will be
		 * provided here.
		 *
		 * @param key signing key to use
		 * @param cert certificate containing corresponding public key
		 * @param encryptionOID digest encryption algorithm OID
		 * @param digestOID digest algorithm OID
		 */
		public void AddSigner(
			AsymmetricKeyParameter	privateKey,
			X509Certificate			cert,
			string					encryptionOID,
			string					digestOID)
		{
			doAddSigner(privateKey, GetSignerIdentifier(cert), encryptionOID, digestOID,
				new DefaultSignedAttributeTableGenerator(), null, null);
		}

	    /**
	     * add a signer - no attributes other than the default ones will be
	     * provided here.
	     */
	    public void AddSigner(
            AsymmetricKeyParameter	privateKey,
	        byte[]					subjectKeyID,
            string					digestOID)
	    {
			AddSigner(privateKey, subjectKeyID, Helper.GetEncOid(privateKey, digestOID), digestOID);
	    }

		/**
		 * add a signer, specifying the digest encryption algorithm to use - no attributes other than the default ones will be
		 * provided here.
		 */
		public void AddSigner(
			AsymmetricKeyParameter	privateKey,
			byte[]					subjectKeyID,
			string					encryptionOID,
			string					digestOID)
		{
			doAddSigner(privateKey, GetSignerIdentifier(subjectKeyID), encryptionOID, digestOID,
				new DefaultSignedAttributeTableGenerator(), null, null);
		}

        /**
        * add a signer with extra signed/unsigned attributes.
		*
		* @param key signing key to use
		* @param cert certificate containing corresponding public key
		* @param digestOID digest algorithm OID
		* @param signedAttr table of attributes to be included in signature
		* @param unsignedAttr table of attributes to be included as unsigned
        */
        public void AddSigner(
            AsymmetricKeyParameter	privateKey,
            X509Certificate			cert,
            string					digestOID,
            Asn1.Cms.AttributeTable	signedAttr,
            Asn1.Cms.AttributeTable	unsignedAttr)
        {
			AddSigner(privateKey, cert, Helper.GetEncOid(privateKey, digestOID), digestOID,
				signedAttr, unsignedAttr);
		}

		/**
		 * add a signer, specifying the digest encryption algorithm, with extra signed/unsigned attributes.
		 *
		 * @param key signing key to use
		 * @param cert certificate containing corresponding public key
		 * @param encryptionOID digest encryption algorithm OID
		 * @param digestOID digest algorithm OID
		 * @param signedAttr table of attributes to be included in signature
		 * @param unsignedAttr table of attributes to be included as unsigned
		 */
		public void AddSigner(
			AsymmetricKeyParameter	privateKey,
			X509Certificate			cert,
			string					encryptionOID,
			string					digestOID,
			Asn1.Cms.AttributeTable	signedAttr,
			Asn1.Cms.AttributeTable	unsignedAttr)
		{
			doAddSigner(privateKey, GetSignerIdentifier(cert), encryptionOID, digestOID,
				new DefaultSignedAttributeTableGenerator(signedAttr),
				new SimpleAttributeTableGenerator(unsignedAttr),
				signedAttr);
		}

	    /**
	     * add a signer with extra signed/unsigned attributes.
		 *
		 * @param key signing key to use
		 * @param subjectKeyID subjectKeyID of corresponding public key
		 * @param digestOID digest algorithm OID
		 * @param signedAttr table of attributes to be included in signature
		 * @param unsignedAttr table of attributes to be included as unsigned
	     */
		public void AddSigner(
			AsymmetricKeyParameter	privateKey,
			byte[]					subjectKeyID,
			string					digestOID,
			Asn1.Cms.AttributeTable	signedAttr,
			Asn1.Cms.AttributeTable	unsignedAttr)
		{
			AddSigner(privateKey, subjectKeyID, Helper.GetEncOid(privateKey, digestOID), digestOID,
				signedAttr, unsignedAttr); 
		}

		/**
		 * add a signer, specifying the digest encryption algorithm, with extra signed/unsigned attributes.
		 *
		 * @param key signing key to use
		 * @param subjectKeyID subjectKeyID of corresponding public key
		 * @param encryptionOID digest encryption algorithm OID
		 * @param digestOID digest algorithm OID
		 * @param signedAttr table of attributes to be included in signature
		 * @param unsignedAttr table of attributes to be included as unsigned
		 */
		public void AddSigner(
			AsymmetricKeyParameter	privateKey,
			byte[]					subjectKeyID,
			string					encryptionOID,
			string					digestOID,
			Asn1.Cms.AttributeTable	signedAttr,
			Asn1.Cms.AttributeTable	unsignedAttr)
		{
			doAddSigner(privateKey, GetSignerIdentifier(subjectKeyID), encryptionOID, digestOID,
				new DefaultSignedAttributeTableGenerator(signedAttr),
				new SimpleAttributeTableGenerator(unsignedAttr),
				signedAttr);
		}

		/**
		 * add a signer with extra signed/unsigned attributes based on generators.
		 */
		public void AddSigner(
			AsymmetricKeyParameter		privateKey,
			X509Certificate				cert,
			string						digestOID,
			CmsAttributeTableGenerator	signedAttrGen,
			CmsAttributeTableGenerator	unsignedAttrGen)
		{
			AddSigner(privateKey, cert, Helper.GetEncOid(privateKey, digestOID), digestOID,
				signedAttrGen, unsignedAttrGen);
		}

		/**
		 * add a signer, specifying the digest encryption algorithm, with extra signed/unsigned attributes based on generators.
		 */
		public void AddSigner(
			AsymmetricKeyParameter		privateKey,
			X509Certificate				cert,
			string						encryptionOID,
			string						digestOID,
			CmsAttributeTableGenerator	signedAttrGen,
			CmsAttributeTableGenerator	unsignedAttrGen)
		{
			doAddSigner(privateKey, GetSignerIdentifier(cert), encryptionOID, digestOID, signedAttrGen,
				unsignedAttrGen, null);
		}

	    /**
	     * add a signer with extra signed/unsigned attributes based on generators.
	     */
	    public void AddSigner(
			AsymmetricKeyParameter		privateKey,
	        byte[]						subjectKeyID,
	        string						digestOID,
	        CmsAttributeTableGenerator	signedAttrGen,
	        CmsAttributeTableGenerator	unsignedAttrGen)
	    {
			AddSigner(privateKey, subjectKeyID, Helper.GetEncOid(privateKey, digestOID), digestOID,
				signedAttrGen, unsignedAttrGen);
	    }

		/**
		 * add a signer, including digest encryption algorithm, with extra signed/unsigned attributes based on generators.
		 */
		public void AddSigner(
			AsymmetricKeyParameter		privateKey,
			byte[]						subjectKeyID,
			string						encryptionOID,
			string						digestOID,
			CmsAttributeTableGenerator	signedAttrGen,
			CmsAttributeTableGenerator	unsignedAttrGen)
		{
			doAddSigner(privateKey, GetSignerIdentifier(subjectKeyID), encryptionOID, digestOID,
				signedAttrGen, unsignedAttrGen, null);
		}

        public void AddSignerInfoGenerator(SignerInfoGenerator signerInfoGenerator)
        {
            signerInfs.Add(new SignerInf(this, signerInfoGenerator.contentSigner, signerInfoGenerator.sigId,
                            signerInfoGenerator.signedGen, signerInfoGenerator.unsignedGen, null));
        }

        private void doAddSigner(
			AsymmetricKeyParameter		privateKey,
			SignerIdentifier            signerIdentifier,
			string                      encryptionOID,
			string                      digestOID,
			CmsAttributeTableGenerator  signedAttrGen,
			CmsAttributeTableGenerator  unsignedAttrGen,
			Asn1.Cms.AttributeTable		baseSignedTable)
		{
			signerInfs.Add(new SignerInf(this, privateKey, signerIdentifier, digestOID, encryptionOID,
				signedAttrGen, unsignedAttrGen, baseSignedTable));
		}

		/**
        * generate a signed object that for a CMS Signed Data object
        */
        public CmsSignedData Generate(
            CmsProcessable content)
        {
            return Generate(content, false);
        }

        /**
        * generate a signed object that for a CMS Signed Data
        * object  - if encapsulate is true a copy
        * of the message will be included in the signature. The content type
        * is set according to the OID represented by the string signedContentType.
        */
        public CmsSignedData Generate(
            string			signedContentType,
			// FIXME Avoid accessing more than once to support CmsProcessableInputStream
            CmsProcessable	content,
            bool			encapsulate,
			string certThumbprint)//,
			//string signerUrl)
        {
            Asn1EncodableVector digestAlgs = new Asn1EncodableVector();
            Asn1EncodableVector signerInfos = new Asn1EncodableVector();

			_digests.Clear(); // clear the current preserved digest state

			//
            // add the precalculated SignerInfo objects.
            //
            foreach (SignerInformation signer in _signers)
            {
				digestAlgs.Add(Helper.FixAlgID(signer.DigestAlgorithmID));

				// TODO Verify the content type and calculated digest match the precalculated SignerInfo
				signerInfos.Add(signer.ToSignerInfo());
            }

			//
            // add the SignerInfo objects
            //
            bool isCounterSignature = (signedContentType == null);

            DerObjectIdentifier contentTypeOid = isCounterSignature
                ?   null
				:	new DerObjectIdentifier(signedContentType);

            foreach (SignerInf signer in signerInfs)
            {
				try
                {
					digestAlgs.Add(signer.DigestAlgorithmID);
					signerInfos.Add(signer.ToSignerInfo(contentTypeOid, content, rand, certThumbprint));//, signerUrl));
				}
                catch (IOException e)
                {
                    throw new CmsException("encoding error.", e);
                }
                catch (InvalidKeyException e)
                {
                    throw new CmsException("key inappropriate for signature.", e);
                }
                catch (SignatureException e)
                {
                    throw new CmsException("error creating signature.", e);
                }
                catch (CertificateEncodingException e)
                {
                    throw new CmsException("error creating sid.", e);
                }
            }

			Asn1Set certificates = null;

			if (_certs.Count != 0)
			{
				certificates = UseDerForCerts
                    ?   CmsUtilities.CreateDerSetFromList(_certs)
                    :   CmsUtilities.CreateBerSetFromList(_certs);
			}

			Asn1Set certrevlist = null;

			if (_crls.Count != 0)
			{
                certrevlist = UseDerForCrls
                    ?   CmsUtilities.CreateDerSetFromList(_crls)
                    :   CmsUtilities.CreateBerSetFromList(_crls);
            }

			Asn1OctetString octs = null;
			if (encapsulate)
            {
                MemoryStream bOut = new MemoryStream();
				if (content != null)
				{
	                try
	                {
	                    content.Write(bOut);
	                }
	                catch (IOException e)
	                {
	                    throw new CmsException("encapsulation error.", e);
	                }
				}
				octs = new BerOctetString(bOut.ToArray());
            }

            ContentInfo encInfo = new ContentInfo(contentTypeOid, octs);

            SignedData sd = new SignedData(
                new DerSet(digestAlgs),
                encInfo,
                certificates,
                certrevlist,
                new DerSet(signerInfos));

            ContentInfo contentInfo = new ContentInfo(CmsObjectIdentifiers.SignedData, sd);

            return new CmsSignedData(content, contentInfo);
        }

        /**
        * generate a signed object that for a CMS Signed Data
        * object - if encapsulate is true a copy
        * of the message will be included in the signature with the
        * default content type "data".
        */
        public CmsSignedData Generate(
            CmsProcessable	content,
            bool			encapsulate)
        {
			return this.Generate(Data, content, encapsulate, null);//,null);
        }

		/**
		* generate a set of one or more SignerInformation objects representing counter signatures on
		* the passed in SignerInformation object.
		*
		* @param signer the signer to be countersigned
		* @param sigProvider the provider to be used for counter signing.
		* @return a store containing the signers.
		*/
		public SignerInformationStore GenerateCounterSigners(
			SignerInformation signer)
		{
			//return this.Generate(null, new CmsProcessableByteArray(signer.GetSignature()), false, null, null).GetSignerInfos();
			return this.Generate(null, new CmsProcessableByteArray(signer.GetSignature()), false,null).GetSignerInfos();
		}
	}
}
