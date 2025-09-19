using BBCuentas.Helpers;
using BusinessLayer;
using ModelLayer;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using BBCuentas.Helpers;
using r3Take.DataAccessLayer;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;

namespace BBCuentas.Controllers
{
    public class LoginController : System.Web.Mvc.Controller
    {

        private Usuario_Business usuarioValid = new Usuario_Business();
        private Mantenimiento_Business mantenimiento = new Mantenimiento_Business();
        // GET: Login
        public async Task<ActionResult> Index()
        {

            return View();
        }

        [HttpPost]
        public JsonResult RecuperaEtiquetaPantallaTipoFinanciamiento()
        {
            bool succes = false;
            string mensajeResp;
            try
            {
                DAL dal = new DAL();
                DataTable dtEtiquetaPantallaTipoFinanciamiento;
                Hashtable hashTableParameters = new Hashtable();

                dtEtiquetaPantallaTipoFinanciamiento = dal.QueryDT("DS_ECWEB", "SELECT EtiquetaPantallaTipoFinanciamiento FROM [dbo].[Configuraciones]", "", hashTableParameters, System.Web.HttpContext.Current);


                if (dtEtiquetaPantallaTipoFinanciamiento.Rows.Count > 0)
                {
                    mensajeResp = dtEtiquetaPantallaTipoFinanciamiento.Rows[0]["EtiquetaPantallaTipoFinanciamiento"].ToString();
                    return Json(new { mensaje = mensajeResp });
                }
                return Json(new { mensaje = "Llene el campo correspondiente según su tipo de contrato " });
            }
            catch (Exception ex)
            {
                succes = false;
                return Json(new { mensaje = "Ocurrio un error al tratar de consultar etiqueta " });
            }
        }

        [HttpPost]
        public async Task<ActionResult> LogIn(string usuario, string password)
        {
            string role = "";
            int result = 0;
            string mensajeResp = "";
            try
            {

                DAL dal = new DAL();
                System.Collections.Hashtable hashTableParameters = new System.Collections.Hashtable();

                DataTable dtPaginaMantenimiento;
                dtPaginaMantenimiento = dal.QueryDT("DS_ECWEB", "SELECT PaginaEnMantenimiento, CorreoAdministrador, MensajeUsuarioPasswordIncorrecto  FROM [dbo].[Configuraciones]", "", hashTableParameters, System.Web.HttpContext.Current);
                bool EsAdministrador = true;

                if (dtPaginaMantenimiento.Rows.Count > 0)
                {
                    mensajeResp = dtPaginaMantenimiento.Rows[0]["MensajeUsuarioPasswordIncorrecto"].ToString();
                    if (dtPaginaMantenimiento.Rows[0]["PaginaEnMantenimiento"].ToString() == "True")
                    {
                        if (usuario != dtPaginaMantenimiento.Rows[0]["CorreoAdministrador"].ToString())
                        {
                            EsAdministrador = false;
                        }
                    }
                }



                if (EsAdministrador == true)
                {
                    password = EncriptaPassword.GetMD5(password);

                    wsEsweb validar = new wsEsweb();
                    ResponseEsweb respuesta = new ResponseEsweb();
                    Usuario usuariox = new Usuario();
                    Usuario otroUser = new Usuario();
                    bool resp = true;
                    usuariox.cEMail = usuario;
                    usuariox.cPasswd = password;
                    otroUser.cEMail = usuario;
                    otroUser.cPasswd = password;

                    usuariox = usuarioValid.ValidarUsuario(usuariox);  //Valida si usuario existe y esta activo en base de datos ecweb ante sp Val_Usuario
                    if (usuariox.iEstatus == 1)
                    {   //11:564
                        //Buscar Rol de usuario en base de datos  "User,Cliente;Admin"
                        role = usuariox.Rol.NombreRol; //"User"; 
                        var authTicket = new FormsAuthenticationTicket(
                                                                            1,                           // version
                                                                            usuario,                     // user name
                                                                            DateTime.Now,                // created
                                                                            DateTime.Now.AddMinutes(20), // expires
                                                                            false,                       // persistent?
                                                                            role      // can be used to store roles
                                                                            );

                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                        System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
                        Response.Cookies.Add(new HttpCookie("Nombre", usuario));
                        Response.Cookies.Add(new HttpCookie("Usuario", usuariox.idUsuario.ToString()));

                        // FormsAuthentication.SetAuthCookie(usuariox.cEMail, false);
                        result = 1;

                        //return Json(new { result = 1, role = role }, JsonRequestBehavior.AllowGet); //Setear en una variable el Rol
                    }   //El usuario Existe en bd
                    else
                    {
                        bool ExistEmail = usuarioValid.ValadaExistUser(otroUser); //Primero validamos que el usuario Exista Ejecutando el SP Val_UsuarioExists
                        if (ExistEmail)   //¿Existe en [ecweb]?
                        {
                            bool valConnaEmailCel;
                            otroUser = usuarioValid.WsContratoUSuario(otroUser);//Devolvemos los datos para el WServices
                            //respuesta = await validar.Consume(otroUser);
                            if (otroUser.iContrato != 0)
                            {
                                //respuesta.opildContrato = 800005258;
                                valConnaEmailCel = usuarioValid.ValidEmailCel(otroUser.iContrato); //Válido en connauto que esten los 2 en 1

                                if (valConnaEmailCel)   //Si existen valores en 1 entonces actualizo mi BD de esWeb
                                {
                                    resp = usuarioValid.UpdUsuarioCelMail(otroUser.idUsuario);


                                    result = 4;
                                    return Json(new { result = result, role = role, mensajeResp = "Usuario autenticado de forma exitosa" }, JsonRequestBehavior.AllowGet); //Setear en una variable el Rol


                                    //Código eliminado por Alcome, ya que es imposible validar contra el Web Service a un usuarios cuando nos e tienen todos los datos completos como el código postal
                                    //if (respuesta.opildContrato != 0)
                                    //{
                                    //    resp = usuarioValid.UpdUsuarioCelMail(otroUser.idUsuario);
                                    //    result = 4;
                                    //}
                                    //else // El usuario no se encuentra en los resultados de WS
                                    //{
                                    //    result = 3;
                                    //    return Json(new { result = result, mensajeResp = "El usuario no se encontro en el sistema, favor de verificarlo." }, JsonRequestBehavior.AllowGet);
                                    //}
                                }
                                else
                                {
                                    result = 6;
                                    return Json(new { result = result, mensajeResp = mensajeResp }, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                        else
                        {
                            result = 2;
                            return Json(new { result = result, mensajeResp = mensajeResp }, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                else {
                    result = 5;
                    return Json(new { result = result, mensajeResp = " La página se encuentra en mantenimiento, el unico usuario que puede acceder es el administrador del sistema" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                return Json(ex);
            }
            return Json(new { result = result, role = role, mensajeResp = mensajeResp }, JsonRequestBehavior.AllowGet); //Setear en una variable el Rol

        }

        [HttpPost]
        public JsonResult ValidaSiExisteContratoPrevio(string Contrato)
        {
            bool succes = false;
            try
            {
                DAL dal = new DAL();
                Hashtable hashTableParameters = new Hashtable();

                DataTable dtExixteContrato;
                hashTableParameters.Add("contrato", Contrato);
                dtExixteContrato = dal.QueryDT("DS_ECWEB", "select idUsuario from [dbo].[Contratos] WHERE iContrato = @0", "H:S:contrato", hashTableParameters, System.Web.HttpContext.Current);

                if (dtExixteContrato.Rows.Count > 0)
                {
                    succes = false;
                    return Json(new { result = succes, mensaje = "Este contrato ya fue registrado previamente, favor de ingresar uno diferente.", contract = Contrato });
                }
                else
                {
                    succes = true;
                    return Json(new { result = succes, mensaje = "El contrato no existe en base de datos, continue con el formulario para agregarlo.", contract = Contrato });
                }

            }
            catch (Exception ex)
            {
                succes = false;
                return Json(new { result = succes, mensaje = "El contrato indicado es incorrecto, debe ser numérico de hasta 9 dígitos, sin espacios o guiones", contract = Contrato });
            }
        }

        [HttpPost]
        public async Task<JsonResult> AgregarUsuario(Usuario Usuario)
        {
            //Usuario usuarioNuevo = new Usuario() { cEMail = usuario, cPasswd = password };
            bool succes = false;
            wsEsweb validar = new wsEsweb();
            string response = "";
            BBCuentas.Helpers.AppRemota appRemota = new Helpers.AppRemota();

            Usuario.cPasswd = EncriptaPassword.GetMD5(Usuario.cPasswd);

            DAL dal = new DAL();
            Hashtable hashTableParameters = new Hashtable();

            DataTable dtExixteContrato;
            hashTableParameters.Add("contrato", Usuario.iContrato);
            hashTableParameters.Add("grupo", Usuario.gpoCte1);
            hashTableParameters.Add("cliente", Usuario.gpoCte2);
            if (Usuario.iContrato > 0)
            {
                dtExixteContrato = dal.QueryDT("DS_ECWEB", "select idUsuario from [dbo].[Contratos] WHERE iContrato = @0", "H:S:contrato", hashTableParameters, System.Web.HttpContext.Current);
                response = "Contrato erroneo, favor de validar";
                if (dtExixteContrato.Rows.Count > 0)
                {
                    response = "Este contrato ya fue registrado previamente, favor de ingresar uno diferente.";
                    return Json(response);
                }
            }

            if (Usuario.gpoCte1 > 0)
            {
                dtExixteContrato = dal.QueryDT("DS_ECWEB", "select idUsuario from [dbo].[Contratos] WHERE iGrupo = @1 AND iCliente = @2", "H:S:contrato;H:S:grupo;H:S:cliente", hashTableParameters, System.Web.HttpContext.Current);
                response = "Grupo / Cliente erroneo, favor de validar";
                if (dtExixteContrato.Rows.Count > 0)
                {
                    response = "Este contrato ya fue registrado previamente, favor de ingresar uno diferente.";
                    return Json(response);
                }
            }

            WSValidaEmpresa.wsecwebObjClient wsValidaEmpresa = new WSValidaEmpresa.wsecwebObjClient();
            WSValidaEmpresa.wsEcwebRequest request = new WSValidaEmpresa.wsEcwebRequest();

            int? opiCodigo;
            string opcMensaje = "";
            int? opilContrato;
            int? opilEmpresa;
            int? opiGrupo;
            int? opiCliente;

            request.ipiContrato = Usuario.iContrato;
            request.ipiGrupo = Usuario.gpoCte1;
            request.ipiCliente = Usuario.gpoCte2;
            request.ipcNombre = Usuario.cNombre;
            request.ipcPrimerap = Usuario.cPrimerApellido;
            request.ipcSegundoap = Usuario.cSegundoApellido;
            request.ipiCp = Usuario.CP;
            request.ipcFecha = Usuario.DateCte.ToString("yyyy-MM-dd");
            try
            {
                wsValidaEmpresa.wsEcweb(request.ipiContrato, request.ipiGrupo, request.ipiCliente, request.ipcNombre, request.ipcPrimerap, request.ipcSegundoap, request.ipiCp, request.ipcFecha, out opiCodigo, out opcMensaje, out opilContrato, out opilEmpresa,  out opiGrupo, out opiCliente);
                Usuario.TipoFina = Convert.ToInt32(opilEmpresa);
                Usuario.iContrato = Convert.ToInt32(opilContrato);
                Usuario.gpoCte1 = Convert.ToInt32(opiGrupo);
                Usuario.gpoCte2 = Convert.ToInt32(opiCliente);

                //ResponseEsweb nuevo = await validar.Consume(Usuario);

                if (opilContrato != 0)
                {
                    if (Usuario.iContrato == 0)
                    {
                        string contrato = Usuario.gpoCte1.ToString() + Usuario.GpoCteString.ToString();
                        Usuario.iContrato = Convert.ToInt32(string.IsNullOrEmpty(contrato) ? "0" : contrato);
                    }
                    succes = usuarioValid.InsertUsuario(Usuario);
                    if (succes)
                    {
                        appRemota.consume(Usuario, Usuario.iContrato);
                    }
                    response = "El usuario ya existe, favor de validar";
                }
                //succes = usuarioValid.InsertUsuario(Usuario);
                if (succes)
                {
                    response = "Gracias por registrarse. Recibirá un mensaje en su correo electrónico y teléfono celular para activar su cuenta";
                    return Json(response);
                }
                else
                {
                    return Json(response);
                }
            }
            catch(Exception ex)
            {
                response = "Ocurrio un error al registrar al usuario";
                return Json(response);
            }
    }

        [HttpPost]
        public ActionResult OlvidoContrasena(string email)
        {
            //Variables
            bool succes = true;
            string carpeta = WebConfigurationManager.AppSettings["UrlNuevo"].ToString();
            string cToken = "";
            string cTokenAnt = "";
            DataTable dtExisteUsuario;
            DataTable dtResultado;
            Hashtable hashTableParameters = new Hashtable();
            DAL dal = new DAL();

            try
            {
                //Valida si existe el usuario
                //hashTableParameters.Add("idUsuario", email);
                dtExisteUsuario = dal.QueryDT("DS_ECWEB", "select cToken from [dbo].[Usuarios] WHERE cEmail = '" + email + "'", "", hashTableParameters, System.Web.HttpContext.Current);

                if (dtExisteUsuario.Rows.Count < 1)
                {
                    return Json("El correo electrónico es incorrecto.");
                }
                cTokenAnt = dtExisteUsuario.Rows[0]["cToken"].ToString();

                //Ejecuta stored procedure para generar un nuevo token
                dal = new DAL();

                List<DBParam> lParam = new List<DBParam>();
                lParam.Add(new DBParam("Usuario", email, "s"));

                dal.ExecuteSP("DS_ECWEB", "Olv_Contrasena", lParam, System.Web.HttpContext.Current);

                //Recupera el token del usuario para mandarlo en el correo
                dtResultado = dal.QueryDT("DS_ECWEB", "select cToken from [dbo].[Usuarios] WHERE cEmail = '" + email + "'", "", hashTableParameters, System.Web.HttpContext.Current);

                if (dtResultado.Rows.Count < 1)
                {
                    return Json("Error en procedimiento de cambio de contraseña");
                }
                cToken = dtResultado.Rows[0]["cToken"].ToString();

                if (cTokenAnt == cToken)
                {
                    return Json("Error al generar token de contraseña");
                }
            } catch(Exception ex) {
                return Json(ex.ToString());
            }

            //Arma el contenido y envía correo de cambio de contraseña
            string htmlBody = "<a href='" + carpeta + cToken +  "'><strong>Aquí cambía tu contraseña</strong></a>";
            string subject = "Restablecer contraseña";
            EnviaEmail emailz = new EnviaEmail();
            string response = "";
            try
            {
                emailz.SendEmail(email, htmlBody, subject);
                succes = true;

            }
            catch (Exception)
            {
                succes = false;
                throw;
            }
            if (succes)
            {
                response = "Se ha enviado un correo electrónico para el cambio de contraseña";
            }
            else
            {
                response = "Hubo un problema en el envío del mensaje";
            }
            return Json(response);
        }

        [Authorize]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ActivaMatenimiento()
        {
            var inactiva = mantenimiento.ObtMantenimiento();
            if (inactiva == 1)
            {
                //  Response.AddHeader("Location", "google.com");
                Response.Redirect("~/Manteni/Index.html");
            }
            return Json(1);
        }

    }
}