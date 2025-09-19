using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DataLayer.UnitOfWork
{
    public class UnitOfWorkFactory
    {
        public static IUnitOfWork Create()
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            var connection = new SqlConnection(connString);
            connection.Open();
            return new UoWUnitOfWork(connection, true);
        }
        public static IUnitOfWorkConauto CreateCanauto()
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionStringConautoss"].ConnectionString;
            var connection = new SqlConnection(connString);
            connection.Open();
            return new UoWUnitOfWorkConauto(connection, true);
        }


    }
}
