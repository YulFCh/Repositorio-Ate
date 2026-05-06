using Microsoft.Data.SqlClient;
using SistemaLicencias.SHARED.DTOs;
using System.Data;
using System.Net;

namespace SISLICBACK.Services {
    public class SesionesService {
        private static readonly HttpClient client = new HttpClient();
        private string _connDB;
        public SesionesService(IConfiguration _configuration) {
            _connDB = _configuration.GetSection("ConexionSQL:Licencia:Conexion").Value;
            
        }

        public async Task<object> valSesion(string parametro, string pass,HttpContext context) {

            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
             ?? context.Connection.RemoteIpAddress?.ToString();

            string host;
            try {
                host = Dns.GetHostEntry(ip).HostName;
            }
            catch {
                host = "Hostname no disponible";
            }

            // Aquí podrías registrar en logs, base de datos o incluir en el resultado.
            Console.WriteLine($"Login desde IP: {ip} | Host: {host}");


            using(var connection = new SqlConnection(_connDB)) {
                try {
                    await connection.OpenAsync();

                    using(var command = new SqlCommand("[Acceso].[sp_LogOut]", connection)) {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@buscar", SqlDbType.Int)).Value = 1;
                        command.Parameters.Add(new SqlParameter("@parametro", SqlDbType.NVarChar, 500)).Value = parametro;
                        command.Parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar, 300)).Value = pass;

                        using(var reader = await command.ExecuteReaderAsync()) {
                            if(await reader.ReadAsync()) {
                                return new {
                                    mensaje = "Autenticado correctamente",
                                    ip = ip,
                                    host = host,
                                    id_usuario = reader["id_usuario"].ToString(),
                                    login = reader["vlogin"].ToString(),
                                    area = Convert.ToInt32(reader["area"]),
                                    cajero = Convert.ToInt32(reader["cajero"]),
                                    id_doc = reader["id_doc"].ToString(),
                                    num_doc = reader["num_doc"].ToString(),
                                    nombre = reader["nombre"].ToString(),
                                    caja = reader["caja"].ToString(),
                                    nestado = Convert.ToInt32(reader["nestado"]),
                                    id_perfil = reader["id_perfil"].ToString(),
                                    nomb_perfil = reader["nomb_perfil"].ToString(),
                                    nomb_area = reader["nomb_area"].ToString(),
                                    encargado = reader["encargado"].ToString()
                                };
                            }
                            else {
                                return new {
                                    mensaje = "Usuario o contraseña incorrectos.",
                                    id_usuario = (object)null
                                };
                            }
                        }
                    }
                }
                catch(Exception ex) {
                    return new {
                        mensaje = $"Error: {ex.Message}",
                        id_usuario = (object)null
                    };
                }
            }
        }




    }
}
