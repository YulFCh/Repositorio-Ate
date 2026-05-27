using ApiSiga.Models;
using Microsoft.Data.SqlClient;

namespace ApiSiga.Services
{
    public class ConsultaPatrimService
    {
        private readonly string _connectionString;

        public ConsultaPatrimService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("cn");
        }

        public List<ConsultaPatrimModel> BuscarPatrimonio(string texto)
        {
            List<ConsultaPatrimModel> lista = new List<ConsultaPatrimModel>();

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();

                string query = @"
                    SELECT 
                        SEC_EJEC,
                        SECUENCIA,
                        CODIGO_ACTIVO,
                        TIPO_PATRIM,
                        ESTADO,
                        TIPO_BIEN,
                        DESCRIPCION,
                        TIPO_ACTIVO,
                        EMPLEADO,
                        EMPLEADO_FINAL,
                        TIPO_UBICAC,
                        MODELO,
                        NRO_SERIE,
                        MARCA,
                        ESTADO_ACTUAL,
                        FECHA_REG,
                        CARACTERISTICAS,
                        VALOR_INICIAL_ORIG
                    FROM SIG_PATRIMONIO
                    WHERE DESCRIPCION LIKE @texto
                ";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@texto", "%" + texto + "%");

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new ConsultaPatrimModel
                            {
                                SEC_EJEC = Convert.ToDecimal(dr["SEC_EJEC"]),
                                SECUENCIA = Convert.ToDecimal(dr["SECUENCIA"]),
                                CODIGO_ACTIVO = dr["CODIGO_ACTIVO"].ToString(),
                                TIPO_PATRIM = Convert.ToDecimal(dr["TIPO_PATRIM"]),
                                ESTADO = dr["ESTADO"].ToString(),
                                TIPO_BIEN = dr["TIPO_BIEN"].ToString(),
                                DESCRIPCION = dr["DESCRIPCION"].ToString(),
                                TIPO_ACTIVO = dr["TIPO_ACTIVO"].ToString(),
                                EMPLEADO = dr["EMPLEADO"].ToString(),
                                EMPLEADO_FINAL = dr["EMPLEADO_FINAL"].ToString(),
                                TIPO_UBICAC = Convert.ToDecimal(dr["TIPO_UBICAC"]),
                                MODELO = dr["MODELO"].ToString(),
                                NRO_SERIE = dr["NRO_SERIE"].ToString(),
                                MARCA = Convert.ToDecimal(dr["MARCA"]),
                                ESTADO_ACTUAL = dr["ESTADO_ACTUAL"].ToString(),
                                FECHA_REG = Convert.ToDateTime(dr["FECHA_REG"]),
                                CARACTERISTICAS = dr["CARACTERISTICAS"].ToString(),
                                VALOR_INICIAL_ORIG = Convert.ToDecimal(dr["VALOR_INICIAL_ORIG"])
                            });
                        }
                    }
                }
            }

            return lista;
        }
    }
}