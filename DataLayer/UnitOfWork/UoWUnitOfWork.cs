using System;
using System.Data;


namespace DataLayer.UnitOfWork
{
    public class UoWUnitOfWork : IUnitOfWork
    {
        public IDbConnection _connection { get; set; }

        public bool _hasConnection { get; set; }

        public IDbTransaction _transaction { get; set; }
        IDbTransaction IUnitOfWork._transaction { get; set; }
        IDbConnection IUnitOfWork._connection { get; set; }

        public IDbCommand CreateCommand()
        {
            var command = _connection.CreateCommand();
            command.Transaction = _transaction;
            return command;
        }
        public UoWUnitOfWork(IDbConnection connection, bool hasConnection)
        {
            _connection = connection;
            _hasConnection = hasConnection;
            _transaction = connection.BeginTransaction();
        }
        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction = null;
            }
            if (_connection != null && _hasConnection)
            {
                _connection.Close();
                _connection = null;
            }
        }

        public void SaveChanges()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("error en la transacción, es nula");
            }
            _transaction.Commit();
            _transaction = null;
        }

        IDbCommand IUnitOfWork.CreateCommand()
        {
            throw new NotImplementedException();
        }
    }
}
