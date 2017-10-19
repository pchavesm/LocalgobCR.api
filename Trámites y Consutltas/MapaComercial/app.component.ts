import { Component, ViewChild, ElementRef } from '@angular/core';
import {ServicioProyectos} from './services/consulta.service';
import {ServicioCoordenadas} from './services/servicioCoordenadas.service';
import * as MarkerClusterer from 'node-js-marker-clusterer';
import {GroupByPipe} from 'ngx-pipes/src/app/pipes/array/group-by';
import {FilterByPipe} from 'ngx-pipes/src/app/pipes/array/filter-by';
import {OrderByPipe} from 'ngx-pipes/src/app/pipes/array/order-by';
declare var google;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  public nombre: string;
  private lat: number = 9.373194;
  private lng: number = -83.702783;
  private zoom: number = 11;
  private map: any;
  private datos: any[] = [];
  private categorias: any;
  private markers: any[] = [];
  private lastInfoW: any;
  private dropdownList = [];
  private selectedItems = [];
  private dropdownSettings = {};
  private markerCluster: any;
  private categoriasSeleccionadas: any[] = [];
  public pagCargada = false;

  @ViewChild('map') mapElement: ElementRef;

  constructor(
    private service: ServicioProyectos, private groupByPipe: GroupByPipe, private serviceCoo: ServicioCoordenadas, private filterByPipe: FilterByPipe, private orderByPipe: OrderByPipe
  ) {

  }

  ngOnInit() {
    this.service.getMapa().subscribe(res => {
      this.datos = res;
      this.markers = res;
      this.loadMap();
      this.cantonPolygon();
      this.pagCargada = true;
      //this.actualizarMarcadores();
    }, error => {
    });
    this.service.getCategorias().subscribe(res => {
      this.categorias = res;
      for (let categoria of this.categorias) {
        this.dropdownList.push({
          'id': categoria.idCategoria,
          'itemName': categoria.nombreCategoria
        })
      }
    }, error => {
    });

    this.selectedItems = [
    ];
    this.dropdownSettings = {
      singleSelection: false,
      text: "Categorías",
      selectAllText: 'Seleccionar todas',
      unSelectAllText: 'Seleccionar ninguna',
      enableSearchFilter: true,
      classes: "myclass custom-class categorias"
    };

  }

  loadMap() {
    this.markers = [];
    let latLng = new google.maps.LatLng(this.lat, this.lng);
    latLng = new google.maps.LatLng(this.lat, this.lng);
    let mapOptions = {
      center: latLng,
      zoom: this.zoom,
      mapTypeId: google.maps.MapTypeId.ROADMAP,
      fullscreenControl: false,
      streetViewControl: false,
      styles: [
        {
          "featureType": "poi",
          "stylers": [
            { "visibility": "off" }
          ]
        }
      ],
      zoomControl: false
    }


    this.map = new google.maps.Map(this.mapElement.nativeElement, mapOptions);

    let time = 0;
    let filteredData = this.groupByPipe.transform(this.orderByPipe.transform(this.datos, 'idCategoria'), 'nombre');
    for (let marcador in filteredData) {
      this.marker(filteredData[marcador][filteredData[marcador].length - 1], time);
      time += 20;
    }

    this.markerCluster = new MarkerClusterer(this.map, this.markers,
      { imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });

  /*  this.map.addListener('zoom_changed', () => {
      if (this.map.getZoom() >= 18) {
        this.markerCluster.clearMarkers();
      }
    })*/
    this.map.setZoom(this.zoom - 1);
    this.map.setZoom(this.zoom);
  }


  limpiarMapa() {
    this.markerCluster.clearMarkers();
    for (let marker of this.markers) {
      marker.setMap(null);
    }
    this.markers = [];
  }


  onItemSelect(item: any) {
    this.nombre = '';
    let time = 0;
    let index = 0;
    let filteredData: any[] = [];
    this.limpiarMapa();


    this.categoriasSeleccionadas.push(item.id);

    for (let categoria of this.categoriasSeleccionadas) {
      index = 0;
      filteredData = filteredData.concat(this.filterByPipe.transform(this.datos, ['idCategoria'], categoria));
    }
    filteredData = this.groupByPipe.transform(this.orderByPipe.transform(filteredData, 'idCategoria'), 'nombre');

    for (let marcador in filteredData) {
      // this.markers.push(marcador);
      this.marker(filteredData[marcador][filteredData[marcador].length - 1], time);
      //this.marker(filteredData[marcador][0], time);
      time += 20;
      index++;
    }
  //  this.actualizarMarcadores();

      this.markerCluster = new MarkerClusterer(this.map, this.markers,
        { imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });

    this.map.setZoom(this.zoom - 1);
    this.map.setZoom(this.zoom);
  }

  OnItemDeSelect(item: any) {
    let time = 0;
    let index = 0;
    let filteredData: any[] = [];
    for (let categoria of this.categoriasSeleccionadas) {
      if (item.id === categoria) {
        this.categoriasSeleccionadas.splice(index, 1);
      }
      index++;
    }
    this.limpiarMapa();


    for (let categoria of this.categoriasSeleccionadas) {
      filteredData = filteredData.concat(this.filterByPipe.transform(this.datos, ['idCategoria'], categoria));
    }
    filteredData = this.groupByPipe.transform(this.orderByPipe.transform(filteredData, 'idCategoria'), 'nombre');
    for (let marcador in filteredData) {
      //this.markers.push(marcador);
      this.marker(filteredData[marcador][filteredData[marcador].length - 1], time);
      time += 20;
    }
      this.markerCluster = new MarkerClusterer(this.map, this.markers,
        { imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });
  //  this.actualizarMarcadores();
    this.map.setZoom(this.zoom - 1);
    this.map.setZoom(this.zoom);
  }
  onSelectAll(items: any) {
    this.limpiarMapa();
    this.loadMap();
  }
  onDeSelectAll(items: any) {
    this.limpiarMapa();
    this.loadMap();
  }

  filtrar() {
    this.selectedItems = [];
    let time = 0;
    this.limpiarMapa()
    let filteredData = this.groupByPipe.transform(this.orderByPipe.transform(this.filterByPipe.transform(this.datos, ['nombre'], this.nombre), 'idCategoria'), 'nombre');
    for (let marcador in filteredData) {
      //this.markers.push(marcador);
      this.marker(filteredData[marcador][filteredData[marcador].length - 1], time);
      time += 20;
    }
    //this.actualizarMarcadores();

    this.markerCluster = new MarkerClusterer(this.map, this.markers,
      { imagePath: 'https://developers.google.com/maps/documentation/javascript/examples/markerclusterer/m' });

    this.map.setZoom(this.zoom - 1);
    this.map.setZoom(this.zoom);
  }

  cantonPolygon() {
    var pzCoords = this.serviceCoo.getCoordenadasCanton();
    let listaPuntosCanton: any[] = pzCoords.split(' ');
    let listaPuntosCantonProcesada: any[] = [];
    for (let parPuntos of listaPuntosCanton) {
      let temp = parPuntos.split(',');
      listaPuntosCantonProcesada.push({ lat: parseFloat(temp[1]), lng: parseFloat(temp[0]) });
    }
    var polygonCanton = new google.maps.Polygon({
      paths: listaPuntosCantonProcesada,
      strokeColor: '#008FD5',
      strokeOpacity: 0.5,
      strokeWeight: 2,
      fillColor: '#008FD5',
      fillOpacity: 0.1
    });
    polygonCanton.setMap(this.map);
  }

/*  actualizarMarcadores() {
    let time = 0;
    for (let marcador of this.markers) {
      let latLng = new google.maps.LatLng(marcador.latitud, marcador.longitud);
      let marker = new google.maps.Marker({
        animation: google.maps.Animation.DROP,
        position: latLng,
        icon: '../assets/' + marcador.categoria
      });
      this.markers.push(marcador);
      setTimeout(() => marker.setMap(this.map), time);
      let content = "<strong>" + marcador.nombre + "</strong>" + "<p> <b>Latitud: </b>" + marcador.latitud + "</p><p><b>Longitud: </b>" + marcador.longitud + "</p>" + " <b>Descripción: </b>" + marcador.descripcion + "</p>" + "<a href=\"index.php/canton/directorio-cantonal/directorio-comercial/item/" + marcador.alias + ".html\" target=\"_blank\">Más información</a>";
      this.addInfoWindow(marker, content);
    }
  }*/





  marker(marcador, time) {
    let latLng = new google.maps.LatLng(Number(marcador.latitud), Number(marcador.longitud));
    let marker = new google.maps.Marker({
      animation: google.maps.Animation.DROP,
      position: latLng,
      icon: '../assets/' + marcador.categoria
    });

    this.markers.push(marker);
    setTimeout(() => marker.setMap(this.map), time);
    let content = "<h4>" + marcador.nombre + "</h4>"+ "<img src=\"../assets/bac.JPG\" class=\"img-stretch\"/>" + "</p>" + " <strong>Descripción: </strong>" + marcador.descripcion + "</p>" + "<a href=\"index.php/canton/directorio-cantonal/directorio-comercial/item/" + marcador.alias + ".html\" target=\"_blank\">Más información</a>";

    this.addInfoWindow(marker, content);
  }




  addInfoWindow(marker, content) {

    let infoWindow = new google.maps.InfoWindow({
      content: content
    });

    google.maps.event.addListener(marker, 'click', () => {
      if (typeof (this.lastInfoW) !== 'undefined') {
        this.lastInfoW.close();
      }
      infoWindow.open(this.map, marker);
      this.lastInfoW = infoWindow;
    });
    google.maps.event.addListener(infoWindow, 'closeclick', () => {
      this.lastInfoW = undefined;
    });
  }
}
