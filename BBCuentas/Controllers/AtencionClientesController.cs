using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBCuentas.Controllers
{
    public class AtencionClientesController : Controller
    {
        [Authorize(Roles = "AtencionClientes")]
        // GET: AtencionClientes
        public ActionResult Index()
        {
            return View();
        }
    }
}