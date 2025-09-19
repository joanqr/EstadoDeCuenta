using DataLayer.Repositories;
using DataLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Mantenimiento_Business
    {
        public int ObtMantenimiento()
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var repository = new Mantenimiento_Repository(uow);
                var response = repository.ObtMantenimiento();
                return response;
            }
        }

        public bool UpdateMantenimento(int opcion)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var repository = new Mantenimiento_Repository(uow);
                var response = repository.UpdateMantenimento(opcion);
                return response;
            }
        }
    }
}
