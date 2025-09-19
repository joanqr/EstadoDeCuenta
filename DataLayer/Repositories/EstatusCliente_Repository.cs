
using DataLayer.UnitOfWork;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public class EstatusCliente_Repository
    {
        private UoWUnitOfWork _unitOfWork;

        public EstatusCliente_Repository(IUnitOfWork uow)
        {
            if (uow == null)
                throw new ArgumentException("uow");
            _unitOfWork = uow as UoWUnitOfWork;
            if (_unitOfWork == null)
            {
                throw new NotSupportedException("Unit of work factory is null, can you validate?");
            }
        }
        public List<EstatusCliente> ObtEstatus()
        {

            List<EstatusCliente> estatus = new List<EstatusCliente>();
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "Select IdEstatus,Descripcion, Estatus FROM Estatus";
                cmd.CommandType = CommandType.Text;

                var table = new List<Dictionary<string, object>>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        estatus.Add(new EstatusCliente()
                        {
                            IdEstatus = (int)reader["IdEstatus"],
                            Descripcion = string.IsNullOrEmpty((string)reader["Descripcion"]) ? "" : (string)reader["Descripcion"],
                            Estatus = (bool)reader["Estatus"]
                        }); ;
                    }
                }
            }
            return estatus;
        }

        public bool UpdateEstatus(int idUsuario, int iEstatus)
        {
            bool succes = false;
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "Update Usuarios set  iEstatus=" + iEstatus + " where idUsuario=" + idUsuario + " ";
                cmd.CommandType = CommandType.Text;

                succes = cmd.ExecuteNonQuery() > 0;
                _unitOfWork.SaveChanges();
            }
            return succes;
        }
    }
}
