using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Repositories;
using DataLayer.UnitOfWork;
using ModelLayer;

namespace BusinessLayer
{
    public class Contrato_Business
    {
        public List<Usuario> ObtieneContratosClientes()
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var repository = new Contrato_Repository(uow);
                var response = repository.ObtContratoUsuario().Select(s => new Usuario
                {
                    cNombreCompleto = string.IsNullOrEmpty(s.cNombreCompleto) ? "" : s.cNombreCompleto,
                    cEMail = string.IsNullOrEmpty(s.cEMail) ? "" : s.cEMail,
                    iContrato = s.iContrato,
                    TipoFina = s.TipoFina
                   
                }).ToList();
                return response;
            }
        }
        public List<Contrato> ObtieneContratosPorCliente(int idCliente)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var repository = new Contrato_Repository(uow);
                var response = repository.ObtContratoCliente(idCliente).Select(s => new Contrato
                {
                    idUsuario = s.idUsuario,
                    iContrato = s.iContrato,
                    grupocliente = s.grupocliente,
                    nombCompania = string.IsNullOrEmpty(s.nombCompania) ? "" : s.nombCompania,
                }).ToList();
                return response;
            }
        }

        public string ObtieneEmpresPorId(int iCompania)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var repository = new Contrato_Repository(uow);
                var response = repository.ObtEmpresaPorId(iCompania);
                return response;
            }
        }
    }
}
