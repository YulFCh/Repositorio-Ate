using MudBlazor;
using SistemaLicencias.SHARED.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace SISFRONT.Components.Pages.EPE {
    public partial class FormAdministrado {
        private MudForm? _form;
        private ContribuyenteModel _model = new();
        private bool _saving;
        private bool _success;

        private SolicitantePublicidad dto = new();
        private SolicitanteAddResponse rsSolicitante = new();


        private bool EsJuridica => _model.tipoPersona == "2";


        private async Task Submit() {
            if(_form is null) return;
            await _form.Validate();
            if(_form.IsValid) {

                dto.TipoSolicitante = int.Parse(_model?.tipoPersona);
                if(_model.tipoPersona != "2") { dto.NroDni = _model.Ruc; } else { dto.NroRuc = _model.Ruc; }
                dto.Operador =  sessions.UsuarioActual.login;

                rsSolicitante = await PublicidadService.addSolicitante(dto);
                Console.WriteLine("Payload OK:");
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(dto));
            }
        }

        private void Reset() {
            _model = new();
            _form?.ResetAsync();
        }



        // --- Validaciones custom ---
        private string? ValidateRuc(string? value) {
            if(string.IsNullOrWhiteSpace(value)) return "El RUC es obligatorio";
            if(!System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d{11}$"))
                return "El RUC debe tener 11 dígitos numéricos";
            return null;
        }

        private string? ValidateTelefono(string? value) {
            if(string.IsNullOrWhiteSpace(value)) return null; // no obligatorio
            if(!System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d{9}$"))
                return "El teléfono debe tener 9 dígitos";
            return null;
        }

        private string? ValidateDniCe(string? value, bool obligatorio) {
            if(string.IsNullOrWhiteSpace(value))
                return obligatorio ? "Documento obligatorio" : null;

            // DNI: 8 dígitos. CE: 9-12 alfanuméricos.
            if(System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d{8}$"))
                return null; // DNI válido
            if(System.Text.RegularExpressions.Regex.IsMatch(value, @"^[A-Za-z0-9]{9,12}$"))
                return null; // CE válido
            return "Ingrese DNI (8 dígitos) o CE (9–12 alfanuméricos)";
        }

        // --- Modelo ---
        public enum TipoPersona { Natural, Juridica }

        public class ContribuyenteModel {
            public string? tipoPersona { get; set; }

            [Required, MaxLength(11)]
            public string? Ruc { get; set; }

            [Required]
            public string? NombreORazon { get; set; }

            [Required]
            public string? DomicilioFiscal { get; set; }

            public string? Telefono { get; set; }

            [Required, EmailAddress]
            public string? Correo { get; set; }

            // Representante legal (solo para Jurídica)
            public string? RepNombre { get; set; }
            public string? RepDniCe { get; set; }
            public string? RepPartidaSunarp { get; set; }
            public string? RepDomicilio { get; set; }
            public string? RepTelefono { get; set; }
            [EmailAddress] public string? RepCorreo { get; set; }
        }
    }
}
