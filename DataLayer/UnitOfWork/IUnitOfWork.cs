using System;
using System.Data;


namespace DataLayer.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        bool _hasConnection { get; set; }
        IDbTransaction _transaction { get; set; }
        IDbConnection _connection { get; set; }
        IDbCommand CreateCommand();
        void SaveChanges();
        void Dispose();

    }

    public interface IUnitOfWorkConauto : IDisposable
    {
        bool _hasConnection { get; set; }
        IDbTransaction _transaction { get; set; }
        IDbConnection _connection { get; set; }
        IDbCommand CreateCommand();
        void SaveChanges();
        void Dispose();

    }
}
