using SistemaLicencias.SHARED.DTOs;

namespace SISFRONT.Service {
    public interface IGirosService {

        Task<List<GiroPrincipal>> ObtenerGirosPrincipalesAsync();
        Task<List<GiroComplementario>> obtGiroComplementarioAsync(int principalId);
        Task<List<GiroDetalle>> obtGiroDetalleAsync(int complementarioId);
        
        Task<List<T>> ObtenerGirosPorTipoAsync<T>(int tipo);


        Task<(bool Success, string Message)> InsertGiroAsync(GiroFlexible flex);
        Task<(bool Success, string Message)> UpadateGiroAsync(GiroFlexible flex);

        //Combo EPE
        Task<List<ComboDTO>> getCombos(int parametro);


    }
}
