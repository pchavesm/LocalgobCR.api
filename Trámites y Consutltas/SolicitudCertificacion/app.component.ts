import { Component } from '@angular/core';
import {NgForm} from '@angular/forms';
import { SolicitudCertificacionService } from '../services/app.service';
import {SelectModule} from 'ng2-select';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styles: [`
      .ng-invalid.ng-touched:not(form) {
        border: 1px solid red;
      }
    `
  ]
})
export class AppComponent {
  title = 'app';

  constructor(private service:SolicitudCertificacionService) { }

  public infoFormulario:any = {
    cedula : null,
    correo : '',
    telefono : ''
  }
  public recaptchaResponse:boolean = false;
  public infoCargada:number = 0;
  public error:number = 0;
  public tipoSolicitud:string = 'Default';

  handleCorrectCaptcha = () => {
    this.recaptchaResponse = true;
  }

  tramitarCertificacion = () => {
    this.infoCargada = 1;
    this.service.obtenerIP().subscribe( res => {
      this.service.solicitarCertificacion(this.infoFormulario.cedula, res.ip, this.infoFormulario.correo, this.infoFormulario.telefono, this.tipoSolicitud).subscribe(res2 =>{
        if (res2.mensaje === 'Realizada'){
          this.infoCargada = 2;
          this.error = 0;
        }
        else if(res2.mensaje === 'Inexistente'){
          this.infoCargada = 2;
          this.error = 1;
        }
        else if(res2.mensaje === 'Moroso'){
          this.infoCargada = 2;
          this.error = 2;
        }
      })
    })
  }

  limpiar = () => {
    this.infoFormulario.cedula = null;
    this.infoFormulario.correo = '';
    this.infoFormulario.telefono = '';
    this.recaptchaResponse = false;
    this.infoCargada = 0;
    this.error = 0;
    this.tipoSolicitud = 'Default';
  }

}
