using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer
{
    public class Usuario
    {
        public int idUsuario { get; set; }
        public bool bAdmin { get; set; }
        public bool bEmpresa { get; set; }
        public string cNombre { get; set; }
        public string cPrimerApellido { get; set; }
        public string cSegundoApellido { get; set; }
        public string cNombreCompleto { get; set; }
        public string cEMail { get; set; }
        public string cTelMovil { get; set; }
        public int idClienteUnico { get; set; }
        public string cPasswd { get; set; }
        public DateTime dtFechaReg { get; set; }
        public string cToken { get; set; }
        public bool bMailVerificado { get; set; }
        public bool bTelVerificado { get; set; }
        public int iEstatus { get; set; }
        public Roles Rol { get; set; }
        public EstatusCliente Estatus { get; set; }
        public int iContrato { get; set; }
        public string cSituacionCliente { get; set; }
        public int TipoCte { get; set; }
        public int GrupoCte { get; set; }
        public int? IdContrato { get; set; }
        public int gpoCte1 { get; set; }
        public int gpoCte2 { get; set; }
        public string GpoCteString { get; set; }
        public int CP { get; set; }
        public DateTime DateCte { get; set; }
        public int Dia { get; set; }
        public int MEs { get; set; }
        public int ano { get; set; }
        public int TipoFina { get; set; }

    }

    public class UsuarioConauto {
        public int idUsuario { get; set; }

        public int inEstatus { get; set; }
    }
}
