using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.UnitOfWork
{
    public class UoWUnitOfWorkConauto : IUnitOfWorkConauto
    {
        public bool _hasConnection { get; set; }
        public IDbTransaction _transaction { get; set; }
        public IDbConnection _connection { get; set; }

        IDbTransaction IUnitOfWorkConauto._transaction { get; set; }
        IDbConnection IUnitOfWorkConauto._connection { get; set; }

        public IDbCommand CreateCommand()
        {
            var command = _connection.CreateCommand();
            command.Transaction = _transaction;
            return command;
        }
        public UoWUnitOfWorkConauto(IDbConnection connection, bool hasConnection)
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
        IDbCommand IUnitOfWorkConauto.CreateCommand()
        {
            throw new NotImplementedException();
        }
    }
}
