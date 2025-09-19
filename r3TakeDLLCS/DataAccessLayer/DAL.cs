#region "Using"

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;

#endregion

namespace r3Take.DataAccessLayer
{
    public class DAL
    {
        public string mMensajeError = string.Empty;
        // Parametros para Encriptar y Desencriptar 
        private readonly string passPhrase = "P@55pr@s3";           // can be any string
        private readonly string saltValue = "5@1tV@1u3";            // can be any string
        private readonly string hashAlgorithm = "SHA1";             // can be "MD5"
        private readonly int passwordIterations = 2;                // can be any number
        private readonly string initVector = "@1B2c3D4e5F6g7H8";    // must be 16 bytes
        private readonly int keySize = 256;                         // can be 192 or 128

        #region "Encrypt"
        /// <summary>
        /// Encrypts specified plaintext using Rijndael symmetric key algorithm
        /// and returns a base64-encoded result.
        /// </summary>
        /// <param name="plainText">
        /// Plaintext value to be encrypted.
        /// </param>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived. The
        /// derived password will be used to generate the encryption key.
        /// Passphrase can be any string. In this example we assume that this
        /// passphrase is an ASCII string.
        /// </param>
        /// <param name="saltValue">
        /// Salt value used along with passphrase to generate password. Salt can
        /// be any string. In this example we assume that salt is an ASCII string.
        /// </param>
        /// <param name="hashAlgorithm">
        /// Hash algorithm used to generate password. Allowed values are: "MD5" and
        /// "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
        /// </param>
        /// <param name="passwordIterations">
        /// Number of iterations used to generate password. One or two iterations
        /// should be enough.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector (or IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be 
        /// exactly 16 ASCII characters long.
        /// </param>
        /// <param name="keySize">
        /// Size of encryption key in bits. Allowed values are: 128, 192, and 256. 
        /// Longer keys are more secure than shorter keys.
        /// </param>
        /// <returns>
        /// Encrypted value formatted as a base64-encoded string.
        /// </returns>
        public static string Encrypt(string plainText,
                                 string passPhrase,
                                 string saltValue,
                                 string hashAlgorithm,
                                 int passwordIterations,
                                 string initVector,
                                 int keySize)
        {
            // Convert strings into byte arrays.
            // Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8 
            // encoding.
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Convert our plaintext into a byte array.
            // Let us assume that plaintext contains UTF8-encoded characters.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // First, we must create a password, from which the key will be derived.
            // This password will be generated from the specified passphrase and 
            // salt value. The password will be created using the specified hash 
            // algorithm. Password creation can be done in several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                            passPhrase,
                                                            saltValueBytes,
                                                            hashAlgorithm,
                                                            passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(keySize / 8);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate encryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(
                                                             keyBytes,
                                                             initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream();

            // Define cryptographic stream (always use Write mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                         encryptor,
                                                         CryptoStreamMode.Write);
            // Start encrypting.
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

            // Finish encrypting.
            cryptoStream.FlushFinalBlock();

            // Convert our encrypted data from a memory stream into a byte array.
            byte[] cipherTextBytes = memoryStream.ToArray();

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert encrypted data into a base64-encoded string.
            string cipherText = Convert.ToBase64String(cipherTextBytes);

            // Return encrypted string.
            return cipherText;
        }

        #endregion

        #region "Decrypt"

        /// <summary>
        /// Decrypts specified ciphertext using Rijndael symmetric key algorithm.
        /// </summary>
        /// <param name="cipherText">
        /// Base64-formatted ciphertext value.
        /// </param>
        /// <param name="passPhrase">
        /// Passphrase from which a pseudo-random password will be derived. The
        /// derived password will be used to generate the encryption key.
        /// Passphrase can be any string. In this example we assume that this
        /// passphrase is an ASCII string.
        /// </param>
        /// <param name="saltValue">
        /// Salt value used along with passphrase to generate password. Salt can
        /// be any string. In this example we assume that salt is an ASCII string.
        /// </param>
        /// <param name="hashAlgorithm">
        /// Hash algorithm used to generate password. Allowed values are: "MD5" and
        /// "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
        /// </param>
        /// <param name="passwordIterations">
        /// Number of iterations used to generate password. One or two iterations
        /// should be enough.
        /// </param>
        /// <param name="initVector">
        /// Initialization vector (or IV). This value is required to encrypt the
        /// first block of plaintext data. For RijndaelManaged class IV must be
        /// exactly 16 ASCII characters long.
        /// </param>
        /// <param name="keySize">
        /// Size of encryption key in bits. Allowed values are: 128, 192, and 256.
        /// Longer keys are more secure than shorter keys.
        /// </param>
        /// <returns>
        /// Decrypted string value.
        /// </returns>
        /// <remarks>
        /// Most of the logic in this function is similar to the Encrypt
        /// logic. In order for decryption to work, all parameters of this function
        /// - except cipherText value - must match the corresponding parameters of
        /// the Encrypt function which was called to generate the
        /// ciphertext.
        /// </remarks>
        public static string Decrypt(string cipherText,
                                     string passPhrase,
                                     string saltValue,
                                     string hashAlgorithm,
                                     int passwordIterations,
                                     string initVector,
                                     int keySize)
        {
            // Convert strings defining encryption key characteristics into byte
            // arrays. Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Convert our ciphertext into a byte array.
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            // First, we must create a password, from which the key will be 
            // derived. This password will be generated from the specified 
            // passphrase and salt value. The password will be created using
            // the specified hash algorithm. Password creation can be done in
            // several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                            passPhrase,
                                                            saltValueBytes,
                                                            hashAlgorithm,
                                                            passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(keySize / 8);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate decryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(
                                                             keyBytes,
                                                             initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            // Define cryptographic stream (always use Read mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                          decryptor,
                                                          CryptoStreamMode.Read);

            // Since at this point we don't know what the size of decrypted data
            // will be, allocate the buffer long enough to hold ciphertext;
            // plaintext is never longer than ciphertext.
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            // Start decrypting.
            int decryptedByteCount = cryptoStream.Read(plainTextBytes,
                                                       0,
                                                       plainTextBytes.Length);

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert decrypted data into a string. 
            // Let us assume that the original plaintext string was UTF8-encoded.
            string plainText = Encoding.UTF8.GetString(plainTextBytes,
                                                       0,
                                                       decryptedByteCount);

            // Return decrypted string.   
            return plainText;
        }

        #endregion

        #region "getDataSources"

        public void getDataSources(HttpContext hc)
        {
            DataTable dtDS = new DataTable();

            string argsConnString = ConfigurationManager.AppSettings["DS_r3"];
            String[] argsConfManager = argsConnString.Split(new Char[] { '#' });
            string ConnectionString = argsConfManager[1];
            DatabaseType dbtype = (DatabaseType)Enum.Parse(typeof(DatabaseType), argsConfManager[0]);
            IDbConnection cnn = DataFactory.CreateConnection(ConnectionString, dbtype);
            IDbCommand cmd = DataFactory.CreateCommand("SELECT aliasDS, connectionString FROM SYS_DataSources WHERE ST = 1", dbtype, cnn);
            DbDataAdapter da = DataFactory.CreateAdapter(cmd, dbtype);

            try
            {
                ConstruyeParametros(argsConfManager[0], da, string.Empty, null);
                da.Fill(dtDS);
                da.Dispose();
                da = null;

                int numDS = dtDS.Rows.Count;

                if (numDS > 0)
                {
                    for (int i = 0; i < numDS; i++)
                    {
                        string encDS = Encrypt(dtDS.Rows[i][0].ToString(), passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize);
                        string encCS = Encrypt(dtDS.Rows[i][1].ToString(), passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize);

                        hc.Application.Add(encDS, encCS);

                        encDS = string.Empty;
                        encCS = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error No: " + ex.Message + "Código de Error: " + ex.GetType());
            }
        }

        #endregion

        #region "objConnectionCrypt"

        public ArrayList objConnectionCrypt(string datasource, string query, HttpContext hc)
        {
            ArrayList arrCon = new ArrayList(4);

            String[] arrDS = hc.Application.AllKeys;

            string dsEnc = Encrypt(datasource, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize);

            bool findDS = false;

            foreach (string varDS in arrDS)
            {
                if (varDS == dsEnc)
                {
                    findDS = true;
                    break;
                }
            }

            if (!findDS)
            {
                getDataSources(hc);
            }

            string argsConnString = hc.Application[dsEnc].ToString();
            string argsConnStringDesc = Decrypt(argsConnString, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize);
            String[] argsConfManager = argsConnStringDesc.Split(new Char[] { '#' });
            string ConnectionString = argsConfManager[1];

            DatabaseType dbtype = (DatabaseType)Enum.Parse(typeof(DatabaseType), argsConfManager[0]);
            IDbConnection cnn = DataFactory.CreateConnection(ConnectionString, dbtype);
            IDbCommand cmd = DataFactory.CreateCommand(query, dbtype, cnn);
            DbDataAdapter da = DataFactory.CreateAdapter(cmd, dbtype);

            arrCon.Add(argsConfManager);
            arrCon.Add(cnn);
            arrCon.Add(cmd);
            arrCon.Add(da);
            arrCon.Add(dbtype);

            return arrCon;
        }

        #endregion

        #region "objConnection"

        public ArrayList objConnection(string datasource, string query, HttpContext hc)
        {
            ArrayList arrCon = new ArrayList(4);

            string argsConnString = ConfigurationManager.AppSettings[datasource];
            String[] argsConfManager = argsConnString.Split(new Char[] { '°' });
            string ConnectionString = argsConfManager[1];

            DatabaseType dbtype = (DatabaseType)Enum.Parse(typeof(DatabaseType), argsConfManager[0]);
            IDbConnection cnn = DataFactory.CreateConnection(ConnectionString, dbtype);
          IDbCommand cmd = DataFactory.CreateCommand(query, dbtype, cnn);
          cmd.CommandTimeout = 400;
            DbDataAdapter da = DataFactory.CreateAdapter(cmd, dbtype);
            
            arrCon.Add(argsConfManager);
            arrCon.Add(cnn);
            arrCon.Add(cmd);
            arrCon.Add(da);
            arrCon.Add(dbtype);

            return arrCon;
        }

        #endregion
        
        #region "QueryDataTable 1"

        /// <summary>
        /// Sobrecarga del método QueryDT, que se encarga de ejecutar una selección sobre la base de datos y regresarlo en un Data Table
        /// </summary>
        /// <param name="datasource">Es el nombre o alias de la conexion del data source</param>
        /// <param name="query">Prepare Statement con la consulta a ejecutar en la base de datos.</param>
        public DataTable QueryDT(string datasource, string query)
        {
            return QueryDT(datasource, query, string.Empty, null, null);
        }

        #endregion

        #region "QueryDataTable 2"

        /// <summary>
        /// Sobrecarga del método QueryDT, que se encarga de ejecutar una selección sobre la base de datos y regresarlo en un Data Table
        /// </summary>
        /// <param name="datasource">Es el nombre o alias de la conexion del data source</param>
        /// <param name="query">Prepare Statement con la consulta a ejecutar en la base de datos.</param>
        /// <param name="argsStr">Argumentos tokenizados mediante punto y coma, con los parámetros a utilizar dentro de la ejecución de la consulta.</param>
        /// <param name="hc">Es el objeto HttpContext.Current heredado de la página que originalmente recibió la solicitud</param>
        public DataTable QueryDT(string datasource, string query, String argsStr, HttpContext hc)
        {
            return QueryDT(datasource, query, argsStr, null, hc);
        }

        #endregion

        #region "QueryDataTable 3"

        /// <summary>
        /// Sobrecarga del método QueryDT, que se encarga de ejecutar una selección sobre la base de datos y regresarlo en un Data Table
        /// </summary>
        /// <param name="datasource">Es el nombre o alias de la conexion del data source</param>
        /// <param name="query">Prepare Statement con la consulta a ejecutar en la base de datos.</param>
        /// <param name="argsStr">Argumentos tokenizados mediante punto y coma, con los parámetros a utilizar dentro de la ejecución de la consulta.</param>
        /// <param name="hc">Es el objeto HttpContext.Current heredado de la página que originalmente recibió la solicitud</param>
        /// <param name="h">Objeto Hastable que contiene los valores necesarios para la ejecución del statement</param>
        public DataTable QueryDT(string datasource, string query, String argsStr, Hashtable h, HttpContext hc)
        {
            ArrayList arrCon = objConnection(datasource, query, hc);
            DbDataAdapter da = (DbDataAdapter)arrCon[3];
            String[] argsConfManager = (String[])arrCon[0];
            DataTable dt = new DataTable();

            try
            {
                ConstruyeParametros(argsConfManager[0], da, argsStr, h, hc);
                da.Fill(dt);
                da.Dispose();
                da = null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error No: " + ex.Message + "Código de Error: " + ex.GetType());
            }

            return dt;
        }

        #endregion

        #region "QueryDataSet 1"

        /// <summary>
        /// Sobrecarga del método QueryDS, que se encarga de ejecutar una selección sobre la base de datos y regresarlo en un Data Set.
        /// </summary>
        /// <param name="datasource">Es el nombre o alias de la conexion del data source</param>
        /// <param name="query">Prepare Statement con la consulta a ejecutar en la base de datos.</param>
        /// <param name="argsStr">Argumentos tokenizados mediante punto y coma, con los parámetros a utilizar dentro de la ejecución de la consulta.</param>
        /// <param name="hc">Es el objeto HttpContext.Current heredado de la página que originalmente recibió la solicitud</param>
        public DataSet QueryDS(string datasource, string query, String argsStr, HttpContext hc)
        {
            return QueryDS(datasource, query, argsStr, null, hc);
        }

        #endregion

        #region "QueryDataSet 2"

        /// <summary>
        /// Sobrecarga del método QueryDS, que se encarga de ejecutar una selección sobre la base de datos y regresarlo en un Data Set.
        /// </summary>
        /// <param name="datasource">Es el nombre o alias de la conexion del data source</param>
        /// <param name="query">Prepare Statement con la consulta a ejecutar en la base de datos.</param>
        /// <param name="argsStr">Argumentos tokenizados mediante punto y coma, con los parámetros a utilizar dentro de la ejecución de la consulta.</param>
        /// <param name="hc">Es el objeto HttpContext.Current heredado de la página que originalmente recibió la solicitud</param>
        /// <param name="h">Objeto Hastable que contiene los valores necesarios para la ejecución del statement</param>
        public DataSet QueryDS(string datasource, string query, String argsStr, Hashtable h, HttpContext hc)
        {
            ArrayList arrCon = objConnection(datasource, query, hc);
            DbDataAdapter da = (DbDataAdapter)arrCon[3];
            String[] argsConfManager = (String[])arrCon[0];
            DataSet ds = new DataSet("DataAccessLayer");

            try
            {
                ConstruyeParametros(argsConfManager[0], da, argsStr, h, hc);
                da.Fill(ds);
                da.Dispose();
                da = null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error No: " + ex.Message + "Código de Error: " + ex.GetType());
            }
            return ds;
        }

        #endregion

        #region "HttpContext Leer Parametros 1"

        /// <summary>
        /// Sobrecarga del método ConstruyeParametros, que se encarga de extraer el valor de las varibles Session, QueryString, Application, Hash, Fixed y convertir su valor al tipo de dato deseado
        /// </summary>
        /// <param name="ProviderWebConfig">Proveedor de </param>
        /// <param name="adapter">Prepare Statement con la consulta a ejecutar en la base de datos.</param>
        /// <param name="argsStr">Argumentos tokenizados mediante punto y coma, con los parámetros a utilizar dentro de la ejecución de la consulta.</param>
        /// <param name="hc">Es el objeto HttpContext.Current heredado de la página que originalmente recibió la solicitud</param>
        public void ConstruyeParametros(string ProviderWebConfig, DbDataAdapter adapter, string argsStr, HttpContext hc)
        {
            ConstruyeParametros(ProviderWebConfig, adapter, argsStr, null, hc);
        }

        #endregion

        #region "HttpContext Leer Parametros 2"

        public void ConstruyeParametros(string ProviderWebConfig, DbDataAdapter adapter, string argsStr, Hashtable h, HttpContext hc)
        {
          argsStr = argsStr.Replace("&#xA;", "₫");
          argsStr = argsStr.Replace("&amp;quot;", "₦");
          argsStr = argsStr.Replace("\n", "");
          argsStr = argsStr.Replace("&amp;", "₪");
          argsStr = argsStr.Replace("&apos;", "β");
          argsStr = argsStr.Replace("apos;", "₱");
          argsStr = argsStr.Replace("&quot;", "ᶲ");
          argsStr = argsStr.Replace("&gt;", "₩");
          argsStr = argsStr.Replace("&lt;", "₮");
          argsStr = argsStr.Replace("&#34;", "Ɽ");
          
          argsStr = argsStr.Replace(";", "҉");

          argsStr = argsStr.Replace("₫", "&#xA;");
          argsStr = argsStr.Replace("₦", "&amp;quot;");
          argsStr = argsStr.Replace("₪", "&amp;");
          argsStr = argsStr.Replace("β", "&apos;");
          argsStr = argsStr.Replace("₱", "apos;");
          argsStr = argsStr.Replace("ᶲ", "&quot;");
          argsStr = argsStr.Replace("₩", "&gt;");
          argsStr = argsStr.Replace("₮", "&lt;");
          argsStr = argsStr.Replace("Ɽ", "&#34;");
          

            String[] argsDomains = argsStr.Split(new Char[] { '҉' });
            String val = "";
            String argsAux = "";

            try
            {
                if (argsStr.Length > 0)
                {
                    String[] argsSubDomains;
                    for (int i = 0; i < argsDomains.Length; i++)
                    {
                        argsAux = argsDomains[i];
                        argsSubDomains = argsDomains[i].Split(new Char[] { ':' });

                        #region "Evaluamos val"
                        // Origenes del Dato
                        if (argsSubDomains[0].Equals("R")) // Form
                        {
                            val = hc.Request.Form.Get(argsSubDomains[2]);
                        }
                        else if (argsSubDomains[0].Equals("Q")) //Querystring
                        {
                            val = hc.Request.QueryString.Get(argsSubDomains[2]);
                        }
                        else if (argsSubDomains[0].Equals("S")) // Session
                        {
                            val = hc.Session[argsSubDomains[2]].ToString();
                        }
                        else if (argsSubDomains[0].Equals("A")) // Application
                        {
                            val = hc.Application[argsSubDomains[2]].ToString();
                        }
                        else if (argsSubDomains[0].Equals("H")) //Hash
                        {
                            val = h[argsSubDomains[2]].ToString();
                        }
                        else if (argsSubDomains[0].Equals("F")) // Fixed
                        {
                            if (argsSubDomains.Length > 3)
                            {
                                // Si la cadena contiene : hay que concatenarla
                                int longitud = argsSubDomains.Length;

                                string var = "";

                                for (int k = 2; k < longitud; k++)
                                {
                                    var += argsSubDomains[k] + ":";
                                }
                                //Removemos el último : que se le agregó a la cadena
                                val = var.Remove(var.Length - 1, 1);
                            }
                            else
                            {
                                val = argsSubDomains[2];
                            }
                        }

                        #endregion

                        #region "Validamos Provider SQLServerClient"

                        if (ProviderWebConfig == "1")
                        {
                            SqlCommand SqlCmd = (SqlCommand)adapter.SelectCommand;

                            if (argsSubDomains[1].Equals("I")) // Integer 
                            {
                                SqlCmd.Parameters.AddWithValue("@" + i, int.Parse(val));
                            }
                            else if (argsSubDomains[1].Equals("S")) // String
                            {
                                SqlCmd.Parameters.AddWithValue("@" + i, val);
                            }
                            else if (argsSubDomains[1].Equals("U")) // Unique Identifier 
                            {
                                SqlCmd.Parameters.AddWithValue("@" + i, SqlGuid.Parse(val));
                            }
                            else if (argsSubDomains[1].Equals("D")) // Double
                            {
                                SqlCmd.Parameters.AddWithValue("@" + i, double.Parse(val));
                            }
                            else if (argsSubDomains[1].Equals("F")) // Float
                            {
                                SqlCmd.Parameters.AddWithValue("@" + i, float.Parse(val));
                            }
                            else if (argsSubDomains[1].Equals("L")) // Long 
                            {
                                SqlCmd.Parameters.AddWithValue("@" + i, long.Parse(val));
                            }
                            else if (argsSubDomains[1].Equals("SH")) // short
                            {
                                SqlCmd.Parameters.AddWithValue("@" + i, short.Parse(val));
                            }
                            else if (argsSubDomains[1].Equals("B")) // Bool
                            {
                                SqlCmd.Parameters.AddWithValue("@" + i, bool.Parse(val));
                            }
                            else if (argsSubDomains[1].Equals("b")) // Byte
                            {
                                SqlCmd.Parameters.AddWithValue("@" + i, byte.Parse(val));
                            }
                            else if (argsSubDomains[1].Equals("Y")) // VarBinary
                            {
                                SqlCmd.Parameters.Add("@" + i, SqlDbType.VarBinary, 128).Value = Encriptar.Text(val);
                            }
                        }

                        #endregion

                        #region "Validamos Provider OracleClient"

                        if (ProviderWebConfig == "2")
                        {
                            OracleCommand OraCmd = (OracleCommand)adapter.SelectCommand;

                            if (argsSubDomains[1].Equals("I")) // Integer 
                            {
                                OraCmd.Parameters.AddWithValue(":" + i, int.Parse(val));
                            }
                            else if (argsSubDomains[1].Equals("S")) // String
                            {
                                OraCmd.Parameters.AddWithValue(":" + i, val);
                            }
                            else if (argsSubDomains[1].Equals("D")) // Double
                            {
                                OraCmd.Parameters.AddWithValue(":" + i, double.Parse(val));
                            }
                            else if (argsSubDomains[1].Equals("F")) // Float
                            {
                                OraCmd.Parameters.AddWithValue(":" + i, float.Parse(val));
                            }
                            else if (argsSubDomains[1].Equals("L")) // Long 
                            {
                                OraCmd.Parameters.AddWithValue(":" + i, long.Parse(val));
                            }
                            else if (argsSubDomains[1].Equals("SH")) // short
                            {
                                OraCmd.Parameters.AddWithValue(":" + i, short.Parse(val));
                            }
                            else if (argsSubDomains[1].Equals("B")) // Bool
                            {
                                OraCmd.Parameters.AddWithValue(":" + i, bool.Parse(val));
                            }
                            else if (argsSubDomains[1].Equals("b")) // Byte
                            {
                                OraCmd.Parameters.AddWithValue(":" + i, byte.Parse(val));
                            }
                            else if (argsSubDomains[1].Equals("Y")) // VarBinary
                            {
                                OraCmd.Parameters.Add("@" + i, OracleType.Blob, 128).Value = Encriptar.Text(val);
                            }
                        }

                        #endregion

                    }
                }
            }
            catch (Exception Exc)
            {
                throw new Exception("No se ha podido procesar el parámetro: " + argsAux + " \n\n" + Exc.ToString());
            }
        }

        #endregion

        #region "ExecuteNonQuery 1"

        /// <summary>
        /// Sobrecarga del método ExecuteNonQuery, que se encarga de ejecutar un Insert, Delete y Update y nos regresa en número de filas afectadas.
        /// </summary>
        /// <param name="datasource">Es el nombre o alias de la conexion del data source</param>
        /// <param name="query">Prepare Statement con la consulta a ejecutar en la base de datos.</param>
        /// <param name="argsStr">Argumentos tokenizados mediante punto y coma, con los parámetros a utilizar dentro de la ejecución de la consulta.</param>
        /// <param name="hc">Es el objeto HttpContext.Current heredado de la página que originalmente recibió la solicitud</param>
        public void ExecuteNonQuery(string datasource, string query, String argsStr, HttpContext hc)
        {
            ExecuteNonQuery(datasource, query, argsStr, null, hc);
        }

        #endregion

        #region "ExecuteNonQuery 2"

        /// <summary>
        /// Sobrecarga del método ExecuteNonQuery, que se encarga de ejecutar un Insert, Delete y Update y nos regresa en número de filas afectadas.
        /// </summary>
        /// <param name="datasource">Es el nombre o alias de la conexion del data source</param>
        /// <param name="query">Prepare Statement con la consulta a ejecutar en la base de datos.</param>
        /// <param name="argsStr">Argumentos tokenizados mediante punto y coma, con los parámetros a utilizar dentro de la ejecución de la consulta.</param>
        /// <param name="hc">Es el objeto HttpContext.Current heredado de la página que originalmente recibió la solicitud</param>
        /// <param name="h">Objeto Hastable que contiene los valores necesarios para la ejecución del statement</param>
        public void ExecuteNonQuery(string datasource, string query, String argsStr, Hashtable h, HttpContext hc)
        {
            ArrayList arrCon = objConnection(datasource, query, hc);
            DbDataAdapter da = (DbDataAdapter)arrCon[3];
            IDbConnection cnn = (IDbConnection)arrCon[1];
            IDbCommand cmd = (IDbCommand)arrCon[2];
           String[] argsConfManager = (String[])arrCon[0];

            try
            {
                ConstruyeParametros(argsConfManager[0], da, argsStr, h, hc);
                cnn.Open();
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }
            catch (SqlException exsql)
            {
                throw new Exception("Error No: " + exsql.Message + "Código de Error: " + exsql.ErrorCode);
            }
            catch (OracleException exora)
            {
                throw new Exception("Error No: " + exora.Message + "Código de Error: " + exora.ErrorCode);
            }
            catch (OleDbException exoledb)
            {
                throw new Exception("Error No: " + exoledb.Message + "Código de Error: " + exoledb.ErrorCode);
            }
            catch (OdbcException exodbc)
            {
                throw new Exception("Error No: " + exodbc.Message + "Código de Error: " + exodbc.ErrorCode);
            }
            catch (Exception ex)
            {
                throw new Exception("Error No: " + ex.Message + "Código de Error: " + ex.GetType());
            }
            finally
            {
                CerrarConexion(cnn);
            }
        }

        #endregion

        #region "ExecuteScalar 1"

        /// <summary>
        /// Sobrecarga del método ExecuteScalar, que se encarga de ejecutar un Insert, Delete y Update y nos regresa el un único valor como un Id de la tabla.
        /// </summary>
        /// <param name="datasource">Es el nombre o alias de la conexion del data source</param>
        /// <param name="query">Prepare Statement con la consulta a ejecutar en la base de datos.</param>
        /// <param name="argsStr">Argumentos tokenizados mediante punto y coma, con los parámetros a utilizar dentro de la ejecución de la consulta.</param>
        /// <param name="hc">Es el objeto HttpContext.Current heredado de la página que originalmente recibió la solicitud</param>
        public int ExecuteScalar(string datasource, string query, String argsStr, HttpContext hc)
        {
            return ExecuteScalar(datasource, query, argsStr, null, hc);
        }

        #endregion

        #region "ExecuteScalar 2"

        /// <summary>
        /// Sobrecarga del método ExecuteScalar, que se encarga de ejecutar un Insert, Delete y Update y nos regresa el un único valor como un Id de la tabla.
        /// </summary>
        /// <param name="datasource">Es el nombre o alias de la conexion del data source</param>
        /// <param name="query">Prepare Statement con la consulta a ejecutar en la base de datos.</param>
        /// <param name="argsStr">Argumentos tokenizados mediante punto y coma, con los parámetros a utilizar dentro de la ejecución de la consulta.</param>
        /// <param name="hc">Es el objeto HttpContext.Current heredado de la página que originalmente recibió la solicitud</param>
        /// <param name="h">Objeto Hastable que contiene los valores necesarios para la ejecución del statement</param>
        public int ExecuteScalar(string datasource, string query, String argsStr, Hashtable h, HttpContext hc)
        {
            ArrayList arrCon = objConnection(datasource, query, hc);
            DbDataAdapter da = (DbDataAdapter)arrCon[3];
            IDbConnection cnn = (IDbConnection)arrCon[1];
            IDbCommand cmd = (IDbCommand)arrCon[2];
            String[] argsConfManager = (String[])arrCon[0];

            try
            {
                ConstruyeParametros(argsConfManager[0], da, argsStr, h, hc);
                cnn.Open();

              Object resultado = null;
              try
              {
                resultado = cmd.ExecuteScalar();
                return  Convert.ToInt32(resultado);
              }
              catch (Exception )
              {
                throw new Exception((string)resultado);
              }  
              
            }
            catch (SqlException exsql)
            {
                throw new Exception("Error No: " + exsql.Message + "Código de Error: " + exsql.ErrorCode);
            }
            catch (OracleException exora)
            {
                throw new Exception("Error No: " + exora.Message + "Código de Error: " + exora.ErrorCode);
            }
            catch (OleDbException exoledb)
            {
                throw new Exception("Error No: " + exoledb.Message + "Código de Error: " + exoledb.ErrorCode);
            }
            catch (OdbcException exodbc)
            {
                throw new Exception("Error No: " + exodbc.Message + "Código de Error: " + exodbc.ErrorCode);
            }
            catch (Exception ex)
            {
                throw new Exception("Error No: " + ex.Message + "Código de Error: " + ex.GetType());
            }
            finally
            {
                CerrarConexion(cnn);
            }
        }

        #endregion

        #region "Execute Reader"

        /// <summary>
        /// Sobrecarga del método ExecuteReader, que se encarga de ejecutar selección sobre una base de datos, y regresar el texto plano de la consulta.
        /// </summary>
        /// <param name="datasource">Es el nombre o alias de la conexion del data source</param>
        /// <param name="query">Prepare Statement con la consulta a ejecutar en la base de datos.</param>
        /// <param name="hc">Es el objeto HttpContext.Current heredado de la página que originalmente recibió la solicitud</param>
        public IDataReader ExecuteReader(string datasource, string query, HttpContext hc)
        {
            ArrayList arrCon = objConnection(datasource, query, hc);
            IDbConnection cnn = (IDbConnection)arrCon[1];
            IDbCommand cmd = (IDbCommand)arrCon[2];

            try
            {
                cnn.Open();
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw new Exception("Error No: " + ex.Message + "Código de Error: " + ex.GetType());
            }
            finally
            {
                CerrarConexion(cnn);
            }
        }

        #endregion

        #region "ExecuteStoreProcedure"

        /// <summary>
        /// Sobrecarga del método ExecuteSP, que se encarga de ejecutar un Stored Procedure sobre la Base de Datos
        /// </summary>
        /// <param name="datasource">Es el nombre o alias de la conexion del data source</param>
        /// <param name="NombreSP">Nombre del Stored Procedure a ejecutar.</param>
        /// <param name="parametrosSP">Parametros enviados al Stored Procedure para su ejecución</param>
        public void ExecuteSP(string datasource, string NombreSP, List<DBParam> parametrosSP, HttpContext hc)
        {
            ArrayList arrCon = objConnection(datasource, string.Empty, hc);
            IDbConnection cnn = (IDbConnection)arrCon[1];
            IDbCommand cmd = (IDbCommand)arrCon[2];

            try
            {
                cmd.CreateParameter();
                int i = 0;

                foreach (DBParam param in parametrosSP)
                {
                    DbType tipo = DbType.String;
                    if (param.Tipo == "s")
                    {
                        tipo = DbType.String;
                    }
                    if (param.Tipo == "i")
                    {
                        tipo = DbType.Int32;
                    }
                    if (param.Tipo == "dt")
                    {
                        tipo = DbType.DateTime;
                    }
                    if (param.Tipo == "n")
                    {
                        tipo = DbType.Double;
                    }
                    if (param.Tipo == "d")
                    {
                        tipo = DbType.Double;
                    }

                    AddParamToCmd(cmd, param.Nombre, tipo, 255, ParameterDirection.Input, parametrosSP[i]);
                    i++;
                }

                SetCommandType(cmd, CommandType.StoredProcedure, NombreSP);

                cnn.Open();
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }
            catch (SqlException exsql)
            {
                throw new Exception("Error No: " + exsql.Message + "Código de Error: " + exsql.ErrorCode);
            }
            catch (OracleException exora)
            {
                throw new Exception("Error No: " + exora.Message + "Código de Error: " + exora.ErrorCode);
            }
            catch (OleDbException exoledb)
            {
                throw new Exception("Error No: " + exoledb.Message + "Código de Error: " + exoledb.ErrorCode);
            }
            catch (OdbcException exodbc)
            {
                throw new Exception("Error No: " + exodbc.Message + "Código de Error: " + exodbc.ErrorCode);
            }
            catch (Exception ex)
            {
                throw new Exception("Error No: " + ex.Message + "Código de Error: " + ex.GetType());
            }
            finally
            {
                CerrarConexion(cnn);
            }
        }

        #endregion

        #region "Execute Transaction General 1"

        /// <summary>
        /// Sobrecarga del método ExecuteTransaction, que se encarga de ejecutar una transacción sobre la Base de Datos, y hasta que se termina se realiza el commit.
        /// </summary>
        /// <param name="datasource">Es el nombre o alias de la conexion del data source</param>
        /// <param name="query">Prepare Statement con la consulta a ejecutar en la base de datos.</param>
        /// <param name="argsStr">Argumentos tokenizados mediante punto y coma, con los parámetros a utilizar dentro de la ejecución de la consulta.</param>
        /// <param name="hc">Es el objeto HttpContext.Current heredado de la página que originalmente recibió la solicitud</param>
        public int ExecuteTransaction(string datasource, string query, String argsStr, HttpContext hc)
        {
            return ExecuteTransaction(datasource, query, argsStr, null, hc);
        }

        #endregion

        #region "Execute Transaction General 2"

        /// <summary>
        /// Sobrecarga del método ExecuteTransaction, que se encarga de ejecutar una transacción sobre la Base de Datos, y hasta que se termina se realiza el commit.
        /// </summary>
        /// <param name="datasource">Es el nombre o alias de la conexion del data source</param>
        /// <param name="query">Prepare Statement con la consulta a ejecutar en la base de datos.</param>
        /// <param name="argsStr">Argumentos tokenizados mediante punto y coma, con los parámetros a utilizar dentro de la ejecución de la consulta.</param>
        /// <param name="hc">Es el objeto HttpContext.Current heredado de la página que originalmente recibió la solicitud</param>
        /// <param name="h">Objeto Hastable que contiene los valores necesarios para la ejecución del statement</param>
        public int ExecuteTransaction(string datasource, string query, String argsStr, Hashtable h, HttpContext hc)
        {
            ArrayList arrCon = objConnection(datasource, query, hc);
            IDbConnection cnn = (IDbConnection)arrCon[1];
            IDbCommand cmd = (IDbCommand)arrCon[2];
            DatabaseType dbtype = (DatabaseType)arrCon[4];
            IDbTransaction trn = DataFactory.CreateTransaction(dbtype, cmd, cnn);

            try
            {
                cmd.Transaction = trn;
                cmd.Connection = cnn;
                int i = cmd.ExecuteNonQuery();
                cmd.Transaction.Commit();
                cmd.Dispose();
                cmd = null;

                if (i > 0)
                {
                    mMensajeError = "La operación se realizó con éxito, se afectaron " + i + " Filas";
                }
                else
                {
                    mMensajeError = "La operación no afectó ninguna fila";
                }

                return i;
            }
            catch (SqlException exsql)
            {
                try
                {
                    trn.Rollback();
                    throw new Exception("Error No: " + exsql.Message + "Código de Error: " + exsql.ErrorCode);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error No: " + ex.Message + "Código de Error: " + ex.GetType());
                }
            }
            catch (OracleException exora)
            {
                try
                {
                    trn.Rollback();
                    throw new Exception("Error No: " + exora.Message + "Código de Error: " + exora.ErrorCode);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error No: " + ex.Message + "Código de Error: " + ex.GetType());
                }
            }
            catch (OleDbException exoledb)
            {
                try
                {
                    trn.Rollback();
                    throw new Exception("Error No: " + exoledb.Message + "Código de Error: " + exoledb.ErrorCode);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error No: " + ex.Message + "Código de Error: " + ex.GetType());
                }
            }
            catch (OdbcException exodbc)
            {
                try
                {
                    trn.Rollback();
                    throw new Exception("Error No: " + exodbc.Message + "Código de Error: " + exodbc.ErrorCode);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error No: " + ex.Message + "Código de Error: " + ex.GetType());
                }
            }
            catch (Exception ex)
            {
                trn.Rollback();
                throw new Exception("Error No: " + ex.Message + " Ocurrió en: " + ex.Source + "Código de Error: " + ex.GetType() + "Ruta del Error: " + ex.StackTrace);
            }
            finally
            {
                CerrarConexion(cnn);
            }
        }

        #endregion

        #region "Execute Transaction Bundle"

        /// <summary>
        /// Método ExecuteTransactionBundle, que se encarga de generar un objeto de transacción
        /// </summary>
        /// <param name="datasource">Es el nombre o alias de la conexion del data source</param>
        public IDbTransaction ExecuteTransactionBundle(string datasource, HttpContext hc)
        {
            ArrayList arrCon = objConnection(datasource, string.Empty, hc);
            IDbConnection cnn = (IDbConnection)arrCon[1];
            IDbCommand cmd = (IDbCommand)arrCon[2];
            DatabaseType dbtype = (DatabaseType)arrCon[4];
            cmd.CommandTimeout = 40;
            IDbTransaction trn = DataFactory.CreateTransaction(dbtype, cmd, cnn);

            return trn;
        }

        #endregion

        #region "ExecuteQuerySinParametrosBundle"

        /// <summary>
        /// Método ExecuteTransactionBundle, que se encarga de ejecutar N número de consultas sobre la base de datos, mediante una transacción.
        /// </summary>
        /// <param name="datasource">Es el nombre o alias de la conexion del data source</param>
        /// <param name="query">Prepare Statement con la consulta a ejecutar en la base de datos.</param>
        /// <param name="trn">Contiene el Objeto de Transacción en el que se ejecutarán las consultas en la base de datos.</param>
        public void ExecuteQuerySinParametrosBundle(string datasource, string query, IDbTransaction trn, HttpContext hc)
        {
            ArrayList arrCon = objConnection(datasource, query, hc);
            IDbCommand cmd = (IDbCommand)arrCon[2];
            cmd.CommandTimeout = 40;

            try
            {
                cmd.Transaction = trn;
                cmd.Connection = trn.Connection;

                int i = cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;

                if (i > 0)
                {
                    mMensajeError = "La operación se realizó con éxito, se afectaron " + i + " Filas";
                }
                else
                {
                    mMensajeError = "La operación no afectó ninguna fila";
                }
            }
            catch (SqlException exsql)
            {
                try
                {
                    trn.Rollback();
                    throw new Exception("Error No: " + exsql.Message + "Código de Error: " + exsql.ErrorCode);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error No: " + ex.Message + "Código de Error: " + ex.GetType());
                }
            }
            catch (OracleException exora)
            {
                try
                {
                    trn.Rollback();
                    throw new Exception("Error No: " + exora.Message + "Código de Error: " + exora.ErrorCode);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error No: " + ex.Message + "Código de Error: " + ex.GetType());
                }
            }
            catch (OleDbException exoledb)
            {
                try
                {
                    trn.Rollback();
                    throw new Exception("Error No: " + exoledb.Message + "Código de Error: " + exoledb.ErrorCode);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error No: " + ex.Message + "Código de Error: " + ex.GetType());
                }
            }
            catch (OdbcException exodbc)
            {
                try
                {
                    trn.Rollback();
                    throw new Exception("Error No: " + exodbc.Message + "Código de Error: " + exodbc.ErrorCode);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error No: " + ex.Message + "Código de Error: " + ex.GetType());
                }
            }
            catch (Exception ex)
            {
                trn.Rollback();
                throw new Exception("Error No: " + ex.Message + " Ocurrió en: " + ex.Source + "Código de Error: " + ex.GetType() + "Ruta del Error: " + ex.StackTrace);
            }
        }

        #endregion

        #region "ExecuteStoreProcedureBundle"

        /// <summary>
        /// Método ExecuteSPBundle, que se encarga de ejecutar N número de Stored Procedures sobre la base de datos, mediante una transacción.
        /// </summary>
        /// <param name="datasource">Es el nombre o alias de la conexion del data source</param>
        /// <param name="NombreSP">Nombre del Stored Procedure a ejecutar.</param>
        /// <param name="parametrosSP">Parametros enviados al Stored Procedure para su ejecución</param>
        /// <param name="trn">Contiene el Objeto de Transacción en el que se ejecutarán las consultas en la base de datos.</param>
        public void ExecuteSPBundle(string datasource, string NombreSP, List<DBParam> parametrosSP, IDbTransaction trn, HttpContext hc)
        {
            ArrayList arrCon = objConnection(datasource, string.Empty, hc);
            IDbCommand cmd = (IDbCommand)arrCon[2];
            cmd.CommandTimeout = 40;
            cmd.CreateParameter();

            try
            {
                int i = 0;
                foreach (DBParam param in parametrosSP)
                {
                    DbType tipo = DbType.String;
                    if (param.Tipo == "s")
                    {
                        tipo = DbType.String;
                    }
                    if (param.Tipo == "i")
                    {
                        tipo = DbType.Int32;
                    }
                    if (param.Tipo == "dt")
                    {
                        tipo = DbType.DateTime;
                    }
                    if (param.Tipo == "n")
                    {
                        tipo = DbType.Double;
                    }
                    if (param.Tipo == "d")
                    {
                        tipo = DbType.Double;
                    }
                    AddParamToCmd(cmd, "prefijo" + i, tipo, 255, ParameterDirection.Input, parametrosSP[i]);
                    i++;
                }

                cmd.Transaction = trn;
                cmd.Connection = trn.Connection;
                SetCommandType(cmd, CommandType.StoredProcedure, NombreSP);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmd = null;
            }
            catch (SqlException exsql)
            {
                throw new Exception("Error No: " + exsql.Message + "Código de Error: " + exsql.ErrorCode);
            }
            catch (OracleException exora)
            {
                throw new Exception("Error No: " + exora.Message + "Código de Error: " + exora.ErrorCode);
            }
            catch (OleDbException exoledb)
            {
                throw new Exception("Error No: " + exoledb.Message + "Código de Error: " + exoledb.ErrorCode);
            }
            catch (OdbcException exodbc)
            {
                throw new Exception("Error No: " + exodbc.Message + "Código de Error: " + exodbc.ErrorCode);
            }
            catch (Exception ex)
            {
                throw new Exception("Error No: " + ex.Message + "Código de Error: " + ex.GetType());
            }
        }

        #endregion

        #region "Add Param"

        /// <summary>
        /// Método AddParamToCmd, que se encarga de ejecutar N número de Stored Procedures sobre la base de datos, mediante una transacción.
        /// </summary>
        /// <param name="dbCmd">Es el Data Base Command</param>
        /// <param name="paramId">Nombre de los parámetros enviados.</param>
        /// <param name="dbType">Tipo de datos de cada parámetro.</param>
        /// <param name="paramSize">Tamaño de los parámetros enviados.</param>
        /// <param name="paramDirec">Dirección de los parámetros, Ejemplo (Input), (Output).</param>
        /// <param name="paramValue">Valores de los parámetros enviados.</param>
        private static void AddParamToCmd(IDbCommand dbCmd, string paramId, DbType dbType, int paramSize, ParameterDirection paramDirec, DBParam paramValue)
        {
            if (dbCmd == null)
            {
                throw new ArgumentNullException("dbCmd");
            }

            if (paramId == string.Empty)
            {
                throw new ArgumentOutOfRangeException("paramId");
            }

            IDbDataParameter newDbParam = dbCmd.CreateParameter();

            newDbParam.ParameterName = paramId;
            newDbParam.DbType = dbType;
            newDbParam.Direction = paramDirec;

            if (paramSize > 0)
            {
                newDbParam.Size = paramSize;
            }

            if ((paramValue != null))
            {
                newDbParam.Value = paramValue.Parametro;
            }

            dbCmd.Parameters.Add(newDbParam);
        }

        #endregion

        #region "SetCommandType"

        /// <summary>
        /// Método SetCommandType, que se encarga de setear el objeto command
        /// </summary>
        /// <param name="dbCmd">Es el Data Base Command</param>
        /// <param name="cmdType">Tipo de command del Stored Procedure.</param>
        /// <param name="cmdText">Nombre del Stored Procedure.</param>
        private static void SetCommandType(IDbCommand dbCmd, CommandType cmdType, string cmdText)
        {
            dbCmd.CommandType = cmdType;
            dbCmd.CommandText = cmdText;
        }

        #endregion

        #region "CerrarConexion"

        /// <summary>
        /// Este método se encarga de cerrar las conexiones abiertas de la base de datos.
        /// </summary>
        /// <param name="cnTarget">Objeto de la conexion que se desea cerrar</param>
        private static void CerrarConexion(IDbConnection cnTarget)
        {
            if (cnTarget != null)
            {
                if ((cnTarget.State == ConnectionState.Open))
                {
                    cnTarget.Close();
                    cnTarget.Dispose();
                    cnTarget = null;
                }
            }
        }

        #endregion
    }

    #region "DBParam"

    public class DBParam
    {
        public string Parametro;
        public string Tipo;
        public string Nombre;

        /// <summary>
        /// Método DBParam, que se encarga de setear el Parametro y el tipo de parametro a utilizar en la ejecución de Stored Procedures.
        /// </summary>
        /// <param name="Parametro">Parámetro.</param>
        /// <param name="Tipo">Tipo de Parámetro.</param>
        public DBParam(string Nombre, string Parametro, string Tipo)
        {
            this.Parametro = Parametro;
            this.Tipo = Tipo;
            this.Nombre = Nombre;
        }
    }

    #endregion

    #region "Encriptar"

    public class Encriptar
    {
        static public Byte[] Text(String strPlainText)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            return md5Hasher.ComputeHash(encoder.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(strPlainText.Trim(), "SHA1")));
        }
    }

    #endregion
}