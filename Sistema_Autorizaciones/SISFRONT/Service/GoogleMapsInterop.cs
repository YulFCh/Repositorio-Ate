using Microsoft.JSInterop;

namespace SISFRONT.Service
{
    public class GoogleMapsInterop
    {
        private readonly IJSRuntime _js;

        public GoogleMapsInterop(IJSRuntime js)
        {
            _js = js;
        }

        public async Task InitMap()
        {
            await _js.InvokeVoidAsync("googleMaps.initMap", (DotNetObjectReference<object>)null);
        }

        public async Task ClearMarkers() {
            await _js.InvokeVoidAsync("googleMaps.clearMarkers");
        }


        public async Task AddMarker(double lat, double lng, string title, string content,string color) {
            await _js.InvokeVoidAsync("googleMaps.addMarker", lat, lng, title, content,color);
        }

        public async Task SetSingleMarker(double lat, double lng, string title, string content) {
            await _js.InvokeVoidAsync("googleMaps.setSingleMarker", lat, lng, title, content);
        }


    }
}
