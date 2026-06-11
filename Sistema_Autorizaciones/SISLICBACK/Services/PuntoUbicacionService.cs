using Microsoft.Data.SqlClient;
using SISLICBACK.Model;
using System.Data;

namespace SISLICBACK.Services
{
    public class PuntoUbicacionService
    {
        private readonly IConfiguration _configuration;

        public PuntoUbicacionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<PuntoUbicacionModel> ObtenerPuntos(int? idZona = null)
        {
            List<PuntoUbicacionModel> lista = new();

            string conexion = _configuration["ConexionSQL:Licencia:Conexion"];

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                string query = @"
            SELECT
                IdSubzona,
                IdZona,
                Nombre,
                Tipo,
                Coordenadas,
                CapacidadBase,
                CapacidadExtra,
                regulacion,
                operadorRegistra,
                punto_ubicacion
            FROM [Autorizacion].[TB_SUBZONAS_AUTORIZACION_COORD]
            WHERE (@IdZona IS NULL OR IdZona = @IdZona)";

                SqlCommand cmd = new SqlCommand(query, cn);

                cmd.Parameters.AddWithValue("@IdZona", (object?)idZona ?? DBNull.Value);

                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new PuntoUbicacionModel
                    {
                        IdSubzona = Convert.ToInt32(dr["IdSubzona"]),
                        IdZona = Convert.ToInt32(dr["IdZona"]),
                        Nombre = dr["Nombre"].ToString(),
                        Tipo = dr["Tipo"].ToString(),
                        Coordenadas = dr["Coordenadas"].ToString(),
                        CapacidadBase = Convert.ToInt32(dr["CapacidadBase"]),
                        CapacidadExtra = Convert.ToInt32(dr["CapacidadExtra"]),
                        Regulacion = dr["regulacion"].ToString(),
                        OperadorRegistra = dr["operadorRegistra"].ToString(),
                        Punto_Ubicacion = dr["punto_ubicacion"].ToString()
                    });
                }
            }

            return lista;
        }
    }
}