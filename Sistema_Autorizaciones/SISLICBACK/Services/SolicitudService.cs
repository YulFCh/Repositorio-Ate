using Microsoft.Data.SqlClient;
using SistemaLicencias.SHARED.DTOs;
using SkiaSharp;
using System.Data;
using System.Net;

namespace SISLICBACK.Services {
    public class SolicitudService {
        private static readonly HttpClient client = new HttpClient();
        private string _ConProduccion;

        public SolicitudService(IConfiguration _configuration) {
            _ConProduccion = _configuration.GetSection("ConexionSQL:Licencia:Conexion").Value;

        }

        public async Task<object> InsertSolicitud(SolicitudDTO solicitud) {
            var resultados = new List<Dictionary<string, object>>();

            using(var connection = new SqlConnection(_ConProduccion)) {
                try {
                    await connection.OpenAsync();

                    using(var command = new SqlCommand("[Autorizacion].[sp_InsertarSolicitudAutorizacion]", connection)) {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@nroSolicitud", "0000001");
                        command.Parameters.AddWithValue("@nroAutorizacion", solicitud.NroAutorizacion);
                        command.Parameters.AddWithValue("@fechaAutorizacion", solicitud.FechaAutorizacion);
                        command.Parameters.AddWithValue("@nroExpediente", solicitud.NroExpediente);
                        command.Parameters.AddWithValue("@fecha_expediente", solicitud.FechaExpediente);
                        command.Parameters.AddWithValue("@nroResolucion", solicitud.NroResolucion);
                        command.Parameters.AddWithValue("@fechaResolucion", solicitud.FechaResolucion);
                        command.Parameters.AddWithValue("@idTipoLicencia", solicitud.IdTipoLicencia);
                        command.Parameters.AddWithValue("@id_concepto", solicitud.IdConcepto);
                        command.Parameters.AddWithValue("@id_solicitante", solicitud.IdSolicitante);
                        command.Parameters.AddWithValue("@estadoTramite", solicitud.EstadoTramite);
                        command.Parameters.AddWithValue("@observacion", solicitud.Observacion);
                        command.Parameters.AddWithValue("@vigencia_hasta", solicitud.VigenciaHasta);
                        command.Parameters.AddWithValue("@operadorRegistro", solicitud.OperadorRegistro);
                        command.Parameters.AddWithValue("@estacionRegistro", solicitud.EstacionRegistro);
                        command.Parameters.AddWithValue("@estado", solicitud.Estado);
                        command.Parameters.AddWithValue("@fechaSolicitud", solicitud.FechaSolicitud);
                        command.Parameters.AddWithValue("@giros", solicitud.Giros);
                        command.Parameters.AddWithValue("@estado_sol", solicitud.EstadoSol);
                        command.Parameters.AddWithValue("@operadorActualiza", solicitud.OperadorActualiza);
                        command.Parameters.AddWithValue("@estacionActualiza", solicitud.EstacionActualiza);
                        command.Parameters.AddWithValue("@fechaActualiza", solicitud.FechaActualiza);
                        command.Parameters.AddWithValue("@nroSerie", solicitud.NroSerie);
                        command.Parameters.AddWithValue("@siglas_resolucion", solicitud.SiglasResolucion);
                        command.Parameters.AddWithValue("@encargado", solicitud.Encargado);
                        command.Parameters.AddWithValue("@responsable", solicitud.Responsable);
                        command.Parameters.AddWithValue("@punto_local", solicitud.PuntoLocal);
                        command.Parameters.AddWithValue("@razon_social", solicitud.RazonSocial);
                        command.Parameters.AddWithValue("@Ordenanza_flag", solicitud.OrdenanzaFlag);
                        command.Parameters.AddWithValue("@tipo_ingreso", solicitud.TipoIngreso);
                        command.Parameters.AddWithValue("@plazo_mes", solicitud.PlazoMes);
                        command.Parameters.AddWithValue("@observacion2", solicitud.Observacion2);
                        command.Parameters.AddWithValue("@idSudZona", solicitud.IdSudZona);
                        command.Parameters.AddWithValue("@aHorario", solicitud.aHorario);
                        command.Parameters.AddWithValue("@idTipoZonif", solicitud.idTipoZonificacion);

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
                    Console.WriteLine($"❌ Error al ejecutar el Stored Procedure: {ex.Message}");
                    throw;
                }
            }

            return new {
                mensaje = "ok",
                response = resultados
            };
        }

        public async Task<object> InsertSolicitudPrueba(SolicitudDTO solicitud) {
            var resultados = new List<Dictionary<string, object>>();

            using(var connection = new SqlConnection(_ConProduccion)) {
                try {
                    await connection.OpenAsync();

                    using(var command = new SqlCommand("[Autorizacion].[uspInsertAutorizacionPB]", connection)) {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@nroLicencia", solicitud.NroAutorizacion);
                        command.Parameters.AddWithValue("@fechaLicencia", solicitud.FechaAutorizacion);
                        command.Parameters.AddWithValue("@nroExpediente", solicitud.NroExpediente);
                        command.Parameters.AddWithValue("@fecha_expediente", solicitud.FechaExpediente);
                        command.Parameters.AddWithValue("@nroResolucion", solicitud.NroResolucion);
                        command.Parameters.AddWithValue("@fechaResolucion", solicitud.FechaResolucion);
                        command.Parameters.AddWithValue("@idTipoLicencia", solicitud.IdTipoLicencia);
                        command.Parameters.AddWithValue("@id_concepto", solicitud.IdConcepto);
                        command.Parameters.AddWithValue("@id_solicitante", solicitud.IdSolicitante);
                        command.Parameters.AddWithValue("@estadoTramite", solicitud.EstadoTramite);
                        command.Parameters.AddWithValue("@observacion", solicitud.Observacion);
                        command.Parameters.AddWithValue("@vigencia_hasta", solicitud.VigenciaHasta);
                        command.Parameters.AddWithValue("@operadorRegistro", solicitud.OperadorRegistro);
                        command.Parameters.AddWithValue("@estacionRegistro", solicitud.EstacionRegistro);
                        command.Parameters.AddWithValue("@estado", solicitud.Estado);
                        command.Parameters.AddWithValue("@fechaSolicitud", solicitud.FechaSolicitud);
                        command.Parameters.AddWithValue("@giros", solicitud.Giros);
                        command.Parameters.AddWithValue("@estado_sol", solicitud.EstadoSol);
                        command.Parameters.AddWithValue("@operadorActualiza", solicitud.OperadorActualiza);
                        command.Parameters.AddWithValue("@estacionActualiza", solicitud.EstacionActualiza);
                        command.Parameters.AddWithValue("@fechaActualiza", solicitud.FechaActualiza);
                        command.Parameters.AddWithValue("@nroSerie", solicitud.NroSerie);
                        command.Parameters.AddWithValue("@siglas_resolucion", solicitud.SiglasResolucion);
                        command.Parameters.AddWithValue("@encargado", solicitud.Encargado);
                        command.Parameters.AddWithValue("@responsable", solicitud.Responsable);
                        command.Parameters.AddWithValue("@direccion_local", solicitud.PuntoLocal);
                        command.Parameters.AddWithValue("@razon_social", solicitud.RazonSocial);
                        command.Parameters.AddWithValue("@Ordenanza_flag", solicitud.OrdenanzaFlag);
                        command.Parameters.AddWithValue("@tipo_ingreso", solicitud.TipoIngreso);
                        command.Parameters.AddWithValue("@plazo_mes", solicitud.PlazoMes);
                        command.Parameters.AddWithValue("@observacion2", solicitud.Observacion2);

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
                    Console.WriteLine($"❌ Error al ejecutar el Stored Procedure: {ex.Message}");
                    throw;
                }
            }

            return new {
                mensaje = "ok",
                response = resultados
            };
        }

        public async Task<object> UpdateEstadoSolicitud(int idSolicitud, string nuevoEstado, string operadorActualiza, string estacionActualiza) {
            using(var connection = new SqlConnection(_ConProduccion)) {
                try {
                    await connection.OpenAsync();

                    //using(var command = new SqlCommand("[Licencia].[usp_UpdateEstadoSolicitud]", connection)) {
                    using(var command = new SqlCommand("[Autorizacion].[usp_UpdateEstadoAutorizacion]", connection)) {

                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@idSolicitud", SqlDbType.Int)).Value = idSolicitud;
                        command.Parameters.Add(new SqlParameter("@nuevoEstado", SqlDbType.VarChar, 10)).Value = nuevoEstado;
                        command.Parameters.Add(new SqlParameter("@operadorActualiza", SqlDbType.NVarChar, 50)).Value = operadorActualiza;
                        command.Parameters.Add(new SqlParameter("@estacionActualiza", SqlDbType.NVarChar, 50)).Value = estacionActualiza;

                        var filasAfectadas = await command.ExecuteNonQueryAsync();

                        return new {
                            mensaje = "Actualización exitosa",
                            filasModificadas = filasAfectadas
                        };
                    }
                }
                catch(Exception ex) {
                    Console.WriteLine($"Error al actualizar estado: {ex.Message}");
                    return new {
                        mensaje = "Error al actualizar estado",
                        error = ex.Message
                    };
                }
            }
        }

        public async Task<object> getSolicitudxParam(int tipoEstado) {
            var resultados = new List<Dictionary<string, object>>();

            using(var connection = new SqlConnection(_ConProduccion)) {
                try {
                    await connection.OpenAsync();

                    using(var command = new SqlCommand("Autorizacion.consultaTotalEstado", connection)) {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@TipoEstado", tipoEstado);

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



        public async Task<object> getZonasxMaps() {
            var resultados = new List<Dictionary<string, object>>();

            using(var connection = new SqlConnection(_ConProduccion)) {
                try {
                    await connection.OpenAsync();


                    string query = "select * from  Autorizacion.TB_ZONAS_AUTORIZACION_COORD;";

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
                mensaje = "OK",
                response = resultados
            };
        }


        public async Task<object> getSolicitudxId(int id) {
            var resultados = new List<Dictionary<string, object>>();

            using(var connection = new SqlConnection(_ConProduccion)) {
                try {
                    await connection.OpenAsync();

                    string query = "SELECT * FROM Licencia.Solicitud WHERE idSolicitud = @id";

                    using(var command = new SqlCommand(query, connection)) {
                        command.CommandType = CommandType.Text;


                        command.Parameters.AddWithValue("@id", id);

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

        //SOLICITUD + ESTECIMIENTO
        public async Task<List<SolicitudesAPI>> getSolicitudxId2(int id) {
            var resultados = new List<SolicitudesAPI>();

            using(var connection = new SqlConnection(_ConProduccion)) {
                try {
                    await connection.OpenAsync();

                    string query = "select s.*,e.aHorario from Licencia.Solicitud s inner join Licencia.Establecimiento e on e.idEstablecimiento = s.idEstablecimiento where idSolicitud = @id";

                    using(var command = new SqlCommand(query, connection)) {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@id", id);

                        using(var reader = await command.ExecuteReaderAsync()) {

                            while(await reader.ReadAsync()) {
                                var solicitud = new SolicitudesAPI {
                                    idSolicitud = reader.GetInt32(reader.GetOrdinal("idSolicitud")),
                                    nroSolicitud = reader["nroSolicitud"]?.ToString(),

                                    nroExpediente = reader["nroExpediente"]?.ToString(),


                                    aHorario = reader["aHorario"]?.ToString()
                                };

                                resultados.Add(solicitud);
                            }


                        }
                    }
                }
                catch(Exception ex) {
                    Console.WriteLine($"Error en la consulta: {ex.Message}");
                    return new List<SolicitudesAPI>(); // retorna vacío si falla
                }
            }

            return resultados;
        }

        //SOLICITUD + SOLICITANTE + ESTEBLECIMIENTO = WORD-DJ
        public async Task<Dictionary<string, object>> GetSolicitudWordPlantilla(int idSolicitud) {
            var datos = new Dictionary<string, object>();

            using(var connection = new SqlConnection(_ConProduccion)) {
                try {
                    await connection.OpenAsync();

                    string query = @"SELECT 
                                s.nroExpediente, s.fecha_expediente, s.razon_social, 
                                s.direccion_local, s.giros, s.vigencia_hasta, 
                                so.nrodni, s.observacion, e.aHorario
                            FROM Licencia.Solicitud s
                            JOIN Licencia.Solicitante so ON s.id_solicitante = so.idSolicitante
                            JOIN Licencia.Establecimiento e ON s.idEstablecimiento = e.idEstablecimiento
                            WHERE Desc_solicitud = 'AUTORIZACION' AND idSolicitud = @idSolicitud";

                    using(var command = new SqlCommand(query, connection)) {
                        command.Parameters.AddWithValue("@idSolicitud", idSolicitud);
                        using(var reader = await command.ExecuteReaderAsync()) {
                            if(await reader.ReadAsync()) {
                                for(int i = 0; i < reader.FieldCount; i++) {
                                    datos[reader.GetName(i)] = reader.IsDBNull(i) ? "" : reader.GetValue(i).ToString();
                                }
                            }
                        }
                    }
                }
                catch(Exception ex) {
                    Console.WriteLine($"Error en la consulta: {ex.Message}");
                    return null;
                }
            }

            return datos;
        }

        //SOLICITUD UPDATE RESOLUCION-VISTA-EMITIR-RESOLUCION
        public async Task<object> updateResolucion(uResolucionDTO dto) {
            var sql = @"
                    UPDATE Autorizacion.Solicitud_AUT 
             SET 
	            nroAutorizacion = @nroAutorizacion,
                 nroResolucion = @nroResolucion,
                 fechaResolucion = @fechaResolucion,
                 siglas_resolucion = @siglasResolucion,
                 fechaAutorizacion = @fechaLicencias,
                 vigencia_hasta = @vigencia_hasta,
                 operadorActualiza = @operadorActualiza,
                 estadoTramite = 5
             WHERE idSolicitud = @idSolicitud
                ";

            using(var connection = new SqlConnection(_ConProduccion)) {
                try {
                    await connection.OpenAsync();

                    using(var command = new SqlCommand(sql, connection)) {
                        command.Parameters.AddWithValue("@nroAutorizacion", dto.nroAutorizacion ?? string.Empty);
                        command.Parameters.AddWithValue("@nroResolucion", dto.nroResolucion ?? string.Empty);
                        command.Parameters.AddWithValue("@fechaResolucion", dto.fechaResolucion);
                        command.Parameters.AddWithValue("@siglasResolucion", dto.siglasResolucion ?? string.Empty);
                        command.Parameters.AddWithValue("@fechaLicencias", dto.fechaLicencias);
                        command.Parameters.AddWithValue("@vigencia_hasta", dto.vigencia_hasta);
                        command.Parameters.AddWithValue("@operadorActualiza", dto.operadorActualiza ?? string.Empty);
                        command.Parameters.AddWithValue("@idSolicitud", dto.idSolicitud);

                        var filasAfectadas = await command.ExecuteNonQueryAsync();

                        return new {
                            mensaje = filasAfectadas > 0 ? "Resolución actualizada correctamente." : "No se encontró la solicitud para actualizar.",
                            resultado = filasAfectadas
                        };
                    }
                }
                catch(Exception ex) {
                    Console.WriteLine($"Error en updateResolucion: {ex.Message}");
                    return new {
                        mensaje = "Error al actualizar la resolución.",
                        detalle = ex.Message
                    };
                }
            }
        }

        //GIROS
        public async Task<List<GiroItemDTO>> ListarGirosPrincipalesAsync() {
            var lista = new List<GiroItemDTO>();

            using(var connection = new SqlConnection(_ConProduccion)) {
                string query = @"SELECT IdGiroPrincipal AS Id, Nombre FROM Autorizacion.GiroPrincipal where estado = 1";

                using(var command = new SqlCommand(query, connection)) {
                    await connection.OpenAsync();
                    using(var reader = await command.ExecuteReaderAsync()) {
                        while(await reader.ReadAsync()) {
                            lista.Add(new GiroItemDTO {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<List<GiroItemDTO>> ListarComplementariosPorPrincipalAsync(int idPrincipal) {
            var lista = new List<GiroItemDTO>();

            using(var connection = new SqlConnection(_ConProduccion)) {
                using(var command = new SqlCommand("Autorizacion.sp_ObtenerGiroComplementario", connection)) {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@estado", 1); // 1 = activos, 0 = todos
                    command.Parameters.AddWithValue("@IdGiroPrincipal", idPrincipal);

                    await connection.OpenAsync();

                    using(var reader = await command.ExecuteReaderAsync()) {
                        while(await reader.ReadAsync()) {
                            lista.Add(new GiroItemDTO {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<List<GiroItemDTO>> ListarDetallePorComplementarioAsync(int idComplementario) {
            var lista = new List<GiroItemDTO>();

            using(var connection = new SqlConnection(_ConProduccion)) {
                using(var command = new SqlCommand("Autorizacion.sp_ObtenerDetallePorComplementario", connection)) {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IdGiroComplementario", idComplementario);
                    command.Parameters.AddWithValue("@estado", 1);// 1 = activos, 0 = todos

                    await connection.OpenAsync();

                    using(var reader = await command.ExecuteReaderAsync()) {
                        while(await reader.ReadAsync()) {
                            lista.Add(new GiroItemDTO {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return lista;
        }


        public async Task<int> ObtenerOCrearGiroSolicitud(int idPrincipal, int? idComplementario, int? idDetalle) {
            using var connection = new SqlConnection(_ConProduccion);
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
                          )
                    ";

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

        public async Task<RenovacionRespuestaDto> RenovarLicencia(RenovacionDTO renovacionDto) {
            try {
                using var connection = new SqlConnection(_ConProduccion);
                await connection.OpenAsync();

                using var command = new SqlCommand("Autorizacion.uspRenovarAutorizacion", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 60;

                // Agregar parámetros
                command.Parameters.AddWithValue("@idOriginal", renovacionDto.IdOriginal);
                command.Parameters.AddWithValue("@nroAutorizacion", renovacionDto.NroAutorizacion ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@fechaLicencia", renovacionDto.FechaLicencia);
                command.Parameters.AddWithValue("@nroExpediente", renovacionDto.NroExpediente ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@fechaExpediente", renovacionDto.FechaExpediente);
                command.Parameters.AddWithValue("@nroResolucion", renovacionDto.NroResolucion ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@fechaResolucion", renovacionDto.FechaResolucion);
                command.Parameters.AddWithValue("@vigenciaHasta", renovacionDto.VigenciaHasta);
                command.Parameters.AddWithValue("@operador", renovacionDto.Operador ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@estacion", renovacionDto.Estacion ?? (object)DBNull.Value);

                // Parámetro de salida para obtener el ID de la nueva solicitud (opcional)
                var outputParam = new SqlParameter("@idNuevaSolicitud", SqlDbType.Int) {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputParam);                

                await command.ExecuteNonQueryAsync();

                var idNuevaSolicitud = outputParam.Value != DBNull.Value
                    ? Convert.ToInt32(outputParam.Value)
                    : (int?)null;

                return new RenovacionRespuestaDto {
                    Exitoso = true,
                    Mensaje = "Licencia renovada exitosamente",
                    IdNuevaSolicitud = idNuevaSolicitud
                };
            }
            catch(SqlException ex) {
                return new RenovacionRespuestaDto {
                    Exitoso = false,
                    Mensaje = $"Error al renovar la licencia: {ex.Message}",
                    IdNuevaSolicitud = null
                };
            }
            catch(Exception ex) {
                return new RenovacionRespuestaDto {
                    Exitoso = false,
                    Mensaje = $"Error inesperado: {ex.Message}",
                    IdNuevaSolicitud = null
                };
            }
        }

    }
}
