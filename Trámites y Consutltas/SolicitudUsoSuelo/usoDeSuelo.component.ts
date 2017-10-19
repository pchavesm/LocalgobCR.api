import { Component, OnInit, ViewChild } from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {NgForm} from '@angular/forms';
import { SubirArchivosService } from '../../services/subirArchivos.service';
import {ReCaptchaComponent} from 'angular2-recaptcha';
import { CompleterService, CompleterData, CompleterItem } from 'ng2-completer';

@Component({
  selector: 'app-usodesuelo',
  templateUrl: './usoDeSuelo.component.html',
  styles: [`
      .ng-invalid.ng-touched:not(form) {
        border: 1px solid red;
      }
    `
  ]
})
export class UsoDeSueloComponent {
  constructor(private _activatedRoute:ActivatedRoute, private service:SubirArchivosService, private completerService: CompleterService) {
    this.service.obtenerTipoPatentes().subscribe(res => {
      this.searchData = res;
      this.dataService = completerService.local(this.searchData, 'descripcion', 'descripcion');
      this.desplegarFormulario = true;
    });
  }

  public infoFormulario:any = {
    cedula : '',
    tipoPatente : '',
    correo : '',
    telefono : ''
  }
  private base64textString:string='';
  private extension = '';
  private error:number = 0;
  private nombreArchivo:string;
  private pendientes:boolean = true;
  private infoCargada:number = 0;
  private paquetesEntregados:number[] = [];
  public progresoCarga:number;
  public cargandoArchivo:boolean = false;
  public recaptchaResponse:boolean = false;
  public archivoCargado:boolean = false;
  public nomContribuyente:string = '';
  protected searchStr: string;
  protected dataService: CompleterData;
  public tipoPatenteSeleccionada:boolean = false;
  protected searchData = [];
  public desplegarFormulario:boolean = false;
  public respuestaCaptcha:String;

  @ViewChild(ReCaptchaComponent) captcha: ReCaptchaComponent;

  handleFileSelected(evt){
    let files = evt.target.files;
    let file = files[0];
    this.nombreArchivo = file.name;
    let partesArchivo = file.name.split('.');
    this.extension = partesArchivo[partesArchivo.length-1];

    if(file.size <= 3048000){
      if(files && file && (this.extension === 'png' || this.extension === 'pdf' || this.extension === 'jpg')){
        this.error = 0;
        let reader = new FileReader();
        reader.onload = this._handleReaderLoaded.bind(this);
        reader.readAsBinaryString(file);
      }
      else{
        this.error = 1;
      }
    }
    else{
      this.error = 2;
    }
    if(this.infoFormulario.tipoPatente.length <= 0) {
      this.error = 5;
    }
  }

  _handleReaderLoaded = (readerEvt) => {
    let binaryString = readerEvt.target.result;
    this.base64textString = btoa(binaryString);
    this.dividirString(this.base64textString);
  }

  dividirString = (cadenaEntrada) => {
    let stringRetorno:string[] = [];
    let stringTemporal = '';
    for(let i=0; i < cadenaEntrada.length; i++)
    {
      stringTemporal += cadenaEntrada[i];
      if(stringTemporal.length === 20000 || i === cadenaEntrada.length -1){
        stringRetorno.push(stringTemporal);
        stringTemporal = '';
      }
    }
    this.subirArchivo(stringRetorno);
  }

  subirArchivo = (stringRetorno) => {
    this.cargandoArchivo = true;
    let nombreArchivo:string = Math.random().toString(36).substr(2,9);
    for(let i=0; i < stringRetorno.length; i++){
      this.service.registrarUsoDeSuelo(stringRetorno[i], i+1, nombreArchivo, stringRetorno.length, this.extension, this.infoFormulario.cedula, this.nomContribuyente, this.infoFormulario.tipoPatente, this.infoFormulario.correo, this.infoFormulario.telefono).subscribe( res => {
        if(res.mensaje === 'Error'){
          this.service.registrarUsoDeSuelo(stringRetorno[i], i+1, nombreArchivo, stringRetorno.length, this.extension, this.infoFormulario.Cedula, this.nomContribuyente, this.infoFormulario.tipoPatente, this.infoFormulario.correo, this.infoFormulario.telefono).subscribe( res2 => {
            
          })
        }
        if(res.mensaje === 'Correcto'){
          this.insertarElementoCorrecto(i+1, stringRetorno.length);
        }
      });
    }
  }

  actualizarProgresoCarga = (cantPartes) => {
    return parseInt(((this.paquetesEntregados.length * 100) / cantPartes).toString());
  }

  insertarElementoCorrecto = (numParte, cantPartes) => {
    console.log(this.paquetesEntregados.indexOf(numParte));
    if(this.paquetesEntregados.indexOf(numParte) === -1) {
      this.paquetesEntregados.push(numParte);
      this.progresoCarga = this.actualizarProgresoCarga(cantPartes);
    }
    if(this.paquetesEntregados.length === cantPartes){
      this.cargandoArchivo = false;
      this.archivoCargado = true;
    }
  }

  tienePendientes = () => {
    this.infoCargada = 1;
    this.service.existeContribuyente(this.infoFormulario.cedula).subscribe(res => {
      if(res.Cedula != null && res.Nombre != null) {
        this.nomContribuyente = res.Nombre;
        this.service.tienePendientes(res.Cedula, this.respuestaCaptcha).subscribe(res => {
          this.pendientes = res.estado;
          this.error = this.pendientes ? 3 : 0;
          this.infoCargada = 2;
        }, error =>{
        });
      }
      else {
        this.error = 4;
        this.infoCargada = 2;
      }
    });
  }

  handleCorrectCaptcha = () => {
    this.recaptchaResponse = true;
    this.respuestaCaptcha = this.captcha.getResponse();
  }

  refrescarPagina = () => {
    window.location.reload();
  }

  limpiar = () => {
    this.base64textString = '';
    this.extension = '';
    this.error = 0;
    this.nombreArchivo = '';
    this.pendientes = true;
    this.infoCargada = 0;
    this.progresoCarga = 0;
    this.paquetesEntregados = [];
    this.cargandoArchivo = false;
    this.recaptchaResponse = false;
    this.archivoCargado = false;
    this.nomContribuyente = '';
    this.searchStr = '';
    this.infoFormulario.cedula = null;
    this.infoFormulario.correo = '';
    this.infoFormulario.telefono = '';
    this.infoFormulario.tipoPatente = '';
  }

  tipoPatenteHandler(item: CompleterItem) {
    this.infoFormulario.tipoPatente = item ? item.title : "";
    this.tipoPatenteSeleccionada = true;
  }

  limpiarTipoPatente(){
    this.tipoPatenteSeleccionada = false;
    this.infoFormulario.tipoPatente = '';
    this.searchStr = '';
  }
}
