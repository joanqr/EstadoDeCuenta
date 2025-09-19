using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BBCuentas.Helpers;
using BusinessLayer;
using r3Take.DataAccessLayer;
using System.Data;
using System.Collections;

namespace BBCuentas.Controllers
{
    public class CambiarContrasenaController : Controller
    {
        // GET: CambiarContrasena
        public ActionResult Index(string token)
        {
            Hashtable hashTableParameters = new Hashtable();
            DataTable dtExisteUsuario;
            DAL dal = new DAL();

            //Valida si el token de cambio de contraseña es válido
            dtExisteUsuario = dal.QueryDT("DS_ECWEB", "select cEmail from [dbo].[Usuarios] WHERE cToken = '" + token + "'", "", hashTableParameters, System.Web.HttpContext.Current);

            if (dtExisteUsuario.Rows.Count < 1)
            {
                return View("notvalid");
            }

            TempData["token"] = token;
            return View();
        }
        private Usuario_Business usuarioValid = new Usuario_Business();

        [HttpPost]
        public JsonResult UpdateContrasena(string token, string password)
        {
            bool succes = false;
            Hashtable hashTableParameters = new Hashtable();
            DataTable dtExisteUsuario;
            DAL dal = new DAL();

            try
            {
                //Valida si el token de cambio de contraseña es válido
                dtExisteUsuario = dal.QueryDT("DS_ECWEB", "select cEmail from [dbo].[Usuarios] WHERE cToken = '" + token + "'", "", hashTableParameters, System.Web.HttpContext.Current);

                if (dtExisteUsuario.Rows.Count < 1)
                {
                    return Json("La solicitud de cambio de contraseña ya no es válida.");
                }

                password = EncriptaPassword.GetMD5(password);
                succes = usuarioValid.updContrasena(token, password);
                if (succes)
                {
                    var response = "El cambio de contraseña se ha realizado";
                    return Json(response);
                }
                else
                {
                    var response = "Los datos no coinciden favor de validarlos";
                    return Json(response);
                }
            } catch (Exception ex)
            {
                return Json(ex.ToString());
            }
        }
    }
}