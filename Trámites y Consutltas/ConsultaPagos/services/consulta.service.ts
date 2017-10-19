import { Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
import 'rxjs/add/operator/map';
@Injectable()

export class ConsultaServicios {

  constructor(private http: Http) { }

  readonly url = 'https://www.perezzeledon.go.cr:8081/MPZ_API.asmx';

  consultaPagosCancelados = (cedulaRecibo, fecInicio, fecFinal, respuesta, ipCliente) => {
    var headers = new Headers();
    headers.append('Content-Type', 'application/x-www-form-urlencoded');
  		var body = 'cedula=' + cedulaRecibo + '&fechaInicio=' + fecInicio.date.day + '/' + fecInicio.date.month + '/' + fecInicio.date.year + '&fechaFin=' + fecFinal.date.day + '/' + fecFinal.date.month + '/' + fecFinal.date.year + '&ipCliente=' + ipCliente + '&respuesta=' + respuesta + '&tipoSolicitud=2';
  		return this.http.post(this.url + '/consultaCancelado', body, { headers: headers }).map(res => {
        return res.json();
      });
  }

  obtenerIP(){
    return this.http.get('https://ipapi.co/json').map(res => {
      return res.json();
    });
  }

}
