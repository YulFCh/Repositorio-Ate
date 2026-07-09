using System;
using System.Collections.Generic;

namespace ApiConsultaDniPlanillas.Models
{
    // 1. Estructura interna para los JSON deserializados
    public class ConceptoDetalle
    {
        public string CodigoInterno { get; set; }
        public string Concepto { get; set; }
        public decimal Monto { get; set; }
    }

    // 2. Modelo de respuesta final para el cliente/frontend
    public class BoletaHistorialResponse
    {
        public int IdPlanilla { get; set; }
        public int IdEmpleado { get; set; }
        public string Entidad { get; set; }
        public string Empleador { get; set; }
        public string Ruc { get; set; }
        public string Rubro { get; set; }
        public string Meta { get; set; }
        public string CentroCosto { get; set; }
        public string Dni { get; set; }
        public string CodEmpleado { get; set; }
        public string NombresCompletos { get; set; }
        public string FechaIngreso { get; set; }
        public string TipoPension { get; set; }
        public string AdminPens { get; set; }
        public string Cuspp { get; set; }
        public string TipoComision { get; set; }
        public string Airhsp { get; set; }
        public string Sede { get; set; }
        public string CondicionLaboral { get; set; }
        public string Condicion { get; set; }
        public string Ocupacional { get; set; }
        public string Estructural { get; set; }
        public string Cargo { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }

        // Datos de Asistencia añadidos
        public int Jornada { get; set; }
        public int DiasLaborados { get; set; }
        public int DiasNoLaborados { get; set; }
        public int Subsidios { get; set; }
        public int Vacaciones { get; set; }

        // Colecciones deserializadas
        public List<ConceptoDetalle> Ingresos { get; set; } = new();
        public List<ConceptoDetalle> Egresos { get; set; } = new();
        public List<ConceptoDetalle> Aportes { get; set; } = new();

        public decimal TotalIngresos { get; set; }
        public decimal TotalEgresos { get; set; }
        public decimal NetoPagar { get; set; }
    }

    // 3. Modelo auxiliar con los nombres exactos del SELECT de tu SP
    public class BoletaDbResult
    {
        public int idPlanilla { get; set; }
        public int idEmpleado { get; set; }
        public string Entidad { get; set; }
        public string Empleador { get; set; }
        public string Ruc { get; set; }
        public string Rubro { get; set; }
        public string Meta { get; set; }
        public string centro_costo { get; set; }
        public string dni { get; set; }
        public string codEmpleado { get; set; }
        public string nombres_completos { get; set; }
        public string fecha_ingreso { get; set; } // Formateada como VARCHAR(10) desde el SP
        public string tipoPension { get; set; }
        public string adminPens { get; set; }
        public string cuspp { get; set; }
        public string tipoComision { get; set; }
        public string AIRHSP { get; set; }
        public string sede { get; set; }
        public string condicionLaboral { get; set; }
        public string condicion { get; set; }
        public string ocupacional { get; set; }
        public string Estructural { get; set; }
        public string cargo { get; set; }
        public int anio { get; set; }
        public int mes { get; set; }

        // Mapeos de asistencia
        public int Jornada { get; set; }
        public int diasLaborados { get; set; }
        public int diasNoLaborados { get; set; }
        public int subsidios { get; set; }
        public int vacaciones { get; set; }

        // Cadenas JSON crudas
        public string ingresos { get; set; }
        public string egresos { get; set; }
        public string aportes { get; set; }

        public decimal totalIngresos { get; set; }
        public decimal totalEgresos { get; set; }
        public decimal netoPagar { get; set; }
    }
}