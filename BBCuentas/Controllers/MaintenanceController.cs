using BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBCuentas.Controllers
{
    public class MaintenanceController : Controller
    {
        private  Mantenimiento_Business mantenimiento = new Mantenimiento_Business();
        // GET: maintenance
        [Authorize(Roles = "Admin")]
        public ActionResult Maintenance()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetMaintenence()
        {
            try
            {
                return Json(mantenimiento.ObtMantenimiento(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        [HttpPost]
        public ActionResult SetMaintenence(int opcion)
        {
            try
            {
                return Json(mantenimiento.UpdateMantenimento(opcion), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}