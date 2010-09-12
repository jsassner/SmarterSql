// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Sassner.SmarterSql.Utils.Security {
	/// <summary>
	/// SymmCrypto is a wrapper of System.Security.Cryptography.SymmetricAlgorithm classes
	/// and simplifies the interface. It supports customized SymmetricAlgorithm as well.
	/// </summary>
	public class SymmCrypto {
		#region Member variables

		/// <remarks>
		/// Supported .Net intrinsic SymmetricAlgorithm classes.
		/// </remarks>
		public enum SymmProvEnum {
			DES,
			RC2,
			Rijndael,
		}

		private readonly SymmetricAlgorithm objCryptoService;

		#endregion

		/// <remarks>
		/// Constructor for using an intrinsic .Net SymmetricAlgorithm class.
		/// </remarks>
		public SymmCrypto(SymmProvEnum symmProv) {
			switch (symmProv) {
				case SymmProvEnum.DES:
					objCryptoService = new DESCryptoServiceProvider();
					break;

				case SymmProvEnum.RC2:
					objCryptoService = new RC2CryptoServiceProvider();
					break;

				case SymmProvEnum.Rijndael:
					objCryptoService = new RijndaelManaged();
					break;
			}
		}

		/// <remarks>
		/// Constructor for using a customized SymmetricAlgorithm class.
		/// </remarks>
		public SymmCrypto(SymmetricAlgorithm ServiceProvider) {
			objCryptoService = ServiceProvider;
		}

		/// <summary>
		/// Encrypt the string with the supplied key
		/// </summary>
		/// <param name="source"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public string Encrypt(string source, string key) {
			byte[] bytIn = Encoding.UTF8.GetBytes(source);
			return Convert.ToBase64String(Encrypt(bytIn, key));
		}

		/// <summary>
		/// Encrypt the string with the supplied key
		/// </summary>
		/// <param name="source"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public byte[] Encrypt(byte[] source, string key) {
			if (null == objCryptoService) {
				return null;
			}

			// create a MemoryStream so that the process can be done without I/O files
			MemoryStream ms = new MemoryStream();

			Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes("This is my salt string"));
			objCryptoService.BlockSize = objCryptoService.LegalBlockSizes[0].MaxSize;
			objCryptoService.KeySize = objCryptoService.LegalKeySizes[0].MaxSize;
			objCryptoService.Key = rfc.GetBytes(objCryptoService.KeySize / 8);
			objCryptoService.IV = rfc.GetBytes(objCryptoService.BlockSize / 8);
			//			objCryptoService.Key = UTF8Encoding.UTF8.GetBytes(key.PadRight(objCryptoService.KeySize / 8).Substring(0, objCryptoService.KeySize / 8));
			//			objCryptoService.IV = UTF8Encoding.UTF8.GetBytes(key.PadRight(objCryptoService.BlockSize / 8).Substring(0, objCryptoService.BlockSize / 8));

			// create an Encryptor from the Provider Service instance
			ICryptoTransform encrypto = objCryptoService.CreateEncryptor();

			// create Crypto Stream that transforms a stream using the encryption
			CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);

			// write out encrypted content into MemoryStream
			cs.Write(source, 0, source.Length);
			cs.FlushFinalBlock();
			return ms.ToArray();
		}

		/// <summary>
		/// Decrypt the string with the supplied key
		/// </summary>
		/// <param name="source"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public string Decrypt(string source, string key) {
			byte[] bytIn = Convert.FromBase64String(source);
			return Encoding.UTF8.GetString(Decrypt(bytIn, key));
		}

		/// <summary>
		/// Decrypt the string with the supplied key
		/// </summary>
		/// <param name="source"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public byte[] Decrypt(byte[] source, string key) {
			if (null == objCryptoService) {
				return null;
			}

			// create a MemoryStream with the input
			MemoryStream ms = new MemoryStream(source, 0, source.Length);

			Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes("This is my salt string"));
			objCryptoService.BlockSize = objCryptoService.LegalBlockSizes[0].MaxSize;
			objCryptoService.KeySize = objCryptoService.LegalKeySizes[0].MaxSize;
			objCryptoService.Key = rfc.GetBytes(objCryptoService.KeySize / 8);
			objCryptoService.IV = rfc.GetBytes(objCryptoService.BlockSize / 8);
			//			objCryptoService.Key = UTF8Encoding.UTF8.GetBytes(key.PadRight(objCryptoService.KeySize / 8).Substring(0, objCryptoService.KeySize / 8));
			//			objCryptoService.IV = UTF8Encoding.UTF8.GetBytes(key.PadRight(objCryptoService.BlockSize / 8).Substring(0, objCryptoService.BlockSize / 8));

			// create a Decryptor from the Provider Service instance
			ICryptoTransform encrypto = objCryptoService.CreateDecryptor();

			// create Crypto Stream that transforms a stream using the decryption
			CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);

			// read out the result from the Crypto Stream
			StreamReader sr = new StreamReader(cs);
			string data = sr.ReadToEnd();
			return Encoding.UTF8.GetBytes(data);
		}
	}
}