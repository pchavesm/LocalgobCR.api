import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/map';
@Injectable()

export class DerechosCementerioService {
  constructor(private http: Http) { }

  getDerechosCementerio() {
      let url = "https://www.perezzeledon.go.cr:8081/MPZ_API.asmx/DerechosCementerio";
      return this.http.get(url).map(res => {
          return res.json();
    });
  }
}
