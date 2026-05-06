using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaLicencias.SHARED.DTOs {
    public class GenerarPdfConImagenDto {
        public int Id { get; set; }
        public string? ImagenBase64 { get; set; } // puede venir null si no se envía
    }
}
