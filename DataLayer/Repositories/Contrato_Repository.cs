using System;
using System.Data;
using System.Data.SqlClient;
using ModelLayer;
using DataLayer.UnitOfWork;
using System.Collections.Generic;

namespace DataLayer.Repositories
{
    public class Contrato_Repository
    {
        private UoWUnitOfWork _unitOfWork;
        public Contrato_Repository(IUnitOfWork uow)
        {
            if (uow == null)
                throw new ArgumentException("uow");
            _unitOfWork = uow as UoWUnitOfWork;
            if (_unitOfWork == null)
            {
                throw new NotSupportedException("Unit of work factory is null, can you validate?");
            }
        }
        public List<Usuario>ObtContratoUsuario()
        {
            List<Usuario> user = new List<Usuario>();
            using (var cmd = _unitOfWork.CreateCommand())
            {
                try
                {
                    cmd.CommandText = "Select cNombre, cPrimerApellido,cSegundoApellido,cEMail,ISNULL(iEstatus,0) AS iEstatus,ISNULL(iContrato,0) AS iContrato, iCompania  from Usuarios u inner join Contratos c on u.idUsuario =c.idUsuario";
                    cmd.CommandType = CommandType.Text;

                    var table = new List<Dictionary<string, object>>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user.Add(new Usuario()
                            {
                                cNombre = string.IsNullOrEmpty(reader["cNombre"].ToString()) ? "" : reader["cNombre"].ToString(),
                                cPrimerApellido = string.IsNullOrEmpty(reader["cPrimerApellido"].ToString()) ? "" : reader["cPrimerApellido"].ToString(),
                                cSegundoApellido = string.IsNullOrEmpty(reader["cSegundoApellido"].ToString()) ? "" : reader["cSegundoApellido"].ToString(),
                                cNombreCompleto = (string.IsNullOrEmpty(reader["cNombre"].ToString()) ? "" : reader["cNombre"].ToString()) + " " + (string.IsNullOrEmpty(reader["cPrimerApellido"].ToString()) ? "" : reader["cPrimerApellido"].ToString()) + " " + (string.IsNullOrEmpty(reader["cSegundoApellido"].ToString()) ? "" : reader["cSegundoApellido"].ToString()),
                                cEMail = string.IsNullOrEmpty((string)reader["cEMail"]) ? "" : (string)reader["cEMail"],
                                iEstatus = (int)reader["iEstatus"],
                                iContrato = (int)reader["iContrato"],
                                TipoFina = (int)reader["iCompania"]
                            });
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
                

            }
            return user;
        }

        public List<Contrato> ObtContratoCliente(int idUsuario)
        {
            List<Contrato> contrato = new List<Contrato>();
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "Select u.idUsuario,ISNULL(iContrato,0) AS iContrato,e.cNombre nombCompania,c.iCompania, CONVERT(varchar(20), c.iGrupo) + RIGHT('00000000' + CONVERT(varchar(20), c.iCliente),3 )   grupocliente " +
                                   "from Usuarios u inner join Contratos c on u.idUsuario = c.idUsuario  " +
                                       "inner join Empresas e on e.iEmpresa = c.iCompania " +
                                   "where u.idUsuario=" + idUsuario + "";
                cmd.CommandType = CommandType.Text;

                var table = new List<Dictionary<string, object>>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        contrato.Add(new Contrato()
                        {
                            idUsuario = (int)reader["idUsuario"],
                            iContrato = (int)reader["iContrato"],
                            grupocliente = reader["grupocliente"].ToString(),
                            nombCompania = string.IsNullOrEmpty((string)reader["nombCompania"]) ? "" : (string)reader["nombCompania"],
                            iCompania = Convert.ToInt32(reader["iCompania"])
                        });
                    }
                }

            }
            return contrato;
        }
        public string ObtEmpresaPorId(int iCompania)
        {
            Contrato empresa = new Contrato();
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "Select cNombre From Empresas Where iEmpresa= " + iCompania + "";
                cmd.CommandType = CommandType.Text;             
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        empresa.nombCompania = string.IsNullOrEmpty((string)reader["cNombre"]) ? "" : (string)reader["cNombre"];
                   
                    }
                }
            }
            return empresa.nombCompania;
        }
    }
}
