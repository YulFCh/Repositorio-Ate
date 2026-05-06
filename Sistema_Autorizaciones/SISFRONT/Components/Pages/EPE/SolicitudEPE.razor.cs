using MudBlazor;
using SISFRONT.Components.Dialogs;
using SISFRONT.Components.Dialogs.EPE;
using SistemaLicencias.SHARED.DTOs;
using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SISFRONT.Components.Pages.EPE {
    public partial class SolicitudEPE {
        private MudForm? _form;
        private bool _saving;
        private List<HorarioItem> horarios = new();
        private bool cargando = true;

        private string prop = "";
        private string fechLic = "";
        private string procedencia = "";

        private readonly string _baseUrl = "http://localhost:5297";
        //private readonly string _baseUrl = "http://192.168.4.225:108";
        //private string _baseUrl = "http://192.168.4.225:181";

        // Modelos        
        private SolicitanteAPI soliApi = new SolicitanteAPI();
        private Utilidad _u = new Utilidad();
        private AnuncioInput _a = new AnuncioInput();
        private DetalleAnuncioInput _det = new DetalleAnuncioInput();
        private List<ResponseZon> lstZonificacion = new();

        private List<ComboDTO> lstUbicacionesFisicas = new List<ComboDTO>();
        private List<ComboDTO> lstTipologias = new List<ComboDTO>();
        private List<ComboDTO> lstClasificacionesTecnicas = new List<ComboDTO>();
        private List<ComboDTO> lstEstructuraMarco = new List<ComboDTO>();
        private List<ComboDTO> lstMaterialPredominante = new List<ComboDTO>();
        private List<ComboDTO> lstRequisitos = new List<ComboDTO>();

        //Estados Tab
        private bool mTab1 = false, mTab2 = false;
        // Privado
        //private bool ckFormato, ckDimensiones, ckFotos, ckAiresTerceros, ckPropiedadComun, ckPago;
        // Estructura
        //private bool ckPlanoUbicacion, ckPlanoFachada, ckMemoriaEstructuras, ckPlanoEstructuras;
        // Eléctrico
        //private bool ckMemoriaElectrica, ckPlanoElectrico, ckCartaFactibilidad;
        // Temporalidad/colindantes
        //private bool ckExhibicionTemporal, ckAutorizacionColindante;
        private bool ck1, ck2, ck3, ck4, ck5, ck6, ck7, ck8, ck9, ck10, ck11, ck12, ck13, ck14, ck15;
        private bool ck16, ck17, ck18, ck19, ck20, ck21, ck22, ck23, ck24, ck25, ck26, ck27, ck28;



        protected override async Task OnInitializedAsync() {

            await loadcbo1();
            await loadcbo2();
            await loadcbo3();
            await loadcbo4();
            await loadcbo5();
            await getTipoZonif();
            await getRequisitos();
        }

        //cargar combos
        private async Task loadcbo1() {
            lstUbicacionesFisicas = await GiroService.getCombos(1);
        }

        private async Task loadcbo2() {
            lstTipologias = await GiroService.getCombos(2);
        }
        private async Task loadcbo3() {
            lstClasificacionesTecnicas = await GiroService.getCombos(3);
        }
        private async Task loadcbo4() {
            lstEstructuraMarco = await GiroService.getCombos(4);
        }
        private async Task loadcbo5() {
            lstMaterialPredominante = await GiroService.getCombos(5);
        }

        public async Task getTipoZonif() {
            var url = $"{_baseUrl}/api/Establecimiento/getallzonif";
            var rs = await Http.GetFromJsonAsync<ZonificacionDTO>(url) ?? new();
            lstZonificacion = rs.response;
        }

        public async Task getRequisitos() {
            try {
                cargando = true;
                StateHasChanged(); // refresca la vista inmediatamente

                var url = $"{_baseUrl}/api/CrudEPE/getrequisitos/LOCAL/2/4";
                lstRequisitos = await Http.GetFromJsonAsync<List<ComboDTO>>(url) ?? new();
            }
            catch(Exception ex) {
                Console.WriteLine($"Error al cargar requisitos: {ex.Message}");
                lstRequisitos = new();
            }
            finally {
                cargando = false;
                StateHasChanged(); // asegura que la UI se actualice
            }
        }


        // Eventos
        private void RecalcularArea() {
            //var area = Math.Round((Convert.ToDouble(_det.ancho)) * ( Convert.ToDouble(_det.altoa)), 2, MidpointRounding.AwayFromZero);
            var area = Math.Round((_u.BaseM ?? 0) * (_u.AlturaM ?? 0), 2, MidpointRounding.AwayFromZero);
            _det.alto = _u.AlturaM.ToString();
            _det.ancho = _u.BaseM.ToString();
            _det.area = area.ToString("0.00");
        }

        private void RecalcularDias() {
            if(_u.FechaInicio.HasValue && _u.FechaFin.HasValue) {
                var ini = _u.FechaInicio.Value.Date;
                var fin = _u.FechaFin.Value.Date;
                var dias = (fin - ini).TotalDays + 1; // inclusivo

                // Evita negativos si el usuario invierte las fechas
                _u.ConteoDias = dias >= 0 ? dias.ToString("0") : "0";

                _a.fechaVigencia = _u.FechaFin.Value.ToString();
            }
            else {
                _u.ConteoDias = string.Empty;
            }
        }



        private void Reset() {
            _u = new();
            _form?.ResetAsync();
            _form?.ResetValidation();
            Snackbar.Add("Formulario limpiado.", Severity.Info);
        }




        private async Task AbrirDialogLic() {

            var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

            var dialog = DialogService.Show<DialogLicencias>("Buscar Licencia", options);
            var result = await dialog.Result;

            if(!result.Canceled) {
                if(result.Data is consultaLicenciaDTO lic) {
                    _u.NumeroLic = lic.nroLicencia;
                    prop = lic.razonSocial;
                    procedencia = "ATE";

                    if(DateTime.TryParse(lic.fechaLicencia, CultureInfo.InvariantCulture,
                       DateTimeStyles.None, out var dt)) {
                        fechLic = dt.ToString("yyyy", CultureInfo.InvariantCulture);
                    }
                    else {
                        fechLic = lic.fechaLicencia ?? "";
                    }

                    //anuncio
                    _det.idLicencia = lic.idSolicitud;
                    _det.nroLicencia = lic.nroLicencia;
                    _det.razsocial = lic.razonSocial;
                    _det.distrito = "ATE";

                }

            }
        }

        private async Task AbrirDialogoCustom() {

            var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

            var dialog = DialogService.Show<DialogCustom>("Buscar Solicitante", options);
            var result = await dialog.Result;

            if(!result.Canceled) {
                if(result.Data is Solicitan⁠teAPI solicitante) {
                    soliApi = solicitante;

                    _a.idSolicitante = soliApi.idSolicitante;

                }

            }
        }

        private async Task AbrirDialogUbiMarker() {
            var options = new DialogOptions {
                MaxWidth = MaxWidth.Large,
                FullWidth = true,
                CloseOnEscapeKey = true,
                CloseButton = true
            };

            var dialog = DialogService.Show<DialogUbicacion>("Ubicación Panel", options);
            var rs = await dialog.Result;
            if(!rs.Canceled) {
                if(rs.Data is string coord) {
                    Console.WriteLine(coord);
                    //_u.Coordenadas = coord;
                    _det.coordLngLat = coord;
                }
            }
        }

        private async Task AbrirDilogHabUrb() {
            var options = new DialogOptions {
                MaxWidth = MaxWidth.Large,
                FullWidth = true,
                CloseOnEscapeKey = true,
                CloseButton = true
            };

            var dialog = DialogService.Show<DialogHabUrb>("Habilitacion Urbana", options);
            var rs = await dialog.Result;
            if(!rs.Canceled) {
                if(rs.Data is string direc) {
                    _det.zonaUrbana = direc;
                    //_u.Coordenadas = coord;

                }
            }
        }

        private void GuardarAnexos() {
            string anexosCk = "";
            var checks = new[]
             {
                ck1, ck2, ck3, ck4, ck5, ck6, ck7,
                ck8, ck9, ck10, ck11, ck12, ck13,
                ck14, ck15, ck16, ck17, ck18, ck19, ck20,
                ck21, ck22, ck23, ck24, ck25, ck26, ck27, ck28
             };


            anexosCk = string.Join("-", checks.Select(c => c ? "1" : "0"));
        }

        private void CargarAnexos(string? cadena) {
            var tokens = (cadena ?? string.Empty).Split('-', StringSplitOptions.TrimEntries);
            var type = GetType();

            for(int i = 1; i <= 50; i++) // ajusta tope máximo
            {
                var field = type.GetField($"ck{i}", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if(field is null || field.FieldType != typeof(bool)) continue;

                bool value = (i - 1) < tokens.Length && tokens[i - 1] == "1";
                field.SetValue(this, value);
            }

            StateHasChanged();
        }

        /*----------------------------------HORARIO*/
        private void AgregarHorario() {
            horarios.Add(new HorarioItem());
        }

        private void EliminarHorario(HorarioItem item) {
            horarios.Remove(item);
        }

        private string ExportarComoJson() {
            var diccionario = horarios
                .Where(h => !string.IsNullOrWhiteSpace(h.Frecuencia))
                .ToDictionary(
                    h => h.Frecuencia,
                    h => $"{h.Desde} - {h.Hasta}"
                );

            return JsonSerializer.Serialize(diccionario);

        }
        private async Task Submit() {
            await _form!.Validate();
            if(!_form.IsValid) { Snackbar.Add("Revisa los campos requeridos.", Severity.Warning); return; }
            try {
                _saving = true;


                //detalle
                _det.horario = ExportarComoJson();
                //anuncio
                _a.fechaAutorizacion = _u.FechaSolicitud.ToString();
                _a.nroExpediente = "";
                _a.fechaExpediente = "";
                _a.idTipoSolicitud = Convert.ToInt16(_u.TipoSolitud);
                _a.idTipoUbicacion = Convert.ToInt16(_u.idUbicacionFisica);
                _a.idTipoCaracteristica = Convert.ToInt16(_u.idClasificacionTecnica);
                _a.idTipoElemento = Convert.ToInt16(_u.idTipologias);
                _a.idTipoEstructura = Convert.ToInt16(_u.idEstructuraMarco);
                _a.idTipoMaterial = Convert.ToInt16(_u.idMaterialPredominante);

                var obj = new {
                    detalle = _det,
                    anuncio = _a
                };

                var jsonOptions = new JsonSerializerOptions {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                var json = JsonSerializer.Serialize(obj, jsonOptions);
                Console.WriteLine(json);

                var rs = await Http.PostAsJsonAsync($"{_baseUrl}/api/CrudEPE/addanuncio/", obj);

                Console.WriteLine(rs);

                // TODO: post al backend
                await Task.Delay(500);
                Snackbar.Add("Solicitud registrada correctamente.", Severity.Success);
            }
            catch(Exception ex) {
                Snackbar.Add($"Error: {ex.Message}", Severity.Error);
            }
            finally { _saving = false; }
        }


        public class HorarioItem {
            public string Frecuencia { get; set; } = "";
            public string Desde { get; set; } = "";
            public string Hasta { get; set; } = "";
        }

        // ViewModel
        public class Utilidad {
            public string TipoSolitud {  get; set; }
            // 1
            public DateTime? FechaSolicitud { get; set; }
            public string? NumeroExpediente { get; set; }
            public DateTime? FechaExpediente { get; set; }

            // 4.1–4.5
            public string idUbicacionFisica { get; set; }
            public string idTipologias { get; set; }
            public string idClasificacionTecnica { get; set; }
            public string idEstructuraMarco { get; set; }
            public string idMaterialPredominante { get; set; }

            // 4.6
            public string? Direccion { get; set; }
            public string? ZonaUrbana { get; set; }
            public string? Zonificacion { get; set; }
            public string? Coordenadas { get; set; }

            // 4.7
            public double? BaseM { get; set; }
            public double? AlturaM { get; set; }
            public string? AreaM2 { get; set; }
            public int? Caras { get; set; } = 1;

            // 4.8
            public bool UsarTemporalidad { get; set; }
            public DateTime? FechaInicio { get; set; }
            public DateTime? FechaFin { get; set; }
            public string? ConteoDias { get; set; }

            // 4.9
            public bool UsarRefLicencia { get; set; }
            public string? NumeroLic { get; set; }
            public int? AnioLic { get; set; }
            public string? DistritoLic { get; set; }

            // 4.10 Horario
            public TimeSpan? LS_Inicio { get; set; } = new TimeSpan(07, 00, 00);
            public TimeSpan? LS_Fin { get; set; } = new TimeSpan(20, 00, 00);
            public TimeSpan? DF_Inicio { get; set; } = new TimeSpan(09, 00, 00);
            public TimeSpan? DF_Fin { get; set; } = new TimeSpan(22, 00, 00);

            // 5 Checklist (persistencia simple)
            public HashSet<string> ChecklistPrivado { get; set; } = new();
            public HashSet<string> ChecklistVias { get; set; } = new();
        }


        public class ListadoSelec {
            public string Id { get; set; }
            public string Descripcion { get; set; }

            public ListadoSelec(string id, string descripcion) {
                Id = id;
                Descripcion = descripcion;
            }
        }
    }
}
