using ApiSiga.Models;
using Microsoft.Data.SqlClient;

namespace ApiSiga.Services
{
    public class CargaDatosService
    {
        private readonly string _connectionString;

        public CargaDatosService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("cn");
        }

        public List<CargaDatosModel> ObtenerPorOrden(int nroOrden, int anio, string? tipoBien)
        {
            var lista = new List<CargaDatosModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT
                OI.ANO_EJE,
                OI.NRO_ORDEN,

                CASE OI.TIPO_BIEN
                    WHEN 'B' THEN 'BIEN'
                    WHEN 'S' THEN 'SERVICIO'
                    ELSE 'OTRO'
                END AS TIPO_BIEN,

                C.CODIGO_ITEM,
                C.NOMBRE_ITEM,
                OI.CANT_ITEM,
                OI.CANT_RECIBIDA,
                OI.PREC_UNIT_MONEDA,
                OI.PREC_TOT_MONEDA

            FROM SIG_ORDEN_ITEM OI

            INNER JOIN CATALOGO_BIEN_SERV C
                ON OI.TIPO_BIEN = C.TIPO_BIEN
                AND OI.GRUPO_BIEN = C.GRUPO_BIEN
                AND OI.CLASE_BIEN = C.CLASE_BIEN
                AND OI.FAMILIA_BIEN = C.FAMILIA_BIEN
                AND OI.ITEM_BIEN = C.ITEM_BIEN

            WHERE OI.NRO_ORDEN = @NRO_ORDEN
              AND OI.ANO_EJE = @ANO_EJE
        ";

                if (!string.IsNullOrEmpty(tipoBien))
                {
                    query += " AND OI.TIPO_BIEN = @TIPO_BIEN ";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NRO_ORDEN", nroOrden);
                    cmd.Parameters.AddWithValue("@ANO_EJE", anio);

                    if (!string.IsNullOrEmpty(tipoBien))
                    {
                        cmd.Parameters.AddWithValue("@TIPO_BIEN", tipoBien);
                    }

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new CargaDatosModel
                            {
                                AnioEje = Convert.ToInt32(reader["ANO_EJE"]),
                                NroOrden = reader["NRO_ORDEN"].ToString(),
                                TipoBien = reader["TIPO_BIEN"].ToString(),
                                CodigoItem = reader["CODIGO_ITEM"].ToString(),
                                NombreItem = reader["NOMBRE_ITEM"].ToString(),
                                CantItem = Convert.ToDecimal(reader["CANT_ITEM"]),
                                CantRecibida = Convert.ToDecimal(reader["CANT_RECIBIDA"]),
                                PrecioUnitario = Convert.ToDecimal(reader["PREC_UNIT_MONEDA"]),
                                PrecioTotal = Convert.ToDecimal(reader["PREC_TOT_MONEDA"])
                            });
                        }
                    }
                }
            }

            return lista;
        }
    }
}