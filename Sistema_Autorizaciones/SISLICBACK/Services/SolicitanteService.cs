using Microsoft.Data.SqlClient;
using SistemaLicencias.SHARED.DTOs;
using System.Data;
using System.Drawing;

namespace SISLICBACK.Services {
    public class SolicitanteService {
        private static readonly HttpClient client = new HttpClient();
        private string _connDB;

        public SolicitanteService(IConfiguration _configuration) {
            _connDB = _configuration.GetSection("ConexionSQL:Licencia:Conexion").Value;
        }

        public async Task<object> InsertSolAutor(SolicitanteDTO dto) {
            using(var connection = new SqlConnection(_connDB)) {
                try {
                    await connection.OpenAsync();

                    using(var command = new SqlCommand("[Autorizacion].[usp_InsertSoliAutor2]", connection)) {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregamos los parámetros del procedimiento
                        command.Parameters.Add(new SqlParameter("@nombre", SqlDbType.NVarChar, 100)).Value = dto.nombre;
                        command.Parameters.Add(new SqlParameter("@nrodni", SqlDbType.Char, 8)).Value = dto.nrodni;
                        command.Parameters.Add(new SqlParameter("@correo", SqlDbType.NVarChar, 100)).Value = dto.correo;
                        command.Parameters.Add(new SqlParameter("@telefono", SqlDbType.NVarChar, 20)).Value = dto.telefono;
                        command.Parameters.Add(new SqlParameter("@nroruc", SqlDbType.NVarChar, 20)).Value = dto.nroRuc;
                        command.Parameters.Add(new SqlParameter("@direccionF", SqlDbType.NVarChar, 500)).Value = dto.direccionFiscal;
                        command.Parameters.Add(new SqlParameter("@flag_dispacidad", SqlDbType.Int)).Value = dto.flag_discapacidad;
                        command.Parameters.Add(new SqlParameter("@nroconadis", SqlDbType.NVarChar, 100)).Value = dto.nroConadis;
                        command.Parameters.Add(new SqlParameter("@tipo_doc", SqlDbType.Int)).Value = dto.TipoDoc;
                        command.Parameters.Add(new SqlParameter("@pertenece", SqlDbType.NVarChar, 50)).Value = dto.Pertenece;

                        using(var reader = await command.ExecuteReaderAsync()) {
                            if(await reader.ReadAsync()) {
                                return new {
                                    mensaje = reader["Mensaje"].ToString(),
                                    id = reader["ID"]
                                };
                            }
                            else {
                                return new {
                                    mensaje = "No se recibió respuesta del procedimiento.",
                                    id = (object)null
                                };
                            }
                        }
                    }
                }
                catch(Exception ex) {
                    return new {
                        mensaje = $"Error: {ex.Message}",
                        id = (object)null
                    };
                }
            }
        }



        public async Task<object> GetSolAutxId(int id) {
            var resultados = new List<Dictionary<string, object>>();

            using(var connection = new SqlConnection(_connDB)) {
                try {
                    await connection.OpenAsync();

                    //string query = "SELECT * FROM MiTabla WHERE id = @id";
                    string query = "select top 20 * from Autorizacion.Solicitante_AUT sol order by sol.fecharegistro desc";


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

        //Licencia.Solicitante
        public async Task<object> GetAllData() {
            var resultados = new List<Dictionary<string, object>>();

            using(var connection = new SqlConnection(_connDB)) {
                try {
                    await connection.OpenAsync();


                    string query = "SELECT * FROM Autorizacion.Solicitante_AUT order by fecharegistro desc";

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

        public async Task<object> GetAllData2(int pageNumber = 1, int pageSize = 10, string searchTerm = null) {
            var resultados = new List<Dictionary<string, object>>();
            int totalRecords = 0;
            int totalPages = 0;
            int currentPage = pageNumber;

            using(var connection = new SqlConnection(_connDB)) {
                try {
                    await connection.OpenAsync();

                    using(var command = new SqlCommand("Autorizacion.sp_ListarSolicitantes_Paginado", connection)) {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar parámetros
                        command.Parameters.AddWithValue("@PageNumber", pageNumber);
                        command.Parameters.AddWithValue("@PageSize", pageSize);
                        command.Parameters.AddWithValue("@SearchTerm", (object)searchTerm ?? DBNull.Value);

                        using(var reader = await command.ExecuteReaderAsync()) {
                            while(await reader.ReadAsync()) {
                                var fila = new Dictionary<string, object>();

                                for(int i = 0; i < reader.FieldCount; i++) {
                                    string columnName = reader.GetName(i);

                                    // Capturar metadatos de paginación con conversión segura
                                    if(columnName == "TotalRecords") {
                                        totalRecords = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader.GetValue(i));
                                    }
                                    else if(columnName == "TotalPages") {
                                        totalPages = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader.GetValue(i));
                                    }
                                    else if(columnName == "CurrentPage") {
                                        currentPage = reader.IsDBNull(i) ? pageNumber : Convert.ToInt32(reader.GetValue(i));
                                    }
                                    else if(columnName != "PageSize") {
                                        // Agregar solo los datos reales, no los metadatos
                                        fila[columnName] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                    }
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
                response = resultados,
                pagination = new {
                    totalRecords = totalRecords,
                    totalPages = totalPages,
                    currentPage = currentPage,
                    pageSize = pageSize,
                    hasNextPage = currentPage < totalPages,
                    hasPreviousPage = currentPage > 1
                }
            };
        }


        public async Task<object> PostAprobacionSoli(string id, string texto) {
            using(var connection = new SqlConnection(_connDB)) {
                try {
                    await connection.OpenAsync();

                    string query = @"
                UPDATE Autorizacion.Solicitante_AUT 
                SET aprobado = @texto                                        
                WHERE idSolicitante = @id";

                    using(var command = new SqlCommand(query, connection)) {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = id;
                        command.Parameters.Add(new SqlParameter("@texto", SqlDbType.NVarChar, 50)).Value = texto;                        

                        int filasAfectadas = await command.ExecuteNonQueryAsync();

                        if(filasAfectadas > 0) {
                            return new {
                                mensaje = "✅ Aprobación actualizada correctamente",
                                response = new {
                                    idSolicitante = id,
                                    aprobado = texto,
                                    filasAfectadas = filasAfectadas
                                }
                            };
                        }
                        else {
                            return new {
                                mensaje = "⚠️ No se encontró el solicitante o no se realizaron cambios",
                                response = (object)null
                            };
                        }
                    }
                }
                catch(Exception ex) {
                    Console.WriteLine($"❌ Error al actualizar aprobación: {ex.Message}");
                    return new {
                        mensaje = $"Error: {ex.Message}",
                        response = (object)null
                    };
                }
            }
        }

        //Publicidad.Solicitante
        public async Task<object> getSolicitante() {
            var resultados = new List<Dictionary<string, object>>();

            using(var connection = new SqlConnection(_connDB)) {
                try {
                    await connection.OpenAsync();


                    string query = "SELECT * FROM Publicidad.Solicitante  order by fecharegistro desc";

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

        //SOLICITANTE PUBLICIDAD
        public async Task<SolicitanteAddResponse> AddSolicitantePublicidad(SolicitantePublicidad dto) {
            using var connection = new SqlConnection(_connDB);
            await connection.OpenAsync();

            using var command = new SqlCommand("[Publicidad].[sp_solicitanteADD]", connection) { CommandType = CommandType.StoredProcedure };

            // Parámetros (mantengo AddWithValue por brevedad, abajo te dejo versión pro con tipos y tamaños)
            command.Parameters.AddWithValue("@tipoSolicitante", dto.TipoSolicitante);
            command.Parameters.AddWithValue("@idSolicitante", dto.IdSolicitante);
            command.Parameters.AddWithValue("@nombrecontribuyente", dto.NombreContribuyente ?? "");
            command.Parameters.AddWithValue("@nrodni", dto.NroDni ?? "");
            command.Parameters.AddWithValue("@nroruc", dto.NroRuc ?? "");
            command.Parameters.AddWithValue("@correo", dto.Correo ?? "");
            command.Parameters.AddWithValue("@telefono", dto.Telefono ?? "");
            command.Parameters.AddWithValue("@direccion", dto.Direccion ?? "");
            command.Parameters.AddWithValue("@numeropred", dto.NumeroPred ?? "");
            command.Parameters.AddWithValue("@interior", dto.Interior ?? "");
            command.Parameters.AddWithValue("@manzana", dto.Manzana ?? "");
            command.Parameters.AddWithValue("@lote", dto.Lote ?? "");
            command.Parameters.AddWithValue("@denominacion", dto.Denominacion ?? "");
            command.Parameters.AddWithValue("@operador", dto.Operador ?? "");
            command.Parameters.AddWithValue("@estacion", dto.Estacion ?? "");
            command.Parameters.AddWithValue("@nombres_rl", dto.NombresRl ?? "");
            command.Parameters.AddWithValue("@idTipoDoc_rl", dto.IdTipoDocRl ?? "");
            command.Parameters.AddWithValue("@nroDoc_rl", dto.NroDocRl ?? "");
            command.Parameters.AddWithValue("@nroTelefono_rl", dto.NroTelefonoRl ?? "");
            command.Parameters.AddWithValue("@correo_rl", dto.CorreoRl ?? "");
            command.Parameters.AddWithValue("@direccion_rl", dto.DireccionRl ?? "");
            command.Parameters.AddWithValue("@nropartida_e", dto.NroPartidaE ?? "");
            command.Parameters.AddWithValue("@nroAsiento_sunarp", dto.NroAsientoSunarp ?? "");
            command.Parameters.AddWithValue("@nombres", dto.Nombres ?? "");
            command.Parameters.AddWithValue("@apellido_pat", dto.ApellidoPat ?? "");
            command.Parameters.AddWithValue("@apellido_mat", dto.ApellidoMat ?? "");
            command.Parameters.AddWithValue("@flag_discapacidad", dto.FlagDiscapacidad);
            command.Parameters.AddWithValue("@departamento", dto.Departamento ?? "");
            command.Parameters.AddWithValue("@provincia", dto.Provincia ?? "");
            command.Parameters.AddWithValue("@distrito", dto.Distrito ?? "");

            try {
                using var reader = await command.ExecuteReaderAsync();
                if(await reader.ReadAsync()) {
                    return new SolicitanteAddResponse {
                        Success = string.Equals(reader[0]?.ToString(), "TRUE", StringComparison.OrdinalIgnoreCase),
                        Mensaje = reader[1]?.ToString() ?? "",
                        Id = reader[2]
                    };
                }

                return new SolicitanteAddResponse {
                    Success = false,
                    Mensaje = "No se recibió respuesta del procedimiento.",
                    Id = null
                };
            }
            catch(Exception ex) {
                return new SolicitanteAddResponse {
                    Success = false,
                    Mensaje = $"Error: {ex.Message}",
                    Id = null
                };
            }
        }


    }
}
