import { Component , ViewChild } from '@angular/core';
import { ConsultaServicios } from './services/consulta.service';
import { ReCaptchaComponent } from 'angular2-recaptcha';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styles: [`
    .ng-invalid.ng-touched:not(form) {
      border: 1px solid red;
    }
  `]
})
export class AppComponent {
  title = 'Consulta de boletas';

  public error:number = 0;
  public infoCargada:number = 0;
  public datos:any[] = [];
  public captchaResponse:boolean = false;
  public placaDeshabilitada = false;
  public boletaDeshabilitada = false;
  public tipoDato = 0;

  public consulta:any = {
    placa : '' ,
    boleta : '',
    tipo : 0, 
    respuesta : ''
  }

  @ViewChild(ReCaptchaComponent) captcha: ReCaptchaComponent;

  constructor(public servicio:ConsultaServicios){}

  consultarBoletas = () => {
    this.infoCargada = 1;
    this.tipoDato = this.consulta.placa === '' ? 2 : 1;
    this.servicio.obtenerIP().subscribe(res => {
      this.servicio.consultaParquimetros(this.tipoDato === 1 ? this.consulta.placa : this.consulta.boleta, res.ip, this.consulta.respuesta, this.tipoDato).subscribe(res => {
        this.datos = res;
        console.log(this.datos);
           
        if(this.datos.length === 0){
          this.error = 1;
        }
        else{
          this.error = 0;
          this.infoCargada = 2;
        }
      },error => {
        this.error = 1;
      })
    }, error => {
      this.error = 1;
    })
  }

  convertDate(date){
    let dateArray=date.split("/");
    let newDate = dateArray[0] + "/" + dateArray[1] + "/" + dateArray[2].split(" ")[0];
    return newDate;
  }

  handleCorrectCaptcha = () => {
    this.captchaResponse = true;
    this.consulta.respuesta = this.captcha.getResponse();
  }

  onFocusPlaca = () => {
    if(this.consulta.placa === ''){
      this.placaDeshabilitada = false;
      this.boletaDeshabilitada = true;
      this.consulta.placa = this.consulta.boleta;
      this.consulta.boleta = '';
    }
    this.boletaDeshabilitada = true;
  }

  onFocusBoleta = () => {
    if(this.consulta.boleta === ''){
      this.placaDeshabilitada = true;
      this.boletaDeshabilitada = false;
      this.consulta.boleta = this.consulta.placa;
      this.consulta.placa = '';
    }
    this.placaDeshabilitada = true;
  }

  limpiar = () => {
    this.error = 0;
    this.infoCargada = 0;
    this.datos = [];
    this.captchaResponse = false;
    this.placaDeshabilitada = false;
    this.boletaDeshabilitada = false;
    this.tipoDato = 0;
    this.consulta.placa = '' ;
    this.consulta.boleta = '';
    this.consulta.tipo = 0;
    this.consulta.respuesta = '';
  }

  imprimir = () => {
    window.print();
  }

  convertirMoneda = (monto) => {
    return monto.replace(',', '.');
  }

  prueba() {
    console.log('sgshsh');    
  }
}