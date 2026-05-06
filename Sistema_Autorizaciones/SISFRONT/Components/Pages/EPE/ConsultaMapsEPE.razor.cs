using Microsoft.JSInterop;
using SistemaLicencias.SHARED.DTOs;

namespace SISFRONT.Components.Pages.EPE {
    public partial class ConsultaMapsEPE {

        // private string _baseUrl = "http://192.168.0.139:148";
        private string _baseUrl = "http://localhost:5297";
        //private string _baseUrl = "http://192.168.4.225:181";

        private bool _buscado = false;
        private List<consultaCoordenadas> coordAnuncios = new();

        private List<SubZonasAutApi> subzonas = new();

        private string color = "";


        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if(firstRender) {
                var dotNetRef = DotNetObjectReference.Create(this);
                await JS.InvokeVoidAsync("googleMaps.initMap", dotNetRef);
            }
        }
        protected override async Task OnInitializedAsync() {
            await BuscarAnuncios();
        }

        private async Task BuscarAnuncios() {


            await MapsInterop.ClearMarkers();

            //await MapsInterop.ClearMarkers();          

            var rs = await Http.GetFromJsonAsync<List<consultaCoordenadas>>($"{_baseUrl}/api/Consultas/coordepe");

            coordAnuncios = rs;

            _buscado = true;

            foreach(var anuncio in coordAnuncios) {
                if(!string.IsNullOrWhiteSpace(anuncio.coordLngLat)) {
                    var partes = anuncio.coordLngLat.Split(',');

                    Console.WriteLine(partes[0]);
                    Console.WriteLine(partes[1]);

                    if(partes.Length >= 2 &&
                        double.TryParse(partes[0], out double lat) &&
                        double.TryParse(partes[1], out double lng)) {

                        string titulo = "Nombre Anuncio";

                        //string contenido = $"<b>subzona.Nombre</b><br>Disponibilidad: subzona.Ocupados/subzona.CapacidadTotal";
                        string contenido = $"<b>{anuncio.coordLngLat}</b>";

                        color = "verde";

                        //await JS.InvokeVoidAsync("googleMaps.addMarker", lat, lng, subzona.Nombre, contenido, color);
                        await MapsInterop.AddMarker(lat, lng, titulo, contenido, color);
                    }
                }
            }


            /*
            var res = await Http.GetFromJsonAsync<SubZonasAutDTO>($"{_baseUrl}/api/MarkMaps/getall/2");
            subzonas = res.response;

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
            */

        }



    }
}
