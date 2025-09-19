using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBCuentas.Models
{
    public class Contract
    {
        public string Empresa { get; set; }
        public int ContractNumber { get; set; }

        public string GrupoCliente { get; set; }

        public Contract()
        {
            Empresa = string.Empty;
            ContractNumber = 0;
            GrupoCliente = string.Empty;
        }
        public Contract( string empresa, int contract, string grupocliente)
        {
            Empresa = empresa;
            ContractNumber = contract;
            GrupoCliente = grupocliente;
        }
        
    }
}