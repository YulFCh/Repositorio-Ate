using Microsoft.Data.SqlClient;
using System.Data;
using SISLICBACK.Model;

namespace SISLICBACK.Services
{
    public class ConsultaSolcitudesService
    {
        private readonly string _connDB;

        public ConsultaSolcitudesService(IConfiguration configuration)
        {
            _connDB = configuration["ConexionSQL:Licencia:Conexion"];
        }

        public async Task<List<ConsultaSolicitudesModel>> ObtenerSolicitudesAsync(string? estado)
        {
            var lista = new List<ConsultaSolicitudesModel>();

            using var connection = new SqlConnection(_connDB);
            await connection.OpenAsync();

            using var command = new SqlCommand(
                "Autorizacion.sp_ConsultarSolicitudesConGiro_yfch",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@estado",
                string.IsNullOrEmpty(estado)
                    ? DBNull.Value
                    : estado);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new ConsultaSolicitudesModel
                {
                    idSolicitud = reader["idSolicitud"] != DBNull.Value
                        ? Convert.ToInt32(reader["idSolicitud"])
                        : 0,

                    nroAutorizacion = reader["nroAutorizacion"]?.ToString(),
                    fechaAutorizacion = reader["fechaAutorizacion"] as DateTime?,

                    nroExpediente = reader["nroExpediente"]?.ToString(),
                    fecha_expediente = reader["fecha_expediente"] as DateTime?,

                    nroResolucion = reader["nroResolucion"]?.ToString(),
                    fechaResolucion = reader["fechaResolucion"] as DateTime?,

                    estadoTramite = reader["estadoTramite"] != DBNull.Value
                        ? Convert.ToInt32(reader["estadoTramite"])
                        : 0,

                    estado = reader["estado"]?.ToString(), // 👈 NUEVO

                    vigencia_hasta = reader["vigencia_hasta"] as DateTime?,

                    fechaRegistro = reader["fechaRegistro"] != DBNull.Value
                        ? Convert.ToDateTime(reader["fechaRegistro"])
                        : DateTime.MinValue,

                    IdGiroSolicitud = reader["IdGiroSolicitud"]?.ToString(),
                    NombreGiro = reader["NombreGiro"]?.ToString(),

                    siglas_resolucion = reader["siglas_resolucion"]?.ToString(),
                    Solicitante = reader["Solicitante"]?.ToString(),
                    punto_local = reader["punto_local"]?.ToString(),
                    aHorario = reader["aHorario"]?.ToString(),
                    NombreSubzona = reader["NombreSubzona"]?.ToString()
                });
            }

            return lista;
        }
    }
}