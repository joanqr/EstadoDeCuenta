using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ModelLayer;

namespace BBCuentas.Helpers
{
    public class wsEsweb
    {
        public async Task<ResponseEsweb> Consume(Usuario user)
        {
            ResponseEsweb usuarioResponse = new ResponseEsweb();
            try
            {
                WSValidaEmpresa.wsecwebObjClient bix = new WSValidaEmpresa.wsecwebObjClient();
                WSValidaEmpresa.wsEcwebResponse response = new WSValidaEmpresa.wsEcwebResponse();
                WSValidaEmpresa.wsEcwebRequest request = new WSValidaEmpresa.wsEcwebRequest();
                
                request.ipiContrato = user.iContrato;
                request.ipiGrupo = user.gpoCte1;
                request.ipiCliente = user.gpoCte2;
                request.ipcNombre = user.cNombre;
                request.ipcPrimerap = user.cPrimerApellido;
                request.ipcSegundoap = user.cSegundoApellido;
                request.ipiCp = user.CP;
                request.ipcFecha = user.DateCte.ToString("yyyy-MM-dd");
                response = await bix.wsEcwebAsync(request);
                usuarioResponse.opcMensaje = response.opcMensaje;
                usuarioResponse.opiCodigo = response.opiCodigo;
                usuarioResponse.opildContrato = response.opiIdcontrato;
                usuarioResponse.result = response.result;
                return usuarioResponse;
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return usuarioResponse;
            }
            
        }
    }
}