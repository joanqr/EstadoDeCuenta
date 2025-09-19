using BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BBCuentas.Models;
using System.IO;
using System.Web.Configuration;
using r3Take.DataAccessLayer;
using System.Collections;
using System.Data;
using BBCuentas.WSGetEstadoCuenta;

namespace BBCuentas.Controllers
{
    public class AdmonController : Controller
    {
        static string carpetaLocal;
        static string empresa;
        static int idCliente;
        static List<FileByYearAndMonth> listaFinal = new List<FileByYearAndMonth>();
        static List<Contract> contractsList = new List<Contract>();
        private EnviaEmail enviaEmail = new EnviaEmail();
        private Contrato_Business contrato = new Contrato_Business();
        private Usuario_Business usuarioValid = new Usuario_Business();
        // GET: Admo
        [Authorize(Roles = "Admin")]
        public ActionResult Admon()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetContratosClientes()
        {
            try
            {                
                return Json(contrato.ObtieneContratosClientes(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return null;
            }
        }


        [HttpGet]
        public ActionResult GetEstadosCuenta(string iContrato)
        {
            try
            {
                return GetPdfFileList(iContrato, "", "", "");
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet]
        public ActionResult GetEstadosCuentaBusqueda(string iContrato, string iGrupo, string iCliente, string idEmpresa)
        {
            try
            {
               

                return GetPdfFileList(iContrato, iGrupo, iCliente, idEmpresa);
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public ActionResult GetPdfFileList(string contrato,string iGrupo, string iCliente, string idEmpresa)
        {
            if (contrato != "default")
            {               

                listaFinal.Clear();
                var listaMesAnio = new List<YearMonth>();

               
                empresa = null;
                carpetaLocal = null;
                string GrupoCliente = "0";

                DAL dal = new DAL();
                Hashtable hashTableParameters = new Hashtable();
                hashTableParameters.Add("contrato", contrato);
                hashTableParameters.Add("idEmpresa", idEmpresa);

                DataTable dtEmpresaXContrato = new DataTable();
                Int32 Cliente = 0;
                string Grupo = "";
                try
                {
                    string ClienteCadena = "";
                    if (contrato.Length > 0)
                    {
                        dtEmpresaXContrato = dal.QueryDT("DS_ECWEB", "SELECT iCompania, iGrupo, iCliente FROM [dbo].[Contratos] WHERE iContrato = @0", "H:S:contrato", hashTableParameters, System.Web.HttpContext.Current);
                        if (dtEmpresaXContrato.Rows.Count > 0)
                        {

                            empresa = dtEmpresaXContrato.Rows[0]["iCompania"].ToString();
                            

                            Cliente = Convert.ToInt32(dtEmpresaXContrato.Rows[0]["iCliente"].ToString());
                            if (Cliente.ToString().Length == 1)
                                ClienteCadena = "00" + Cliente.ToString();
                            if (Cliente.ToString().Length == 2)
                                ClienteCadena = "0" + Cliente.ToString();
                            if (Cliente.ToString().Length == 3)
                                ClienteCadena = Cliente.ToString();

                            Grupo = dtEmpresaXContrato.Rows[0]["iGrupo"].ToString();
                            GrupoCliente = Grupo + ClienteCadena;
                        }
                    }


                    if (iCliente.ToString().Length == 1)
                        ClienteCadena = "00" + iCliente.ToString();
                    if (iCliente.ToString().Length == 2)
                        ClienteCadena = "0" + iCliente.ToString();
                    if (iCliente.ToString().Length == 3)
                        ClienteCadena = iCliente.ToString();

                    if (iGrupo.Length > 0 && iCliente.Length > 0 && idEmpresa.Length > 0)
                    {
                        Cliente = Convert.ToInt32(iCliente);
                        Grupo = iGrupo;
                        empresa = idEmpresa;
                        GrupoCliente = Grupo + ClienteCadena;
                    }else if (iGrupo.Length == 0 && iCliente.Length == 0 && idEmpresa.Length > 0)
                    {

                        if(idEmpresa == "1")
                        {
                            var lenContrato = contrato.Length;


                            Cliente = Convert.ToInt32(contrato.Substring(lenContrato-3, 3));
                            Grupo = contrato.Remove(lenContrato - 3, 3);
                            empresa = idEmpresa;
                            GrupoCliente = contrato;
                        }
                        
                    }



                    wsSolpdfRequest PDFRequest = new wsSolpdfRequest();
                        PDFRequest.ipiCliente = Cliente;
                        PDFRequest.ipiGrupo = Convert.ToInt32(Grupo);
                        PDFRequest.ipiEmpresa = Convert.ToInt32(empresa);
                        WSGetEstadoCuenta.solpdfObjClient GetPDF = new solpdfObjClient();

                        string EmpresaCadena = "";

                        if (empresa == "1")
                        {
                            empresa = "Fina";
                        }
                        if (empresa == "2")
                        {
                            empresa = "Cona";
                        }

                        int? CodigoRespuestaGetPDF = 0;
                        string MensajeRespuestaGetPDF = "";

                        string RutaEstadoCuentaActual = "";
                        DateTime CurrentDate = DateTime.Today;
                        int currentDay = CurrentDate.Day;
                        if (currentDay < 4)
                            CurrentDate = CurrentDate.AddMonths(-1);

                        string currentMonth = CurrentDate.Month.ToString();
                        string currentYear = CurrentDate.Year.ToString();
                        RutaEstadoCuentaActual = WebConfigurationManager.AppSettings["CarpetaLocal"].ToString() + "//" + empresa + "//" + currentYear + "//" + currentMonth + "//";

                        var cadenaBusquedaEstadoCuentaActual = $"*" + GrupoCliente + ".pdf";

                        if (!Directory.Exists(RutaEstadoCuentaActual))
                            Directory.CreateDirectory(RutaEstadoCuentaActual);

                        IEnumerable<string> filesEstadoCuentaActual = Directory.EnumerateFiles(RutaEstadoCuentaActual, cadenaBusquedaEstadoCuentaActual, SearchOption.AllDirectories).OrderByDescending(filename => filename); ;
                        var listExisteEstadoCuentaActual = new List<string>(filesEstadoCuentaActual);

                        if (listExisteEstadoCuentaActual.Count == 0)                                
                            GetPDF.wsSolpdf(PDFRequest.ipiEmpresa, PDFRequest.ipiGrupo, PDFRequest.ipiCliente, out CodigoRespuestaGetPDF, out MensajeRespuestaGetPDF);
                      
                       
                    if (GrupoCliente.Length > 3 && CodigoRespuestaGetPDF == 0)
                    {
                        var sessionData = new { empresa = empresa, contrato = contrato };

                        carpetaLocal = WebConfigurationManager.AppSettings["CarpetaLocal"].ToString();
                        /*var sourceDirectory = carpetaLocal; *///Server.MapPath("~/" + carpetaLocal);

                        var sourceDirectory = $"/{sessionData.empresa}";
                        var cadenaBusqueda = $"*{sessionData.contrato}.pdf";

                        //Alcome


                        var cadenaBusqueda2 = $"*" + GrupoCliente + ".pdf";
                        IEnumerable<string> files2 = Directory.EnumerateFiles(carpetaLocal + sourceDirectory, cadenaBusqueda2, SearchOption.AllDirectories).OrderByDescending(filename => filename); ;
                        var list = new List<string>(files2);


                        DataTable dtNumeroEstadosCuenta;
                        dtNumeroEstadosCuenta = dal.QueryDT("DS_ECWEB", "SELECT NumeroEstadosCuentaAMostrarAtencionClientes FROM [dbo].[Configuraciones]", "", hashTableParameters, System.Web.HttpContext.Current);

                        int NumeroEstadosCuenta = Convert.ToInt16(dtNumeroEstadosCuenta.Rows[0]["NumeroEstadosCuentaAMostrarAtencionClientes"].ToString());
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
                            if (NumeroEstadosCuentaContador < NumeroEstadosCuenta)
                            {
                                listaFinal.Add(new FileByYearAndMonth(year, month, name, carpetaLocal, empresaD));
                            }

                            NumeroEstadosCuentaContador = NumeroEstadosCuentaContador + 1;
                        }

                        if (listaFinal.Count == 0)
                        {
                            listaFinal.Add(new FileByYearAndMonth("", "", "No hay estados de cuenta disponibles", "", ""));
                        }

                        //var data = new ListData(list, listaFinal, listaMesAnio);
                        
                    }
                    else
                    {
                        listaFinal.Add(new FileByYearAndMonth("", "", MensajeRespuestaGetPDF.Trim() != ""? MensajeRespuestaGetPDF : "No hay estados de cuenta disponibles", "", ""));
                    }
                    return Json(listaFinal, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    listaFinal.Add(new FileByYearAndMonth("", "", "Error - No hay estados de cuenta disponibles", "", ""));
                    return Json(listaFinal, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("");
            }
                
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

        public List<Contract> GetContract(int idCliente)
        {
            try
            {
                var list = contrato.ObtieneContratosPorCliente(idCliente);
                foreach (var item in list)
                {
                    contractsList.Add(new Contract(item.nombCompania, item.iContrato, item.grupocliente));
                }
                return contractsList;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        [HttpPost]
        public ActionResult GetPdfHtml(string anio, string mes, string RutaPDF)
        {
            string conv64;
            try
            {
                string pdfFilePath = " " + carpetaLocal + "/" + empresa + "/" + anio + "/" + mes + "/" + RutaPDF;
                byte[] bytes = System.IO.File.ReadAllBytes(pdfFilePath);
                conv64 = System.Convert.ToBase64String(bytes);

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            return Json(conv64);
        }

        [HttpPost]
        public ActionResult GetPdfHtmlBusqueda(string anio, string mes, string RutaPDF)
        {
            string conv64;
            try
            {
                string pdfFilePath = " " + carpetaLocal + "/" + "CONA" + "/" + anio + "/" + mes + "/" + RutaPDF;
                if (!System.IO.File.Exists(pdfFilePath))
                {
                     pdfFilePath = " " + carpetaLocal + "/" + "FINA" + "/" + anio + "/" + mes + "/" + RutaPDF;
                }

                byte[] bytes = System.IO.File.ReadAllBytes(pdfFilePath);
                conv64 = System.Convert.ToBase64String(bytes);

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            return Json(conv64);
        }

    }
}