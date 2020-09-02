/*
 *  Copyright 2013-2017 Vitaliy Fedorchenko
 *
 *  Licensed under PdfGenerator Source Code Licence (see LICENSE file).
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS 
 *  OF ANY KIND, either express or implied.
 */ 

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Reflection;
#if !NET_STANDARD
using System.Configuration;
#endif
using System.Security.Cryptography;

namespace NReco.PdfGenerator {
	
	/// <summary>
	/// Represents PdfGenerator commercial license information.
	/// </summary>
	public sealed class LicenseInfo {

		/// <summary>
		/// Determines if component has activated license key.
		/// </summary>
		public bool IsLicensed {
			get { return I.IsLicensed; }
		}

		/// <summary>
		/// License owner identifier.
		/// </summary>
		public string LicenseOwner {
			get { return I.Owner; }
		}

		private Info I;

		internal LicenseInfo() {
			I = new Info();
#if LICENSE_FULL
			I.IsLicensed = true;
			I.Owner = "Full Source Code License";
#else
			I.IsLicensed = false;
#endif
		}

		internal void Check() {
#if !LICENSE_FULL
			if (IsLicensed && !String.IsNullOrEmpty(LicenseOwner)) return;
			
			#if NET_STANDARD || NET_STANDARD2
			string licenseKey = null;
			string licenseOwner = null;
			#else
			var licenseKey = ConfigurationSettings.AppSettings["NReco.PdfGenerator.LicenseKey"];
			var licenseOwner = ConfigurationSettings.AppSettings["NReco.PdfGenerator.LicenseOwner"];
			#endif

			if (!String.IsNullOrEmpty(licenseKey) && !String.IsNullOrEmpty(licenseOwner)) {
				SetLicenseKey(licenseOwner, licenseKey);
				if (IsLicensed && !String.IsNullOrEmpty(LicenseOwner)) return;
			}
			
			throw new Exception("This feature requires PdfGenerator commercial license key: http://www.nrecosite.com/pdf_generator_net.aspx");		
#endif
		}

		/// <summary>
		/// Activate component license and enable restricted features.
		/// </summary>
		/// <param name="owner">license owner ID</param>
		/// <param name="key">unique license key from component order's page</param>
		public void SetLicenseKey(string owner, string key) {
#if LICENSE_FULL
			I.IsLicensed = true;
			I.Owner = owner;
#else
			var licenseKeyBytes = GetLicenseKeyBytes(key);
			#if NET_STANDARD
			var publicKeyBytes = typeof(LicenseInfo).GetTypeInfo().Assembly.GetName().GetPublicKey();
			#else
			var publicKeyBytes = typeof(LicenseInfo).Assembly.GetName().GetPublicKey();
			#endif
			if (publicKeyBytes==null)
				throw new Exception("PdfGenerator is not strongly signed");
#if NET_STANDARD
			using (var rsa = System.Security.Cryptography.RSACng.Create()) {
				var rsaParams = GetPublicKeyRSAParameters(publicKeyBytes);
				rsa.ImportParameters(rsaParams);

				try { 
					var ownerBytes = System.Text.Encoding.UTF8.GetBytes(owner);
					if (!rsa.VerifyData(ownerBytes, licenseKeyBytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1 ))
						throw new Exception();
					I.Owner = owner;
					I.IsLicensed = !String.IsNullOrEmpty(I.Owner);
				} catch (Exception ex) {
					throw new Exception("Invalid license owner or key");
				}
			}
#else

#if NET_STANDARD2
			using (var rsa = new RSACryptoServiceProvider()) {
#else
			CspParameters cspParams = new CspParameters();
			// workaround for: CryptographicException: The system cannot find the file specified
			// in ASP.NET with highly restricted credentials that have no user keystore
			cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
			using (var rsa = new RSACryptoServiceProvider(cspParams)) {
#endif
				var rsaParams = GetPublicKeyRSAParameters(publicKeyBytes);
				rsa.PersistKeyInCsp = false;
				rsa.ImportParameters(rsaParams);

				var hashAlgorithm = new SHA1CryptoServiceProvider();
				
				try { 
					var ownerBytes = System.Text.Encoding.UTF8.GetBytes(owner);
					if (!rsa.VerifyData(ownerBytes, hashAlgorithm, licenseKeyBytes))
						throw new Exception();
					I.Owner = owner;
					I.IsLicensed = !String.IsNullOrEmpty(I.Owner);
				} catch (Exception ex) {
					throw new Exception("Invalid license owner or key");
				}
			}

#endif

#endif
		}

#if !LICENSE_FULL
		private byte[] GetLicenseKeyBytes(string key) {
			try {
				return  Convert.FromBase64String( key );
			} catch {
				throw new Exception("Invalid license key");
			}
		}

		private const int magic_pub_idx = 0x14;
		private const int magic_size = 4;

		private static RSAParameters GetPublicKeyRSAParameters(byte[] keyBytes) {
			RSAParameters ret = new RSAParameters();

			if ((keyBytes == null) || (keyBytes.Length < 1))
				throw new ArgumentNullException("keyBytes");

			bool pubonly = true;

			int magic_idx = magic_pub_idx;

			// Bitlen is stored here, but note this class is only set up for 1024 bit length keys
			int bitlen_idx = magic_idx + magic_size;
			int bitlen_size = 4;  // DWORD

			// Exponent, In read file, will usually be { 1, 0, 1, 0 } or 65537
			int exp_idx = bitlen_idx + bitlen_size;
			int exp_size = 4;


			//BYTE modulus[rsapubkey.bitlen/8]; == MOD; Size 128
			int mod_idx = exp_idx + exp_size;
			int mod_size = 128;

			//BYTE prime1[rsapubkey.bitlen/16]; == P; Size 64
			int p_idx = mod_idx + mod_size;
			int p_size = 64;

			//BYTE prime2[rsapubkey.bitlen/16]; == Q; Size 64
			int q_idx = p_idx + p_size;
			int q_size = 64;

			//BYTE exponent1[rsapubkey.bitlen/16]; == DP; Size 64
			int dp_idx = q_idx + q_size;
			int dp_size = 64;

			//BYTE exponent2[rsapubkey.bitlen/16]; == DQ; Size 64
			int dq_idx = dp_idx + dp_size;
			int dq_size = 64;

			//BYTE coefficient[rsapubkey.bitlen/16]; == InverseQ; Size 64
			int invq_idx = dq_idx + dq_size;
			int invq_size = 64;

			//BYTE privateExponent[rsapubkey.bitlen/8]; == D; Size 128
			int d_idx = invq_idx + invq_size;
			int d_size = 128;


			// Figure public params, Must reverse order (little vs. big endian issue)
			ret.Exponent = BlockCopy(keyBytes, exp_idx, exp_size);
			Array.Reverse(ret.Exponent);
			ret.Modulus = BlockCopy(keyBytes, mod_idx, mod_size);
			Array.Reverse(ret.Modulus);

			if (pubonly) return ret;

			// Figure private params
			// Must reverse order (little vs. big endian issue)
			ret.P = BlockCopy(keyBytes, p_idx, p_size);
			Array.Reverse(ret.P);

			ret.Q = BlockCopy(keyBytes, q_idx, q_size);
			Array.Reverse(ret.Q);

			ret.DP = BlockCopy(keyBytes, dp_idx, dp_size);
			Array.Reverse(ret.DP);

			ret.DQ = BlockCopy(keyBytes, dq_idx, dq_size);
			Array.Reverse(ret.DQ);

			ret.InverseQ = BlockCopy(keyBytes, invq_idx, invq_size);
			Array.Reverse(ret.InverseQ);

			ret.D = BlockCopy(keyBytes, d_idx, d_size);
			Array.Reverse(ret.D);

			return ret;
		}

		private static byte[] BlockCopy(byte[] source, int startAt, int size) {
			if ((source == null) || (source.Length < (startAt + size)))
				return null;

			byte[] ret = new byte[size];
			Buffer.BlockCopy(source, startAt, ret, 0, size);
			return ret;
		}
#endif

		internal sealed class Info {
			internal Info() { }
			internal bool IsLicensed;
			internal string Owner;
		}


	}

}
