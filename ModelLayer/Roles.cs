using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer
{
    public class Roles
    {
        public int? IdRolUsuario { get; set; }
        public string NombreRol { get; set; }
        public bool? Activo { get; set; }

        public Roles()
        {
            this.IdRolUsuario = null;
            this.NombreRol = null;
            this.Activo = null;
        }

        public Roles(int IdRolUsuario, string NombreRol, bool Activo = false)
        {
            this.IdRolUsuario = IdRolUsuario;
            this.NombreRol = NombreRol;
            this.Activo = Activo;
        }

    }
}
