import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { MyDatePicker, IMyDpOptions, IMyDateModel } from 'mydatepicker';
import { ReCaptchaComponent } from 'angular2-recaptcha';
import { GroupByPipe } from 'ngx-pipes/src/app/pipes/array/group-by';
import { OrderByPipe } from 'ngx-pipes/src/app/pipes/array/order-by';
import { ConsultaServicios } from './services/consulta.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styles: [`
    .ng-invalid.ng-touched:not(form) {
      border: 1px solid red;
    }
  `]
})
export class AppComponent implements OnInit {
  title = 'Consulta de pagos';

  private today = new Date();
  public clienteIP: string;
  public respuestaCaptcha: String;
  public datos: any[] = [];
  public infoCargada = 0;
  public btnDisabled = true;
  public error = 0;
  public vista = 1;
  public nombreContribuyente: string;
  public cedulaContribuyente: string;
  public datosAgrupados: any[] = [];
  public infoDesplegada = true;
  public accionBoton: string = 'Desagrupar';
  public direccionIP: string = '';
  public captchaResponse: boolean = false;

  public consulta: any = {
    cedula: '',
    fecInicio: { date: { year: this.today.getFullYear() - 1, month: this.today.getMonth() + 1, day: this.today.getDate() } },
    fecFin: { date: { year: this.today.getFullYear(), month: this.today.getMonth() + 1, day: this.today.getDate() } }
  }

  private datePickerOptions: IMyDpOptions = {
    dateFormat: 'dd/mm/yyyy',
    todayBtnTxt: 'Hoy',
    monthLabels: {
      1: 'Ene',
      2: 'Feb',
      3: 'Mar',
      4: 'Abr',
      5: 'May',
      6: 'Jun',
      7: 'Jul',
      8: 'Ago',
      9: 'Set',
      10: 'Oct',
      11: 'Nov',
      12: 'Dic'
    },
    dayLabels: {
      mo: 'Lun',
      tu: 'Mar',
      we: 'Mie',
      th: 'Jue',
      fr: 'Vie',
      sa: 'Sáb',
      su: 'Dom'
    },
    firstDayOfWeek: "mo",
    disableSince: { year: this.today.getFullYear(), month: this.today.getMonth() + 1, day: this.today.getDate() + 1 }
  };

  @ViewChild(ReCaptchaComponent) captcha: ReCaptchaComponent;

  constructor(private servicio: ConsultaServicios, private groupByPipe: GroupByPipe) { }

  ngOnInit() {
    this.infoCargada = -1;
    this.servicio.obtenerIP().subscribe(res => {
      this.direccionIP = res.ip;
      this.infoCargada = 0;
    }, error => {
      this.infoCargada = 0;
      this.error = 3;
    });
  }

  handleCorrectCaptcha = () => {
    this.captchaResponse = true;
    this.respuestaCaptcha = this.captcha.getResponse();
  }

  consultarPago = () => {
    this.infoCargada = 1;
    this.servicio.consultaPagosCancelados(this.consulta.cedula, this.consulta.fecInicio, this.consulta.fecFin, this.respuestaCaptcha, this.direccionIP).subscribe(res => {
      this.datos = res;
      if (this.datos.length === 0) {
        this.error = 1;
      }
      else {
        this.nombreContribuyente = this.datos[0]['nom_contribuyente'];
        this.cedulaContribuyente = this.datos[0]['ced_contribuyente'];
        this.infoCargada = 2;
        this.agruparDatos();
      }
    }, error => {
      this.error = 3;
    });
  }

  agruparDatos = () => {
    let tempDatos = this.groupByPipe.transform(this.datos, 'num_recibo');
    for (let linea in tempDatos) {
      this.datosAgrupados.push(tempDatos[linea][0]);
    }
  }

  convertDate(date) {
    let dateArray = date.split("/");
    let newDate = dateArray[0] + "/" + dateArray[1] + "/" + dateArray[2].split(" ")[0];
    return newDate;
  }

  limpiar() {
    this.captchaResponse = false;
    this.consulta.cedula = '';
    this.infoCargada = 0;
    this.btnDisabled = true;
    this.error = 0;
    this.vista = 1;
    this.infoDesplegada = true;
    this.consulta.fecInicio = { date: { year: this.today.getFullYear() - 1, month: this.today.getMonth() + 1, day: this.today.getDate() } };
    this.consulta.fecFin = { date: { year: this.today.getFullYear(), month: this.today.getMonth() + 1, day: this.today.getDate() } };
  }

  cedulaHandler = () => {
    if (this.consulta.cedula.length >= 4) {
      this.btnDisabled = false;
    }
    else {
      this.btnDisabled = true;
    }
  }

  cambiarVista = () => {
    this.infoDesplegada = !this.infoDesplegada;
    this.accionBoton = this.accionBoton === 'Desagrupar' ? 'Agrupar' : 'Desagrupar';
  }

  calculaTotal = (numRecibo) => {
    let calculo = 0;
    for (let item of this.datos) {
      if (item.num_recibo === numRecibo) {
        calculo += item.monto;
      }
    }
    return calculo;
  }

  imprimir = () => {
    window.print();
  }

  refrescar = () => {
    window.location.reload();
  }
}
