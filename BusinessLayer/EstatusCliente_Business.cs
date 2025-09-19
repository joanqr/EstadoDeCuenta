using DataLayer.Repositories;
using DataLayer.UnitOfWork;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class EstatusCliente_Business
    {
        public List<EstatusCliente> ObtieneEstatusCliente()
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var repository = new EstatusCliente_Repository(uow);
                var response = repository.ObtEstatus().Select(s => new EstatusCliente
                {
                    IdEstatus = s.IdEstatus,
                    Descripcion = string.IsNullOrEmpty(s.Descripcion) ? "" : s.Descripcion,
                    Estatus = s.Estatus
                }).ToList();
                return response;
            }
        }

        public bool UpdateEstatusCliente(int idUsuario, int idEstatus)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var repository = new EstatusCliente_Repository(uow);
                var response = repository.UpdateEstatus(idUsuario, idEstatus);
                return response;
            }
        }
    }
}
