using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.OpenSsl;

namespace ShopDev.Utils.Security
{
    public static class CryptographyUtils
    {
        /// <summary>
        /// Tính toán hash SHA 256
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string ComputeSha256Hash(params string[] values)
        {
            string rawData = string.Join("", values);
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));
            return Convert.ToHexString(bytes);
        }

        public static string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        /// <summary>
        /// Tính toán hash MD5
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string ComputeMD5(params string[] values)
        {
            string rawData = string.Join("", values);
            byte[] inputBytes = Encoding.UTF8.GetBytes(rawData);
            byte[] hashBytes = MD5.HashData(inputBytes);
            return Convert.ToHexString(hashBytes);
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = MD5.HashData(inputBytes);
            return Convert.ToHexString(hashBytes);
        }

        public static (string, string) CreateAES()
        {
            Aes myAes = Aes.Create();
            return (Convert.ToHexString(myAes.Key), Convert.ToHexString(myAes.IV));
        }

        /// <summary>
        /// Mã hoá AES
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="keyHex"></param>
        /// <param name="IVHex"></param>
        /// <returns></returns>
        public static string EncryptString_Aes(string plainText, string keyHex, string IVHex)
        {
            byte[] result = EncryptStringToBytes_Aes(
                plainText,
                Convert.FromHexString(keyHex),
                Convert.FromHexString(IVHex)
            );
            return Convert.ToHexString(result);
        }

        /// <summary>
        /// Giải mã AES
        /// </summary>
        /// <param name="cipherTextHex"></param>
        /// <param name="keyHex"></param>
        /// <param name="IVHex"></param>
        /// <returns></returns>
        public static string DecryptString_Aes(string cipherTextHex, string keyHex, string IVHex)
        {
            return DecryptStringFromBytes_Aes(
                Convert.FromHexString(cipherTextHex),
                Convert.FromHexString(keyHex),
                Convert.FromHexString(IVHex)
            );
        }

        public static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException(nameof(Key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using MemoryStream msEncrypt = new();
                using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    //Write all data to the stream.
                    swEncrypt.Write(plainText);
                }
                encrypted = msEncrypt.ToArray();
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException(nameof(Key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));

            // Declare the string used to hold
            // the decrypted text.
            string? plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using MemoryStream msDecrypt = new(cipherText);
                using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new(csDecrypt);

                // Read the decrypted bytes from the decrypting stream
                // and place them in a string.
                plaintext = srDecrypt.ReadToEnd();
            }

            return plaintext;
        }

        /// <summary>
        /// aes-256-cbc
        /// </summary>
        public static string DecryptAES256CBC(string encryptedText, string key, string iv)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return string.Empty;
            encryptedText = encryptedText.Replace("%2b", "+");
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Key = keyBytes;
            aes.IV = ivBytes;
            ICryptoTransform decryptor = aes.CreateDecryptor();
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] plainTextBytes = decryptor.TransformFinalBlock(
                cipherTextBytes,
                0,
                cipherTextBytes.Length
            );
            return Encoding.UTF8.GetString(plainTextBytes);
        }

        public static RsaSecurityKey ReadKey(string publicKeyPath, string privateKeyPath)
        {
            // Đọc khóa công khai từ file
            string publicKeyPem = File.ReadAllText(publicKeyPath);
            var publicKeyObject = new PemReader(new StringReader(publicKeyPem)).ReadPemObject();
            byte[] publicKey = publicKeyObject.Content;

            // Đọc khóa bí mật từ file
            string privateKeyPem = File.ReadAllText(privateKeyPath);
            var privateKeyObject = new PemReader(new StringReader(privateKeyPem)).ReadPemObject();
            byte[] privateKey = privateKeyObject.Content;

            RSACryptoServiceProvider rsa = new();
            rsa.ImportRSAPublicKey(publicKey, out int _);
            rsa.ImportRSAPrivateKey(privateKey, out int _);

            //rsaSecurityKey.ExportParameters(true); //param kèm private key
            return new RsaSecurityKey(rsa.ExportParameters(true));
        }

        public static string StreamToBase64(MemoryStream stream)
        {
            // Convert the MemoryStream to a byte array
            byte[] byteArray = stream.ToArray();

            // Convert the byte array to a Base64 string
            string base64String = Convert.ToBase64String(byteArray);

            return base64String;
        }

        public static bool VerifySignature(string data, string signature, string publicKeyPath)
        {
            string publicKeyPem = File.ReadAllText(publicKeyPath);
            var publicKeyObject = new PemReader(new StringReader(publicKeyPem)).ReadPemObject();
            byte[] publicKey = publicKeyObject.Content;
            RSACryptoServiceProvider rsa = new();
            rsa.ImportSubjectPublicKeyInfo(publicKey, out _);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] signatureBytes = Convert.FromBase64String(signature);

            return rsa.VerifyData(
                dataBytes,
                signatureBytes,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );
        }

        public static bool ValidateSignature(string inputHash, string secretKey, string rspRaw)
        {
            string myChecksum = HmacSHA512(secretKey, rspRaw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
