using BusinessLayer;
using ModelLayer;
using System.Web.Mvc;
using r3Take.DataAccessLayer;
using System.Collections;
using BBCuentas.Helpers;
using System;
using System.Data;
using System.Text;

namespace BBCuentas.Controllers
{
    public class SecurityController : Controller
    {
        [Authorize(Roles = "User")]

        // GET: Security
        public ActionResult Index()
        {
            return View();
        }

        private Usuario_Business usuarioValid = new Usuario_Business();

        [HttpPost]
        public JsonResult UpdateUsuario(string usuario, string password, string newPAss, string cNombres, string cApePat, string cApeMat, string cCorreo)
        {
            bool succes = false;
            try
            {
                DAL dal = new DAL();
                Hashtable hashTableParameters = new Hashtable();

                cCorreo = ""; 

                hashTableParameters.Add("usuario", usuario);
                hashTableParameters.Add("newPassword", newPAss);
                hashTableParameters.Add("password", password);
                hashTableParameters.Add("nombres", cNombres);
                hashTableParameters.Add("apePat", cApePat);
                hashTableParameters.Add("apeMat", cApeMat);
                hashTableParameters.Add("correo", cCorreo);

                StringBuilder sbQueryUpdateUsuario = new StringBuilder();
                var response = "";

                if (cNombres.Length > 0)
                {
                    sbQueryUpdateUsuario.Append("UPDATE[dbo].[Usuarios] SET cNombre = @0, cPrimerApellido = @1, cSegundoApellido = @2 ");
                    response = "Los datos se han actualizado correctamente.";
                }
                    
                if (newPAss.Length > 0)
                {
                    sbQueryUpdateUsuario.Append("UPDATE[dbo].[Usuarios] SET cEMail = @5, cPasswd = '" + EncriptaPassword.GetMD5(newPAss) + "' ");
                    response = "Se ha cambiado correctamente su contraseña.";
                }
                                          


                if (dal.QueryDT("DS_ECWEB", "SELECT cEmail FROM [dbo].[Usuarios] WHERE  cEmail = @0 AND cPasswd = '" + EncriptaPassword.GetMD5(password) + "'", "H:S:usuario", hashTableParameters, System.Web.HttpContext.Current).Rows.Count > 0)
                {
                    dal.ExecuteScalar("DS_ECWEB", sbQueryUpdateUsuario +  " WHERE cEmail = @5 AND cPasswd = '" + EncriptaPassword.GetMD5(password) + "'", "H:S:nombres;H:S:apePat;H:S:apeMat;H:S:correo;H:S:newPassword;H:S:usuario;H:S:password", hashTableParameters, System.Web.HttpContext.Current);
                    return Json(response);
                }
                else
                {
                    response = "(E1) El usuario y / o contraseña no coinciden con algun usuario en el sistema, favor de verificarlos.";
                    return Json(response);
                }

                
            }
            catch(Exception ex)
            {
                var response = "(E1) Los datos no coinciden favor de validarlos.";
                return Json(response);
            }
        }

        [HttpPost]
        public JsonResult ValidaEstatusMantenimiento()
        {
            string EstaEnMantenimiento = "";
            try
            {
                DAL dal = new DAL();
                Hashtable hashTableParameters = new Hashtable();

                DataTable dtPaginaMantenimiento;

                dtPaginaMantenimiento = dal.QueryDT("DS_ECWEB", "SELECT PaginaEnMantenimiento, MensajePaginaEnMantenimiento FROM [dbo].[Configuraciones]", "", hashTableParameters, System.Web.HttpContext.Current);

                if (dtPaginaMantenimiento.Rows[0]["PaginaEnMantenimiento"].ToString() == "True")
                {
                    EstaEnMantenimiento = "1#" + dtPaginaMantenimiento.Rows[0]["MensajePaginaEnMantenimiento"].ToString();
                    return Json(EstaEnMantenimiento);
                }
                else
                {
                    EstaEnMantenimiento = "0#No esta en mantenimiento";
                    return Json(EstaEnMantenimiento);
                }
            }
            catch (Exception ex)
            {
                var response = "Error al acceder a base de datos";
                return Json(response);
            }
        }
    }
}