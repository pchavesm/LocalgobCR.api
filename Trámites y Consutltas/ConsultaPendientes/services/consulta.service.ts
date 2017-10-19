import { Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
import 'rxjs/add/operator/map';
@Injectable()

export class ConsultaServicios {

  constructor(private http: Http) { }

  readonly url = 'https://www.perezzeledon.go.cr:8081/MPZ_API.asmx/';
  //readonly url = 'http://localhost:50937/MPZ_API.asmx/';

  consultaPendientes = (cedula, respuesta, ipCliente) => {
    var headers = new Headers();
    headers.append('Content-Type', 'application/x-www-form-urlencoded');
    var body = 'pCedula=' + cedula + '&ipCliente=' + ipCliente + '&respuesta=' + respuesta + '&tipoSolicitud=2';
    return this.http.post(this.url + 'consultaPendientes', body, { headers: headers }).map(res => {
      return res.json();
    });
  }

  existeContribuyente = (numCedula) => {
    var headers = new Headers();
    headers.append('Content-Type', 'application/x-www-form-urlencoded');
    let body = 'pCedula=' + numCedula;
    return this.http.post(this.url + 'existeContribuyente', body, { headers: headers }).map(res => {
      return res.json();
    }, error => {
      return error.json();
    })
  }

  obtenerIP() {
    return this.http.get('https://ipapi.co/json').map(res => {
      return res.json();
    });
  }

}
