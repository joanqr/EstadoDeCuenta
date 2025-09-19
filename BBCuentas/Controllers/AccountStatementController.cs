using BBCuentas.Models;
using ModelLayer;
using BusinessLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using r3Take.DataAccessLayer;
using System.Collections;
using System.Data;
using System.Threading.Tasks;
using BBCuentas.WSGetEstadoCuenta;

namespace BBCuentas.Controllers
{
    public class AccountStatementController : System.Web.Mvc.Controller
    {
        static string carpetaLocal;
        static string empresa;
        static int idCliente;
        static List<FileByYearAndMonth> listaFinal = new List<FileByYearAndMonth>();
        static List<Contract> contractsList = new List<Contract>();
        private EnviaEmail enviaEmail = new EnviaEmail();
        private Contrato_Business contrato = new Contrato_Business();
        private Usuario_Business usuarioValid = new Usuario_Business();

        [Authorize(Roles = "User")]
        // GET: AccountStatement
        public ActionResult AccountStatement()
        {
            Session["contractsList"] = null;

            idCliente = 0;
            idCliente = Convert.ToInt32(Request.Cookies["Usuario"].Value.ToString());
            var userContracts = GetContract(idCliente);

            if (userContracts != null && userContracts.Count > 0)
                if (contractsList.Count > 0)
            {
                    ViewBag.EmpresaId = DetermineCompanyId(userContracts);
                    return View();
            }
            else
            {
                ViewData["Message"] = "Lo sentimos , por momento no cuenta con algun contrato definido";
                return RedirectToAction("Index", "Home", new { parametro = ViewData["Message"] });
            }
        }
        private static int DetermineCompanyId(IEnumerable<Contract> contracts)
        {
            if (contracts == null)
            {
                return 0;
            }

            var companyIds = contracts.Select(c => c.CompanyId)
                                      .Where(id => id > 0)
                                      .Distinct()
                                      .ToList();

            if (companyIds.Count == 1)
            {
                return companyIds[0];
            }

            if (companyIds.Count > 1)
            {
                return companyIds.First();
            }

            if (contracts.Any(c => string.Equals(c.Empresa, "Fina", StringComparison.OrdinalIgnoreCase)))
            {
                return 1;
            }

            if (contracts.Any(c => string.Equals(c.Empresa, "Cona", StringComparison.OrdinalIgnoreCase)))
            {
                return 2;
            }

            return 0;
        }
        [HttpPost]
        public ActionResult GetPdfFileList(string contrato)
        {
            if (contrato != "default")
            {
                //Se llena en el Login la empresa 
                var listaContratos = (List<Contract>)Session["contractsList"];

                listaFinal.Clear();
                var listaMesAnio = new List<YearMonth>();

                //if (listaContratos.Any(a => a.ContractNomber == contrato))
                //{
                empresa = null;
                carpetaLocal = null;
                empresa = listaContratos.Where(b => b.ContractNumber == Convert.ToInt32(contrato)).Select(a => a.Empresa).FirstOrDefault();
                //}
                var sessionData = new { empresa = empresa, contrato = contrato };

                carpetaLocal = WebConfigurationManager.AppSettings["CarpetaLocal"].ToString();
                /*var sourceDirectory = carpetaLocal; *///Server.MapPath("~/" + carpetaLocal);

                var sourceDirectory = $"/{sessionData.empresa}";
                var cadenaBusqueda = $"*{sessionData.contrato}.pdf";
                string GrupoCliente = "0";
                int iCompania = 0;
                //Alcome
                try
                {
                    DAL dal = new DAL();
                    Hashtable hashTableParametersContratos = new Hashtable();

                    DataTable dtGrupoCliente;
                    hashTableParametersContratos.Add("contrato", contrato);
                    dtGrupoCliente = dal.QueryDT("DS_ECWEB", "select iGrupo, iCliente, iCompania from [dbo].[Contratos] WHERE iContrato = @0", "H:S:contrato", hashTableParametersContratos, System.Web.HttpContext.Current);

                    if (dtGrupoCliente.Rows.Count > 0)
                    {
                        string ClienteCadena = "";
                        Int32 Cliente = Convert.ToInt32(dtGrupoCliente.Rows[0]["iCliente"].ToString());

                        if (Cliente.ToString().Length == 1)
                            ClienteCadena = "00" + Cliente.ToString();
                        if (Cliente.ToString().Length == 2)
                            ClienteCadena = "0" + Cliente.ToString();
                        if (Cliente.ToString().Length == 3)
                            ClienteCadena = Cliente.ToString();

                        string Grupo = dtGrupoCliente.Rows[0]["iGrupo"].ToString();


                        GrupoCliente = Grupo + ClienteCadena;


                        iCompania = Convert.ToInt32(dtGrupoCliente.Rows[0]["iCompania"].ToString());
                        wsSolpdfRequest PDFRequest = new wsSolpdfRequest();
                        PDFRequest.ipiCliente = Cliente;
                        PDFRequest.ipiGrupo = Convert.ToInt32(Grupo);
                        PDFRequest.ipiEmpresa = iCompania;
                        WSGetEstadoCuenta.solpdfObjClient GetPDF = new solpdfObjClient();

                        int? CodigoRespuestaGetPDF = 0;
                        string MensajeRespuestaGetPDF = "";

                        
                        string EmpresaCadena = "";

                        if (iCompania == 1)
                        {
                            EmpresaCadena = "Fina";
                        }
                        if (iCompania == 2)
                        {
                            EmpresaCadena = "Cona";
                        }

                        string RutaEstadoCuentaActual = "";
                        DateTime CurrentDate = DateTime.Today;
                        int currentDay = CurrentDate.Day;
                        if (currentDay < 4)
                            CurrentDate = CurrentDate.AddMonths(-1);   
                        
                        string currentMonth = CurrentDate.Month.ToString();
                        string currentYear = CurrentDate.Year.ToString();
                        RutaEstadoCuentaActual = WebConfigurationManager.AppSettings["CarpetaLocal"].ToString() + "//" + EmpresaCadena + "//" + currentYear + "//" + currentMonth + "//";
                                               
                        var cadenaBusquedaEstadoCuentaActual = $"*" + GrupoCliente + ".pdf";

                        if (!Directory.Exists(RutaEstadoCuentaActual))
                            Directory.CreateDirectory(RutaEstadoCuentaActual);

                        IEnumerable<string> filesEstadoCuentaActual = Directory.EnumerateFiles(RutaEstadoCuentaActual, cadenaBusquedaEstadoCuentaActual, SearchOption.AllDirectories).OrderByDescending(filename => filename); ;
                            var listExisteEstadoCuentaActual = new List<string>(filesEstadoCuentaActual);

                            if (listExisteEstadoCuentaActual.Count == 0)
                                GetPDF.wsSolpdf(PDFRequest.ipiEmpresa, PDFRequest.ipiGrupo, PDFRequest.ipiCliente, out CodigoRespuestaGetPDF, out MensajeRespuestaGetPDF);
                                       

                    }


                    var cadenaBusqueda2 = $"*" + GrupoCliente + ".pdf";
                    IEnumerable<string> files2 = Directory.EnumerateFiles(carpetaLocal + sourceDirectory, cadenaBusqueda2, SearchOption.AllDirectories).OrderByDescending(filename => filename); ;
                    var list = new List<string>(files2);
                  


                    Hashtable hashTableParameters = new Hashtable();
                    DataTable dtNumeroEstadosCuenta;
                    dtNumeroEstadosCuenta = dal.QueryDT("DS_ECWEB", "SELECT NumeroEstadosCuentaAMostrar FROM [dbo].[Configuraciones]", "", hashTableParameters, System.Web.HttpContext.Current);

                    int NumeroEstadosCuenta = Convert.ToInt16(dtNumeroEstadosCuenta.Rows[0]["NumeroEstadosCuentaAMostrar"].ToString());
                    int NumeroEstadosCuentaContador = 0;
                    foreach (var f in list)
                    {
                        var total = f.Split('\\').Length - 1;
                        var year = f.Split('\\')[total - 2];
                        var month = f.Split('\\')[total - 1];
                        var name = f.Split('\\')[total - 0];
                        // Descomentar para liberar
                        //var carpeta = f.Split('\\')[total - 4];
                        string carpeta = WebConfigurationManager.AppSettings["IPCarpeta"].ToString();
                        var empresaD = f.Split('\\')[total - 3];
                        if(NumeroEstadosCuentaContador < NumeroEstadosCuenta)
                        {
                            listaFinal.Add(new FileByYearAndMonth(year, month, name, carpetaLocal, empresaD));
                        }

                        NumeroEstadosCuentaContador = NumeroEstadosCuentaContador + 1;
                    }

                    foreach (var f in listaFinal)
                    {
                        Console.WriteLine($"Año:{f.Year} Mes:{f.Month} Archivo:{f.FilePath}");
                    }

                    var years = listaFinal.Select(x => x.Year).Distinct().ToList();

                    foreach (var y in years)
                    {
                        Console.WriteLine($"Año:{y}");
                    }

                    foreach (var y in years)
                    {
                        var months = listaFinal.Where(x => x.Year == y).Select(x => x.Month).Distinct().ToList();
                        listaMesAnio.Add(new YearMonth(y, months));
                    }

                    foreach (var y in listaMesAnio)
                    {
                        foreach (var m in y.Months)
                        {
                            Console.WriteLine($"Año:{y.Year} Mes:{m}");
                        }
                    }
                    var data = new ListData(list, listaFinal, listaMesAnio);
                    return Json(data);
                }
                catch(Exception ex)
                { return Json(""); }
                
                
                
                //var data = { "years": [{"2018", "2019" }], "monthbyyear": [{ "year": "2018", "months":{[ "01", "02"]} ]}],
                //    "files": [{"year": "2018", "month": "01", "file": "ruta"}]};
                
            }
            else
                return Json("");
        }
        [HttpPost]
        public ActionResult GetPdf(string id)
        {
            var anio = id.Split('-')[0];
            var mes = id.Split('-')[1];


            var nombrePDF = listaFinal.Where(f => f.Year == anio && f.Month == mes).Select(s => s.FilePath).FirstOrDefault();
            string pdfFilePath = " " + carpetaLocal + "/" + empresa + "/" + anio + "/" + mes + "/" + nombrePDF;
            var result = enviaEmail.EnviaEmailC(Request.Cookies["Nombre"].Value.ToString(), pdfFilePath);
            return Json(new { resp = result });
        }


        //public ActionResult GetPdfHtml(string id)
        //{
        //    string x;
        //    try
        //    {


        //        var anio = id.Split('-')[0];
        //        var mes = id.Split('-')[1];

        //        string embed = "<object data=\"data:application/pdf;base64,{0}\" type=\"application/pdf\" width=\"100%\" height=\"800px\">";
        //        embed += "<embed src=\"data: application/pdf; base64, {0}\" type=\"application/pdf\" />";
        //        embed += "</object>";
        //        var nombrePDF = listaFinal.Where(f => f.Year == anio && f.Month == mes).Select(s => s.FilePath).FirstOrDefault();

        //        string pdfFilePath = " " + carpetaLocal + "/" + empresa + "/" + anio + "/" + mes + "/" + nombrePDF;
        //        byte[] bytes = System.IO.File.ReadAllBytes(pdfFilePath);
        //        var conv64 = System.Convert.ToBase64String(bytes);
        //        x = string.Format(embed, conv64);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex);
        //    }
        //    return Json(x);
        //}
        [HttpPost]
        public ActionResult GetPdfHtml(string id)
        {
            string conv64;
            try
            {
                var anio = id.Split('-')[0];
                var mes = id.Split('-')[1];

                //string embed = "<object data=\"data:application/pdf;base64,{0}\" type=\"application/pdf\" width=\"100%\" height=\"800px\">";
                //embed += "<embed src=\"data: application/pdf; base64, {0}\" type=\"application/pdf\" />";
                //embed += "</object>";

                var nombrePDF = listaFinal.Where(f => f.Year == anio && f.Month == mes).Select(s => s.FilePath).FirstOrDefault();
                string pdfFilePath = " " + carpetaLocal + "/" + empresa + "/" + anio + "/" + mes + "/" + nombrePDF;
                byte[] bytes = System.IO.File.ReadAllBytes(pdfFilePath);
                conv64 = System.Convert.ToBase64String(bytes);

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            return Json(conv64);
        }

        //Nota Importante: GUARDA EL CONTRATO NUEVO CONTRATO
        //[HttpPost]
        //public ActionResult SetContract(string idContrato)
        //{
        //    var contract = new Contract();
        //    bool resp = false;
        //    if (!contractsList.Any(x => x.ContractNumber == Convert.ToInt32(idContrato)))
        //    {
        //        //Corregir la empresa para que traiga la informacion de Nuevo contrato
        //        contract = new Contract("Financiamiento", Convert.ToInt32(idContrato));
        //        contractsList.Add(contract);
        //        resp = true;
        //    }
        //    else
        //        resp = false;
        //    return Json(new { result = resp, contract = contract, idContract = idContrato }); ;
        //}

        public List<Contract> GetContract(int idCliente)
        {
            try
            {
                contractsList.Clear();
                var list = contrato.ObtieneContratosPorCliente(idCliente);
                foreach (var item in list)
                {
                    contractsList.Add(new Contract(item.nombCompania, item.iContrato, item.grupocliente, item.iCompania));
                }
                return contractsList;
            }
            catch (Exception e)
            {
                contractsList.Clear();
                return new List<Contract>();
            }

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
                return Json(new { result = succes, mensaje = "Ocurrio un error al tratar de validar el contrato ", contract = Contrato });
            }
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
        public JsonResult InsertContract(Usuario usuario)
        {

            usuario.cEMail = Request.Cookies["Nombre"].Value.ToString();
            bool succes = false;
            Contract contract = new Contract();
            string empresa = string.Empty;
            
            try
            {
                var mensajeResp = "";
                DAL dal = new DAL();
                Hashtable hashTableParameters = new Hashtable();

                DataTable dtExixteContrato;
                hashTableParameters.Add("contrato", usuario.iContrato);
                hashTableParameters.Add("grupo", usuario.gpoCte1);
                hashTableParameters.Add("cliente", usuario.gpoCte2);
                if (usuario.iContrato > 0)
                {
                    dtExixteContrato = dal.QueryDT("DS_ECWEB", "select idUsuario from [dbo].[Contratos] WHERE iContrato = @0", "H:S:contrato", hashTableParameters, System.Web.HttpContext.Current);
                    mensajeResp = "Contrato erroneo, favor de validar";
                    if (dtExixteContrato.Rows.Count > 0)
                    {
                        return Json(new { result = succes, mensaje = "Este contrato ya fue registrado previamente, favor de ingresar uno diferente." });
                    }
                }

                if (usuario.gpoCte1 > 0)
                {
                    dtExixteContrato = dal.QueryDT("DS_ECWEB", "select idUsuario from [dbo].[Contratos] WHERE iGrupo = @1 AND iCliente = @2", "H:S:contrato;H:S:grupo;H:S:cliente", hashTableParameters, System.Web.HttpContext.Current);
                    mensajeResp = "Grupo / Cliente erroneo, favor de validar";
                    if (dtExixteContrato.Rows.Count > 0)
                    {
                        return Json(new { result = succes, mensaje = "Este Grupo / Cliente ya fue registrado previamente, favor de ingresar uno diferente." });
                    }
                }


                WSValidaEmpresa.wsecwebObjClient wsValidaEmpresa = new WSValidaEmpresa.wsecwebObjClient();
                WSValidaEmpresa.wsEcwebResponse response = new WSValidaEmpresa.wsEcwebResponse();
                WSValidaEmpresa.wsEcwebRequest request = new WSValidaEmpresa.wsEcwebRequest();

                int? opiCodigo;
                string opcMensaje = "";
                int? opilContrato;
                int? opilEmpresa;
                int? opiGrupo;
                int? opiCliente;

                request.ipiContrato = usuario.iContrato;
                request.ipiGrupo = usuario.gpoCte1;
                request.ipiCliente = usuario.gpoCte2;
                request.ipcNombre = usuario.cNombre;
                request.ipcPrimerap = usuario.cPrimerApellido;
                request.ipcSegundoap = usuario.cSegundoApellido;
                request.ipiCp = usuario.CP;
                request.ipcFecha = usuario.DateCte.ToString("yyyy-MM-dd");
                wsValidaEmpresa.wsEcweb(request.ipiContrato, request.ipiGrupo, request.ipiCliente, request.ipcNombre, request.ipcPrimerap, request.ipcSegundoap, request.ipiCp, request.ipcFecha,  out opiCodigo, out opcMensaje, out opilContrato, out opilEmpresa, out opiGrupo, out opiCliente);
                response.opcMensaje = opcMensaje;
                response.opiCodigo = opiCodigo;
                response.opiIdcontrato = opilContrato;       

                usuario.TipoFina = Convert.ToInt32(opilEmpresa);
                usuario.iContrato = Convert.ToInt32(opilContrato);
                usuario.gpoCte1 = Convert.ToInt32(opiGrupo);
                usuario.gpoCte2 = Convert.ToInt32(opiCliente);

                if (opilEmpresa > 0)
                {
                    if (!contractsList.Any(x => x.ContractNumber == Convert.ToInt32(usuario.iContrato)))
                    {

                        succes = usuarioValid.InsertContract(usuario);
                        empresa = contrato.ObtieneEmpresPorId(usuario.TipoFina);

                        contract = new Contract(empresa, Convert.ToInt32(usuario.iContrato), Convert.ToString(usuario.gpoCte1) + Convert.ToString(usuario.gpoCte2), usuario.TipoFina);
                        contractsList.Add(contract);

                        succes = true;
                        return Json(new { result = succes, mensaje = "Se guardo contrato correctamente", contract = contract });
                    }
                    else
                    {
                        succes = false;
                        return Json(new { result = succes, mensaje = "El número de contrato ya esta dado de alta", contract = contract });
                    }
                }
                else
                {
                    return Json(new { result = succes, mensaje = mensajeResp, contract = contract });
                }

                
            }
            catch (Exception ex)
            {
                succes = false;
                return Json(new { result = succes, mensaje = "Ocurrio un error al tratar de insertar el contrato", contract = contract });
            }
        }
        //public JsonResult InsertContract(Usuario usuario)
        //{
        //    usuario.cEMail = Request.Cookies["Nombre"].Value.ToString();
        //    bool succes = usuarioValid.InsertContract(usuario);
        //    //succes = usuarioValid.InsertUsuario(Usuario);
        //    if (succes)
        //    {
        //        var response = "El contrato ha sido generado";
        //        return Json(response);
        //    }
        //    else
        //    {
        //        var response = "Favor de validarl su contrato";
        //        return Json(response);
        //    }
        //}

    }
}