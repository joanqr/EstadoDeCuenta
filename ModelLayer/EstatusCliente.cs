using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer
{
    public class EstatusCliente
    {
        public int? IdEstatus{ get; set; }
        public string Descripcion { get; set; }
        public bool? Estatus { get; set; }

        public EstatusCliente()
        {
            this.IdEstatus = null;
            this.Descripcion = null;
            this.Estatus = null;
        }

        public EstatusCliente(int IdEstatus, string Descripcion, bool Estatus = false)
        {
            this.IdEstatus = IdEstatus;
            this.Descripcion = Descripcion;
            this.Estatus = Estatus;
        }
    }
}
