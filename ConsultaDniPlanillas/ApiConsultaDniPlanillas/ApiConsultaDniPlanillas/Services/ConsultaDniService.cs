using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ApiConsultaDniPlanillas.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ApiConsultaDniPlanillas.Services
{
    public class ConsultaDniService
    {
        private readonly string _connectionString;

        public ConsultaDniService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<BoletaHistorialResponse>> ObtenerHistorialBoletasAsync(string dni, int? anio, int? mesDesde, int? mesHasta)
        {
            using var connection = new SqlConnection(_connectionString);

            var parameters = new DynamicParameters();
            parameters.Add("@dni", dni, DbType.String, size: 10);
            parameters.Add("@anio", anio, DbType.Int32);
            parameters.Add("@mesDesde", mesDesde, DbType.Int32);
            parameters.Add("@mesHasta", mesHasta, DbType.Int32);

            // Dapper mapea en base a las propiedades de BoletaDbResult
            var dbResult = await connection.QueryAsync<BoletaDbResult>(
                "dbo.sp_boletas_historial_dni",
                parameters,
                commandType: CommandType.StoredProcedure
            );


            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Transformación final con todos los campos del SP asignados correspondientemente
            var response = dbResult.Select(b => new BoletaHistorialResponse
            {
                IdPlanilla = b.idPlanilla,
                IdEmpleado = b.idEmpleado,
                Entidad = b.Entidad,
                NombrePlanilla = b.nombrePlanilla,
                Empleador = b.Empleador,
                Ruc = b.Ruc,
                Rubro = b.Rubro,
                Meta = b.Meta,
                CentroCosto = b.centro_costo,
                Dni = b.dni,
                CodEmpleado = b.codEmpleado,
                NombresCompletos = b.nombres_completos,
                FechaIngreso = b.fecha_ingreso,
                TipoPension = b.tipoPension,
                AdminPens = b.adminPens,
                Cuspp = b.cuspp,
                TipoComision = b.tipoComision,
                Airhsp = b.AIRHSP,
                Sede = b.sede,
                CondicionLaboral = b.condicionLaboral,
                Condicion = b.condicion,
                Ocupacional = b.ocupacional,
                Estructural = b.Estructural,
                Cargo = b.cargo,
                Situacion = b.situacion,
                Anio = b.anio,
                Mes = b.mes,

                // Asistencia
                Jornada = b.Jornada,
                DiasLaborados = b.diasLaborados,
                DiasNoLaborados = b.diasNoLaborados,
                Subsidios = b.subsidios,
                Vacaciones = b.vacaciones,

                // Totales Financieros
                TotalIngresos = b.totalIngresos,
                TotalEgresos = b.totalEgresos,
                NetoPagar = b.netoPagar,

                // Deserialización de JSON con protección ante valores nulos/vacíos
                Ingresos = string.IsNullOrWhiteSpace(b.ingresos) ? new List<ConceptoDetalle>() : JsonSerializer.Deserialize<List<ConceptoDetalle>>(b.ingresos, jsonOptions),
                Egresos = string.IsNullOrWhiteSpace(b.egresos) ? new List<ConceptoDetalle>() : JsonSerializer.Deserialize<List<ConceptoDetalle>>(b.egresos, jsonOptions),
                Aportes = string.IsNullOrWhiteSpace(b.aportes) ? new List<ConceptoDetalle>() : JsonSerializer.Deserialize<List<ConceptoDetalle>>(b.aportes, jsonOptions)
            }).ToList();

            return response;
        }

        public async Task<EmpleadoEmailModel> ObtenerEmpleadoPorDniAsync(string dni)
        {
            using var connection = new SqlConnection(_connectionString);

            var query = @"
        SELECT 
            dni AS Dni,
            email AS Email
        FROM dbo.empleado
        WHERE dni = @dni";

            var empleado = await connection.QueryFirstOrDefaultAsync<EmpleadoEmailModel>(
                query,
                new { dni }
            );

            return empleado;
        }
    }
}