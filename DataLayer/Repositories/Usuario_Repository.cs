using DataLayer.UnitOfWork;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DataLayer.Repositories
{
    public class Usuario_Repository
    {
        private UoWUnitOfWork _unitOfWork;
        private UoWUnitOfWorkConauto _unitOfWorkConauto;
        public Usuario_Repository(IUnitOfWork uow)
        {
            if (uow == null)
                throw new ArgumentException("uow");
            _unitOfWork = uow as UoWUnitOfWork;
            if (_unitOfWork == null)
            {
                throw new NotSupportedException("Unit of work factory is null, can you validate?");
            }
        }

        public Usuario_Repository(IUnitOfWorkConauto uow)
        {
            if (uow == null)
                throw new ArgumentException("uow");
            _unitOfWorkConauto = uow as UoWUnitOfWorkConauto;
            if (_unitOfWorkConauto == null)
            {
                throw new NotSupportedException("Unit of work factory is null, can you validate?");
            }
        }

        public Usuario WsContratoUSuario(Usuario userio)
        {
            Usuario user = new Usuario();
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "Sel_Usuario"; ;
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter() { Value = userio.cEMail, ParameterName = "@cEmail" };
                cmd.Parameters.Add(parameters[0]);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user.idUsuario = Convert.ToInt32(reader["idUsuario"].ToString());
                        user.TipoFina = Convert.ToInt32(reader["iCompania"].ToString());
                        user.iContrato = (int)reader["iContrato"];
                        user.gpoCte1 = Convert.ToInt32(string.IsNullOrEmpty(reader["iGrupo"].ToString())?"0": reader["iGrupo"].ToString());
                        user.gpoCte2 = Convert.ToInt32(string.IsNullOrEmpty(reader["iCliente"].ToString())?"0":reader["iCliente"].ToString());
                        user.cNombre = string.IsNullOrEmpty(reader["cNombre"].ToString()) ? "" : reader["cSegundoApellido"].ToString();
                        user.cPrimerApellido = string.IsNullOrEmpty(reader["cPrimerApellido"].ToString()) ? "" : reader["cPrimerApellido"].ToString();
                        user.cSegundoApellido = string.IsNullOrEmpty(reader["cSegundoApellido"].ToString()) ? "" : reader["cEMail"].ToString();
                        user.CP = 15000;
                        user.DateCte = Convert.ToDateTime(reader["dtFechaReg"].ToString());
                    }
                }

            }
            return user;
        }

        public Usuario ValidarUsuario(Usuario usuario)
        {
            Usuario success = new Usuario();
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "Val_Usuario";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] parameters = new SqlParameter[2];
                parameters[0] = new SqlParameter() { Value = usuario.cEMail, ParameterName = "@Usuario" };
                parameters[1] = new SqlParameter() { Value = usuario.cPasswd, ParameterName = "@Password" };
                cmd.Parameters.Add(parameters[0]);
                cmd.Parameters.Add(parameters[1]);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        success.iEstatus = Convert.ToInt32(reader["Activo"]);
                        success.Rol = new Roles();
                        success.Rol.NombreRol = reader["NombreRol"].ToString();
                        success.idUsuario = Convert.ToInt32(reader["idUsuario"]);
                    }
                }
            }
            return success;
        }

        public bool InsertUsuario(Usuario usuario)
        {
            bool succes = false;

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "Crud_Usuario";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] parameter = new SqlParameter[11];
                parameter[0] = new SqlParameter() { Value = 1, ParameterName = "@Tipo" };
                parameter[1] = new SqlParameter() { Value = usuario.cEMail, ParameterName = "@Usuario" };
                parameter[2] = new SqlParameter() { Value = usuario.cPasswd, ParameterName = "@Password" };
                parameter[3] = new SqlParameter() { Value = usuario.cNombre, ParameterName = "@Nombre" };
                parameter[4] = new SqlParameter() { Value = usuario.cPrimerApellido, ParameterName = "@PrimerAp" };
                parameter[5] = new SqlParameter() { Value = usuario.cSegundoApellido, ParameterName = "@SegundoAp" };
                parameter[6] = new SqlParameter() { Value = usuario.cTelMovil, ParameterName = "@Cel" };
                parameter[7] = new SqlParameter() { Value = usuario.TipoFina, ParameterName = "@iCompania" };
                parameter[8] = new SqlParameter() { Value = usuario.gpoCte1, ParameterName = "@iGrupo" };
                parameter[9] = new SqlParameter() { Value = usuario.gpoCte2, ParameterName = "@iCliente" };
                parameter[10] = new SqlParameter() { Value = usuario.iContrato, ParameterName = "@iContrato" };
                cmd.Parameters.Add(parameter[0]);
                cmd.Parameters.Add(parameter[1]);
                cmd.Parameters.Add(parameter[2]);
                cmd.Parameters.Add(parameter[3]);
                cmd.Parameters.Add(parameter[4]);
                cmd.Parameters.Add(parameter[5]);
                cmd.Parameters.Add(parameter[6]);
                cmd.Parameters.Add(parameter[7]);
                cmd.Parameters.Add(parameter[8]);
                cmd.Parameters.Add(parameter[9]);
                cmd.Parameters.Add(parameter[10]);

                succes = cmd.ExecuteNonQuery() > 0;
                _unitOfWork.SaveChanges();
            }


            return succes;
        }

        public bool UpdUsuario(string email, string password, string NewPAssword)
        {
            bool succes = false;
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "Upd_Usuario";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] parameter = new SqlParameter[4];
                parameter[0] = new SqlParameter() { Value = 1, ParameterName = "@Tipo" };
                parameter[1] = new SqlParameter() { Value = email, ParameterName = "@Usuario" };
                parameter[2] = new SqlParameter() { Value = password, ParameterName = "@Password" };
                parameter[3] = new SqlParameter() { Value = NewPAssword, ParameterName = "@NewPaword" };
                cmd.Parameters.Add(parameter[0]);
                cmd.Parameters.Add(parameter[1]);
                cmd.Parameters.Add(parameter[2]);
                cmd.Parameters.Add(parameter[3]);
                succes = cmd.ExecuteNonQuery() > 0;
                _unitOfWork.SaveChanges();
            }
            return succes;
        }


        public bool UpdContrasena(string token, string password)
        {
            bool succes = false;
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "Upd_Contrasena";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] parameter = new SqlParameter[4];
                parameter[0] = new SqlParameter() { Value = 1, ParameterName = "@Tipo" };
                parameter[1] = new SqlParameter() { Value = token, ParameterName = "@Usuario" };
                parameter[2] = new SqlParameter() { Value = password, ParameterName = "@Password" };
                cmd.Parameters.Add(parameter[0]);
                cmd.Parameters.Add(parameter[1]);
                cmd.Parameters.Add(parameter[2]);
                succes = cmd.ExecuteNonQuery() > 0;
                _unitOfWork.SaveChanges();
            }
            return succes;
        }


        public List<Usuario> ObtUsuario()
        {
            List<Usuario> user = new List<Usuario>();
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "Select u.idUsuario, u.cNombre, u.cPrimerApellido,u.cSegundoApellido,u.cEMail,u.bMailVerificado,u.bTelVerificado, u.iEstatus AS IdEstatus, e.Descripcion AS cDescripcion from Usuarios u INNER JOIN Estatus e on u.iEstatus =e.IdEstatus";
                cmd.CommandType = CommandType.Text;

                var table = new List<Dictionary<string, object>>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user.Add(new Usuario()
                        {
                            idUsuario = (int)reader["idUsuario"],
                            cNombre = string.IsNullOrEmpty(reader["cNombre"].ToString()) ? "" : reader["cNombre"].ToString(),
                            cPrimerApellido = string.IsNullOrEmpty(reader["cPrimerApellido"].ToString()) ? "" : reader["cPrimerApellido"].ToString(),
                            cSegundoApellido = string.IsNullOrEmpty(reader["cSegundoApellido"].ToString()) ? "" : reader["cSegundoApellido"].ToString(),
                            cNombreCompleto = (string.IsNullOrEmpty(reader["cNombre"].ToString()) ? "" : reader["cNombre"].ToString()) + " " + (string.IsNullOrEmpty(reader["cPrimerApellido"].ToString()) ? "" : reader["cPrimerApellido"].ToString()) + " " + (string.IsNullOrEmpty(reader["cSegundoApellido"].ToString()) ? "" : reader["cSegundoApellido"].ToString()),
                            cEMail = string.IsNullOrEmpty(reader["cEMail"].ToString()) ? "" : reader["cEMail"].ToString(),
                            bMailVerificado = (bool)reader["bMailVerificado"],
                            bTelVerificado = (bool)reader["bTelVerificado"],
                            Estatus = new EstatusCliente((int)reader["IdEstatus"], string.IsNullOrEmpty(reader["cDescripcion"].ToString()) ? "" : reader["cDescripcion"].ToString())
                        });
                    }
                }

            }
            return user;
        }

        public List<UsuarioConauto> ObtUsuarioConauto()
        {
            List<UsuarioConauto> user = new List<UsuarioConauto>();
            using (var cmd = _unitOfWorkConauto.CreateCommand())
            {
                cmd.CommandText = "Select InEstatus from TblDtValida";
                cmd.CommandType = CommandType.Text;

                var table = new List<Dictionary<string, object>>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user.Add(new UsuarioConauto()
                        {
                            inEstatus = (int)reader["InEstatus"]
                        });
                    }
                }
            }
            return user;
        }

        public bool ValidEmailCel(int? contrato)
        {
            bool succes = false;
            int returns = 0;
            if (contrato==null)
            {
                contrato = 0;
            }
            using (var cmd = _unitOfWorkConauto.CreateCommand())
            {
                cmd.CommandText = "SelValEmailCel";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter() { Value = contrato, ParameterName = "@Contrato1" };
                cmd.Parameters.Add(parameters[0]);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        returns = Convert.ToInt32(reader["Result"]);
                       
                    }
                }
            }
            succes = Convert.ToBoolean(returns);
            return succes;
        }

        public bool ValidateExistUser(Usuario usuarios)
        {
            bool succes = false;
            int dos = 0;
            try
            {
                using (var cmd = _unitOfWork.CreateCommand())
                {
                    cmd.CommandText = "Val_UsuarioExists";
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter[] parameters = new SqlParameter[2];
                    parameters[0] = new SqlParameter() { Value = usuarios.cEMail, ParameterName = "@Usuario" };
                    parameters[1] = new SqlParameter() { Value = usuarios.cPasswd, ParameterName = "@Password" };
                    cmd.Parameters.Add(parameters[0]);
                    cmd.Parameters.Add(parameters[1]);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            succes = Convert.ToBoolean(string.IsNullOrEmpty(reader["Activo"].ToString()) ? "0" : reader["Activo"].ToString());
                        }
                    }
                    _unitOfWork.SaveChanges();
                }
                return succes;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
            
        }
        public bool UpdUsuarioCelMail(int idUsuario)
        {
            bool succes = false;
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "upd_UsuarioCelMail";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter() { Value = idUsuario, ParameterName = "@Idusuario" };
                cmd.Parameters.Add(parameters[0]);
                succes = cmd.ExecuteNonQuery() > 0;
                _unitOfWork.SaveChanges();
            }
            return succes;
        }

        public bool InsertContract(Usuario usuario)
        {
            bool succes = false;
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "Ins_Contrato";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] parameter = new SqlParameter[6];
                parameter[0] = new SqlParameter() { Value = 1, ParameterName = "@Tipo" };
                parameter[1] = new SqlParameter() { Value = usuario.TipoFina, ParameterName = "@iCompania" };
                parameter[2] = new SqlParameter() { Value = usuario.gpoCte1, ParameterName = "@iGrupo" };
                parameter[3] = new SqlParameter() { Value = usuario.gpoCte2, ParameterName = "@iCliente" };
                parameter[4] = new SqlParameter() { Value = usuario.iContrato, ParameterName = "@iContrato" };
                parameter[5] = new SqlParameter() { Value = usuario.cEMail, ParameterName = "@Email" };
                cmd.Parameters.Add(parameter[0]);
                cmd.Parameters.Add(parameter[1]);
                cmd.Parameters.Add(parameter[2]);
                cmd.Parameters.Add(parameter[3]);
                cmd.Parameters.Add(parameter[4]);
                cmd.Parameters.Add(parameter[5]);

                succes = cmd.ExecuteNonQuery() > 0;
                _unitOfWork.SaveChanges();
            }
            return succes;
        }




    }
}

