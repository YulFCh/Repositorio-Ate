using Microsoft.Data.SqlClient;
using SistemaLicencias.SHARED.DTOs;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SISLICBACK.Services {
    public class MarkMapsService {
        private static readonly HttpClient client = new HttpClient();
        private string _connDB;
        private readonly SolicitudService ss;

        public MarkMapsService(IConfiguration _configuration) {
            _connDB = _configuration.GetSection("ConexionSQL:Licencia:Conexion").Value;
        }

        //capacidad x idsubsuzona
        public async Task<object> GetCapacidadSubzonas(int idZona,string anio) {
            var resultados = new List<Dictionary<string, object>>();

            using(var connection = new SqlConnection(_connDB)) {
                try {
                    await connection.OpenAsync();

                    using(var command = new SqlCommand("Autorizacion.uspListarCapacidadSZonasPB22", connection)) {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idZona", idZona);
                        command.Parameters.AddWithValue("@Anio", anio);

                        using(var reader = await command.ExecuteReaderAsync()) {
                            while(await reader.ReadAsync()) {
                                var fila = new Dictionary<string, object>();

                                for(int i = 0; i < reader.FieldCount; i++) {
                                    fila[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                }

                                resultados.Add(fila);
                            }
                        }
                    }
                }
                catch(Exception ex) {
                    Console.WriteLine($"Error en la consulta: {ex.Message}");

                    return new {
                        mensaje = $"Error: {ex.Message}",
                        response = (object)null
                    };
                }
            }

            return new {
                mensaje = "OK",
                response = resultados
            };
        }

        //todas las zonas capacidad
        public async Task<object> GetAllCapacidad() {
            var resultados = new List<Dictionary<string, object>>();

            using(var connection = new SqlConnection(_connDB)) {
                try {
                    await connection.OpenAsync();

                    using(var command = new SqlCommand("[Autorizacion].[uspLstCapacidadTodasSZonas]", connection)) {
                      

                        using(var reader = await command.ExecuteReaderAsync()) {
                            while(await reader.ReadAsync()) {
                                var fila = new Dictionary<string, object>();

                                for(int i = 0; i < reader.FieldCount; i++) {
                                    fila[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                }

                                resultados.Add(fila);
                            }
                        }
                    }
                }
                catch(Exception ex) {
                    Console.WriteLine($"Error en la consulta: {ex.Message}");

                    return new {
                        mensaje = $"Error: {ex.Message}",
                        response = (object)null
                    };
                }
            }

            return new {
                mensaje = "OK",
                response = resultados
            };
        }

        //insert into Autorizacion.TB_SUBZONAS_AUTORIZACION_COORD values (1,'Z1-DC22','Point','-75.43243243,-12.5464565,0',6,0,'','');
        public async Task<bool> AddSubZonasSqlClient(SubZonaDTO sz) {
            var query = @"INSERT INTO Autorizacion.TB_SUBZONAS_AUTORIZACION_COORD 
                 (IdZona, Nombre, Tipo, Coordenadas, CapacidadBase, CapacidadExtra, regulacion, operadorRegistra)
                  VALUES (@IdZona, @Nombre, @Tipo, @Coordenadas, @CapacidadBase, @CapacidadExtra, @Regulacion, @OperadorRegistra);";

            using(var connection = new SqlConnection(_connDB))
            using(var command = new SqlCommand(query, connection)) {
                command.Parameters.AddWithValue("@IdZona", sz.IdZona);
                command.Parameters.AddWithValue("@Nombre", sz.Nombre);
                command.Parameters.AddWithValue("@Tipo", "Point");
                command.Parameters.AddWithValue("@Coordenadas", sz.Coordenadas);
                command.Parameters.AddWithValue("@CapacidadBase", 6);
                command.Parameters.AddWithValue("@CapacidadExtra", 0);
                command.Parameters.AddWithValue("@Regulacion", sz.Regulacion ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@OperadorRegistra", sz.operadorRegistra ?? (object)DBNull.Value);

                try {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    return true;
                }
                catch(Exception ex) {
                    Console.WriteLine("💣 Error SQL: " + ex.Message);
                    return false;
                }
            }
        }

        //update Autorizacion.TB_SUBZONAS_AUTORIZACION_COORD set CapacidadExtra =0  where IdSubzona = 35
        public async Task<string> UpdateSizeSubZona(int subZona, int capacidad) {
            
            var query = @"update Autorizacion.TB_SUBZONAS_AUTORIZACION_COORD 
                  SET CapacidadExtra = @capacidad 
                  WHERE IdSubzona = @subzona";

            try {
                using(var connection = new SqlConnection(_connDB))
                using(var command = new SqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@capacidad", capacidad);
                    command.Parameters.AddWithValue("@subzona", subZona);

                    await connection.OpenAsync();
                    int filasAfectadas = await command.ExecuteNonQueryAsync();

                    return filasAfectadas > 0
                        ? $"Subzona {subZona} actualizada con capacidad {capacidad}"
                        : $"No se encontró la subzona {subZona} para actualizar.";
                }
            }
            catch(Exception ex) {
                
                return $"Error al actualizar la subzona: {ex.Message}";
            }
        }


    }
}
