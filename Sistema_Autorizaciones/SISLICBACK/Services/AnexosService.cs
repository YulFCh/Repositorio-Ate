using Microsoft.Data.SqlClient;
using SistemaLicencias.SHARED.DTOs;

namespace SISLICBACK.Services {
    public class AnexosService {
        private static readonly HttpClient client = new HttpClient();
        private string _connDB;

        public AnexosService(IConfiguration _configuration) {
            _connDB = _configuration.GetSection("ConexionSQL:Licencia:Conexion").Value;
            

        }



        public async Task<List<AnexosAPI>> getAnexosxId(int idSolicitud) {
            var datos = new List<AnexosAPI>();

            using(var connection = new SqlConnection(_connDB)) {
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
                    FROM Autorizacion.Doc_Solicitud_Anexos_Aut a 
                    JOIN Licencia.TipoAnexos_doc d ON a.id_anexos_doc = d.id_anexos_doc
                    WHERE a.idSolicitud = @idSolicitud AND a.flag_estado = 1";

                    using(var command = new SqlCommand(query, connection)) {
                        command.Parameters.AddWithValue("@idSolicitud", idSolicitud);
                        

                        using(var reader = await command.ExecuteReaderAsync()) {
                            while(await reader.ReadAsync()) {
                                var anexo = new AnexosAPI {
                                    IdDocSolAnexos = reader.GetInt32(reader.GetOrdinal("id_doc_sol_anexos")),
                                    IdSolicitud = reader.GetInt32(reader.GetOrdinal("idSolicitud")),
                                    IdAnexosDoc = reader.GetInt32(reader.GetOrdinal("id_anexos_doc")),
                                    DescAnexosDoc = reader.IsDBNull(reader.GetOrdinal("desc_anexos_doc")) ? "" : reader.GetString(reader.GetOrdinal("desc_anexos_doc")),
                                    NumeroDoc = reader.IsDBNull(reader.GetOrdinal("numero_doc")) ? "" : reader.GetString(reader.GetOrdinal("numero_doc")),
                                    FechaDoc = reader.IsDBNull(reader.GetOrdinal("fecha_doc")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("fecha_doc")),
                                    FechaReg = reader.IsDBNull(reader.GetOrdinal("fecha_registro")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("fecha_registro")),
                                    RutaDoc = reader.IsDBNull(reader.GetOrdinal("RutaDoc")) ? "" : reader.GetString(reader.GetOrdinal("RutaDoc")),
                                    NombreDoc = reader.IsDBNull(reader.GetOrdinal("NombreDoc")) ? "" : reader.GetString(reader.GetOrdinal("NombreDoc"))
                                };

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

        public async Task<bool> EliminarAnexoAsync(int idAnexo, int estado) {
            using(var connection = new SqlConnection(_connDB))
            using(var command = new SqlCommand()) {
                command.Connection = connection;
                command.CommandText = @"UPDATE Autorizacion.Doc_Solicitud_Anexos_Aut
                                SET flag_estado = @estado 
                                WHERE id_doc_sol_anexos = @idAnexo";

                command.Parameters.AddWithValue("@estado", estado);
                command.Parameters.AddWithValue("@idAnexo", idAnexo);

                await connection.OpenAsync();

                int filasAfectadas = await command.ExecuteNonQueryAsync();

                return filasAfectadas > 0;
            }
        }



    }
}