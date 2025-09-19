using System.Text;
using System.Security.Cryptography;

namespace r3Take.Utils
{
    #region Enumeration

    public enum EncryptionType
    {
        MD5, SHA1, SHA256, SHA384, SHA512, BASE64,
    }
    public enum DecryptionType
    {
        BASE64,
    }

    #endregion

    class Encrypt
    {
        #region MD5
        /// <summary>
        /// Se encarga de generar un ByteArray con la cadena encriptada por medio del algoritmo MD5
        /// </summary>
        /// <param name="text">Cadena a Encriptar por medio del algoritmo MD5</param>
        /// <returns></returns>
        private string getMd5(string text)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            MD5CryptoServiceProvider md5hasher = new MD5CryptoServiceProvider();
            byte[] hashedDataBytes = md5hasher.ComputeHash(encoder.GetBytes(text));
            return byteArrayToString(hashedDataBytes);
        }
        #endregion

        #region SHA-1
        /// <summary>
        /// Se encarga de generar un ByteArray con la cadena encriptada por medio del algoritmo SHA-1
        /// </summary>
        /// <param name="text">Cadena a Encriptar por medio del Malgoritmo SHA-1</param>
        /// <returns></returns>
        private string getSHA1(string text)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            SHA1CryptoServiceProvider sha1hasher = new SHA1CryptoServiceProvider();
            byte[] hashedDataBytes = sha1hasher.ComputeHash(encoder.GetBytes(text));
            return byteArrayToString(hashedDataBytes);
        }
        #endregion

        #region SHA-256
        /// <summary>
        /// Se encarga de generar un ByteArray con la cadena encriptada por medio del algoritmo SHA-256
        /// </summary>
        /// <param name="text">Cadena a Encriptar por medio del algoritmo SHA-256</param>
        /// <returns></returns>
        private string getSHA256(string text)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            SHA256Managed sha256hasher = new SHA256Managed();
            byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(text));
            return byteArrayToString(hashedDataBytes);
        }
        #endregion

        #region SHA-384
        /// <summary>
        /// Se encarga de generar un ByteArray con la cadena encriptada por medio del algoritmo SHA-384
        /// </summary>
        /// <param name="text">Cadena a Encriptar por medio del Malgoritmo SHA-384</param>
        /// <returns></returns>
        private string getSHA384(string text)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            SHA384Managed sha384hasher = new SHA384Managed();
            byte[] hashedDataBytes = sha384hasher.ComputeHash(encoder.GetBytes(text));
            return byteArrayToString(hashedDataBytes);
        }
        #endregion

        #region SHA-512
        /// <summary>
        /// Se encarga de generar un ByteArray con la cadena encriptada por medio del algoritmo SHA-512
        /// </summary>
        /// <param name="text">Cadena a Encriptar por medio del algoritmo SHA-512</param>
        /// <returns></returns>
        private static string getSHA512(string text)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            SHA512Managed sha512hasher = new SHA512Managed();
            byte[] hashedDataBytes = sha512hasher.ComputeHash(encoder.GetBytes(text));
            return byteArrayToString(hashedDataBytes);
        }
        #endregion

        #region Encode Base64
        /// <summary>
        /// Se encarga de generar una cadena encriptada por medio del algoritmo Encode Base-64
        /// </summary>
        /// <param name="text">Cadena a Encriptar por medio del algoritmo Encode Base-64</param>
        /// <returns></returns>
        private string getEncodeBase64(string text)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(text);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
        #endregion

        #region Decode Base64
        /// <summary>
        /// Se encarga de generar una cadena encriptada por medio del algoritmo Decode Base-64
        /// </summary>
        /// <param name="text">Cadena a Encriptar por medio del algoritmo Decode Base-64</param>
        /// <returns></returns>
        private string getDecodeBase64(string text)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(text);
            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }
        #endregion

        #region GET Encryption
        /// <summary>
        /// Se encarga de obtener la cadena encriptada por medio de alguno de los algoritmos implementados.
        /// </summary>
        /// <param name="type">Tipo de Encriptadción deseada</param>
        /// <param name="text">Cadena a Encriptar por medio del algoritmo Encode Base-64</param>
        /// <returns></returns>
        public string getEncryptionCode(EncryptionType type, string text)
        {
            switch (type)
            {
                case EncryptionType.MD5:
                    { return getMd5(text); }
                case EncryptionType.SHA1:
                    { return getSHA1(text); }
                case EncryptionType.SHA256:
                    { return getSHA256(text); }
                case EncryptionType.SHA384:
                    { return getSHA384(text); }
                case EncryptionType.SHA512:
                    { return getSHA512(text); }
                case EncryptionType.BASE64:
                    { return getEncodeBase64(text); }
                default: return "";
            }
        }
        #endregion

        #region GET Decryption
        /// <summary>
        /// Se encarga de obtener la cadena desencriptada solo del algoritmo Decode Base-64.
        /// </summary>
        /// <param name="type">Tipo de Encriptadción deseada</param>
        /// <param name="text">Cadena a Desencriptar por medio del algoritmo Decode Base-64</param>
        /// <returns></returns>
        public string getDecryptionCode(DecryptionType type, string text)
        {
            switch (type)
            {
                case DecryptionType.BASE64:
                    { return getDecodeBase64(text); }
                default: return "";
            }
        }
        #endregion

        #region ByteArray ToString
        /// <summary>
        /// Se encarga de hacer un Cats del Byte Array Obtenido de los algoritmos de encriptación a cadena.
        /// </summary>
        /// <param name="inputArray">Byte Array con la cadena encriptada</param>
        /// <returns></returns>
        private static string byteArrayToString(byte[] inputArray)
        {
            StringBuilder output = new StringBuilder("");
            for (int i = 0; i < inputArray.Length; i++)
            {
                output.Append(inputArray[i].ToString("X2"));
            }
            return output.ToString();
        }
        #endregion
    }
}
