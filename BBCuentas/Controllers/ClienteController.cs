using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;

namespace BBCuentas.Controllers
{
    public class ClienteController : Controller
    {
        private Usuario_Business usuario = new Usuario_Business();
        private EstatusCliente_Business estatusCliente = new EstatusCliente_Business();
        // GET: Role
        [Authorize(Roles = "Admin")]
        public ActionResult Cliente()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetClientes(string alcome)
        {
            try
            {
                return Json(usuario.ObtieneUsuarios(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet]
        public ActionResult GetEstatus()
        {
            try
            {
                return Json(estatusCliente.ObtieneEstatusCliente(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult SetEstatusCliente(int idUsuario, int idEstatus)
        {
            bool resp;
            try
            {
                resp = estatusCliente.UpdateEstatusCliente(idUsuario, idEstatus);
                if (resp)
                {
                    var response = "Se ha cambiado estatus al cliente correctamente";
                    return Json(response);
                }
                else
                {
                    var response = "Los datos no coinciden favor de validarlos";
                    return Json(response);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        [HttpGet]
        public ActionResult GetEstadisticas()
        {
            try
            {
              
                //var listBBCuenta = usuario.ObtieneUsuarios();
                //var qry = listConauto.GroupJoin(listBBCuenta,
                //                  foo => foo.idUsuario,
                //                  b => b.idUsuario,
                //                  (x, y) => new { listConauto = x, listBBCuenta = y })
                //            .SelectMany(
                //                  x => x.listBBCuenta.DefaultIfEmpty(),
                //                  (x, y) => new { x.listConauto, listBBCuenta = y });


                var totalActivos = usuario.ObtieneUsuariosConauto().Where(d => d.inEstatus  == 1).Count();
                var totalFaltantes = usuario.ObtieneUsuariosConauto().Where(d => d.inEstatus == 0).Count();
                var totalMigrados = usuario.ObtieneUsuariosConauto().Where(d => d.inEstatus == 1).Count();

                return Json(new { totalAct = totalActivos, totalFalt = totalFaltantes, totalMig = totalMigrados }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}