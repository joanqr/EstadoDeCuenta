using System;
using System.Data.SqlClient;
using ModelLayer;
using DataLayer.UnitOfWork;
namespace DataLayer.Repositories
{
    public class Pagos_Repository
    {
        private UoWUnitOfWork _unitOfWork;

        public Pagos_Repository(IUnitOfWork uow)
        {
            if (uow == null)
                throw new ArgumentException("uow");
            _unitOfWork = uow as UoWUnitOfWork;
            if (_unitOfWork == null)
            {
                throw new NotSupportedException("Unit of work factory is null, can you validate?");
            }
        }


    }
}
