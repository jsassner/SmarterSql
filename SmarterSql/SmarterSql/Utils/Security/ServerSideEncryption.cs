// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
namespace Sassner.SmarterSql.Utils.Security {
	internal static class ServerSideEncryption {
		//		public static void RSAEncrypt(string source, out string encryptedCipher, out string encryptedSignature) {
		//			byte[] cipher;
		//			byte[] signature;
		//			RSAEncrypt(source, out cipher, out signature);
		//			encryptedCipher = Convert.ToBase64String(cipher);
		//			encryptedSignature = Convert.ToBase64String(signature);
		//		}
		//
		//		public static void RSAEncrypt(string source, out byte[] cipher, out byte[] signature) {
		//			byte[] cleartext = UTF8Encoding.UTF8.GetBytes(source);
		//
		//			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
		//			rsa.FromXmlString(Common.client_public);
		//			cipher = rsa.Encrypt(cleartext, false);
		//
		//			rsa.FromXmlString(Common.server_private);
		//			signature = rsa.SignData(cipher, new SHA1CryptoServiceProvider());
		//		}
		//
		//		public static string RSADecrypt(string encryptedCipher, string encryptedSignature) {
		//			byte[] cipher = Convert.FromBase64String(encryptedCipher);
		//			byte[] signature = Convert.FromBase64String(encryptedSignature);
		//
		//			return RSADecrypt(cipher, signature);
		//		}
		//
		//		public static string RSADecrypt(byte[] cipher, byte[] signature) {
		//			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
		//			rsa.FromXmlString(Common.client_public);
		//			if (rsa.VerifyData(cipher, new SHA1CryptoServiceProvider(), signature)) {
		//				rsa.FromXmlString(Common.server_private);
		//				byte[] cleartext_after_decryption = rsa.Decrypt(cipher, false);
		//				return UTF8Encoding.UTF8.GetString(cleartext_after_decryption);
		//			}
		//			return "";
		//		}
	}
}