import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/map';
@Injectable()

export class ServicioProyectos {
  constructor(private http: Http) { }

  getMapa() {
  		let url = "https://www.perezzeledon.go.cr:8081/MPZ_API.asmx/ObtenerUbicacionesActivas";
  		return this.http.get(url).map(res => {
  				return res.json();
    });
  }
  getCategorias() {
    let url = "https://www.perezzeledon.go.cr:8081/MPZ_API.asmx/ObtenerCategoriasComercio";
    return this.http.get(url).map(res => {
      return res.json();
    });
  }
}
