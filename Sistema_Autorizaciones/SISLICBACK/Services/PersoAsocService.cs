using System.Data;
using System.Data.SqlClient;
using SISLICBACK.Model;
using Microsoft.Data.SqlClient;

namespace SISLICBACK.Services
{
    public class PersoAsocService
    {
        private readonly string _connectionString;

        public PersoAsocService(IConfiguration configuration)
        {
            _connectionString = configuration["ConexionSQL:Licencia:Conexion"];
        }

        public List<PersoAsocModel> GetAll()
        {
            var lista = new List<PersoAsocModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT 
                        id_person_asoc,
                        nombre_pers_asoc,
                        dni_pers_asoc,
                        domicilio_pers_asoc,
                        fecha_creacion,
                        usuario_creacion,
                        fecha_modifica,
                        usuario_modifica,
                        nombre_asociacion
                    FROM Licencia.Asociacion
                    ORDER BY id_person_asoc DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new PersoAsocModel
                        {
                            IdPersonAsoc = Convert.ToInt32(reader["id_person_asoc"]),
                            NombrePersAsoc = reader["nombre_pers_asoc"].ToString(),
                            DniPersAsoc = reader["dni_pers_asoc"].ToString(),
                            DomicilioPersAsoc = reader["domicilio_pers_asoc"].ToString(),
                            FechaCreacion = Convert.ToDateTime(reader["fecha_creacion"]),
                            UsuarioCreacion = reader["usuario_creacion"].ToString(),
                            FechaModifica = reader["fecha_modifica"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(reader["fecha_modifica"]),
                            UsuarioModifica = reader["usuario_modifica"].ToString(),
                            NombreAsociacion = reader["nombre_asociacion"].ToString()
                        });
                    }
                }
            }

            return lista;
        }
    }
}