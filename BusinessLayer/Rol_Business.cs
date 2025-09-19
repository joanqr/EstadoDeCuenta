using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Repositories;
using DataLayer.UnitOfWork;
using ModelLayer;

namespace BusinessLayer
{
    public class Rol_Business
    {
        public List<Roles> ObtieneRoles()
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var repository = new Rol_Repository(uow);
                var response = repository.ObtRoles().Select(s => new Roles
                {
                    IdRolUsuario = s.IdRolUsuario,
                    NombreRol = string.IsNullOrEmpty(s.NombreRol) ? "" : s.NombreRol,
                    Activo = s.Activo
                }).ToList();
                return response;
            }
        }

        public bool UpdateRol(int idUsuario,int idRol)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var repository = new Rol_Repository(uow);
                var response = repository.UpdateRol(idUsuario, idRol);
                return response;
            }
        }
    }
}
