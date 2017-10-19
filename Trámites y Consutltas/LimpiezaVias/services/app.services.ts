import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/map';
@Injectable()

export class LimpiezaViasService {
  constructor(private http: Http) { }

  getInfoLimpieza() {
      let url = "https://www.perezzeledon.go.cr:8081/MPZ_API.asmx/LimpiezaVias";
      return this.http.get(url).map(res => {
          return res.json();
    });
  }
}
