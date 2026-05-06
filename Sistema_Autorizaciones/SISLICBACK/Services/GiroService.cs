using Microsoft.Data.SqlClient;
using SistemaLicencias.SHARED.DTOs;
using System.Data;

namespace SISLICBACK.Services {
    public class GiroService {
        private static readonly HttpClient client = new HttpClient();
        private string _connDB;
        

        public GiroService(IConfiguration _configuration) {
            _connDB = _configuration.GetSection("ConexionSQL:Licencia:Conexion").Value;
        }


        //GIROS
        public async Task<List<GiroPrincipal>> ListarGirosPrincipalesAsync() {
            var lista = new List<GiroPrincipal>();

            using(var connection = new SqlConnection(_connDB)) {
                string query = @"select * from Autorizacion.GiroPrincipal where estado = 1";

                using(var command = new SqlCommand(query, connection)) {
                    await connection.OpenAsync();
                    using(var reader = await command.ExecuteReaderAsync()) {
                        while(await reader.ReadAsync()) {
                            lista.Add(new GiroPrincipal {
                                IdGiroPrincipal = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                estado = reader.GetInt32(2),
                            });
                        }
                    }
                }
            }

            return lista;
        }
        public async Task<List<GiroComplementario>> ListarComplementariosPorPrincipalAsync(int idPrincipal) {
            var lista = new List<GiroComplementario>();

            using(var connection = new SqlConnection(_connDB)) {
                using(var command = new SqlCommand("Autorizacion.sp_ObtenerGiroComplementario", connection)) {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@estado", 1); // 1 = activos, 0 = todos
                    command.Parameters.AddWithValue("@IdGiroPrincipal", idPrincipal);

                    await connection.OpenAsync();

                    using(var reader = await command.ExecuteReaderAsync()) {
                        while(await reader.ReadAsync()) {
                            lista.Add(new GiroComplementario {
                                IdGiroComplementario = reader.GetInt32(0),
                                IdGiroPrincipal = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                estado = reader.GetInt32(3),
                            });
                        }
                    }
                }
            }

            return lista;
        }
        public async Task<List<GiroDetalle>> ListarDetallePorComplementarioAsync(int idComplementario) {
            var lista = new List<GiroDetalle>();

            using(var connection = new SqlConnection(_connDB)) {
                using(var command = new SqlCommand("Autorizacion.sp_ObtenerDetallePorComplementario", connection)) {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdGiroComplementario", idComplementario);
                    command.Parameters.AddWithValue("@estado", 1);// 1 = activos, 0 = todos

                    await connection.OpenAsync();

                    using(var reader = await command.ExecuteReaderAsync()) {
                        while(await reader.ReadAsync()) {
                            lista.Add(new GiroDetalle {
                                IdDetalleGiro = reader.GetInt32(0),
                                IdGiroComplementario = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                estado= reader.GetInt32(3),
                            });
                        }
                    }
                }
            }

            return lista;
        }
        public async Task<List<object>> LstAllGirosxTipo(int tipo) {
            var lista = new List<object>();

            using(var connection = new SqlConnection(_connDB)) {
                using(var command = new SqlCommand("Autorizacion.sp_ListarGirosPorTipo", connection)) {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TipoGiro", tipo);

                    await connection.OpenAsync();

                    using(var reader = await command.ExecuteReaderAsync()) {
                        while(await reader.ReadAsync()) {
                            switch(tipo) {
                                case 1: // Principal
                                lista.Add(new GiroPrincipal {
                                    IdGiroPrincipal = reader.GetInt32(reader.GetOrdinal("IdGiroPrincipal")),
                                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                    estado = reader.GetInt32(reader.GetOrdinal("estado"))
                                });
                                break;

                                case 2: // Complementario
                                lista.Add(new GiroComplementario {
                                    IdGiroComplementario = reader.GetInt32(reader.GetOrdinal("IdGiroComplementario")),
                                    IdGiroPrincipal = reader.GetInt32(reader.GetOrdinal("IdGiroPrincipal")),
                                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                    estado = reader.GetInt32(reader.GetOrdinal("estado"))
                                });
                                break;

                                case 3: // Detalle
                                lista.Add(new GiroDetalle {
                                    IdDetalleGiro = reader.GetInt32(reader.GetOrdinal("IdDetalleGiro")),
                                    IdGiroComplementario = reader.GetInt32(reader.GetOrdinal("IdGiroComplementario")),
                                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                    estado = reader.GetInt32(reader.GetOrdinal("estado"))
                                });
                                break;
                            }
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<int> ObtenerOCrearGiroSolicitud(int idPrincipal, int? idComplementario, int? idDetalle) {
            using var connection = new SqlConnection(_connDB);
            await connection.OpenAsync();

            // 1. Buscar si ya existe
            string querySelect = @"
         SELECT IdGiroSolicitud 
FROM Autorizacion.GiroSolicitudAutorizacion 
WHERE IdGiroPrincipal = @idPrincipal
  AND (
        (@idComplementario IS NULL AND IdGiroComplementario IS NULL)
        OR (@idComplementario IS NOT NULL AND IdGiroComplementario = @idComplementario)
      )
  AND (
        (@idDetalle IS NULL AND IdDetalleGiro IS NULL)
        OR (@idDetalle IS NOT NULL AND IdDetalleGiro = @idDetalle)
      )";

            using var commandSelect = new SqlCommand(querySelect, connection);
            commandSelect.Parameters.AddWithValue("@idPrincipal", idPrincipal);
            commandSelect.Parameters.AddWithValue("@idComplementario", (object?)idComplementario ?? DBNull.Value);
            commandSelect.Parameters.AddWithValue("@idDetalle", (object?)idDetalle ?? DBNull.Value);

            var result = await commandSelect.ExecuteScalarAsync();
            if(result != null)
                return Convert.ToInt32(result);

            // 2. Insertar si no existe
            string queryInsert = @"
        INSERT INTO Autorizacion.GiroSolicitudAutorizacion (IdGiroPrincipal, IdGiroComplementario, IdDetalleGiro)
        OUTPUT INSERTED.IdGiroSolicitud
        VALUES (@idPrincipal, @idComplementario, @idDetalle);";

            using var commandInsert = new SqlCommand(queryInsert, connection);
            commandInsert.Parameters.AddWithValue("@idPrincipal", idPrincipal);
            commandInsert.Parameters.AddWithValue("@idComplementario", (object?)idComplementario ?? DBNull.Value);
            commandInsert.Parameters.AddWithValue("@idDetalle", (object?)idDetalle ?? DBNull.Value);

            var insertedId = await commandInsert.ExecuteScalarAsync();
            return Convert.ToInt32(insertedId);
        }

        public async Task InsertarGiroFlexible(GiroFlexible dto) {
            using var connection = new SqlConnection(_connDB);
            await connection.OpenAsync();

            using var command = new SqlCommand("Autorizacion.sp_InsertarGiroFlexible", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@TipoRegistro", dto.TipoRegistro);
            command.Parameters.AddWithValue("@Nombre", dto.Nombre);
            command.Parameters.AddWithValue("@Estado", dto.Estado);
            command.Parameters.AddWithValue("@IdGiroPrincipal", (object?)dto.IdGiroPrincipal ?? DBNull.Value);
            command.Parameters.AddWithValue("@IdGiroComplementario", (object?)dto.IdGiroComplementario ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<(bool Success, string Message)> ActualizarGiroFlexibleAsync(GiroFlexible giro) {
            using var connection = new SqlConnection(_connDB);
            using var command = new SqlCommand("Autorizacion.sp_ActualizarGiroFlexible", connection) {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@TipoRegistro", giro.TipoRegistro);
            command.Parameters.AddWithValue("@Nombre", giro.Nombre ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Estado", giro.Estado);

            // Parámetros opcionales con control de nulos
            command.Parameters.AddWithValue("@IdGiroPrincipal", giro.IdGiroPrincipal ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IdGiroComplementario", giro.IdGiroComplementario ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IdGiroDetalle", giro.IdGiroDetalle ?? (object)DBNull.Value);

            try {
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
                return (true, "Giro actualizado correctamente.");
            }
            catch(SqlException ex) {
                return (false, $"Error SQL: {ex.Message}");
            }
            catch(Exception ex) {
                return (false, $"Error inesperado: {ex.Message}");
            }
        }


    }
}
