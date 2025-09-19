using System;
using System.Data;
using System.Data.SqlClient;
using ModelLayer;
using DataLayer.UnitOfWork;
using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public class Rol_Repository
    {
        private UoWUnitOfWork _unitOfWork;
        public Rol_Repository(IUnitOfWork uow)
        {
            if (uow == null)
                throw new ArgumentException("uow");
            _unitOfWork = uow as UoWUnitOfWork;
            if (_unitOfWork == null)
            {
                throw new NotSupportedException("Unit of work factory is null, can you validate?");
            }
        }

        public List<Roles> ObtRoles()
        {
            List<Roles> roles = new List<Roles>();
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "Select IdRolUsuario AS iRol, NombreRol AS cRol, Activo FROM Roles";
                cmd.CommandType = CommandType.Text;

                var table = new List<Dictionary<string, object>>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roles.Add(new Roles()
                        {
                            IdRolUsuario = (int)reader["iRol"],
                            NombreRol = string.IsNullOrEmpty((string)reader["cRol"]) ? "" : (string)reader["cRol"],
                            Activo = (bool)reader["Activo"]
                        }); ;
                    }
                }

            }
            return roles;
        }

        public bool UpdateRol(int idUsuario, int idRol)
        {
            bool succes = false;
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "Update Usuarios set  IdRol=" + idRol + " where idUsuario=" + idUsuario + " ";
                cmd.CommandType = CommandType.Text;
                
                succes = cmd.ExecuteNonQuery() > 0;
                _unitOfWork.SaveChanges();
            }
            return succes;
        }

    }
}
