import { Component, OnInit } from '@angular/core';
import { DerechosCementerioService } from './services/app.service';
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

  constructor (private limpiezaViasService : DerechosCementerioService){ }
  ngOnInit(){
    this.infoCargada = 1;
    this.cargaInformacion();
  }

  cargaInformacion = () => {
    this.limpiezaViasService.getDerechosCementerio().subscribe(res => {
      this.datos = res;
      this.infoCargada = 2;
    }, error => {
    });
  }
}
