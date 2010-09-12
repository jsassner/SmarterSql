// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Security.Cryptography;
using System.Text;

namespace Sassner.SmarterSql.Utils.Security {
	internal static class ClientSideEncryption {
		// Encrypt message to send to client ----------------------------
		public static void RSAEncryptToClient(string source, out string encryptedCipher, out string encryptedSignature) {
			byte[] cleartext = Encoding.UTF8.GetBytes(source);
			byte[] cipher;
			byte[] signature;
			RSAEncryptToClient(cleartext, out cipher, out signature);
			encryptedCipher = Convert.ToBase64String(cipher);
			encryptedSignature = Convert.ToBase64String(signature);
		}

		public static void RSAEncryptToClient(byte[] source, out string encryptedCipher, out string encryptedSignature) {
			byte[] cipher;
			byte[] signature;
			RSAEncryptToClient(source, out cipher, out signature);
			encryptedCipher = Convert.ToBase64String(cipher);
			encryptedSignature = Convert.ToBase64String(signature);
		}

		public static void RSAEncryptToClient(byte[] source, out byte[] cipher, out byte[] signature) {
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
			rsa.FromXmlString(Common.client_private);
			cipher = rsa.Encrypt(source, false);
			signature = rsa.SignData(cipher, new SHA1CryptoServiceProvider());
		}

		// Encrypt message to send to server ----------------------------
		public static void RSAEncryptToServer(string source, out string encryptedCipher, out string encryptedSignature) {
			byte[] cleartext = Encoding.UTF8.GetBytes(source);

			byte[] cipher;
			byte[] signature;
			RSAEncryptToServer(cleartext, out cipher, out signature);
			encryptedCipher = Convert.ToBase64String(cipher);
			encryptedSignature = Convert.ToBase64String(signature);
		}

		public static void RSAEncryptToServer(byte[] source, out string encryptedCipher, out string encryptedSignature) {
			byte[] cipher;
			byte[] signature;
			RSAEncryptToServer(source, out cipher, out signature);
			encryptedCipher = Convert.ToBase64String(cipher);
			encryptedSignature = Convert.ToBase64String(signature);
		}

		public static void RSAEncryptToServer(byte[] source, out byte[] cipher, out byte[] signature) {
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
			rsa.FromXmlString(Common.server_public);
			cipher = rsa.Encrypt(source, false);

			rsa.FromXmlString(Common.client_private);
			signature = rsa.SignData(cipher, new SHA1CryptoServiceProvider());
		}

		// Decrypt message sent from client ----------------------------
		public static byte[] RSADecryptFromClient(string encryptedCipher, string encryptedSignature) {
			byte[] cipher = Convert.FromBase64String(encryptedCipher);
			byte[] signature = Convert.FromBase64String(encryptedSignature);

			return RSADecryptFromClient(cipher, signature);
		}

		public static byte[] RSADecryptFromClient(byte[] cipher, byte[] signature) {
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
			rsa.FromXmlString(Common.client_private);
			if (rsa.VerifyData(cipher, new SHA1CryptoServiceProvider(), signature)) {
				return rsa.Decrypt(cipher, false);
			}
			return null;
		}

		// Decrypt message sent from server ----------------------------
		public static byte[] RSADecryptFromServer(string encryptedCipher, string encryptedSignature) {
			byte[] cipher = Convert.FromBase64String(encryptedCipher);
			byte[] signature = Convert.FromBase64String(encryptedSignature);

			return RSADecryptFromServer(cipher, signature);
		}

		public static byte[] RSADecryptFromServer(byte[] cipher, byte[] signature) {
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
			rsa.FromXmlString(Common.server_public);
			if (rsa.VerifyData(cipher, new SHA1CryptoServiceProvider(), signature)) {
				rsa.FromXmlString(Common.client_private);
				return rsa.Decrypt(cipher, false);
			}
			return null;
		}
	}
}