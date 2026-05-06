using SistemaLicencias.SHARED.DTOs;

namespace SISFRONT.Service.Interface {
    public interface IPublicidadService {
        Task<SolicitanteAddResponse> addSolicitante(SolicitantePublicidad dto);
        Task<string> lstSolicituds(string dto);
    }
}
