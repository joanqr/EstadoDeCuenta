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
        public Contract(string empresa, int contract, string grupocliente, int companyId = 0)
        {
            Empresa = empresa;
            ContractNumber = contract;
            GrupoCliente = grupocliente;
            CompanyId = companyId;

            if (CompanyId == 0 && !string.IsNullOrEmpty(Empresa))
            {
                if (Empresa.Equals("Fina", StringComparison.OrdinalIgnoreCase))
                {
                    CompanyId = 1;
                }
                else if (Empresa.Equals("Cona", StringComparison.OrdinalIgnoreCase))
                {
                    CompanyId = 2;
                }
            }
        }        
    }
}