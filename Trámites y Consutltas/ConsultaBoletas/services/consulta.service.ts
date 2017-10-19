import { Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
import 'rxjs/add/operator/map';
@Injectable()

export class ConsultaServicios {

    constructor(private http: Http) { }

    readonly url = 'https://www.perezzeledon.go.cr:8081/MPZ_API.asmx/';
    //readonly url = 'http://localhost:50937/MPZ_API.asmx/';


    consultaParquimetros = (boletaPlaca, ipCliente, respuesta, tipoDato) => {
        var headers = new Headers();
        headers.append('Content-Type', 'application/x-www-form-urlencoded');
        var body = 'boleta_placa=' + boletaPlaca + '&ipCliente=' + ipCliente + '&respuesta=' + respuesta + '&tipoSolicitud=2' + '&tipoDato=' + tipoDato;
        return this.http.post(this.url + 'ObtenerInfracciones', body, { headers: headers }).map(res => {
            return res.json();
        });
    }

    obtenerIP() {
        return this.http.get('https://ipv4.myexternalip.com/json').map(res => {
            return res.json();
        });
    }

}