import { Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
import 'rxjs/add/operator/map';
@Injectable()

export class SolicitudCertificacionService {
  readonly url = "https://www.perezzeledon.go.cr:8081/MPZ_API.asmx/";

  constructor(private http: Http) {  }

  solicitarCertificacion = (numCedula, ipCliente, correoContribuyente, telefonoContribuyente, tipoSolicitud) => {
    var headers = new Headers();
    headers.append('Content-Type', 'application/x-www-form-urlencoded');
    correoContribuyente = correoContribuyente === '' ? 'N' : correoContribuyente;
    telefonoContribuyente = telefonoContribuyente === '' ? 'N' : telefonoContribuyente;
    let body = 'numCedula=' + numCedula + '&ipCliente=' + ipCliente + '&correoContribuyente=' + correoContribuyente + '&telefonoContribuyente=' + telefonoContribuyente + '&tipoSolicitud=' + tipoSolicitud;
    return this.http.post(this.url + 'SolicitudCertificacion', body, {headers: headers}).map(res => {
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
