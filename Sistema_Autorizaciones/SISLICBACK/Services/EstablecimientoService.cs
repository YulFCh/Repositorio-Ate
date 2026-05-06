using Microsoft.Data.SqlClient;
using System.Data;
using SistemaLicencias.SHARED.DTOs;
using System.Text.Json;
using System.Text;
namespace SISLICBACK.Services {
    public class EstablecimientoService {
        private static readonly HttpClient client = new HttpClient();
        private string _connDB, _connCatastro;
        



        public EstablecimientoService(IConfiguration _configuration) {
            _connDB = _configuration.GetSection("ConexionSQL:Licencia:Conexion").Value;
            _connCatastro = _configuration.GetSection("ConexionSQL:Catastro:Prod").Value;
        }

        public async Task<object> InsertEstaAutor(EstablecimientoDTO e) {
            var resultados = new List<Dictionary<string, object>>();

            using(var connection = new SqlConnection(_connDB)) {
                try {
                    await connection.OpenAsync();

                    using(var command = new SqlCommand("[Licencia].[usp_InsertEstableAutor]", connection)) {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@nombreContrib", SqlDbType.VarChar)).Value = e.nombreContrib;
                        command.Parameters.Add(new SqlParameter("@direcPred", SqlDbType.VarChar)).Value = e.direcPred;
                        command.Parameters.Add(new SqlParameter("@idTipoZonif", SqlDbType.VarChar)).Value = e.idTipoZonif;
                        command.Parameters.Add(new SqlParameter("@operadorRegistro", SqlDbType.VarChar)).Value = e.operadorRegistro;
                        command.Parameters.Add(new SqlParameter("@estacionRegistro", SqlDbType.VarChar)).Value = e.estacionRegistro;
                        command.Parameters.Add(new SqlParameter("@descipZonif", SqlDbType.VarChar)).Value = e.descipZonif;
                        command.Parameters.Add(new SqlParameter("@abrevZonif", SqlDbType.VarChar)).Value = e.abrevZonif;
                        command.Parameters.Add(new SqlParameter("@aHorario", SqlDbType.VarChar)).Value = e.aHorario;

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
                    Console.WriteLine($"Error al ejecutar el Stored Procedure: {ex.Message}");
                    throw;
                }
            }

            return new {
                mensaje = "ok",
                response = resultados
            };
        }

        public async Task<object> GetSolAutxId(int id) {
            var resultados = new List<Dictionary<string, object>>();

            using(var connection = new SqlConnection(_connDB)) {
                try {
                    await connection.OpenAsync();

                    //string query = "SELECT * FROM MiTabla WHERE id = @id";
                    string query = "select top 20 * from Licencia.Solicitante sol order by sol.fecharegistro desc";


                    using(var command = new SqlCommand(query, connection)) {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = id;

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
                    Console.WriteLine($"❌ Error al ejecutar la consulta: {ex.Message}");
                    return new {
                        mensaje = $"Error: {ex.Message}",
                        response = (object)null
                    };
                }
            }

            return new {
                mensaje = "✅ Consulta ejecutada correctamente",
                response = resultados
            };
        }

        public async Task<object> GetAllDataEst() {

            var resultados = new List<EstablecimientoAPI>();

            using(var connection = new SqlConnection(_connDB)) {
                try {
                    await connection.OpenAsync();

                    string query = "select e.idEstablecimiento,e.nombreContrib,e.direcPred,e.fechaRegistro,e.descipZonif,e.abrevZonif,e.tipo_via,e.aHorario from  Licencia.Establecimiento e order by e.fecharegistro desc ";

                    using(var command = new SqlCommand(query, connection)) {
                        command.CommandType = CommandType.Text;

                        using(var reader = await command.ExecuteReaderAsync()) {
                            while(await reader.ReadAsync()) {
                                var est = new EstablecimientoAPI {
                                    idEstablecimiento = reader.GetInt32(0),
                                    nombreContrib = reader.GetString(1),
                                    direcPred = reader.GetString(2),
                                    fechaRegistro = reader.GetDateTime(3).ToString(),
                                    descipZonif = reader.GetString(4),
                                    abrevZonif = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    tipo_via = reader.GetString(6),
                                    aHorario = reader.GetString(7)
                                };
                                resultados.Add(est);
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
                mensaje = "Consulta ejecutada correctamente",
                response = resultados
            };
        }

        public async Task<object> GetDataEstPaginado(int pageNumber, int pageSize) {
            var resultados = new List<EstablecimientoAPI>();
            int offset = (pageNumber - 1) * pageSize;

            using(var connection = new SqlConnection(_connDB)) {
                try {
                    await connection.OpenAsync();

                    string query = @"
                SELECT e.idEstablecimiento, e.nombreContrib, e.direcPred, e.fechaRegistro,
                       e.descipZonif, e.abrevZonif, e.tipo_via, e.aHorario
                FROM Licencia.Establecimiento e
                ORDER BY e.fechaRegistro DESC
                OFFSET @offset ROWS
                FETCH NEXT @pageSize ROWS ONLY";

                    using(var command = new SqlCommand(query, connection)) {
                        command.Parameters.AddWithValue("@offset", offset);
                        command.Parameters.AddWithValue("@pageSize", pageSize);

                        using(var reader = await command.ExecuteReaderAsync()) {
                            while(await reader.ReadAsync()) {
                                var est = new EstablecimientoAPI {
                                    idEstablecimiento = reader.GetInt32(0),
                                    nombreContrib = reader.GetString(1),
                                    direcPred = reader.GetString(2),
                                    fechaRegistro = reader.GetDateTime(3).ToString(),
                                    descipZonif = reader.GetString(4),
                                    abrevZonif = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    tipo_via = reader.GetString(6),
                                    aHorario = reader.GetString(7)
                                };
                                resultados.Add(est);
                            }
                        }
                    }

                    return new {
                        mensaje = "OK",
                        response = resultados,

                    };
                }
                catch(Exception ex) {
                    Console.WriteLine($"Error en la consulta: {ex.Message}");
                    return new {
                        mensaje = $"Error: {ex.Message}",
                        response = (object)null
                    };
                }
            }
        }

        public async Task<object> getZonificacion() {
            var resultados = new List<Dictionary<string, object>>();

            using(var connection = new SqlConnection(_connDB)) {
                try {
                    await connection.OpenAsync();


                    string query = "select idTipoZonif,CONCAT(TRIM(abrevTipoZonif),CONCAT(' | ',TRIM(descTipoZonif))) AS 'descripcion' from Licencia.TipoZonificacion where estado = 1;";

                    using(var command = new SqlCommand(query, connection)) {
                        command.CommandType = CommandType.Text;

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
                mensaje = "Consulta ejecutada correctamente",
                response = resultados
            };
        }

        //HABILITTACION URBANA CATASTRO SIGDT
        public async Task<object> getHabUrb() {
            var resultados = new List<Dictionary<string, object>>();

            using(var connection = new SqlConnection(_connCatastro)) {
                try {
                    await connection.OpenAsync();


                    string query = @" 
                      SELECT 
                    th.COD_HABILI,
                    CASE
                        WHEN th.NOM_HAB_UR IS NULL OR LTRIM(RTRIM(th.NOM_HAB_UR)) = '' THEN 'S/D'
                        ELSE th.NOM_HAB_UR
                    END AS NOM_HAB_UR_CORREGIDO
                FROM SIGT_GEO.dbo.TG_HABILITACION th
                     ";

                    using(var command = new SqlCommand(query, connection)) {
                        command.CommandType = CommandType.Text;

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
                    Console.WriteLine($"Error Consulta HabUrb.: {ex.Message}");
                    return new {
                        mensaje = $"Error: {ex.Message}",
                        response = (object)null
                    };
                }
            }

            return new {
                mensaje = "Consulta ejecutada correctamente",
                response = resultados
            };
        }

    }
}
