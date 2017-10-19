import { Component, OnInit } from '@angular/core';
import { LimpiezaViasService } from './services/app.services';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'app';
  public datos : any[] = [];
  public infoCargada = 0;

  constructor (private limpiezaViasService : LimpiezaViasService){ }
  ngOnInit(){
    this.infoCargada = 1;
    this.cargaInformacion();
  }

  cargaInformacion = () => {
    this.limpiezaViasService.getInfoLimpieza().subscribe(res => {
      this.datos = res;
      this.infoCargada = 2;
    }, error => {

    });
  }
}
