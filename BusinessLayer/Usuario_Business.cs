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
    public class Usuario_Business
    {
        public Usuario ValidarUsuario(Usuario usuario)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var repository = new Usuario_Repository(uow);
                var response = repository.ValidarUsuario(usuario);
                return response;
            }
        }

         public Usuario WsContratoUSuario(Usuario user)
        {
            using (var uow= UnitOfWorkFactory.Create())
            {
                var repository = new Usuario_Repository(uow);
                var response = repository.WsContratoUSuario(user);
                return response;
            }
        }
            
        public bool ValidEmailCel(int? contrato)
        {
            using (var uow = UnitOfWorkFactory.CreateCanauto())
            {
                var repository = new Usuario_Repository(uow);
                var response = repository.ValidEmailCel(contrato);
                return response;
            }
        }

        public bool ValadaExistUser(Usuario usuario)
        {
            using (var uow= UnitOfWorkFactory.Create())
            {
                var repository = new Usuario_Repository(uow);
                var response = repository.ValidateExistUser(usuario);
                return response;
            }
        }


        public bool InsertUsuario(Usuario user)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var repository = new Usuario_Repository(uow);
                var response = repository.InsertUsuario(user);
                return response;
            }
        }

        public bool UpdUsuarioCelMail(int idUsuario)
        {
            using (var uow=UnitOfWorkFactory.Create())
            {
                var repository = new Usuario_Repository(uow);
                var response = repository.UpdUsuarioCelMail(idUsuario);
                return response;
            }
        }

        public bool InsertContract(Usuario usuario)
        {
            if (usuario.iContrato == 0)
            {
                string contrato = usuario.gpoCte1.ToString() + usuario.GpoCteString.ToString();
                usuario.iContrato = Convert.ToInt32(string.IsNullOrEmpty(contrato) ? "0" : contrato);
            }
            using (var uow = UnitOfWorkFactory.Create())
            {
                var repository = new Usuario_Repository(uow);
                var response = repository.InsertContract(usuario);
                return response;
            }
        }

        public bool updUsuario(string email, string pass, string NewPass)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var repository = new Usuario_Repository(uow);
                var response = repository.UpdUsuario(email, pass, NewPass);
                return response;
            }
        }

        public bool updContrasena(string token, string pass)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var repository = new Usuario_Repository(uow);
                var response = repository.UpdContrasena(token, pass);
                return response;
            }
        }

        public List<Usuario> ObtieneUsuarios()
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var repository = new Usuario_Repository(uow);
                var response = repository.ObtUsuario().Select(s => new Usuario
                {
                    idUsuario = s.idUsuario,
                    cNombreCompleto = string.IsNullOrEmpty(s.cNombreCompleto) ? "" : s.cNombreCompleto,
                    cEMail = string.IsNullOrEmpty(s.cEMail) ? "" : s.cEMail,
                    bMailVerificado = s.bMailVerificado,
                    bTelVerificado = s.bTelVerificado,
                    cSituacionCliente = (s.bMailVerificado == true && s.bTelVerificado == true) ? "Correo y teléfono validados" : (s.bMailVerificado == false && s.bTelVerificado == true) ? "E-mail pediente de validar" : (s.bTelVerificado == false && s.bMailVerificado == true) ? "Teléfono pendiente de validar" : "Correo y teléfono pendientes de validar",

                    Estatus = s.Estatus
                }).ToList();
                return response;
            }
            //cNombre = string.IsNullOrEmpty(s.cNombre) ? "" : s.cNombre,
            //        cPrimerApellido = string.IsNullOrEmpty(s.cPrimerApellido) ? "" : s.cPrimerApellido,
            //        cSegundoApellido = string.IsNullOrEmpty(s.cSegundoApellido) ? "" : s.cSegundoApellido,
        }

        public List<UsuarioConauto> ObtieneUsuariosConauto()
        {
            using (var uow = UnitOfWorkFactory.CreateCanauto())
            {
                var repository = new Usuario_Repository(uow);
                var response = repository.ObtUsuarioConauto().Select(s => new UsuarioConauto
                {
                    inEstatus = s.inEstatus,   
                }).ToList();
                return response;
            }
        }


        
    }
}
