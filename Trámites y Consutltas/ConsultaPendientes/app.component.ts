import { Component, ViewChild, OnInit } from '@angular/core';
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
  title = 'Consulta de pendientes';

  public error: number = 0;
  public infoCargada: number = 0;
  public datos: any[] = [];
  public nombreContribuyente: string;
  public cedulaContribuyente: string;
  public captchaResponse: boolean = false;
  public direccionIP: string = '';

  public consulta: any = {
    cedula: '',
    respuesta: ''
  }

  public pendienteAlCobro: any = {
    subtotal: 0,
    intereses: 0,
    multas: 0,
    descuento: 0,
    total: 0
  }

  public pendienteTotal: any = {
    subtotal: 0,
    intereses: 0,
    multas: 0,
    descuento: 0,
    total: 0
  }

  @ViewChild(ReCaptchaComponent) captcha: ReCaptchaComponent;

  constructor(private servicio: ConsultaServicios) { }

  ngOnInit() {
    this.infoCargada = -1;
    this.servicio.obtenerIP().subscribe(res => {
      this.direccionIP = res.ip;
      this.infoCargada = 0;
    }, error => {
      this.infoCargada = 0;
      this.error = 3;
    })
  }

  handleCorrectCaptcha = () => {
    this.captchaResponse = true;
    this.consulta.respuesta = this.captcha.getResponse();
  }

  consultarPendientes = () => {
    this.infoCargada = 1;
    this.servicio.existeContribuyente(this.consulta.cedula).subscribe(res => {
      if (res.Cedula != null && res.Nombre != null) {
        this.nombreContribuyente = res.Nombre;
        this.servicio.consultaPendientes(this.consulta.cedula, this.consulta.respuesta, this.direccionIP).subscribe(res => {
          this.datos = res;
          if (this.datos.length === 0) {
            this.error = 1;
          }
          else {
            this.nombreContribuyente = this.datos[0]['contribuyente'];
            this.cedulaContribuyente = this.datos[0]['cedula'];

            for (let item of this.datos) {
              if (item.estado === 'Pendiente al cobro' || item.estado === 'Pendiente vencido') {
                this.pendienteAlCobro.subtotal += Number(item.monto);
                this.pendienteAlCobro.intereses += Number(item.intereses);
                this.pendienteAlCobro.multas += Number(item.multa);
                this.pendienteAlCobro.descuento += Number(item.descuento);
              }
              this.pendienteTotal.subtotal += Number(item.monto);
              this.pendienteTotal.intereses += Number(item.intereses);
              this.pendienteTotal.multas += Number(item.multa);
              this.pendienteTotal.descuento += Number(item.descuento);
            }
            this.pendienteAlCobro.total = (this.pendienteAlCobro.subtotal + this.pendienteAlCobro.intereses + this.pendienteAlCobro.multas) - this.pendienteAlCobro.descuento;
            this.pendienteTotal.total = (this.pendienteTotal.subtotal + this.pendienteTotal.intereses + this.pendienteTotal.multas) - this.pendienteTotal.descuento;
            this.infoCargada = 2;
          }
        }, (error) => {
          console.log('error', error)
          this.error = 1;
        });
      }
      else {
        this.error = 2;
      }
    }, error => {
      this.error = 3;
    });
  }

  convertDate(date) {
    let dateArray = date.split("/");
    let newDate = dateArray[0] + "/" + dateArray[1] + "/" + dateArray[2].split(" ")[0];
    return newDate;
  }

  limpiar() {
    this.consulta.cedula = '';
    this.consulta.respuesta = '';
    this.error = 0;
    this.infoCargada = 0;
    this.datos = [];
    this.nombreContribuyente = '';
    this.cedulaContribuyente = '';
    this.captchaResponse = false;
  }

  imprimir = () => {
    window.print();
  }

  refrescar = () => {
    window.location.reload();
  }
}
