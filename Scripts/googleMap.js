function initMap() {
    var center = {
        lat: 35.716048,   //経度
        lng: 139.695019   //緯度
    }
    var map = new google.maps.Map(document.getElementById("map"), {
        center: center,
        zoom: 17,
        styles: [
            {
                "featureType": "administrative.land_parcel",
                "elementType": "labels",
                "stylers": [
                    {
                        "visibility": "off"
                    }
                ]
            },
            {
                "featureType": "landscape.man_made",
                "stylers": [
                    {
                        "visibility": "off"
                    }
                ]
            },
            {
                "featureType": "poi",
                "elementType": "labels.text",
                "stylers": [
                    {
                        "visibility": "off"
                    }
                ]
            },
            {
                "featureType": "poi.business",
                "stylers": [
                    {
                        "visibility": "off"
                    }
                ]
            },
            {
                "featureType": "poi.school",
                "stylers": [
                    {
                        "visibility": "off"
                    }
                ]
            },
            {
                "featureType": "poi.medical",
                "stylers": [
                    {
                        "visibility": "off"
                    }
                ]
            },
            {
                "featureType": "road",
                "elementType": "labels.icon",
                "stylers": [
                    {
                        "visibility": "off"
                    }
                ]
            },
            {
                "featureType": "road.local",
                "elementType": "labels",
                "stylers": [
                    {
                        "visibility": "off"
                    }
                ]
            },
        ]
    });
    var marker = new google.maps.Marker({ // マーカーの追加
        position: center, // マーカーを立てる位置を指定
        map: map // マーカーを立てる地図を指定
    });
    var infoWindow = new google.maps.InfoWindow({ // 吹き出しの追加
        content: '<div class="sample">西武新宿線<br>下落合駅から徒歩５分</div>' // 吹き出しに表示する内容
    });
    marker.addListener('click', function () { // マーカーをクリックしたとき
        infoWindow.open(map, marker); // 吹き出しの表示
    });
}
