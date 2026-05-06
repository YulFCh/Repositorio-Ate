using Microsoft.JSInterop;
using MudBlazor;
using SistemaLicencias.SHARED.DTOs;
using System.Drawing;

namespace SISFRONT.Components.Pages {
    public partial class ConsultaMaps {

        private string _baseUrl = "http://192.168.0.139:148";
        //private string _baseUrl = "http://localhost:5297";

        private int _zonaSeleccionada;
        private bool _buscado = false;
        private List<ZonasAutApi> zonas = new();
        private List<SubZonasAutApi> subzonas = new();
        private string _filtroNombre = string.Empty;
        private string color = "";


        private IEnumerable<SubZonasAutApi> filteredSubzonas =>
            string.IsNullOrWhiteSpace(_filtroNombre)
                ? subzonas
                : subzonas.Where(s => s.Nombre.Contains(_filtroNombre, StringComparison.OrdinalIgnoreCase));

        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if(firstRender) {
                var dotNetRef = DotNetObjectReference.Create(this);
                await JS.InvokeVoidAsync("googleMaps.initMap", dotNetRef);
            }
        }
        protected override async Task OnInitializedAsync() {
            var rs = await Http.GetFromJsonAsync<ZonasAutDTO>($"{_baseUrl}/api/Solicitud/getzonam");
            zonas = rs.response;
        }

        private async Task BuscarSubzonas() {

            subzonas = new List<SubZonasAutApi>();
            _filtroNombre = string.Empty;
            Console.WriteLine("Solicitando limpieza de marcadores...");
            await MapsInterop.ClearMarkers();

            await MapsInterop.ClearMarkers();

            var res = await Http.GetFromJsonAsync<SubZonasAutDTO>($"{_baseUrl}/api/MarkMaps/getall/{_zonaSeleccionada}");

            subzonas = res.response;
            _buscado = true;

            foreach(var subzona in subzonas) {
                if(!string.IsNullOrWhiteSpace(subzona.Coordenadas)) {
                    var partes = subzona.Coordenadas.Split(',');
                    if(partes.Length >= 2 &&
                        double.TryParse(partes[1], out double lat) &&
                        double.TryParse(partes[0], out double lng)) {
                        string titulo = subzona.Nombre;
                        string contenido = $"<b>{subzona.Nombre}</b><br>Disponibilidad: {subzona.Ocupados}/{subzona.CapacidadTotal}";



                        if(subzona.Ocupados >= subzona.CapacidadTotal)
                            color = "rojo";
                        else if(subzona.Ocupados == 3)
                            color = "ambar";
                        else
                            color = "verde";

                        //await JS.InvokeVoidAsync("googleMaps.addMarker", lat, lng, subzona.Nombre, contenido, color);
                        await MapsInterop.AddMarker(lat, lng, titulo, contenido, color);
                    }
                }
            }

        }


        /*
        private string _baseUrl = "http://192.168.0.139:148";
        //private string _baseUrl = "http://localhost:5297";

        private List<ZonasAutApi> zonas = new();
        private List<SubZonasAutApi> subzonas = new();
        private List<CoordenadasDTO> coordenadas;

        private int _zonaSeleccionada;
        private int _subzonasSeleccionada;


        protected override async Task OnInitializedAsync() {
            var rs = await Http.GetFromJsonAsync<ZonasAutDTO>($"{_baseUrl}/api/Solicitud/getzonam");
            zonas = rs.response;
            
        }

        private async Task OnZonaChanged(int nuevaZona) {
            _zonaSeleccionada = nuevaZona;

            var res = await Http.GetFromJsonAsync<SubZonasAutDTO>($"{_baseUrl}/api/MarkMaps/getall/{_zonaSeleccionada}");
            subzonas = res.response;

           
        }

   
        */


        /*
        protected override async Task OnInitializedAsync() {
            try {

                var rs = await Http.GetFromJsonAsync<SubZonasAutDTO>($"{_baseUrl}/api/MarkMaps/getallCap");
                sb = rs.response;

                coordenadas = new List<CoordenadasDTO>();

                foreach(var item in sb) {
                    if(!string.IsNullOrWhiteSpace(item.Coordenadas) && item.Coordenadas.Contains(",")) {

                        var parts = item.Coordenadas.Split(',');

                        if(double.TryParse(parts[0], out var lng) && double.TryParse(parts[1], out var lat)) {
                            coordenadas.Add(new CoordenadasDTO {
                                Lat = lat,
                                Lng = lng,
                                Titulo = item.Nombre ?? "S/D",
                                Contenido = $@"
    <div style=""max-width:350px; font-family:Arial, sans-serif;"">
    <div style=""background-color:#1F618D; color:white; padding:8px 12px; border-radius:6px 6px 0 0; font-size:16px;"">
        📍 <strong>Zona{item.IdZona}</strong>
    </div>
    <div style=""padding:10px 12px; border:1px solid #ddd; border-top:0; border-radius:0 0 6px 6px; background:#fff;"">
        <p style=""margin:6px 0;""><strong>Nombre:</strong>{item.Nombre}</p>
        <p style=""margin:6px 0;""><strong>Capacidad:</strong> {item.Ocupados}/{item.CapacidadTotal}</p>

        <a href=""https://maps.google.com/maps?hl=en&ie=UTF8&spn=0.115896,0.239639&z=12&layer=c&cbll={lat},{lng}&cbp=12,0,,0,-4.1&view=map&ll={lat},{lng}""
           target=""_blank"" style=""color:#007bff; text-decoration:none;"">🔗 Ver más</a>
    </div>
    </div>",

                            Color = item.Ocupados >= item.CapacidadTotal ? "rojo": item.Ocupados >= item.CapacidadTotal / 2 ? "ambar" : "verde"

                            //VIGENTE - VENCIDO
                        });
                        }
                    }
                }

                StateHasChanged(); // 🔥 Este es clave para forzar re-render
            }
            catch(Exception ex) {
                Console.WriteLine($"Error cargando coordenadas: {ex.Message}");
                coordenadas = new();
            }
        }
        */
    }
}
