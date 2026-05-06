using Microsoft.Data.SqlClient;
using SistemaLicencias.SHARED.DTOs;

namespace SISLICBACK.Services {
    public class GraficosServices {
        private static readonly HttpClient client = new HttpClient();
        private string _connDB;        
        private readonly SolicitudService ss;

        public GraficosServices(IConfiguration _configuration) {
            _connDB = _configuration.GetSection("ConexionSQL:Licencia:Conexion").Value;            
        }


        //GIROS
        public async Task<List<MesConGirosDTO>> DatosGraficoAsync() {
            var lista = new List<GraficoGiro>();

            using(var connection = new SqlConnection(_connDB)) {
                string query = @"
        SELECT 
            YEAR(s.fechaRegistro) AS Anio,
            MONTH(s.fechaRegistro) AS Mes,
            CASE 
                WHEN p.Nombre = ' - ALIMENTOS * N0' THEN 'ORDENANZA N° 317-2013-MDA - VENTA DE ALIMENTOS'
                WHEN p.Nombre = ' - BEBIDAS CALIENTES Y COMPLEMENTOS * N0' THEN 'ORDENANZA N° 301-2012-MDA Y SUS MODIFICATORIAS - BEBIDAS CALIENTA Y COMPLEMENTOS'
                WHEN p.Nombre = ' - GOLOSINAS Y FLORES * N0' THEN 'ORDENANZA N° 302-2012-MDA Y SUS MODIFICATORIAS - GOLOSINAS Y FLORES'
                WHEN p.Nombre= ' - DIARIOS, REVISTAS, BILLETES DE LOTERIA Y PRODUCTOS COMPLEMENTARIOS * N0' THEN  'ORDENANZA N° 354-2014-MDA - DIARIOS, REVISTAS, BILLETES DE LOTERIA Y SUS PRODUCTOS COMPLEMENTARIOS'
                WHEN p.Nombre= ' - CERRAJERIA FINA (LLAVEROS) * N0' THEN  'ORDENANZA N° 387-2014-MDA - CERRAJERIA FINA (LLAVEROS)'
                WHEN p.Nombre= ' - LUSTRADORES DE CALZADO * N0' THEN  'ORDENANZA N° 335-2014-MDA - LUSTRADORES DE CALZADO'
                ELSE p.Nombre 
            END AS GiroPrincipal,
            COUNT(*) AS TotalSolicitudes
        FROM Autorizacion.Solicitud_AUT s
        JOIN Autorizacion.GiroSolicitudAutorizacion g 
            ON TRY_CAST(s.giros AS INT) = g.IdGiroSolicitud
        JOIN Autorizacion.GiroPrincipal p 
            ON g.IdGiroPrincipal = p.IdGiroPrincipal
        WHERE s.estado = '1'
        GROUP BY 
            YEAR(s.fechaRegistro),
            MONTH(s.fechaRegistro),
            CASE 
                WHEN p.Nombre = ' - ALIMENTOS * N0' THEN 'ORDENANZA N° 317-2013-MDA - VENTA DE ALIMENTOS'
                WHEN p.Nombre = ' - BEBIDAS CALIENTES Y COMPLEMENTOS * N0' THEN 'ORDENANZA N° 301-2012-MDA Y SUS MODIFICATORIAS - BEBIDAS CALIENTA Y COMPLEMENTOS'
                WHEN p.Nombre = ' - GOLOSINAS Y FLORES * N0' THEN 'ORDENANZA N° 302-2012-MDA Y SUS MODIFICATORIAS - GOLOSINAS Y FLORES'
                WHEN p.Nombre= ' - DIARIOS, REVISTAS, BILLETES DE LOTERIA Y PRODUCTOS COMPLEMENTARIOS * N0' THEN  'ORDENANZA N° 354-2014-MDA - DIARIOS, REVISTAS, BILLETES DE LOTERIA Y SUS PRODUCTOS COMPLEMENTARIOS'
                WHEN p.Nombre= ' - CERRAJERIA FINA (LLAVEROS) * N0' THEN  'ORDENANZA N° 387-2014-MDA - CERRAJERIA FINA (LLAVEROS)'
                WHEN p.Nombre= ' - LUSTRADORES DE CALZADO * N0' THEN  'ORDENANZA N° 335-2014-MDA - LUSTRADORES DE CALZADO'
                ELSE p.Nombre 
            END
        ORDER BY Anio, Mes, GiroPrincipal";

                using(var command = new SqlCommand(query, connection)) {
                    await connection.OpenAsync();
                    using(var reader = await command.ExecuteReaderAsync()) {
                        while(await reader.ReadAsync()) {
                            var dato = new GraficoGiro {
                                Anio = reader.GetInt32(0),
                                Mes = reader.GetInt32(1),
                                GiroPrincipal = reader.GetString(2),
                                TotalSolicitudes = reader.GetInt32(3)
                            };
                            lista.Add(dato);
                        }
                    }
                }

            }
            var agrupado = lista
.GroupBy(x => new { x.Anio, x.Mes })
.Select(grupo => new MesConGirosDTO {
    Anio = grupo.Key.Anio,
    Mes = grupo.Key.Mes,
    Giros = grupo.Select(g => new GiroMesDTO {
        Nombre = g.GiroPrincipal,
        Total = (int)g.TotalSolicitudes // g.TotalSolicitudes si ya es int
    }).ToList()
})
.OrderBy(x => x.Anio).ThenBy(x => x.Mes)
.ToList();
            return agrupado;
        }

    }
}
