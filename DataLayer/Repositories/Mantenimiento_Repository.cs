using DataLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public class Mantenimiento_Repository
    {

        private UoWUnitOfWork _unitOfWork;

        public Mantenimiento_Repository(IUnitOfWork uow)
        {
            if (uow == null)
                throw new ArgumentException("uow");
            _unitOfWork = uow as UoWUnitOfWork;
            if (_unitOfWork == null)
            {
                throw new NotSupportedException("Unit of work factory is null, can you validate?");
            }
        }
        public int ObtMantenimiento()
        {
            int opcion = 0;
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "SELECT PaginaEnMantenimiento FROM Configuraciones ";
                cmd.CommandType = CommandType.Text;


                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        opcion = Convert.ToInt32(reader["PaginaEnMantenimiento"]);

                    }
                }
            }
            return opcion;
        }

        public bool UpdateMantenimento(int opcion)
        {
            bool succes = false;
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "UPDATE Configuraciones SET PaginaEnMantenimiento =  " + opcion;
                cmd.CommandType = CommandType.Text;

                succes = cmd.ExecuteNonQuery() > 0;
                _unitOfWork.SaveChanges();
            }
            return succes;
        }

    }
}
