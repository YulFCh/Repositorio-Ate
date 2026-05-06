using Microsoft.Data.SqlClient;
using SistemaLicencias.SHARED.DTOs;
using System;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xceed.Words.NET;
using static QuestPDF.Helpers.Colors;
using static System.Net.WebRequestMethods;

namespace SISLICBACK.Services {
    public class ConsultaService {
        private static readonly HttpClient client = new HttpClient();
        private string _conn;


        public ConsultaService(IConfiguration _configuration) {            
            _conn = _configuration.GetSection("ConexionSQL:Licencia:Conexion").Value;
        }

        public async Task<(int Total, List<consultaLicenciaDTO>)> getLicencia(string nroLicencia, string razSocial, int pagenumber) {
            var lst = new List<consultaLicenciaDTO>();
            var total = 0;

            using(var connection = new SqlConnection(_conn)) {
                await connection.OpenAsync();

                using(var command = new SqlCommand("Publicidad.usp_consulta_licencia_paged", connection)) {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 30;


                    command.Parameters.Add("@nroLicencia", SqlDbType.VarChar, 50)
                        .Value = string.IsNullOrWhiteSpace(nroLicencia) ? (object)DBNull.Value : nroLicencia.Trim();
                    command.Parameters.Add("@razSocial", SqlDbType.VarChar, 200)
                        .Value = string.IsNullOrWhiteSpace(razSocial) ? (object)DBNull.Value : razSocial.Trim();
                    command.Parameters.Add("@PageNumber", SqlDbType.Int).Value = pagenumber < 1 ? 1 : pagenumber;
                    command.Parameters.Add("@PageSize", SqlDbType.Int).Value = 20;


                    using(var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection)) {

                        while(await reader.ReadAsync()) {
                            lst.Add(new consultaLicenciaDTO {
                                idSolicitud = reader.GetInt32(0),
                                nroLicencia = reader.GetString(1),
                                fechaLicencia = reader.IsDBNull(2) ? null : reader.GetDateTime(2).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),//DATETIME
                                nroExpediente = reader.GetString(3),
                                fechaExpediente = reader.GetDateTime(4).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),//DATETIME
                                nroResolucion = reader.IsDBNull(5) ? null : reader.GetString(5),
                                fechaResolucion = reader.IsDBNull(6) ? null : reader.GetDateTime(6).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),//DATETIME
                                descSolicitud = reader.GetString(7),
                                razonSocial = reader.GetString(8),
                                descEstado = reader.GetString(9),
                                estado = reader.GetString(10),
                                obsCese = reader.IsDBNull(11) ? null : reader.GetString(10),
                            });
                        }

                        if(await reader.NextResultAsync() && await reader.ReadAsync()) {
                            total = reader.GetInt32(0);
                        }
                    }
                }
            }

            return (total, lst);
        }

        public async Task<List<ComboDTO>> getCombos(int tipo) {
            List<ComboDTO> lst = new List<ComboDTO>();

            using(var connection = new SqlConnection(_conn)) {
                await connection.OpenAsync();

                using(var command = new SqlCommand("Publicidad.usp_GetCombos", connection)) {
                    command.CommandType = CommandType.StoredProcedure; 

                    command.Parameters.Add("@Tipo", SqlDbType.Int).Value = tipo;

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

        public async Task<List<consultaAnuncio>> getListAnuncio(CancellationToken ct = default) {
            var lst = new List<consultaAnuncio>();

            var sql = @"
  select a.idAnuncio,a.fechaRegistra,s.nombre,a.nroExpediente,a.fechaExpediente,a.nroAutorizacion,a.fechaAutorizacion,a.fechaVigencia,a.nroInformeEsp,e.descripcionEstado,tu.descripcion
from Publicidad.AnuncioPublicitario a 
join Licencia.EstadoTramite e on a.idEstadoTramite = e.idEstado
left join Licencia.Solicitante s on a.idSolictitante = s.idSolicitante
left join Publicidad.TipoUbicacion tu on a.idTipoUbicacion = tu.idTipoUbicacion
where a.idEstadoTramite = 1;
    ";

            using var cn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, cn);
            await cn.OpenAsync(ct);
            using var rd = await cmd.ExecuteReaderAsync(ct);

            // Cachea los índices por nombre (más claro y a prueba de reordenamientos)
            int iId = rd.GetOrdinal("idAnuncio");
            int FecReg = rd.GetOrdinal("fechaRegistra");
            int iNombres = rd.GetOrdinal("nombre");
            int iNroExp = rd.GetOrdinal("nroExpediente");
            int iFecExp = rd.GetOrdinal("fechaExpediente");
            int iNroAut = rd.GetOrdinal("nroAutorizacion");
            int iFecAut = rd.GetOrdinal("fechaAutorizacion");
            int iFecVig = rd.GetOrdinal("fechaVigencia");
            int iNroInf = rd.GetOrdinal("nroInformeEsp");
            int iDescEstado = rd.GetOrdinal("descripcionEstado");

            while(await rd.ReadAsync(ct)) {
                lst.Add(new consultaAnuncio {
                    idAnuncio = rd.GetInt32(iId),
                    fechaRegistra = ReadDateAsString(rd, FecReg),
                    nombres = ReadString(rd, iNombres),
                    nroExpediente = ReadString(rd, iNroExp),
                    fechaExpediente = ReadDateAsString(rd, iFecExp),   // formatea si es Date/DateTime
                    nroAutorizacion = ReadString(rd, iNroAut),
                    fechaAutorizacion = ReadDateAsString(rd, iFecAut),
                    fechaVigencia = ReadDateAsString(rd, iFecVig),
                    nroInformeEsp = ReadString(rd, iNroInf),
                    descripcionEstado = ReadString(rd, iDescEstado) ?? string.Empty
                });
            }

            return lst;
        }

        // Helpers null-safe:
        static string? ReadString(SqlDataReader r, int idx)
            => r.IsDBNull(idx) ? null : r.GetString(idx);

        static string? ReadDateAsString(SqlDataReader r, int idx) {
            if(r.IsDBNull(idx)) return null;

            var t = r.GetFieldType(idx);
            if(t == typeof(DateTime))
                return r.GetDateTime(idx).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            if(t == typeof(DateTimeOffset))
                return r.GetDateTimeOffset(idx).UtcDateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            // Si en tu BD aún están como VARCHAR, devuélvelo crudo
            return r.GetValue(idx)?.ToString();
        }

        //listar coordenadas
        public async Task<List<consultaCoordenadas>> getCoordenadasEPE() {
            List<consultaCoordenadas> lst = new List<consultaCoordenadas>();
            string query = "select a.idAnuncio, d.coordLngLat from Publicidad.AnuncioPublicitario a join Publicidad.DetalleAnuncio d on a.idDetalle = d.idDetalle";
            using(var connection = new SqlConnection(_conn)) {

                using(var command = new SqlCommand(query, connection)) {
                    await connection.OpenAsync();
                    using(var reader = await command.ExecuteReaderAsync()) {
                        while(await reader.ReadAsync()) {
                            lst.Add(new consultaCoordenadas {
                                idAnuncio = reader.GetInt32(0),
                                // Verificamos si la columna 1 es DBNull
                                // Si no es nula, leemos el string. Si es nula, asignamos null o string.Empty
                                coordLngLat = !reader.IsDBNull(1) ? reader.GetString(1) : null
                            });
                        }
                    }
                }
            }

            return lst;
        }

        //listar estados tramite 
        public async Task<object> getLstEstTram() {
            var resultados = new List<Dictionary<string, object>>();

            using(var connection = new SqlConnection(_conn)) {
                try {
                    await connection.OpenAsync();


                    string query = @" 
                    select idEstado as 'Id',descripcionEstado as 'Nombre',estado as 'Estado' from Licencia.EstadoTramite where idEstado in (8,11,13,28)
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

        // LISTAR X ID (usando el SP)
        public async Task<object> getAnuncioxID(int id, byte opcion = 1) {
            var resultados = new List<Dictionary<string, object>>();

            using(var connection = new SqlConnection(_conn)) {
                try {
                    await connection.OpenAsync();

                    using(var command = new SqlCommand("[Publicidad].[sp_AnuncioPublicitario_GetById]", connection)) {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@IdAnuncio", SqlDbType.Int).Value = id;
                        command.Parameters.Add("@Opcion", SqlDbType.TinyInt).Value = opcion;

                        using(var reader = await command.ExecuteReaderAsync()) {
                            var schema = reader.GetColumnSchema();
                            while(await reader.ReadAsync()) {
                                var fila = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                                foreach(var col in schema) {
                                    int ord = col.ColumnOrdinal ?? -1;
                                    fila[col.ColumnName] = reader.IsDBNull(ord) ? null : reader.GetValue(ord);
                                }
                                resultados.Add(fila);
                            }
                        }
                    }
                }
                catch(SqlException ex) {
                    Console.WriteLine($"SQL Error getAnuncioxID: {ex.Message}");
                    return new { mensaje = $"SQL Error: {ex.Message}", response = (object)null };
                }
                catch(Exception ex) {
                    Console.WriteLine($"Error getAnuncioxID: {ex.Message}");
                    return new { mensaje = $"Error: {ex.Message}", response = (object)null };
                }
            }

            return new { mensaje = "OK", resultado = resultados };
        }

        //CONSULTA ANEXOS FIRMADOS PUBLICIDAD
        public async Task<List<dynamic>> getAnexosPubxId(int idSolicitud) {
            var datos = new List<dynamic>();

            using(var connection = new SqlConnection(_conn)) {
                try {
                    await connection.OpenAsync();
                    string query = @" SELECT
                a.id_doc_sol_anexos,
                a.idSolicitud,
                a.id_anexos_doc,
                d.desc_anexos_doc,
                a.numero_doc,
                a.fecha_doc,
                a.fecha_registro,
                a.RutaDoc,
                a.NombreDoc
            FROM Publicidad.Doc_Solicitud_Anexos_Pub a
            JOIN Licencia.TipoAnexos_doc d ON a.id_anexos_doc = d.id_anexos_doc
            WHERE a.idSolicitud = @idSolicitud AND a.flag_estado = 1";

                    using(var command = new SqlCommand(query, connection)) {
                        command.Parameters.AddWithValue("@idSolicitud", idSolicitud);

                        using(var reader = await command.ExecuteReaderAsync()) {
                            while(await reader.ReadAsync()) {
                                // Se crea un nuevo objeto ExpandoObject para cada fila
                                dynamic anexo = new ExpandoObject();
                                var anexoDict = (IDictionary<string, object>)anexo;

                                // Se asignan las propiedades dinámicamente
                                for(int i = 0; i < reader.FieldCount; i++) {
                                    anexoDict[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                }

                                datos.Add(anexo);
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





    }
}
