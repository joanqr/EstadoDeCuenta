#region "Using"

using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Data.Odbc;
using System.Web.UI.WebControls;

#endregion

namespace r3Take.DataAccessLayer
{
    #region "enum DatabaseType"

    public enum DatabaseType
    {
        Access,
        SQLServer,
        Oracle,
        SQLServerOLEDB,
        OracleOLEDB,
        SQLServerODBC,
        OracleODBC
    }

    #endregion

    #region "enum DatabaseProvider"

    public enum DatabaseProvider
    {
        SQLServerClient,
        OracleClient,
        OLEDB,
        ODBC
    }

    #endregion

    public class DataFactory
    {
        #region "CreateConection"

        /// <summary>
        /// Método CreateConnection, que se encarga de establecer una conexión para comunicarse con la Base de Datos.
        /// </summary>
        /// <param name="connectionString">Caena de conexión de la Base de Datos elegida.</param>
        /// <param name="dbtype">Tipo de Base de Datos al que se desea conectar.</param>
        public static IDbConnection CreateConnection(string connectionString, DatabaseType dbtype)
        {
            IDbConnection cnn;

            switch (dbtype)
            {
                case DatabaseType.Access:
                    cnn = new OleDbConnection
                        (connectionString);
                    break;
                case DatabaseType.SQLServer:
                  cnn = new SqlConnection
                        (connectionString);
                    break;
                case DatabaseType.Oracle:
                    cnn = new OracleConnection
                        (connectionString);
                    break;
                case DatabaseType.SQLServerOLEDB:
                    cnn = new OleDbConnection
                        (connectionString);
                    break;
                case DatabaseType.OracleOLEDB:
                    cnn = new OleDbConnection
                        (connectionString);
                    break;
                case DatabaseType.SQLServerODBC:
                    cnn = new OdbcConnection
                        (connectionString);
                    break;
                case DatabaseType.OracleODBC:
                    cnn = new OdbcConnection
                        (connectionString);
                    break;
                default:
                    cnn = new SqlConnection
                        (connectionString);
                    break;
            }

            return cnn;
        }

        #endregion

        #region "CreateCommand"

        /// <summary>
        /// Método CreateCommand, que se encarga de generar un Command.
        /// </summary>
        /// <param name="CommandText">Instrucción a ejecutar dentro de la Base de Datos.</param>
        /// <param name="dbtype">Tipo de Base de Datos al que se desea conectar.</param>
        /// <param name="cnn">Objeto conexión para establecer la comunicación con la Base de Datos.</param>
        public static IDbCommand CreateCommand(string CommandText, DatabaseType dbtype, IDbConnection cnn)
        {
            IDbCommand cmd;
            switch (dbtype)
            {
                case DatabaseType.Access:
                    cmd = new OleDbCommand(CommandText, (OleDbConnection)cnn);
                    break;

                case DatabaseType.SQLServer:
                    cmd = new SqlCommand(CommandText, (SqlConnection)cnn);
                    break;

                case DatabaseType.Oracle:
                    cmd = new OracleCommand(CommandText, (OracleConnection)cnn);
                    break;

                case DatabaseType.SQLServerOLEDB:
                    cmd = new OleDbCommand(CommandText, (OleDbConnection)cnn);
                    break;

                case DatabaseType.OracleOLEDB:
                    cmd = new OleDbCommand(CommandText, (OleDbConnection)cnn);
                    break;

                case DatabaseType.SQLServerODBC:
                    cmd = new OdbcCommand(CommandText, (OdbcConnection)cnn);
                    break;

                case DatabaseType.OracleODBC:
                    cmd = new OdbcCommand(CommandText, (OdbcConnection)cnn);
                    break;

                default:
                    cmd = new SqlCommand(CommandText, (SqlConnection)cnn);
                    break;
            }
            return cmd;
        }

        #endregion

        #region "CreateParameter"

        /// <summary>
        /// Método CreateParameter, que se encarga de crear los parámetros dependiendo el tipo de Base de Datos.
        /// </summary>
        /// <param name="dbtype">Tipo de Base de Datos al que se desea conectar.</param>
        /// <param name="cnn">Objeto conexión para la comunicación con la Base de Datos.</param>
        public static IDbDataParameter CreateParameter(DatabaseType dbtype, IDbConnection cnn)
        {
            switch (dbtype)
            {
                case DatabaseType.Access:
                    IDbDataParameter prm = new OleDbParameter();
                    return prm;

                case DatabaseType.SQLServer:
                    SqlParameter prm1 = new SqlParameter();
                    return prm1;

                case DatabaseType.Oracle:
                    OracleParameter prm2 = new OracleParameter();
                    return prm2;

                default:
                    SqlParameter prm3 = new SqlParameter();
                    return prm3;
            }
        }

        #endregion

        #region "CreateAdapter"

        /// <summary>
        /// Método CreateAdapter, que se encarga de crear un adapter dependiendo de el tipo de Base de Datos.
        /// </summary>
        /// <param name="dbtype">Tipo de Base de Datos al que se desea conectar.</param>
        /// <param name="cmd">Es el command a ejecutar en la Base de Datos.</param>
        public static DbDataAdapter CreateAdapter(IDbCommand cmd, DatabaseType dbtype)
        {
            DbDataAdapter da;
            switch (dbtype)
            {
                case DatabaseType.Access:
                    da = new OleDbDataAdapter
                        ((OleDbCommand)cmd);
                    break;

                case DatabaseType.SQLServer:
                    da = new SqlDataAdapter
                        ((SqlCommand)cmd);
                    break;

                case DatabaseType.Oracle:
                    da = new OracleDataAdapter
                        ((OracleCommand)cmd);
                    break;

                case DatabaseType.SQLServerOLEDB:
                    da = new OleDbDataAdapter
                        ((OleDbCommand)cmd);
                    break;

                case DatabaseType.OracleOLEDB:
                    da = new OleDbDataAdapter
                        ((OleDbCommand)cmd);
                    break;

                case DatabaseType.SQLServerODBC:
                    da = new OdbcDataAdapter
                        ((OdbcCommand)cmd);
                    break;

                case DatabaseType.OracleODBC:
                    da = new OdbcDataAdapter
                        ((OdbcCommand)cmd);
                    break;

                default:
                    da = new SqlDataAdapter
                        ((SqlCommand)cmd);
                    break;
            }

            return da;
        }

        #endregion

        #region "Create Transaction"

        /// <summary>
        /// Método CreateTransaction, que se encarga de crear un objeto de tipo Transacción dependiedo el tipo de Base de Datos.
        /// </summary>
        /// <param name="dbtype">Tipo de Base de Datos al que se desea conectar.</param>
        /// <param name="cmd">Es el command a ejecutar en la Base de Datos.</param>
        /// <param name="cnn">Objeto conexión para establecer la comunicación con la Base de Datos.</param>
        public static DbTransaction CreateTransaction(DatabaseType dbtype, IDbCommand cmd, IDbConnection cnn)
        {
            cnn.Open();
            if (dbtype.ToString() == "Oracle")
            {
                OracleTransaction transaction;
                transaction = (OracleTransaction)cnn.BeginTransaction();
                return transaction;
            }
            else if (dbtype.ToString() == "SQLServer")
            {
                SqlTransaction transaction2;
                transaction2 = (SqlTransaction)cnn.BeginTransaction();
                return transaction2;
            }

            DbTransaction transactionnulo = null;
            return transactionnulo;
        }

        #endregion

        #region "Create DataSource"

        /// <summary>
        /// Método CreateDataSource, que se encarga de crear un objeto de tipo Data Source dependiedo el tipo de Base de Datos.
        /// </summary>
        /// <param name="dbtype">Tipo de Base de Datos al que se desea conectar.</param>
        public static SqlDataSource CreateDataSource(DatabaseType dbtype) //, IDbCommand cmd, IDbConnection cnn)
        {
            switch (dbtype)
            {
                case DatabaseType.SQLServer:
                    SqlDataSource sqldSource = new SqlDataSource();
                    return sqldSource;
            }

            SqlDataSource DataSource = new SqlDataSource();
            return DataSource;
        }

        #endregion

        #region "CreateCommandBundle"

        /// <summary>
        /// Método CreateCommandBundle, que se encarga de crear un objeto de tipo Bundle Transaction dependiedo el tipo de Base de Datos.
        /// </summary>
        /// <param name="dbtype">Tipo de Base de Datos al que se desea conectar.</param>
        public static IDbCommand CreateCommandBundle(DatabaseType dbtype)
        {
            IDbCommand cmd;
            switch (dbtype)
            {
                case DatabaseType.Access:
                    cmd = new OleDbCommand();
                    break;

                case DatabaseType.SQLServer:
                    cmd = new SqlCommand();
                    break;

                case DatabaseType.Oracle:
                    cmd = new OracleCommand();
                    break;

                case DatabaseType.SQLServerOLEDB:
                    cmd = new OleDbCommand();
                    break;

                case DatabaseType.OracleOLEDB:
                    cmd = new OleDbCommand();
                    break;

                case DatabaseType.SQLServerODBC:
                    cmd = new OdbcCommand();
                    break;

                case DatabaseType.OracleODBC:
                    cmd = new OdbcCommand();
                    break;

                default:
                    cmd = new SqlCommand();
                    break;
            }

            return cmd;
        }

        #endregion
    }
}