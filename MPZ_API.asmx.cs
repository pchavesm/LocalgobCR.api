using MPZ_API.Clases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Services;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using PdfSharp.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PdfSharp;

namespace MPZ_API
{
    /// <summary>
    /// Summary description for MPZ_API
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [EnableCors("*","*","*")]
    //[EnableCors(origins: "http://https://www.perezzeledon.go.cr,http://.perezzeledon.go.cr", headers:"*", methods:"*")]
    //[EnableCors(origins: "http://https://www.perezzeledon.go.cr", headers:"*", methods:"*")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MPZ_API : System.Web.Services.WebService
    {
        #region Métodos Web

        /// <summary>
        /// Método para obtener los cobros cancelados. Recibe el número de cédula del contribuyente o el número de recibo; además, recibe un rango de fechas
        /// de inicio y de fin para establecer entre cuáles fechas debe estar; también recibe la dirección IP del cliente que hizo la consulta (para registrar bitácora)
        /// y, finalmente, recibe la respuesta que digita el usuario por medio del ReCaptcha.
        /// </summary>
        /// <param name="cedulaORecibo"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <param name="ipCliente"></param>
        /// <param name="respuesta"></param>
        [WebMethod]
        public void consultaCancelado(string cedula, string fechaInicio, string fechaFin, string ipCliente, string respuesta, int tipoSolicitud)
        {
            //if (this.esCaptchaValido(respuesta, tipoSolicitud) == true && (HttpContext.Current.Request.Url.Host.Equals("https://www.perezzeledon.go.cr") || HttpContext.Current.Request.Url.Host.Equals("https://perezzeledon.go.cr")))//Si la respuesta obtenida del ReCaptcha es correcta && Si el dominio de solicitud es correcto
            if(this.esCaptchaValido(respuesta, tipoSolicitud) == true)
            {
                string s = new JavaScriptSerializer().Serialize(this.obtenerCancelados(cedula, fechaInicio, fechaFin, ipCliente)); //Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s; 
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            else
            {
                base.Context.Response.Write("Captcha inválido");//Captcha inválido
            }
        }

        /// <summary>
        /// Obtiene el listado de tipo de patentes
        /// </summary>
        [WebMethod]
        public void listarTipoPatentes()
        {
            string s = new JavaScriptSerializer().Serialize(this.obtenerTipoPatente());//Cadena de retorno
            s = base.Context.Request.QueryString["callback"] + s;
            base.Context.Response.Clear();
            base.Context.Response.ContentType = "application/json";//Formato JSon
            base.Context.Response.Flush();
            base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
        }

        /// <summary>
        /// Método que permite obtener las patentes junto con sus atributos y cualidades más importantes. Recibe de parámetro el número de patente o el número de cédula
        /// del contribuyente poseedor de la patente que se desea consultar; también recibe la dirección IP del cliente que hizo la consulta (para registrar bitácora)
        /// y, finalmente, recibe la respuesta que digita el usuario por medio del ReCaptcha.
        /// </summary>
        /// <param name="cedula_patente"></param>
        /// <param name="ipCliente"></param>
        /// <param name="respuesta"></param>
        [WebMethod]
        public void consultaPatentes(string cedula_patente, string ipCliente, string respuesta, int tipoSolicitud)
        {
            //if (this.esCaptchaValido(respuesta, tipoSolicitud) == true && (HttpContext.Current.Request.Url.Host.Equals("https://www.perezzeledon.go.cr") || HttpContext.Current.Request.Url.Host.Equals("https://perezzeledon.go.cr")))//Si la respuesta obtenida del ReCaptcha es correcta && Si el dominio de solicitud es correcto
            if(this.esCaptchaValido(respuesta, tipoSolicitud) == true)
            {
                string s = new JavaScriptSerializer().Serialize(this.obtenerPatentes(cedula_patente, ipCliente));//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            else
            {
                base.Context.Response.Write("Captcha inválido");//Captcha inválido
            }
        }

        /// <summary>
        /// Método para realizar la consulta de cobros pendientes de pago. Recibe por parámetro el número de cédula del contribuyente al cual se le 
        /// desea hacer la consulta; también recibe la dirección IP del cliente que hizo la consulta (para registrar bitácora)
        /// y, finalmente, recibe la respuesta que digita el usuario por medio del ReCaptcha.
        /// </summary>
        /// <param name="pCedula"></param>
        /// <param name="ipCliente"></param>
        /// <param name="respuesta"></param>
        [WebMethod]
        public void consultaPendientes(string pCedula, string ipCliente, string respuesta, int tipoSolicitud)
        {
            //if (this.esCaptchaValido(respuesta, tipoSolicitud) == true && (HttpContext.Current.Request.Url.Host.Equals("https://www.perezzeledon.go.cr") || HttpContext.Current.Request.Url.Host.Equals("https://perezzeledon.go.cr")))//Si la respuesta obtenida del ReCaptcha es correcta && Si el dominio de solicitud es correcto     
            //if (this.esCaptchaValido(respuesta, tipoSolicitud) == true)
            //{
                    string s = new JavaScriptSerializer().Serialize(this.listaPendientes(pCedula, ipCliente));//Cadena de retorno
                    s = base.Context.Request.QueryString["callback"] + s;
                    base.Context.Response.Clear();
                    base.Context.Response.ContentType = "application/json";//Formato JSon
                    base.Context.Response.Flush();
                    base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
             /*}
             else
             {
                base.Context.Response.Write("Captcha inválido");//Captcha inválido
             }*/
        }

        /// <summary>
        /// Método que permite hacer las consultas de las propiedades inscritas en el sistema de la municipalidad. Recibe como parámetro el número
        /// de cédula del contribuyente; también recibe la dirección IP del cliente que hizo la consulta (para registrar bitácora)
        /// y, finalmente, recibe la respuesta que digita el usuario por medio del ReCaptcha.
        /// </summary>
        /// <param name="cedula_folio"></param>
        /// <param name="ipCliente"></param>
        /// <param name="respuesta"></param>
        [WebMethod]
        public void consultaPropiedad(string cedula_folio, string ipCliente, string respuesta, int tipoSolicitud)
        {
            //if (this.esCaptchaValido(respuesta, tipoSolicitud) == true && (HttpContext.Current.Request.Url.Host.Equals("https://www.perezzeledon.go.cr") || HttpContext.Current.Request.Url.Host.Equals("https://perezzeledon.go.cr")))//Si la respuesta obtenida del ReCaptcha es correcta && Si el dominio de solicitud es correcto
            if (this.esCaptchaValido(respuesta, tipoSolicitud) == true)
            {
                string s = new JavaScriptSerializer().Serialize(this.obtenerPropiedades(cedula_folio, ipCliente));//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            else
            {
                base.Context.Response.Write("Captcha inválido");//Captcha inválido
            }
        }

        /// <summary>
        /// Método para registrar partes de los archivos en la base de datos, para posteriormente ser guardados de forma real
        /// </summary>
        /// <param name="pCantPartes"></param>
        /// <param name="pNumParte"></param>
        /// <param name="pData"></param>
        /// <param name="pNomArchivo"></param>
        [WebMethod]
        public void guardarArchivo(int pCantPartes, int pNumParte, string pData, string pNomArchivo, string pFormato, int tipoSolicitud, string numCedula, string nomContribuyente, string tipoPatente, string correoContribuyente, string telefonoContribuyente)
        {
            if (!this.archivoCompleto(pNomArchivo, pCantPartes))// Si el archivo aún no está completo
            {
                string s = new JavaScriptSerializer().Serialize(this.registrarArchivo(pCantPartes, pNumParte, pData, pNomArchivo));//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            
            if(this.archivoCompleto(pNomArchivo, pCantPartes))
            {
                ArmarArchivo(pNomArchivo, pFormato, numCedula, nomContribuyente, tipoPatente, correoContribuyente, telefonoContribuyente);
            }
        }
        
        /// <summary>
        /// Método que permite hacer consultas sobre trámites realizados por un contribuyente en particular. Recibe de parámetro el número de cédula
        /// del contribuyente o el número de trámite que quiere realizar; también recibe la dirección IP del cliente que hizo la consulta (para registrar bitácora)
        /// y, finalmente, recibe la respuesta que digita el usuario por medio del ReCaptcha.
        /// </summary>
        /// <param name="cedula_tramite"></param>
        /// <param name="ipCliente"></param>
        /// <param name="respuesta"></param>
        [WebMethod]
        public void consultaTramite(string cedula_tramite, string ipCliente, string respuesta, int tipoSolicitud)
        {
            //if (this.esCaptchaValido(respuesta, tipoSolicitud) == true && (HttpContext.Current.Request.Url.Host.Equals("https://www.perezzeledon.go.cr") || HttpContext.Current.Request.Url.Host.Equals("https://perezzeledon.go.cr")))//Si la respuesta obtenida del ReCaptcha es correcta && Si el dominio de solicitud es correcto
            if (this.esCaptchaValido(respuesta, tipoSolicitud) == true)
            {
                string s = new JavaScriptSerializer().Serialize(this.obtenerTramites(cedula_tramite, ipCliente));//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            else
            {
                base.Context.Response.Write("Captcha inválido");//Captcha inválido
            }
        }

        /// <summary>
        /// Método que permite obtener el cronograma de cobro completo
        /// </summary>
        [WebMethod]
        public void cronogramaCobro()
        {
            string s = new JavaScriptSerializer().Serialize(this.obtenerCronogramaCobro());//Cadena de retorno
            s = base.Context.Request.QueryString["callback"] + s;
            base.Context.Response.Clear();
            base.Context.Response.ContentType = "application/json";//Formato JSon
            base.Context.Response.Flush();
            base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
        }

        /// <summary>
        /// Método que permite obtener la información que respecta a los derechos del uso del cementerio
        /// </summary>
        [WebMethod]
        public void DerechosCementerio()
        {
            string s = new JavaScriptSerializer().Serialize(this.obtenerTarifasCementerio());//Cadena de retorno
            s = base.Context.Request.QueryString["callback"] + s;
            base.Context.Response.Clear();
            base.Context.Response.ContentType = "application/json";//Formato JSon
            base.Context.Response.Flush();
            base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
        }        

        /// <summary>
        /// Método que permite obtener la información del contribuyente a partir de su número de cédula
        /// </summary>
        /// <param name="pCedula"></param>
        [WebMethod]
        public void existeContribuyente(string pCedula)
        {
            string s = new JavaScriptSerializer().Serialize(this.obtenerContribuyente(pCedula));//Cadena de retorno
            s = base.Context.Request.QueryString["callback"] + s;
            base.Context.Response.Clear();
            base.Context.Response.ContentType = "application/json";//Formato JSon
            base.Context.Response.Flush();
            base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
        }

        /// <summary>
        /// Método que permite obtener la información del contribuyente a partir de su número de cédula
        /// </summary>
        [WebMethod]
        public void LimpiezaVias()
        {
            string s = new JavaScriptSerializer().Serialize(this.obtenerTarifasLimpieza());//Cadena de retorno
            s = base.Context.Request.QueryString["callback"] + s;
            base.Context.Response.Clear();
            base.Context.Response.ContentType = "application/json";//Formato JSon
            base.Context.Response.Flush();
            base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
        }

        /// <summary>
        /// Método que permite obtener información sobre las boletas de parquímetro que se encuentran pendientes de cancelación. Recibe de parámetro el
        /// número de boleta o el número de placa del vehículo; también recibe la dirección IP del cliente que hizo la consulta (para registrar bitácora)
        /// y, finalmente, recibe la respuesta que digita el usuario por medio del ReCaptcha.
        /// </summary>
        /// <param name="boleta_placa"></param>
        /// <param name="respuesta"></param>
        [WebMethod]
        public void ObtenerBoletas(string ipCliente, string boleta_placa, string respuesta, int tipoSolicitud)
        {
            //if (this.esCaptchaValido(respuesta, tipoSolicitud) == true && (HttpContext.Current.Request.Url.Host.Equals("https://www.perezzeledon.go.cr") || HttpContext.Current.Request.Url.Host.Equals("https://perezzeledon.go.cr")))//Si la respuesta obtenida del ReCaptcha es correcta && Si el dominio de solicitud es correcto
            if (this.esCaptchaValido(respuesta, tipoSolicitud) == true)
            {
                string s = new JavaScriptSerializer().Serialize(this.obtenerBoletas(boleta_placa, ipCliente));//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            else
            {
                base.Context.Response.Write("Captcha inválido");//Captcha inválido
            }
        }

        [WebMethod]
        public void ObtenerInfracciones(string ipCliente, string boleta_placa, string respuesta, int tipoSolicitud, int tipoDato)
        {
            //if (this.esCaptchaValido(respuesta, tipoSolicitud) == true && (HttpContext.Current.Request.Url.Host.Equals("https://www.perezzeledon.go.cr") || HttpContext.Current.Request.Url.Host.Equals("https://perezzeledon.go.cr")))//Si la respuesta obtenida del ReCaptcha es correcta && Si el dominio de solicitud es correcto
            if (this.esCaptchaValido(respuesta, tipoSolicitud) == true)
            {
                string s = new JavaScriptSerializer().Serialize(this.obtenerInfracciones(boleta_placa, ipCliente, tipoDato));//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            else
            {
                base.Context.Response.Write("Captcha inválido");//Captcha inválido
            }
        }

        /// <summary>
        /// Método que permite obtener la información sobre uno o varios permisos de construcción registrados en la Municipalidad. Recibe de parámetro el número de 
        /// cédula del contribuyente o el número de permiso que desea consultar; también recibe la dirección IP del cliente que hizo la consulta (para registrar bitácora)
        /// y, finalmente, recibe la respuesta que digita el usuario por medio del ReCaptcha.
        /// </summary>
        /// <param name="permiso_cedula"></param>
        /// <param name="respuesta"></param>
        [WebMethod]
        public void ObtenerPermisoConstruccion(string ipCliente, string permiso_cedula, string respuesta, int tipoSolicitud)
        {
            //if (this.esCaptchaValido(respuesta, tipoSolicitud) == true && (HttpContext.Current.Request.Url.Host.Equals("https://www.perezzeledon.go.cr") || HttpContext.Current.Request.Url.Host.Equals("https://perezzeledon.go.cr")))//Si la respuesta obtenida del ReCaptcha es correcta && Si el dominio de solicitud es correcto
            if (this.esCaptchaValido(respuesta, tipoSolicitud) == true)
            {
                string s = new JavaScriptSerializer().Serialize(this.obtenerPermisos(permiso_cedula, ipCliente));//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            else
            {
                base.Context.Response.Write("Captcha inválido");//Captcha inválido
            }
        }

        /// <summary>
        /// Obtiene la información que respecta a los datos de la recolección de basura.
        /// </summary>
        [WebMethod]
        public void RecoleccionBasura()
        {
            string s = new JavaScriptSerializer().Serialize(this.obtenerRecoleccionBasura());//Cadena de retorno
            s = base.Context.Request.QueryString["callback"] + s;
            base.Context.Response.Clear();
            base.Context.Response.ContentType = "application/json";//Formato JSon
            base.Context.Response.Flush();
            base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
        }

        [WebMethod]
        public void ObtenerPatentesActivas(string ipCliente)
        {
            string s = new JavaScriptSerializer().Serialize(this.obtenerPatentesActivas(ipCliente));//Cadena de retorno
            s = base.Context.Request.QueryString["callback"] + s;
            base.Context.Response.Clear();
            base.Context.Response.ContentType = "application/json";//Formato JSon
            base.Context.Response.Flush();
            base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
        }

        [WebMethod]
        public void TienePendientes(string pCedula)
        {
            /*if (this.esCaptchaValido(respuesta, tipoSolicitud) == true)
            {*/
                string s = new JavaScriptSerializer().Serialize(this.tienePendientes(pCedula));//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            /*}
            else
            {
                base.Context.Response.Write("Captcha inválido");//Captcha inválido
            }*/
        }

        [WebMethod]
        public void SolicitudCertificacion(string numCedula, string ipCliente, string correoContribuyente, string telefonoContribuyente, string tipoSolicitud)
        {
            string s = new JavaScriptSerializer().Serialize(this.solicitudCertificacion(numCedula, ipCliente, correoContribuyente, telefonoContribuyente, tipoSolicitud));//Cadena de retorno
            s = base.Context.Request.QueryString["callback"] + s;
            base.Context.Response.Clear();
            base.Context.Response.ContentType = "application/json";//Formato JSon
            base.Context.Response.Flush();
            base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
        }

        #endregion

        #region Sources

        /// <summary>
        /// Función para determinar si un contribuyente tiene cobros pendientes de pago
        /// </summary>
        /// <param name="pCedula"></param>
        /// <returns></returns>
        public RespuestaPendientes tienePendientes(string pCedula)
        {
            RespuestaPendientes retorno;//Lista de retorno
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString)) //Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_CantidadCobrosPendientes", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    command.Parameters.Add(new SqlParameter("@NumCedula", pCedula));//Se agrega parámetro de cédula
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        while(reader.Read())
                        {
                            if(reader["CANTIDAD"] != null)
                            {
                                retorno = new RespuestaPendientes(1);
                                return retorno;
                            }
                        }
                    }
                }
            }
            return new RespuestaPendientes(0);
        }


        /// <summary>
        /// Función que registra la solicitud de certificación y envía la notificación 
        /// </summary>
        /// <param name="numCedula"></param>
        /// <param name="ipCliente"></param>
        /// <param name="correoContribuyente"></param>
        /// <returns></returns>
        public Respuesta solicitudCertificacion(string numCedula, string ipCliente, string correoContribuyente, string telefonoContribuyente, string tipoSolicitud)
        {
            try
            {
                if(this.obtenerContribuyente(numCedula).Nombre == null)
                {
                    return new Respuesta("Inexistente");
                }
                if (this.tienePendientes(numCedula).estado == 1)//Tiene pendientes
                {
                    return new Respuesta("Moroso");
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString)) //Se realiza la conexión a la BD
                    {
                        connection.Open();//Se abre la conexión
                        using (SqlCommand command = new SqlCommand("sp_RegistrarSolicitudCertificacion", connection))//Se llama el procedimiento almacenado
                        {
                            command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                            command.Parameters.Add(new SqlParameter("@Cedula", numCedula));//Se agrega parámetro de cédula
                            command.Parameters.Add(new SqlParameter("@IpCliente", ipCliente));//Se agrega parámetro de cédula
                            command.Parameters.Add(new SqlParameter("@Tipo", tipoSolicitud.Contains("Constancia") ? 1 : 2));
                            command.ExecuteNonQuery();//Se ejecuta y se carga la información en el reader
                        }                      
                    }
                    Respuesta enviarCorreo = enviarExchangeEmail(tipoSolicitud, this.obtenerContribuyente(numCedula).Nombre, numCedula, null, null, correoContribuyente, telefonoContribuyente);
                    return new Respuesta("Realizada");
                }
            }
            catch(Exception ex)
            {
                return new Respuesta("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Funcion para registrar partes del archivo
        /// </summary>
        /// <param name="pCantPartes"></param>
        /// <param name="pNumParte"></param>
        /// <param name="pData"></param>
        /// <param name="pNomArchivo"></param>
        /// <returns></returns>
        public Respuesta registrarArchivo(int pCantPartes, int pNumParte, string pData, string pNomArchivo)
        {
            try
            {
                pData = pData.Replace(" ", "+");
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("sp_InsertarParteArchivo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@NombreArchivo", pNomArchivo));
                        command.Parameters.Add(new SqlParameter("@CantPartes", pCantPartes));
                        command.Parameters.Add(new SqlParameter("@NumParte", pNumParte));
                        command.Parameters.Add(new SqlParameter("@InfoArchivo", pData));
                        command.ExecuteNonQuery();
                    }
                    return new Respuesta("Correcto", pNumParte);
                }
            }
            catch
            {
                return new Respuesta("Error", pNumParte);
            }
        }

        /// <summary>
        /// Envía el correo de notificación al contribuyente que hizo la solicitud del trámite
        /// </summary>
        /// <param name="tipoSolicitud"></param>
        /// <param name="nombreContribuyente"></param>
        /// <param name="cedulaContribuyente"></param>
        /// <param name="tipoPatente"></param>
        /// <param name="correoContribuyente"></param>
        public void enviarNotificacionContribuyente(string tipoSolicitud, string nombreContribuyente, string cedulaContribuyente, string tipoPatente, string correoContribuyente)
        {
            Parametro paramCorreo = this.obtenerCredencialesCorreo();
            String userName = paramCorreo.valor.Split(';')[0];
            String password = paramCorreo.valor.Split(';')[1];
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress(correoContribuyente));
            msg.From = new MailAddress(userName);
            msg.Subject = "Solicitud en línea";

            if (tipoSolicitud.Contains("Uso de suelo"))
            {
                string body = System.IO.File.ReadAllText(base.Server.MapPath(@"./EmailTemplate/BodyUsoSueloContribuyente.html")).Replace("{TipoSolicitud}", tipoSolicitud);
                body = body.Replace("{NombreContribuyente}", nombreContribuyente);
                body = body.Replace("{CedulaContribuyente}", cedulaContribuyente);
                body = body.Replace("{TipoPatente}", tipoPatente);
                msg.Body = body;
                msg.IsBodyHtml = true;
                SmtpClient client = new SmtpClient();
                client.Host = "smtp.office365.com";
                client.Credentials = new System.Net.NetworkCredential(userName, password);
                client.Port = 587;
                client.EnableSsl = true;
                client.Send(msg);
            }
            else if(tipoSolicitud.Contains("Certificación") || tipoSolicitud.Contains("Constancia"))
            {
                string body = System.IO.File.ReadAllText(base.Server.MapPath(@"./EmailTemplate/BodyCertificacionContribuyente.html")).Replace("{TipoSolicitud}", tipoSolicitud);
                body = body.Replace("{NombreContribuyente}", nombreContribuyente);
                msg.Body = body;
                msg.IsBodyHtml = true;
                SmtpClient client = new SmtpClient();
                client.Host = "smtp.office365.com";
                client.Credentials = new System.Net.NetworkCredential(userName, password);
                client.Port = 587;
                client.EnableSsl = true;
                client.Send(msg);
            }
        }

        /// <summary>
        /// Envia el correo electrónico notificando una solicitud en específico
        /// </summary>
        /// <param name="tipoSolicitud"></param>
        /// <param name="nombreContribuyente"></param>
        /// <param name="cedulaContribuyente"></param>
        /// <param name="tipoPatente"></param>
        /// <param name="rutaArchivo"></param>
        /// <param name="correoContribuyente"></param>
        /// <returns></returns>
        public Respuesta enviarExchangeEmail(string tipoSolicitud, string nombreContribuyente, string cedulaContribuyente, string tipoPatente, string rutaArchivo, string correoContribuyente, string telefonoContribuyente)
        {
            try
            {
                Parametro paramCorreo = this.obtenerCredencialesCorreo();
                String userName = paramCorreo.valor.Split(';')[0];
                String password = paramCorreo.valor.Split(';')[1];
                MailMessage msg = new MailMessage();
                msg.To.Add(new MailAddress("ciat@mpz.go.cr"));
                msg.From = new MailAddress(userName);
                msg.Subject = "Municipalidad de Pérez Zeledón: Solicitud en línea";

                if (tipoSolicitud.Contains("Uso de suelo"))
                {
                    string body = System.IO.File.ReadAllText(base.Server.MapPath(@"./EmailTemplate/BodyUsoSuelo.html")).Replace("{TipoSolicitud}", tipoSolicitud);
                    body = body.Replace("{NombreContribuyente}", nombreContribuyente);
                    body = body.Replace("{CedulaContribuyente}", cedulaContribuyente);
                    body = body.Replace("{TipoPatente}", tipoPatente);
                    body = body.Replace("{CorreoContribuyente}", correoContribuyente == "N" ? "No especifica" : correoContribuyente);
                    body = body.Replace("{TelefonoContribuyente}", telefonoContribuyente == "N" ? "No especifica" : telefonoContribuyente);
                    body = body.Replace("{rutaArchivo}", rutaArchivo);
                    msg.Body = body;
                    msg.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient();
                    client.Host = "smtp.office365.com";
                    client.Credentials = new System.Net.NetworkCredential(userName, password);
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.Send(msg);
                }           
                else if(tipoSolicitud.Contains("Certificación") || tipoSolicitud.Contains("Constancia"))
                {
                    string body = System.IO.File.ReadAllText(base.Server.MapPath(@"./EmailTemplate/BodyCertificacion.html")).Replace("{TipoSolicitud}", tipoSolicitud);
                    body = body.Replace("{NombreContribuyente}", nombreContribuyente);
                    body = body.Replace("{CedulaContribuyente}", cedulaContribuyente);
                    body = body.Replace("{CorreoContribuyente}", correoContribuyente == "N" ? "No especifica" : correoContribuyente);
                    body = body.Replace("{TelefonoContribuyente}", telefonoContribuyente == "N" ? "No especifica" : telefonoContribuyente);
                    msg.Body = body;
                    msg.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient();
                    client.Host = "smtp.office365.com";
                    client.Credentials = new System.Net.NetworkCredential(userName, password);
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.Send(msg);
                }

                if (correoContribuyente.Contains("@"))
                    this.enviarNotificacionContribuyente(tipoSolicitud, nombreContribuyente, cedulaContribuyente, tipoPatente, correoContribuyente);
                return new Respuesta("Correcto");
            }
            catch (Exception ex)
            {
                return new Respuesta("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el los credenciales del correo de notificaciones
        /// </summary>
        /// <returns></returns>
        public Parametro obtenerCredencialesCorreo()
        {
            Parametro elmRetorno = new Parametro();
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString)) //Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_obtenerParametro", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    command.Parameters.Add(new SqlParameter("@Key", "Correo_Notificaciones"));//Se agrega parámetro de cédula
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        while (reader.Read())
                        {
                            elmRetorno.clave = reader["Llave_Parametro"].ToString();
                            elmRetorno.valor = reader["Valor_Parametro"].ToString();
                        }
                    }
                }
            }
            return elmRetorno;
        }

        /// <summary>
        /// Retorna lista de cobros que se encuentran pendientes de pagar
        /// </summary>
        /// <param name="pCedula"></param>
        /// <param name="ipCliente"></param>
        /// <returns></returns>
        public List<Pendiente> listaPendientes(string pCedula, string ipCliente)
        {
            List<Pendiente> listaRetorno;//Lista de retorno
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString)) //Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_CobrosPendientes", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    command.Parameters.Add(new SqlParameter("@cedula", pCedula));//Se agrega parámetro de cédula
                    command.Parameters.Add(new SqlParameter("@IpCliente", ipCliente));//Se agrega parámetro de la IP
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        List<Pendiente> listaTemporal = new List<Pendiente>();
                        string str = DateTime.Now.Year.ToString();//Año actual
                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            double descuento = 0.0;
                            DateTime fecVencimiento = Convert.ToDateTime(reader["FEC_VENCIMIENTO"].ToString());//Fecha de vencimiento del pago
                            double montoCobro = Convert.ToDouble(reader["MON_COBRO"]);
                            double multa = 0;


                            //Cálculo para el monto de multas
                            multa = this.CalcularMultas(Convert.ToDouble(reader["MON_COBRO"].ToString()), reader["COD_CONCEPTOCOBRO"].ToString(), Convert.ToDateTime(reader["FEC_VENCIMIENTO"].ToString()));
                            montoCobro += multa;

                            //Cálculo para el monto de intereses
                            int intereses = CalcularIntereses(Convert.ToDouble(reader["MON_COBRO"].ToString()), multa, reader["COD_CONCEPTOCOBRO"].ToString(), Convert.ToDateTime(reader["FEC_VENCIMIENTO"].ToString()));


                            //Cálculo para el monto de timbres
                            double timbres = this.CalcularTimbres(Convert.ToDouble(reader["MON_COBRO"].ToString()), reader["COD_CONCEPTOCOBRO"].ToString(), Convert.ToDateTime(reader["FEC_VENCIMIENTO"].ToString()));

                            //Cálculo para descuentos
                            descuento = this.CalcularDescuento(Convert.ToDouble(reader["MON_COBRO"].ToString()), reader["COD_CONCEPTOCOBRO"].ToString(), Convert.ToDateTime(reader["FEC_VENCIMIENTO"].ToString()));

                            List<string> cronograma = this.obtenerCronogramaPorCodigo(reader["COD_CONCEPTOCOBRO"].ToString());//Lista con el cronograma de cobro
                            string pEstado = string.Empty;//Estado del cobro
                            int periodo = 0;//Número de periodo 
                            if (fecVencimiento < DateTime.Now)//Si la fecha de vencimiento es menor a la fecha actual (si ya venció el pago)
                            {
                                pEstado = "Pendiente vencido";
                            }
                            else
                            {
                                for (int i = 0; i < 12; i++)
                                {
                                    if (cronograma.Count < i + 2 || cronograma.Count < i + 1)
                                    {
                                        break;
                                    }
                                    else if ((Convert.ToDateTime(cronograma[i]) < DateTime.Now) && (Convert.ToDateTime(cronograma[i + 1]) > DateTime.Now))//Si la fecha actual se encuentra en medio de un periodo de cobro, para determinar el periodo del cobro
                                    {
                                        periodo = i + 2;
                                        break;
                                    }
                                }
                                int ind = Convert.ToInt32(reader["NUM_PERIODO"]);
                                if (Convert.ToInt32(reader["NUM_PERIODO"]) == periodo)//Si está en el periodo actual de cobro
                                {
                                    pEstado = "Pendiente al cobro";
                                    intereses = 0;
                                }
                                else if (Convert.ToInt32(reader["NUM_PERIODO"]) > periodo)//Si está en el próximo periodo de cobro
                                {
                                    pEstado = "Pendiente no vencido";
                                    intereses = 0;
                                }
                            }

                            Pendiente item = new Pendiente(reader["NUM_COBRO"].ToString(), reader["DSC_CONCEPTOCOBRO"].ToString(), reader["NUM_PERIODO"].ToString(), reader["AÑO_COBRO"].ToString(), pEstado == "" ? "Pendiente no vencido" : pEstado, Convert.ToDouble(reader["MON_COBRO"]), reader["COD_ESTANDAR"].ToString(), reader["FEC_VENCIMIENTO"].ToString(), (intereses > 0) ? ((double)intereses) : ((double)0), reader["CED_CONTRIBUYENTE"].ToString(), reader["NOM_CONTRIBUYENTE"].ToString(), descuento.ToString(), multa < (double)0 ? 0 : (double)multa, reader["COD_CONCEPTOCOBRO"].ToString(), timbres);
                            listaTemporal.Add(item);
                        }
                        connection.Close();//Se cierra la conexión
                        listaRetorno = listaTemporal;
                    }
                }
            }
            return listaRetorno;
        }

        /// <summary>
        /// Función para listar las boletas pendientes de pago a partir del número de boleta o número de placa del vehículo al cual se la hicieron; además, recibe
        /// de parámetro la dirección IP del cliente que solicita la información.
        /// </summary>
        /// <param name="boleta_placa"></param>
        /// <param name="ipCliente"></param>
        /// <returns></returns>
        public List<Boleta> obtenerBoletas(string boleta_placa, string ipCliente)
        {
            List<Boleta> listaRetorno;//Lista de retorno
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString))//Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_ObtenerBoletas", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    command.Parameters.Add(new SqlParameter("@Boleta_Placa", boleta_placa));//Se agrega el parámetro del número de boleta o placa
                    command.Parameters.Add(new SqlParameter("@IpCliente", ipCliente));//Se agrega el parámetro de la dirección IP del cliente
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        List<Boleta> listaTemporal = new List<Boleta>();
                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            Boleta item = new Boleta(reader["NUM_BOLETA"].ToString(), reader["TIP_PLACA"].ToString(), reader["NUM_PLACA"].ToString(), reader["IND_ESTADO"].ToString(), (reader["COD_UBICACION1"].ToString() == string.Empty) ? "No especifica" : reader["COD_UBICACION1"].ToString(), (reader["COD_UBICACION2"].ToString() == string.Empty) ? "No especifica" : reader["COD_UBICACION2"].ToString(), (reader["COD_UBICACION3"].ToString() == string.Empty) ? "No especifica" : reader["COD_UBICACION3"].ToString(), reader["MON_PARTE"].ToString(), reader["FEC_BOLETA"].ToString());
                            listaTemporal.Add(item);
                        }
                        connection.Close();//Se cierra la conexión
                        listaRetorno = listaTemporal;
                    }
                }
            }
            return listaRetorno;
        }

        /// <summary>
        /// Función para listar las boletas pendientes de pago a partir del número de boleta o número de placa del vehículo al cual se la hicieron; además, recibe
        /// de parámetro la dirección IP del cliente que solicita la información.
        /// </summary>
        /// <param name="boleta_placa"></param>
        /// <param name="ipCliente"></param>
        /// <param name="tipoDato"></param>
        /// <returns></returns>
        public List<Boleta> obtenerInfracciones(string boleta_placa, string ipCliente, int tipoDato)
        {
            List<Boleta> listaRetorno;//Lista de retorno
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString))//Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_ObtenerInfracciones", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    command.Parameters.Add(new SqlParameter("@Placa_Boleta", boleta_placa));//Se agrega el parámetro del número de boleta o placa
                    command.Parameters.Add(new SqlParameter("@IpCliente", ipCliente));//Se agrega el parámetro de la dirección IP del cliente
                    command.Parameters.Add(new SqlParameter("@Campo", tipoDato == 1 ? "NUM_PLACA" : "NUM_BOLETA"));//Se agrega el parámetro de campo a consultar
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        List<Boleta> listaTemporal = new List<Boleta>();
                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            Boleta item = new Boleta(reader["NUM_BOLETA"].ToString(), reader["TIP_PLACA"].ToString(), reader["NUM_PLACA"].ToString(), reader["IND_ESTADO"].ToString(), (reader["COD_UBICACION1"].ToString() == string.Empty) ? "No especifica" : reader["COD_UBICACION1"].ToString(), (reader["COD_UBICACION2"].ToString() == string.Empty) ? "No especifica" : reader["COD_UBICACION2"].ToString(), (reader["COD_UBICACION3"].ToString() == string.Empty) ? "No especifica" : reader["COD_UBICACION3"].ToString(), reader["MON_PARTE"].ToString(), reader["FEC_BOLETA"].ToString());
                            listaTemporal.Add(item);
                        }
                        connection.Close();//Se cierra la conexión
                        listaRetorno = listaTemporal;
                    }
                }
            }
            return listaRetorno;
        }

        /// <summary>
        /// Función para obtener un listado de tipos de patentes
        /// </summary>
        /// <returns></returns>

        public List<TipoPatente> obtenerTipoPatente()
        {
            List<TipoPatente> listaRetorno;//Lista de retorno
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString))//Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_listarTiposPatentes", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        List<TipoPatente> listaTemporal = new List<TipoPatente>();
                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            TipoPatente item = new TipoPatente(Convert.ToInt32(reader["PK_TipoPatente"].ToString()), reader["Descripcion_TipoPatente"].ToString());
                            listaTemporal.Add(item);
                        }
                        connection.Close();//Se cierra la conexión
                        listaRetorno = listaTemporal;
                    }
                }
            }
            return listaRetorno;
        }

        /// <summary>
        /// Función para listar las patentes que se encuentran activas en el momento que se hace la consulta
        /// </summary>
        /// <param name="ipCliente"></param>
        /// <returns></returns>
        public List<PatenteActiva> obtenerPatentesActivas(string ipCliente)
        {
            List<PatenteActiva> listaPatentesActivas; //Lista de retorno
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString))// Se realiza la conexión a la BD
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("sp_ObtenerPatentesActivas", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@IpCliente", ipCliente));
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        List<PatenteActiva> listaTemporal = new List<PatenteActiva>();
                        while(reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            PatenteActiva item = new PatenteActiva(reader["CED_CONTRIBUYENTE"].ToString(), reader["DSC_PATENTE"].ToString(), reader["DSC_MODALIDAD"].ToString() == string.Empty ? "No especifica" : reader["DSC_MODALIDAD"].ToString(), reader["DIR_LOCAL"].ToString(), reader["NOM_LOCAL"].ToString(), reader["TEL_LOCAL"].ToString() == String.Empty ? "No especifica" : reader["TEL_LOCAL"].ToString(), reader["FAX_LOCAL"].ToString() == String.Empty ? "No especifica" : reader["FAX_LOCAL"].ToString(), reader["EML_LOCAL"].ToString() == String.Empty ? "No especifica" : reader["EML_LOCAL"].ToString(), reader["APD_LOCAL"].ToString() == String.Empty ? "No especifica" : reader["APD_LOCAL"].ToString(), reader["DSC_DISTRITO"].ToString(), reader["NOM_CONTRIBUYENTE"].ToString());
                            listaTemporal.Add(item);
                        }
                        connection.Close();
                        listaPatentesActivas = listaTemporal;
                    }
                }
            }
            return listaPatentesActivas;
        }

        /// <summary>
        /// Función que permite obtener un listado de los cobros cancelados a partir del número de cédula del contribuyente y un rango de fechas que delimita
        /// la búsqueda que el usuario desea realizar. Además, recibe la dirección IP del cliente para llevar un control en la bitácora del sistema.
        /// </summary>
        /// <param name="cedula_recibo"></param>
        /// <param name="fecInicio"></param>
        /// <param name="fecFin"></param>
        /// <param name="ipCliente"></param>
        /// <returns></returns>
        public List<Cancelado> obtenerCancelados(string cedula, string fecInicio, string fecFin, string ipCliente)
        {
            List<Cancelado> listaRetorno;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString))//Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_ObtenerPagosCancelados", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    command.CommandTimeout = 0x3e8;
                    command.Parameters.Add(new SqlParameter("@Cedula", cedula));//Se agrega el parámetro del número de cédula del contribuyente o número de recibo
                    command.Parameters.Add(new SqlParameter("@Fecha_Inicio", fecInicio));//Se agrega el parámetro de fecha de inicio
                    command.Parameters.Add(new SqlParameter("@Fecha_Fin", fecFin));//Se agrega el parámetro de fecha de fin
                    command.Parameters.Add(new SqlParameter("@IpCliente", ipCliente));//Se agrega el parámetro de la dirección IP del cliente
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        List<Cancelado> listaTemporal = new List<Cancelado>();
                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            Cancelado item = new Cancelado(Convert.ToDouble(reader["MON_COBRO"]), reader["FEC_CANCELACION"].ToString(), reader["NUM_RECIBO"].ToString(), reader["COD_USUARIO"].ToString(), reader["COD_ESTANDAR"].ToString(), Convert.ToInt32(reader["NUM_PERIODO"]), reader["AÑO_COBRO"].ToString(), reader["DSC_CONCEPTOCOBRO"].ToString(), reader["NOM_CONTRIBUYENTE"].ToString(), reader["CED_CONTRIBUYENTE"].ToString());
                            listaTemporal.Add(item);
                        }
                        connection.Close();//Se cierra la conexión
                        listaRetorno = listaTemporal;
                    }
                }
            }
            return listaRetorno;
        }

        /// <summary>
        /// Función que permite obtener la información de un contribuyente en particular a partir de su número de cédula
        /// </summary>
        /// <param name="pCedula"></param>
        /// <returns></returns>
        public Contribuyente obtenerContribuyente(string pCedula)
        {
            Contribuyente contribuyenteRetorno;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString))//Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_ExisteCedula", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    command.Parameters.Add(new SqlParameter("@Cedula", pCedula));//Se agrega el parámetro del número de cédula
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        Contribuyente contribuyenteTemporal = new Contribuyente();
                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            contribuyenteTemporal = new Contribuyente(reader["NOM_Contribuyente"].ToString(), reader["CED_Contribuyente"].ToString());
                        }
                        connection.Close();//Se cierra la conexión
                        contribuyenteRetorno = contribuyenteTemporal;
                    }
                }
            }
            return contribuyenteRetorno;
        }

        /// <summary>
        /// Función que permite obtener una lista del cronograma de cobro
        /// </summary>
        /// <returns></returns>
        public List<Cronograma> obtenerCronogramaCobro()
        {
            List<Cronograma> listaRetorno;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString))//Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_ObtenerCronogramaCobro", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        List<Cronograma> listaTemporal = new List<Cronograma>();
                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            Cronograma item = new Cronograma(reader["COD_CONCEPTOCOBRO"].ToString(), reader["FEC_COBRO1"].ToString(), reader["FEC_COBRO2"].ToString(), reader["FEC_COBRO3"].ToString(), reader["FEC_COBRO4"].ToString(), reader["FEC_COBRO5"].ToString(), reader["FEC_COBRO6"].ToString(), reader["FEC_COBRO7"].ToString(), reader["FEC_COBRO8"].ToString(), reader["FEC_COBRO9"].ToString(), reader["FEC_COBRO10"].ToString(), reader["FEC_COBRO11"].ToString(), reader["FEC_COBRO12"].ToString());
                            listaTemporal.Add(item);
                        }
                        connection.Close();//Se cierra la conexión
                        listaRetorno = listaTemporal;
                    }
                }
            }
            return listaRetorno;
        }

        /// <summary>
        /// Función que permite obtener el cronograma de un código estándar en específico
        /// </summary>
        /// <param name="pCodigo"></param>
        /// <returns></returns>
        public List<string> obtenerCronogramaPorCodigo(string pCodigo)
        {
            List<string> listaRetorno;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString))//Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_CronogramaPorCodigo", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    command.Parameters.Add(new SqlParameter("@Codigo", pCodigo));//Se agrega el parámetro del código estandar
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        List<string> listaTemporal = new List<string>();
                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            for (int i = 1; i <= 12; i++)
                            {
                                if (reader["FEC_COBRO" + i.ToString()].ToString() == string.Empty)//Si el elemento de fecha de cobro viene vacío
                                {
                                    continue;//Pasa a la siguiente iteración
                                }
                                listaTemporal.Add(reader["FEC_COBRO" + i.ToString()].ToString());
                            }
                        }
                        connection.Close();//Se cierra la conexión
                        listaRetorno = listaTemporal;
                    }
                }
            }
            return listaRetorno;
        }

        /// <summary>
        /// Función que obtiene un listado de las patentes a partir de la cédula del contribuyente o el número de la patente. Además se recibe la dirección IP
        /// del cliente que está haciendo la consulta para que sea registrado en la bitácora.
        /// </summary>
        /// <param name="cedula_patente"></param>
        /// <param name="ipCliente"></param>
        /// <returns></returns>
        public List<Patente> obtenerPatentes(string cedula_patente, string ipCliente)
        {
            List<Patente> listaRetorno;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString))//Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_ConsultaPatentes", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    command.Parameters.Add(new SqlParameter("@Cedula_Patente", cedula_patente));//Se agrega el parámetro del número de cédula del contribuyente o el número de patente
                    command.Parameters.Add(new SqlParameter("@IpCliente", ipCliente));//Se agrega el parámetro de la dirección IP del cliente
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        List<Patente> listaTemporal = new List<Patente>();
                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            Patente item = new Patente(Convert.ToInt32(reader["NUM_PATENTE"]), reader["DSC_PATENTE"].ToString(), reader["NOM_LOCAL"].ToString(), reader["DIR_LOCAL"].ToString(), (reader["NUM_FOLIOREAL"].ToString() != string.Empty) ? reader["NUM_FOLIOREAL"].ToString() : "No existe", reader["DSC_DISTRITO"].ToString(), (Convert.ToInt32(reader["IND_ESTADOPATENTE"]) == 0) ? "Inactiva" : "Activa");
                            listaTemporal.Add(item);
                        }
                        connection.Close();//Se cierra la conexión
                        listaRetorno = listaTemporal;
                    }
                }
            }
            return listaRetorno;
        }

        /// <summary>
        /// Función que obtiene un listado de los permisos de construcción
        /// </summary>
        /// <param name="permiso_cedula"></param>
        /// <param name="ipCliente"></param>
        /// <returns></returns>
        public List<PermisoConstruccion> obtenerPermisos(string permiso_cedula, string ipCliente)
        {
            List<PermisoConstruccion> listaRetorno;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString))//Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_ObtenerPermisoConstruccion", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    command.Parameters.Add(new SqlParameter("@Permiso_Cedula", permiso_cedula));//Se agrega el parámetro del número de cédula del contribuyente o el número de permiso
                    command.Parameters.Add(new SqlParameter("@IpCliente", ipCliente));//Se agrega el parámetro de la dirección IP del cliente que hace la consulta
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        List<PermisoConstruccion> listaTemporal = new List<PermisoConstruccion>();
                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            PermisoConstruccion item = new PermisoConstruccion(reader["NUM_PERMISO"].ToString(), reader["CED_CONTRIBUYENTE"].ToString(), reader["NUM_FOLIOREAL"].ToString(), reader["FEC_SOLICITUD"].ToString(), reader["CAN_METROSDEFRENTE"].ToString(), reader["TIP_OBRA"].ToString(), reader["DSC_USOOBRA"].ToString(), reader["CAN_AREACONSTRUCCION"].ToString(), reader["NOM_INGENIERO"].ToString(), reader["CED_INGENIERO"].ToString(), reader["NUM_ASOCIADO"].ToString(), reader["IND_ESTADOPERMISO"].ToString(), reader["NOM_CONTRIBUYENTE"].ToString());
                            listaTemporal.Add(item);
                        }
                        connection.Close();//Se cierra la conexión
                        listaRetorno = listaTemporal;
                    }
                }
            }
            return listaRetorno;
        }

        /// <summary>
        /// Función para obtener un listado de las propiedades pertenecientes a un contribuyente en particular. Recibe el número de cédula del contribuyente y
        /// la dirección IP del cliente que hace la solicitud.
        /// </summary>
        /// <param name="pCedula"></param>
        /// <param name="ipCliente"></param>
        /// <returns></returns>
        public List<Propiedad> obtenerPropiedades(string pCedula, string ipCliente)
        {
            List<Propiedad> listaRetorno;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString))//Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_ConsultaPropiedades", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    command.Parameters.Add(new SqlParameter("@Cedula_Folio", pCedula));//Se agrega el parámetro del número de boleta o placa
                    command.Parameters.Add(new SqlParameter("@IpCliente", ipCliente));//Se agrega el parámetro del número de boleta o placa
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        List<Propiedad> listaTemporal = new List<Propiedad>();
                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            Propiedad item = new Propiedad(reader["NUM_FOLIOREAL"].ToString(), reader["NUM_GIS"].ToString(), reader["NUM_PLANO"].ToString(), Convert.ToDouble(reader["MTS_FRENTE1"]), Convert.ToDouble(reader["MTS_FRENTE2"]), Convert.ToDouble(reader["MTS_FONDO"]), Convert.ToDouble(reader["MTS_AREA"]), Convert.ToDouble(reader["MTS_AREAPRIVADA"]), Convert.ToDouble(reader["MTS_AREACOMUN"]), Convert.ToDouble(reader["MTS_AREACCONSTRUIDA"]), Convert.ToDouble(reader["MTS_AREACLIBRE"]), reader["DIR_PROPIEDAD"].ToString(), reader["IND_PAGAIMPUESTO"].ToString(), reader["IND_PAGABASURA"].ToString(), Convert.ToDouble(reader["CAN_METROSLIMPIEZA"]), reader["TIP_ZONA"].ToString(), (reader["DSC_COLINDANTES"].ToString() != string.Empty) ? reader["DSC_COLINDANTES"].ToString() : "No existe", reader["DSC_PROVINCIA"].ToString(), reader["DSC_CANTON"].ToString(), reader["DSC_DISTRITO"].ToString());
                            listaTemporal.Add(item);
                        }
                        connection.Close();//Se cierra la conexión
                        listaRetorno = listaTemporal;
                    }
                }
            }
            return listaRetorno;
        }

        /// <summary>
        /// Función que retorna un listado con la información que respecta a la recolección de basura.
        /// </summary>
        /// <returns></returns>
        public List<TarifasBasura> obtenerRecoleccionBasura()
        {
            List<TarifasBasura> listaRetorno;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString))//Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_RecoleccionBasura", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        List<TarifasBasura> listaTemporal = new List<TarifasBasura>();
                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            TarifasBasura item = new TarifasBasura(reader["COD_CAT"].ToString(), Convert.ToDouble(reader["MON_TARIFA"]));
                            listaTemporal.Add(item);
                        }
                        connection.Close();//Se cierra la conexión
                        listaRetorno = listaTemporal;
                    }
                }
            }
            return listaRetorno;
        }

        /// <summary>
        /// Función que retorna un listado con la información que respecta a los derechos del cementerio
        /// </summary>
        /// <returns></returns>
        public List<TarifasCementerio> obtenerTarifasCementerio()
        {
            List<TarifasCementerio> listaRetorno;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString))//Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_DerechosCementerio", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        List<TarifasCementerio> listaTemporal = new List<TarifasCementerio>();
                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            TarifasCementerio item = new TarifasCementerio(reader["TIP_TARIFA"].ToString(), Convert.ToDouble(reader["MON_TARIFA"]));
                            listaTemporal.Add(item);
                        }
                        connection.Close();//Se cierra la conexión
                        listaRetorno = listaTemporal;
                    }
                }
            }
            return listaRetorno;
        }

        /// <summary>
        /// Función que retorna un listado con la información que respecta a las tarifas de limpieza de vías
        /// </summary>
        /// <returns></returns>
        public List<TarifasLimpieza> obtenerTarifasLimpieza()
        {
            List<TarifasLimpieza> listaRetorno;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString))//Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_LimpiezaVias", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        List<TarifasLimpieza> listaTemporal = new List<TarifasLimpieza>();
                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            string[] strArray = reader["DSC_TARIFA"].ToString().Split('/');//Array con la descripción de la tarifa dividida por un '/'
                            TarifasLimpieza item = new TarifasLimpieza(Convert.ToDouble(reader["MON_TARIFA"]), (strArray[0] == "RESID") ? "Residencial" : "Comercial", strArray[1]);
                            listaTemporal.Add(item);
                        }
                        connection.Close();//Se cierra la conexión
                        listaRetorno = listaTemporal;
                    }
                }
            }
            return listaRetorno;
        }

        /// <summary>
        /// Función que obtiene un listado de los trámites realizados por un contribuyente a partir de su número de cédula o el número de trámite. Además se envía
        /// la dirección IP del cliente para temas de control y bitácora.
        /// </summary>
        /// <param name="pCedula_pTramite"></param>
        /// <param name="ipCliente"></param>
        /// <returns></returns>
        public List<Tramite> obtenerTramites(string pCedula_pTramite, string ipCliente)
        {
            List<Tramite> listaRetorno;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString))//Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_consultaTramites", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    command.Parameters.Add(new SqlParameter("@Cedula_Tramite", pCedula_pTramite));//Se agrega el parámetro del número de cédula o número de trámite
                    command.Parameters.Add(new SqlParameter("@IpCliente", ipCliente));//Se agrega el parámetro de la dirección IP del cliente que hace la solicitud
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        List<Tramite> listaTemporal = new List<Tramite>();
                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            Tramite item = new Tramite(reader["Num_Tramite"].ToString(), reader["Fec_Solicitud"].ToString(), reader["Fec_Inicio"].ToString(), reader["Fec_Finalizacion"].ToString(), reader["Fec_Notificacion"].ToString(), reader["Dsc_Tipo"].ToString(), reader["Dsc_Estado"].ToString(), reader["NOM_CONTRIBUYENTE"].ToString(), reader["CED_CONTRIBUYENTE"].ToString());
                            listaTemporal.Add(item);
                        }
                        connection.Close();//Se cierra la conexión
                        listaRetorno = listaTemporal;
                    }
                }
            }
            return listaRetorno;
        }

        #endregion

        #region Utilidades

        /// <summary>
        /// Valida si la respuesta enviada del ReCaptcha es válida
        /// </summary>
        /// <param name="respuesta"></param>
        /// <returns></returns>
        public bool esCaptchaValido(string respuesta, int tipoSolicitud)
        {
            string recaptchaKey;

            if(tipoSolicitud == 1) //Movil
                recaptchaKey = "6LcsjC4UAAAAAKMy0HH0qIqr9tX-L4-DiynvqLPg";
            else //Web
                recaptchaKey = "6LezCCYTAAAAAD2fIKgE-eHGfvHjuujSW0id392Z";

            var requestString = string.Format("https://www.google.com/recaptcha/api/siteverify?secret=" + recaptchaKey + "&response=" + respuesta+"&remoteip=13.92.141.240");
            bool respuestaSolicitud = false;

            HttpWebRequest solicitud = (HttpWebRequest)WebRequest.Create(requestString);
            try
            {
                using (WebResponse respuestaCaptcha = solicitud.GetResponse())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ReCaptcha));
                    ReCaptcha recaptcha = serializer.ReadObject(respuestaCaptcha.GetResponseStream()) as ReCaptcha;
                    respuestaSolicitud = recaptcha.Success;

                }
                return respuestaSolicitud;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Función que permite calcular los intereses a partir del monto de cobro, su fecha de vencimiento, el porcentaje de intereses y la cantidad de días del mes actual
        /// </summary>
        /// <param name="montoCobro"></param>
        /// <param name="montoMulta"></param>
        /// <param name="conceptoCobro"></param>
        /// <param name="fecVencimiento"></param>
        /// <returns></returns>
        private int CalcularIntereses(double montoCobro, double montoMulta, string conceptoCobro, DateTime fecVencimiento)
        {
            int intereses = 0;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString)) //Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_CalcularInteres", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    command.Parameters.Add(new SqlParameter("@Mon_Cobro", montoCobro));
                    command.Parameters.Add(new SqlParameter("@Mon_Multa", montoMulta));
                    command.Parameters.Add(new SqlParameter("@ConceptoCobro", conceptoCobro));
                    command.Parameters.Add(new SqlParameter("@Fec_Vence", fecVencimiento));
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        while (reader.Read())
                        {
                            string test = reader["MONTO_INTERES"].ToString();
                            intereses = Convert.ToInt32(this.TruncateDecimal(Convert.ToDouble(reader["MONTO_INTERES"].ToString()), 0));
                        }
                    }
                }
            }
            return intereses;
        }

        /// <summary>
        /// Función que permite calcular la multa con la que cuenta un cobro, a partir del monto de cobro, fecha de vencimiento, porcentaje de multa y cantidad de días del mes
        /// </summary>
        /// <param name="montoCobro"></param>
        /// <param name="conceptoCobro"></param>
        /// <param name="fecVencimiento"></param>
        /// <returns></returns>
        private double CalcularMultas(double montoCobro, string conceptoCobro, DateTime fecVencimiento)
        {
            double multa = 0;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString)) //Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_CalcularMulta", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    command.Parameters.Add(new SqlParameter("@Mon_Cobro", montoCobro));
                    command.Parameters.Add(new SqlParameter("@ConceptoCobro", conceptoCobro));
                    command.Parameters.Add(new SqlParameter("@Fec_Vence", fecVencimiento));
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        while (reader.Read())
                        {
                            multa = Convert.ToDouble(reader["MONTO_MULTA"].ToString());
                        }
                    }
                }
            }
            return multa;
        }

        /// <summary>
        /// Función para calcular cargos por concepto de timbres
        /// </summary>
        /// <param name="montoCobro"></param>
        /// <param name="conceptoCobro"></param>
        /// <param name="fecVencimiento"></param>
        /// <returns></returns>
        private double CalcularTimbres(double montoCobro, string conceptoCobro, DateTime fecVencimiento)
        {
            double timbres = 0;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString)) //Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_CalcularTimbres", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    command.Parameters.Add(new SqlParameter("@Mon_Cobro", montoCobro));
                    command.Parameters.Add(new SqlParameter("@ConceptoCobro", conceptoCobro));
                    command.Parameters.Add(new SqlParameter("@Fec_Vence", fecVencimiento));
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        while (reader.Read())
                        {
                            timbres = Convert.ToDouble(reader["MONTO_TIMBRES"].ToString());
                        }
                    }
                }
            }
            return timbres;
        }

        /// <summary>
        /// Función que permite calcular el descuento 
        /// </summary>
        /// <param name="montoCobro"></param>
        /// <param name="conceptoCobro"></param>
        /// <param name="fecVencimiento"></param>
        /// <returns></returns>
        private double CalcularDescuento(double montoCobro, string conceptoCobro, DateTime fecVencimiento)
        {
            double descuento = 0;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString)) //Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_CalcularDescuento", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    command.Parameters.Add(new SqlParameter("@Mon_Cobro", montoCobro));
                    command.Parameters.Add(new SqlParameter("@ConceptoCobro", conceptoCobro));
                    command.Parameters.Add(new SqlParameter("@Fec_Vence", fecVencimiento));
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        while (reader.Read())
                        {
                            descuento = Convert.ToDouble(reader["MONTO_DESCUENTO"].ToString());
                        }
                    }
                }
            }
            return descuento;
        }

        /// <summary>
        /// Permite truncar decimales
        /// </summary>
        /// <param name="value"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public double TruncateDecimal(double value, int precision)
        {
            double step = (double)Math.Pow(10, precision);
            double tmp = Math.Truncate(step * value);
            return tmp / step;
        }

        /// <summary>
        /// Obtiene todas las partes del archivo y lo arma para posteriormente ser almacenado en disco
        /// </summary>
        /// <param name="pNombreArchivo"></param>
        /// <param name="pFormato"></param>
        /// <param name="numCedula"></param>

        public void ArmarArchivo(string pNombreArchivo, string pFormato, string numCedula, string nomContribuyente, string tipoPatente, string correoContribuyente, string telefonoContribuyente)
        {
            string archivoFinal = "";
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString)) //Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_obtenerPaquetesArchivo", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    command.Parameters.Add(new SqlParameter("@nombreArchivo", pNombreArchivo));//Se agrega parámetro de cédula
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        while (reader.Read())
                        {
                            archivoFinal += reader["Informacion_PartesArchivo"].ToString();
                        }
                    }
                }
                connection.Close();
            }
            try
            {
                using (var impersonation = new ImpersonatedUser("tramites", "mpz", "8waFepHU8hag"))
                {
                    string nombreCarpeta = numCedula;
                    nombreCarpeta += "-" + DateTime.Now.ToString().Replace("/", "");
                    nombreCarpeta = nombreCarpeta.Replace(" ", "");
                    nombreCarpeta = nombreCarpeta.Replace(":", "");
                    string rutaCarpeta = "\\\\172.19.0.67\\MPZStoreOnceTramites\\Usos de suelo\\" + nombreCarpeta;
                    System.IO.Directory.CreateDirectory(rutaCarpeta);
                    string rutaArchivo = "\\\\172.19.0.67\\MPZStoreOnceTramites\\Usos de suelo\\" + nombreCarpeta + "\\" + pNombreArchivo + "." + pFormato;
                    FileInfo fi = new FileInfo(rutaArchivo);
                    FileStream fstr = fi.Create();
                    Bitmap bmp = new Bitmap(50, 50);
                    bmp.Save(fstr, ImageFormat.Png);
                    fstr.Close();
                    fi.Delete();
                    Byte[] bytes = Convert.FromBase64String(archivoFinal);
                    File.WriteAllBytes(rutaArchivo, bytes);

                    Respuesta enviarCorreo = this.enviarExchangeEmail("Uso de suelo", nomContribuyente, numCedula, tipoPatente, rutaArchivo, correoContribuyente, telefonoContribuyente);
                    using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString)) //Se realiza la conexión a la BD
                    {
                        connection.Open();//Se abre la conexión
                        using (SqlCommand command = new SqlCommand("sp_RegistrarSolicitudUsoSuelo", connection))//Se llama el procedimiento almacenado
                        {
                            command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                            command.Parameters.Add(new SqlParameter("@Cedula", numCedula));//Se agrega parámetro de cédula
                            command.Parameters.Add(new SqlParameter("@RutaArchivo", rutaArchivo));
                            command.Parameters.Add(new SqlParameter("@IpCliente", " "));
                            command.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                }

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString)) //Se realiza la conexión a la BD
                {
                    connection.Open();//Se abre la conexión
                    using (SqlCommand command = new SqlCommand("sp_EliminarArchivo", connection))//Se llama el procedimiento almacenado
                    {
                        command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                        command.Parameters.Add(new SqlParameter("@NombreArchivo", pNombreArchivo));//Se agrega parámetro de cédula
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

        }

        /// <summary>
        /// Permite determinar si un archivo está completo para ser almacenado
        /// </summary>
        /// <param name="pNombreArchivo"></param>
        /// <param name="pCantPartes"></param>
        /// <returns></returns>
        public bool archivoCompleto(string pNombreArchivo, int pCantPartes)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Db_ConnectionString"].ConnectionString)) //Se realiza la conexión a la BD
            {
                connection.Open();//Se abre la conexión
                using (SqlCommand command = new SqlCommand("sp_ObtenerCantidadPartesArchivo", connection))//Se llama el procedimiento almacenado
                {
                    command.CommandType = CommandType.StoredProcedure;//Se especifica que es un procedimiento almacenado
                    command.Parameters.Add(new SqlParameter("@NombreArchivo", pNombreArchivo));//Se agrega parámetro de cédula
                    using (SqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        while (reader.Read())
                        {
                            if (pCantPartes == Convert.ToInt32(reader["CANTIDAD"]))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return false;
        }

        #endregion

        #region Pago electrónico

        //private string privateToken = "4xEf3yfg2zit7rst";
        private int[] privateTokenArray = { 4, 3, 2, 7 };


        [WebMethod]
        public void convertirHtmlAPdf()
        {
            try
            {
                string html = System.IO.File.ReadAllText(base.Server.MapPath(@"./EmailTemplate/Recibo.html"));
                PdfDocument pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);
                pdf.Save("C:\\Test\\document.pdf");
                base.Context.Response.Write("OK");//Captcha inválido
            }
            catch (Exception ex)
            {
                base.Context.Response.Write("Error: " + ex.Message);//Captcha inválido
            }
        }



        [WebMethod]
        public void generarPago(string datos, string token)
        {
            if (this.validarToken(token))
            {
                string s = new JavaScriptSerializer().Serialize(new Respuesta("Works"));//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            else
            {
                string s = new JavaScriptSerializer().Serialize(new Respuesta("Error al validar token"));//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
        }

        public bool validarToken(string token)
        {
            string s = "";
            int[] respuestas = new int[5];
            int[] pos1 = new int[2];
            int[] pos2 = new int[2];
            int[] pos3 = new int[2];
            int[] pos4 = new int[2];
            for (int i = 0; i < token.Length; i++)
            {
                int temporal = (int)Char.GetNumericValue(token[i]);
                s += temporal.ToString();
                s += "-";
            }

            s = s.Replace("--1--1--1-", ".");
            string[] value = s.Split('.');
            for(int i=0; i < value.Length; i++)
            {
                respuestas[i] = Convert.ToInt32(value[i].Replace("-", string.Empty));
            }

            switch(respuestas[4])
            {
                case 1:
                    pos1[0] = 1;
                    pos1[1] = 4;
                    pos2[0] = 2;
                    pos2[1] = 3;
                    pos3[0] = 3;
                    pos3[1] = 2;
                    pos4[0] = 4;
                    pos4[1] = 1;
                    break;
                case 2:
                    pos1[0] = 1;
                    pos1[1] = 3;
                    pos2[0] = 2;
                    pos2[1] = 1;
                    pos3[0] = 3;
                    pos3[1] = 4;
                    pos4[0] = 4;
                    pos4[1] = 2;
                    break;
                case 3:
                    pos1[0] = 1;
                    pos1[1] = 2;
                    pos2[0] = 2;
                    pos2[1] = 4;
                    pos3[0] = 3;
                    pos3[1] = 3;
                    pos4[0] = 4;
                    pos4[1] = 1;
                    break;
                case 4:
                    pos1[0] = 1;
                    pos1[1] = 1;
                    pos2[0] = 2;
                    pos2[1] = 4;
                    pos3[0] = 3;
                    pos3[1] = 3;
                    pos4[0] = 4;
                    pos4[1] = 2;
                    break;
            }
            if (respuestas[pos1[1]-1] % privateTokenArray[pos1[0]-1] == 0 && respuestas[pos2[1]-1] % privateTokenArray[pos2[0]-1] == 0 && respuestas[pos3[1]-1] % privateTokenArray[pos3[0]-1] == 0 && respuestas[pos4[1]-1] % privateTokenArray[pos4[0]-1] == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion




        #region Mapas

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void ObtenerUbicacionesActivas()
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string s = new JavaScriptSerializer().Serialize(this.obtenerMapasActivos());//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            catch (Exception ex)
            {
                base.Context.Response.Write(ex);
            }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void ObtenerCategoriasComercio()
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string s = new JavaScriptSerializer().Serialize(this.obtenerCategorias());//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            catch (Exception ex)
            {
                base.Context.Response.Write(ex);
            }
        }

        public List<Mapa> obtenerMapasActivos()
        {
            List<Mapa> listaUbicaciones = new List<Mapa>();
            using (MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Db_MapasConexion"].ConnectionString))// Se realiza la conexión a la BD
            {
                connection.Open();
                //string Query = "SELECT name, elements FROM l87uy_zoo_item WHERE state = 1";
                string Query = "SELECT  `l87uy_zoo_item`.`name` AS  `nombre_empresa` , `l87uy_zoo_item`.`elements` AS `datos_empresa`, `l87uy_zoo_item`.`alias`, `l87uy_zoo_category`.`name` AS  `categoria` , `l87uy_zoo_category`.`alias` AS  `alias_categoria` ,  `l87uy_zoo_item`.`alias` AS  `alias_empresa` ,  `l87uy_zoo_category_item`.`category_id` FROM  `l87uy_zoo_item` JOIN  `l87uy_zoo_category_item` ON  `l87uy_zoo_category_item`.`item_id` =  `l87uy_zoo_item`.`id` JOIN  `l87uy_zoo_category` ON  `l87uy_zoo_category`.`id` =  `l87uy_zoo_category_item`.`category_id` WHERE  `l87uy_zoo_category_item`.`category_id` > 0 AND `l87uy_zoo_item`.`state` = 1 ORDER BY `l87uy_zoo_item`.`name` ASC ,  `l87uy_zoo_category_item`.`category_id` ASC";
                using (MySqlCommand command = new MySqlCommand(Query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        List<Mapa> listaTemporal = new List<Mapa>();
                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            dynamic convertidor = JsonConvert.DeserializeObject(reader["datos_empresa"].ToString());

                            string ubicacion = "", telefono = "", descripcion = "", imagen = "";

                            if (convertidor["160bd40a-3e0e-48de-b6cd-56cdcc9db892"].ToString() != "{}") 
                            {
                                if (convertidor["160bd40a-3e0e-48de-b6cd-56cdcc9db892"]["location"].ToString() != "{}")
                                {
                                    ubicacion = convertidor["160bd40a-3e0e-48de-b6cd-56cdcc9db892"]["location"].ToString();
                                }
                            }
                            if (convertidor["b870164b-fe78-45b0-b840-8ebceb9b9cb6"].ToString() != "{}")
                            {
                                if (convertidor["b870164b-fe78-45b0-b840-8ebceb9b9cb6"]["0"].ToString() != "{}")
                                {
                                    if (convertidor["b870164b-fe78-45b0-b840-8ebceb9b9cb6"]["0"]["value"].ToString() != "{}")
                                    {
                                        telefono = convertidor["b870164b-fe78-45b0-b840-8ebceb9b9cb6"]["0"]["value"].ToString();
                                    }
                                }
                            }
                            if (convertidor["ed9cdd4c-ae8b-4ecb-bca7-e12a5153bc02"].ToString() != "{}")
                            {
                                if (convertidor["ed9cdd4c-ae8b-4ecb-bca7-e12a5153bc02"]["0"].ToString() != "{}")
                                {
                                    if (convertidor["ed9cdd4c-ae8b-4ecb-bca7-e12a5153bc02"]["0"]["value"].ToString() != "{}")
                                    {
                                        descripcion = convertidor["ed9cdd4c-ae8b-4ecb-bca7-e12a5153bc02"]["0"]["value"].ToString();
                                    }
                                }
                            }
                            if (convertidor["ffcc1c50-8dbd-4115-b463-b43bdcd44a57"].ToString() != "{}")
                            {
                                if (convertidor["ffcc1c50-8dbd-4115-b463-b43bdcd44a57"]["file"].ToString() != "{}")
                                {
                                    imagen = convertidor["ffcc1c50-8dbd-4115-b463-b43bdcd44a57"]["file"].ToString();
                                }
                            }

                            string[] arrayUbicacion = { "0", "0" };
                            if (ubicacion != "")
                            {
                                arrayUbicacion = ubicacion.Split(',');
                            }

                            string categoria = "default.png";
                            switch (reader["categoria"].ToString())
                            {
                                case "COMERCIOS":
                                    categoria = "comercio.png";
                                    break;
                                case "INSTITUCIONES PÚBLICAS":
                                    categoria = "institucion.png";
                                    break;
                                case "HOGAR":
                                    categoria = "hogar.png";
                                    break;
                                case "AUTOMOTRIZ":
                                    categoria = "taller.png";
                                    break;
                                case "SERVICIOS PROFESIONALES":
                                    categoria = "profesional.png";
                                    break;
                                case "EDUCACIÓN":
                                    categoria = "educacion.png";
                                    break;
                                case "TRANSPORTE":
                                    categoria = "transporte.png";
                                    break;
                                case "COMBUSTIBLE":
                                    categoria = "combustible.png";
                                    break;
                                case "SALUD":
                                    categoria = "salud.png";
                                    break;
                                case "TURISMO":
                                    categoria = "turismo.png";
                                    break;
                                case "ALIMENTACIÓN":
                                    categoria = "alimentacion.png";
                                    break;
                                case "HOSPEDAJE":
                                    categoria = "hospedaje.png";
                                    break;
                                case "INSTITUCIONES FINANCIERAS":
                                    categoria = "institucion.png";
                                    break;
                                case "TALLER":
                                    categoria = "taller.png";
                                    break;
                                case "TECNOLOGIA":
                                    categoria = "tecnologia.png";
                                    break;
                                case "SUPERMERCADO":
                                    categoria = "supermercado.png";
                                    break;
                                case "FERRETERIA":
                                    categoria = "comercio.png";
                                    break;
                                case "Universidades":
                                    categoria = "educacion.png";
                                    break;
                                case "RESTAURANTE":
                                    categoria = "alimentacion.png";
                                    break;
                                default:
                                    categoria = "default.png";
                                    break;
                            }
                            Mapa item = new Mapa(reader["nombre_empresa"].ToString(), telefono, descripcion, imagen, arrayUbicacion[0], arrayUbicacion[1].Substring(0,1) == " " ? arrayUbicacion[1].Substring(1) : arrayUbicacion[1], categoria, Convert.ToInt32(reader["category_id"]), reader["alias"].ToString());
                            if(item.latitud != "0" && item.longitud != "0")
                                listaTemporal.Add(item);           
                        }
                        connection.Close();
                        listaUbicaciones = listaTemporal;
                    }
                }
            }
            return listaUbicaciones;
        }

        public List<Categoria> obtenerCategorias()
        {
            List<Categoria> listaCategorias = new List<Categoria>();
            using (MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Db_MapasConexion"].ConnectionString))// Se realiza la conexión a la BD
            {
                connection.Open();
                //string Query = "SELECT name, elements FROM l87uy_zoo_item WHERE state = 1";
                string Query = "SELECT `l87uy_zoo_category`.`name`, `l87uy_zoo_category`.`id` FROM `l87uy_zoo_category` WHERE `l87uy_zoo_category`.`published`=1";
                using (MySqlCommand command = new MySqlCommand(Query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {
                        List<Categoria> listaTemporal = new List<Categoria>();
                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            Categoria categoria = new Categoria(Convert.ToInt32(reader["id"].ToString()), reader["name"].ToString());
                            listaTemporal.Add(categoria);
                        }
                        listaCategorias = listaTemporal;
                    }
                }
            }
            return listaCategorias;
        }
        #endregion



        #region Métodos de App Móvil

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void Obtener_Informacion(int id)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string s = new JavaScriptSerializer().Serialize(this.Obtener_Datos(id));//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            catch (Exception ex)
            {
                base.Context.Response.Write(ex);
            }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void Obtener_Categorias_Archivos(int id)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string s = new JavaScriptSerializer().Serialize(this.Obtener_Cat(id));//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            catch (Exception ex)
            {
                base.Context.Response.Write(ex);
            }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void Obtener_Archivos(int id)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string s = new JavaScriptSerializer().Serialize(this.Obtener_Arch(id));//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            catch (Exception ex)
            {
                base.Context.Response.Write(ex);
            }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void Obtener_Elementos_Home()
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string s = new JavaScriptSerializer().Serialize(this.Obtener_Home());//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            catch (Exception ex)
            {
                base.Context.Response.Write(ex);
            }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void Obtener_Categorias_Preguntas_Frecuentes()
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string s = new JavaScriptSerializer().Serialize(this.Obtener_Categorias_Preguntas());//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            catch (Exception ex)
            {
                base.Context.Response.Write(ex);
            }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void Obtener_Eventos_Agenda()
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string s = new JavaScriptSerializer().Serialize(this.Obtener_Eventos());//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            catch (Exception ex)
            {
                base.Context.Response.Write(ex);
            }
        }


        public List<Agenda_Eventos> Obtener_Eventos()
        {
            List<Agenda_Eventos> listaTiquetes = new List<Agenda_Eventos>(); //Lista de retorno

            using (MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["mpzapp"].ConnectionString))// Se realiza la conexión a la BD
            {
                connection.Open();

                string Query = "SELECT l87uy_icagenda_events.id AS id, l87uy_icagenda_events.title AS titulo,  l87uy_icagenda_category.title AS categoria, " +
                "l87uy_icagenda_events.image AS imagen, l87uy_icagenda_events.next AS fechaInicio, l87uy_icagenda_events.enddate AS fechaFin, l87uy_icagenda_events.email AS email, " +
                "l87uy_icagenda_events.place AS lugar, l87uy_icagenda_events.phone AS telefono, l87uy_icagenda_events.city AS ciudad, l87uy_icagenda_events.address AS direccion, l87uy_icagenda_events.lat AS latitud, " +
                "l87uy_icagenda_events.lng AS longitud, l87uy_icagenda_events.shortdesc AS descripCorta, l87uy_icagenda_events.desc AS descripcion " +
                "FROM l87uy_icagenda_events, l87uy_icagenda_category " +
                "WHERE l87uy_icagenda_events.state = 1 AND l87uy_icagenda_events.catid = l87uy_icagenda_category.id " +
                "ORDER BY l87uy_icagenda_events.ordering ASC ";

                using (MySqlCommand command = new MySqlCommand(Query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {

                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            Agenda_Eventos info = new Agenda_Eventos(Convert.ToInt32(reader["id"]),
                                                                                    reader["titulo"].ToString(),
                                                                                    reader["imagen"].ToString(),
                                                                                    reader["fechaInicio"].ToString(),
                                                                                    reader["email"].ToString(),
                                                                                    reader["lugar"].ToString(),
                                                                                    reader["telefono"].ToString(),
                                                                                    reader["ciudad"].ToString(),
                                                                                    reader["direccion"].ToString(),
                                                                                    reader["latitud"].ToString(),
                                                                                    reader["longitud"].ToString(),
                                                                                    reader["descripCorta"].ToString(),
                                                                                    reader["descripcion"].ToString(),
                                                                                    reader["categoria"].ToString());

                            listaTiquetes.Add(info);
                        }

                    }
                }
            }
            return listaTiquetes;
        }

        public List<Categoria_Preguntas> Obtener_Categorias_Preguntas()
        {
            List<Categoria_Preguntas> lista = new List<Categoria_Preguntas>(); //Lista de retorno

            using (MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["mpzapp"].ConnectionString))// Se realiza la conexión a la BD
            {
                connection.Open();

                string Query = "SELECT DISTINCT categoria.id AS id, categoria.title AS titulo, categoria.description AS descripcion, categoria.image as imagen FROM  l87uy_fsf_faq_cat categoria where categoria.published = 1 ORDER BY categoria.ordering";


                using (MySqlCommand command = new MySqlCommand(Query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {

                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            Categoria_Preguntas info = new Categoria_Preguntas(Convert.ToInt32(reader["id"]),
                                                                                reader["titulo"].ToString(),
                                                                                reader["descripcion"].ToString(),
                                                                                reader["imagen"].ToString());
                            lista.Add(info);
                        }

                    }
                }
            }
            return lista;
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void Obtener_Preguntas_Frecuentes(string ids)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string s = new JavaScriptSerializer().Serialize(this.Obtener_Preguntas(ids));//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            catch (Exception ex)
            {
                base.Context.Response.Write(ex);
            }
        }

        public List<Preguntas_Item> Obtener_Preguntas(string ids)
        {
            List<Preguntas_Item> lista = new List<Preguntas_Item>(); //Lista de retorno

            using (MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["mpzapp"].ConnectionString))// Se realiza la conexión a la BD
            {
                connection.Open();

                string Query = "SELECT DISTINCT itemPregunta.id AS id,itemPregunta.question	 AS pregunta, itemPregunta.answer AS respuesta FROM  l87uy_fsf_faq_faq itemPregunta, l87uy_fsf_faq_cat categoria where categoria.id in (" + ids + ") AND categoria.id = itemPregunta.faq_cat_id AND itemPregunta.published = 1 ORDER BY itemPregunta.ordering";


                using (MySqlCommand command = new MySqlCommand(Query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {

                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            Preguntas_Item info = new Preguntas_Item(Convert.ToInt32(reader["id"]),
                                                                                reader["pregunta"].ToString(),
                                                                                reader["respuesta"].ToString());
                            lista.Add(info);
                        }

                    }
                }
            }
            return lista;
        }

        public List<Menu_Items> Obtener_Home()
        {
            List<Menu_Items> lista = new List<Menu_Items>(); //Lista de retorno

            using (MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["mpzapp"].ConnectionString))// Se realiza la conexión a la BD
            {
                connection.Open();

                string Query = "SELECT menu_home.id AS id, " +
                "menu_home.menu_Imagen AS imagen, " +
                "menu_home.menu_titulo AS titulo, " +
                "menu_home.menu_navegar_id AS navegarA " +
                "FROM menu_home ORDER BY menu_home.menu_orden ASC ";

                using (MySqlCommand command = new MySqlCommand(Query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {

                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            Menu_Items item = new Menu_Items(Convert.ToInt32(reader["id"]),
                                                                                    reader["imagen"].ToString(),
                                                                                    reader["titulo"].ToString(),
                                                                                    reader["navegarA"].ToString());

                            lista.Add(item);
                        }

                    }
                }
            }
            return lista;
        }

        public List<ArchivoDesc> Obtener_Arch(int id)
        {
            List<ArchivoDesc> listaArchivos = new List<ArchivoDesc>(); //Lista de retorno

            using (MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["mpzapp"].ConnectionString))// Se realiza la conexión a la BD
            {
                connection.Open();

                string Query = "SELECT l87uy_phocadownload.title AS titulo, l87uy_phocadownload.id AS id, l87uy_phocadownload.filename AS nombreArchivo, l87uy_phocadownload.date AS fecha, l87uy_phocadownload.filesize as tamanno FROM l87uy_phocadownload WHERE l87uy_phocadownload.catid = " + id + " AND l87uy_phocadownload.published = 1 ORDER BY l87uy_phocadownload.ordering";


                using (MySqlCommand command = new MySqlCommand(Query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {

                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            ArchivoDesc arch = new ArchivoDesc(Convert.ToInt32(reader["id"]),
                                                                    reader["nombreArchivo"].ToString(),
                                                                    reader["titulo"].ToString(),
                                                                    reader["fecha"].ToString(),
                                                                    reader["tamanno"].ToString());
                            listaArchivos.Add(arch);
                        }

                    }
                }
            }
            return listaArchivos;
        }



        public List<Archivo> Obtener_Cat(int id)
        {
            List<Archivo> listaTiquetes = new List<Archivo>(); //Lista de retorno

            using (MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["mpzapp"].ConnectionString))// Se realiza la conexión a la BD
            {
                connection.Open();
                string Query = "SELECT l87uy_phocadownload_categories.title AS titulo , l87uy_phocadownload_categories.id AS id FROM l87uy_phocadownload_categories where l87uy_phocadownload_categories.parent_id = " + id + " AND l87uy_phocadownload_categories.published = 1 ORDER BY l87uy_phocadownload_categories.ordering ASC";


                using (MySqlCommand command = new MySqlCommand(Query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {

                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            Archivo info = new Archivo(Convert.ToInt32(reader["id"]),
                                                    reader["titulo"].ToString());
                            listaTiquetes.Add(info);
                        }

                    }
                }
            }
            return listaTiquetes;
        }

        public Informacion Obtener_Datos(int id)
        {
            Informacion info = null;

            using (MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["mpzapp"].ConnectionString))// Se realiza la conexión a la BD
            {
                connection.Open();
                string Query = "SELECT l87uy_content.title AS titulo , l87uy_content.introtext AS texto FROM l87uy_content where l87uy_content.id = " + id;


                using (MySqlCommand command = new MySqlCommand(Query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {

                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            info = new Informacion(reader["titulo"].ToString(),
                                                              reader["texto"].ToString());

                        }

                    }
                }
            }
            return info;
        }


        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void Obtener_Informacion_modulos(int id)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string s = new JavaScriptSerializer().Serialize(this.Obtener_Datos_modulos(id));//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            catch (Exception ex)
            {
                base.Context.Response.Write(ex);
            }
        }

        public Informacion Obtener_Datos_modulos(int id)
        {
            Informacion info = null;

            using (MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["mpzapp"].ConnectionString))// Se realiza la conexión a la BD
            {
                connection.Open();
                string Query = "SELECT l87uy_modules.title AS titulo , l87uy_modules.content AS texto FROM l87uy_modules where l87uy_modules.id = " + id;


                using (MySqlCommand command = new MySqlCommand(Query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {

                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            info = new Informacion(reader["titulo"].ToString(),
                                                    reader["texto"].ToString());

                        }

                    }
                }
            }
            return info;
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void Obtener_Items_Directorio(string id)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string s = new JavaScriptSerializer().Serialize(this.Obtener_items_Directorio(id));//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            catch (Exception ex)
            {
                base.Context.Response.Write(ex);
            }
        }

        public List<DirectorioItems> Obtener_items_Directorio(string ids)
        {
            List<DirectorioItems> lista = new List<DirectorioItems>(); //Lista de retorno

            using (MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["mpzapp"].ConnectionString))// Se realiza la conexión a la BD
            {
                connection.Open();

                string Query = "SELECT DISTINCT item.id AS id,item.name AS nombre, item.elements AS elementos FROM  l87uy_zoo_category categoria, l87uy_zoo_category_item cat_item,l87uy_zoo_item item where categoria.id in (" + ids + ") AND categoria.id = cat_item.category_id AND cat_item.item_id = item.id AND item.state = 1";


                using (MySqlCommand command = new MySqlCommand(Query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {

                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            DirectorioItems info = new DirectorioItems(Convert.ToInt32(reader["id"]),
                                                                                reader["elementos"].ToString(),
                                                                                reader["nombre"].ToString());
                            lista.Add(info);
                        }

                    }
                }
            }
            return lista;
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void Obtener_Categorias_Directorio()
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string s = new JavaScriptSerializer().Serialize(this.Obtener_Categ_Directorio());//Cadena de retorno
                s = base.Context.Request.QueryString["callback"] + s;
                base.Context.Response.Clear();
                base.Context.Response.ContentType = "application/json";//Formato JSon
                base.Context.Response.Flush();
                base.Context.Response.Write(s);//Escribir la respuesta en el contexto que se brinda
            }
            catch (Exception ex)
            {
                base.Context.Response.Write(ex);
            }
        }


        public List<DirectorioComercial> Obtener_Categ_Directorio()
        {
            List<DirectorioComercial> listaTiquetes = new List<DirectorioComercial>(); //Lista de retorno

            using (MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["mpzapp"].ConnectionString))// Se realiza la conexión a la BD
            {
                connection.Open();
                string Query = "SELECT l87uy_zoo_category.name AS nombre, l87uy_zoo_category.id AS id FROM l87uy_zoo_category where l87uy_zoo_category.application_id = 9 ORDER BY l87uy_zoo_category.ordering ASC";


                using (MySqlCommand command = new MySqlCommand(Query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())//Se ejecuta y se carga la información en el reader
                    {

                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            DirectorioComercial info = new DirectorioComercial(Convert.ToInt32(reader["id"]),
                                                                                reader["nombre"].ToString());
                            listaTiquetes.Add(info);
                        }

                    }
                }
            }
            return listaTiquetes;
        }



        #endregion
    }
}
