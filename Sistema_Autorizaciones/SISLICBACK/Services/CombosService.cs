using Microsoft.Data.SqlClient;
using SistemaLicencias.SHARED.DTOs;

namespace SISLICBACK.Services {
    public class CombosService {
        private static readonly HttpClient client = new HttpClient();
        private string _connDB;

        public CombosService(IConfiguration _configuration) {
            //_conn = _configuration.GetSection("ConexionSQL:Licencia:Prod").Value;
            _connDB = _configuration.GetSection("ConexionSQL:Licencia:Conexion").Value;
        }

        public async Task<List<ComboDTO>> getTipoSolicitud(int tipo) {
            List<ComboDTO> lst = new List<ComboDTO>();

            string query = "select idTipoSolicitud as Id,descTipo as Nombre, estadoTipo as Estado from Publicidad.TipoSolicitud where estadoTipo = 1";             

            using(var connection = new SqlConnection(_connDB)) {


                using(var command = new SqlCommand(query, connection)) {
                    await connection.OpenAsync();
                    using(var reader = await command.ExecuteReaderAsync()) {
                        while(await reader.ReadAsync()) {
                            lst.Add(new ComboDTO {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Estado = reader.GetInt32(2),
                            });
                        }
                    }
                }
            }

            return lst;
        }


    }
}
