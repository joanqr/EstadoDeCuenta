using BBCuentas.AppRemota;
using ModelLayer;
using System.Text;
using System.Web.Configuration;

namespace BBCuentas.Helpers
{
    public class AppRemota
    {
        public bool consume(Usuario usuario, int? opilContrato)
        {
            bool respuesta = true;
            try
            {
                Service1Client nuev = new Service1Client();
                Encripta objectEncript = new Encripta();
                char c = (char)1;

                var Empresa = "";
                if (usuario.TipoFina == 1)
                    Empresa = "FINA";
                if (usuario.TipoFina == 2)
                    Empresa = "CONA";

                string s = Encoding.ASCII.GetString(new byte[] { 1 });
                string email = "VALIDAT" + c + "PRODUCCION" + c + WebConfigurationManager.AppSettings["UrlServicioSMS"].ToString() + c + Empresa + " CORREO ecweb" + c + usuario.cEMail + "|Validacion|||0|" + opilContrato + "|4|0|0|0|1|" + usuario.cNombre + "|" + usuario.cPrimerApellido + "|" + usuario.cSegundoApellido + "";
                string EncriptEmail = objectEncript.RSAEncrypt(email);
                string respEmail = nuev.EjecutaAppRemota(EncriptEmail);
                string desenEmail = objectEncript.RSADecrypt(respEmail);

                if(WebConfigurationManager.AppSettings["PermitirEnvioSMS"].ToString() == "1")
                {
                    string Cel = "VALIDAT" + c + "PRODUCCION" + c + WebConfigurationManager.AppSettings["UrlServicioSMS"].ToString() + c + Empresa + " CELULAR ecweb" + c + usuario.cTelMovil + "|||||" + opilContrato + "|1|0|0|0|1|" + usuario.cNombre + "|" + usuario.cPrimerApellido + "|" + usuario.cSegundoApellido + "";
                    string EncriptCel = objectEncript.RSAEncrypt(Cel);
                    string respCel = nuev.EjecutaAppRemota(EncriptCel);
                    string respDecript = objectEncript.RSADecrypt(respCel);
                    
                }
                return respuesta;

            }
            catch (System.Exception)
            {
                return respuesta = false;
            }
            
        }
    }
}