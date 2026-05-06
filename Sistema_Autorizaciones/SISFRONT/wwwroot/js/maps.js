let currentInfoWindow = null;
let selectedMarker = null; // 🔥 NUEVO: marcador único para selección manual
let markers = []; // ahora sí está definido

window.googleMaps = {

    initMap: function (dotNetHelper) {
        const center = { lat: -12.0207786, lng: -76.9092195 };
        window.map = new google.maps.Map(document.getElementById("my_map"), {
            center: center,
            zoom: 13,
            streetViewControl: false
        });

        // Si se pasó un helper (solo para mapa selector)
        if (dotNetHelper) {
            window.map.addListener("click", function (event) {
                const lat = event.latLng.lat();
                const lng = event.latLng.lng();
                dotNetHelper.invokeMethodAsync("OnMapClick", lat, lng);
            });
        }
    },

    // 🔁  múltiples marcadores
    addMarker: function (lat, lng, title, content,color) {
        let iconUrl = `/img/svg/${color}.svg`; // color = "verde" , "rojo" o "ambar"

        const marker = new google.maps.Marker({
            position: { lat: lat, lng: lng },
            map: window.map,
            title: title || "Ubicación",
            icon: {
                url: iconUrl,
                scaledSize: new google.maps.Size(32, 32) // opcional: controla tamaño
            },
            animation: google.maps.Animation.DROP
        });


        const infoWindow = new google.maps.InfoWindow({
            content: content || "Sin Contenido"
        });

        marker.addListener("click", function () {
            if (currentInfoWindow) {
                currentInfoWindow.close();
            }
            infoWindow.open(window.map, marker);
            currentInfoWindow = infoWindow;
        });

        markers.push(marker); // ✅ NECESARIO PARA PODER BORRARLOS LUEGO

    },

    clearMarkers: function () {
        console.log("Limpiando " + markers.length + " marcadores.");
        for (let i = 0; i < markers.length; i++) {
            markers[i].setMap(null);
        }
        markers = [];
    },


    // ✅ Para marcador único (selección en mapa)
    setSingleMarker: function (lat, lng, title, content) {
        let iconUrl = `/img/svg/rojo.svg`;
        // Limpiar marcador anterior
        if (selectedMarker) {
            selectedMarker.setMap(null);
        }

        selectedMarker = new google.maps.Marker({
            position: { lat: lat, lng: lng },
            map: window.map,
            title: title || "Ubicación seleccionada",
            icon: {
                url: iconUrl,
                scaledSize: new google.maps.Size(32, 32) // opcional: controla tamaño
            },
            animation: google.maps.Animation.DROP
        });

        const infoWindow = new google.maps.InfoWindow({
            content: content || "Ubicación seleccionada"
        });

        selectedMarker.addListener("click", function () {
            if (currentInfoWindow) {
                currentInfoWindow.close();
            }
            infoWindow.open(window.map, selectedMarker);
            currentInfoWindow = infoWindow;
        });

        // 🔄 Opcional: centrar el mapa en el marcador seleccionado
        window.map.setCenter({ lat: lat, lng: lng });
    }
};

