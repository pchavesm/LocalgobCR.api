import { Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
import 'rxjs/add/operator/map';
@Injectable()

export class SubirArchivosService {
  readonly url = "https://www.perezzeledon.go.cr:8081/MPZ_API.asmx/";
  private respuesta:string = '';
  //readonly url = "http://localhost:50937/MPZ_API.asmx/";

  constructor(private http: Http) { }

  registrarUsoDeSuelo = (parteArchivo, numParte, nombreArchivo, cantPartes, pFormato, numCedula, nomContribuyente, tipoPatente, correoContribuyente, telefonoContribuyente) => {
      var headers = new Headers();
      headers.append('Content-Type', 'application/x-www-form-urlencoded');
      telefonoContribuyente = telefonoContribuyente === '' ? 'N' : telefonoContribuyente;
      correoContribuyente = correoContribuyente === '' ? 'N' : correoContribuyente;
      let body = 'pCantPartes=' + cantPartes.toString() + '&pNumParte=' + numParte.toString() + '&pNomArchivo=' + nombreArchivo + '&pData=' + parteArchivo + '&pFormato=' + pFormato + '&numCedula=' + numCedula + '&tipoSolicitud=2' + '&nomContribuyente=' + nomContribuyente + '&tipoPatente=' + tipoPatente + '&correoContribuyente=' + correoContribuyente + '&telefonoContribuyente=' +  telefonoContribuyente;
      return this.http.post(this.url + 'guardarArchivo', body, { headers: headers }).map(res => {
        return res.json();
      }, error => {
        return error.json();
      })
  }

  tienePendientes = (numCedula, respuesta) => {
    var headers = new Headers();
    headers.append('Content-Type', 'application/x-www-form-urlencoded');
    let body = 'pCedula=' + numCedula + '&respuesta=' + respuesta + '&tipoSolicitud=2';
    return this.http.post(this.url + 'TienePendientes', body, {headers: headers}).map(res => {
      return res.json();
    }, error => {
      return error.json();
    })
  }

  existeContribuyente = (numCedula) => {
    var headers = new Headers();
    headers.append('Content-Type', 'application/x-www-form-urlencoded');
    let body = 'pCedula=' + numCedula;
    return this.http.post(this.url + 'existeContribuyente', body, {headers: headers}).map(res => {
      return res.json();
    }, error => {
      return error.json();
    })
  }

  obtenerTipoPatentes = () => {
    var headers = new Headers();
    let body = '';
    headers.append('Content-Type', 'application/x-www-form-urlencoded');
    return this.http.post(this.url + 'listarTipoPatentes', body, {headers: headers}).map(res => {
      return res.json();
    }, error => {
      return error.json();
    })
  }

  obtenerIP = () => {
    return this.http.get('https://ipv4.myexternalip.com/json').map(res => {
      return res.json();
    });
  }
}
