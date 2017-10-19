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

namespace MPZ_API
{
    /// <summary>
    /// Summary description for MPZ_API
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [EnableCors("*","*","*")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
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
        public void TienePendientes(string pCedula, string respuesta, int tipoSolicitud)
        {
            if (this.esCaptchaValido(respuesta, tipoSolicitud) == true)
            {
                string s = new JavaScriptSerializer().Serialize(this.tienePendientes(pCedula));//Cadena de retorno
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
                        bool flag = false;//Para determinar descuento
                        while (reader.Read())//Se recorre el reader para leer cada uno de sus atributos
                        {
                            double descuento = 0.0;
                            DateTime fecVencimiento = Convert.ToDateTime(reader["FEC_VENCIMIENTO"].ToString());//Fecha de vencimiento del pago
                            double montoCobro = Convert.ToDouble(reader["MON_COBRO"]);
                            double multa = 0;

                            DateTime fecLimite = DateTime.MinValue;//Fecha límite para aplicar descuento

                            if (reader["FEC_LIMITE"].ToString() != String.Empty)//Si existe descuento
                            {
                                fecLimite = Convert.ToDateTime(reader["FEC_LIMITE"].ToString());
                            }


                            //Cálculo para el monto de multas


                            if(reader["POR_MULTAS"].ToString() != String.Empty)
                            {
                                multa = CalcularMultas(montoCobro, fecVencimiento, Convert.ToDouble(reader["POR_MULTAS"]), Convert.ToDouble(reader["Calculo"]));
                                montoCobro += multa;
                            }

                            //Cálculo para el monto de intereses

                            int intereses = CalcularIntereses(montoCobro, fecVencimiento, Convert.ToDouble(reader["POR_INTERESES"]), Convert.ToDouble(reader["Calculo"]));
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
                                    if (cronograma.Count < i+2 || cronograma.Count < i + 1)
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
                            if (fecLimite != DateTime.MinValue)
                            {
                                if (((fecLimite > DateTime.Now) && (fecVencimiento.Year.ToString() == str)) && (((reader["COD_CONCEPTOCOBRO"].ToString() == "CC-01") || (reader["COD_CONCEPTOCOBRO"].ToString() == "CC-02")) || (reader["COD_CONCEPTOCOBRO"].ToString() == "CC-03") || (reader["COD_CONCEPTOCOBRO"].ToString() == "CC-56")))// Si la fecha de vencimiento no ha pasado && Si es el año actual && (Si es el código CC-01, CC-02, CC-03 o CC-56)
                                {
                                    if (Convert.ToInt32(reader["NUM_PERIODO"]) == 1)//Si es el primer periodo del año
                                    {
                                        if (reader["POR_DESCUENTO"].ToString() == String.Empty)
                                            descuento = 0;
                                        else
                                            descuento = (Convert.ToDouble(reader["MON_COBRO"]) * Convert.ToDouble(reader["Descuento"])) / 100.0;//Cálculo de descuento
                                        flag = true;
                                    }
                                    else if (flag)         
                                    {
                                        if (reader["POR_DESCUENTO"].ToString() == String.Empty)
                                            descuento = 0;
                                        else
                                            descuento = (Convert.ToDouble(reader["MON_COBRO"]) * Convert.ToDouble(reader["Descuento"])) / 100.0;//Cálculo de descuento
                                    }
                                }
                            }
                            else
                            {
                                if (((fecVencimiento.Year.ToString() == str)) && (((reader["COD_CONCEPTOCOBRO"].ToString() == "CC-01") || (reader["COD_CONCEPTOCOBRO"].ToString() == "CC-02")) || (reader["COD_CONCEPTOCOBRO"].ToString() == "CC-03") || (reader["COD_CONCEPTOCOBRO"].ToString() == "CC-56")))// Si la fecha de vencimiento no ha pasado && Si es el año actual && (Si es el código CC-01, CC-02, CC-03 o CC-56)
                                {
                                    if (Convert.ToInt32(reader["NUM_PERIODO"]) == 1)//Si es el primer periodo del año
                                    {
                                        if (reader["POR_DESCUENTO"].ToString() == String.Empty)
                                            descuento = 0;

                                        else
                                            descuento = (Convert.ToDouble(reader["MON_COBRO"]) * Convert.ToDouble(reader["Descuento"])) / 100.0;//Cálculo de descuento
                                        flag = true;
                                    }
                                    else if (flag)
                                    {
                                        if (reader["POR_DESCUENTO"].ToString() == String.Empty)
                                            descuento = 0;
                                        else
                                            descuento = (Convert.ToDouble(reader["MON_COBRO"]) * Convert.ToDouble(reader["Descuento"])) / 100.0;//Cálculo de descuento
                                    }
                                }
                            }
                            Pendiente item = new Pendiente(reader["DSC_CONCEPTOCOBRO"].ToString(), reader["NUM_PERIODO"].ToString(), reader["AÑO_COBRO"].ToString(), pEstado == "" ? "Pendiente no vencido" : pEstado, Convert.ToDouble(reader["MON_COBRO"]), reader["COD_ESTANDAR"].ToString(), reader["FEC_VENCIMIENTO"].ToString(), (intereses > 0) ? ((double)intereses) : ((double)0), reader["CED_CONTRIBUYENTE"].ToString(), reader["NOM_CONTRIBUYENTE"].ToString(), descuento.ToString(), multa < (double)0 ? 0 : (double)multa);
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
                recaptchaKey = "CaptchaKey";
            else //Web
                recaptchaKey = "CaptchaKey";

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
        /// <param name="pMontoCobro"></param>
        /// <param name="pFecVencimiento"></param>
        /// <param name="pPorcentajeIntereses"></param>
        /// <param name="pCalculo"></param>
        /// <returns></returns>
        private int CalcularIntereses(double pMontoCobro, DateTime pFecVencimiento, double pPorcentajeIntereses, double pCalculo)
        {
            int intereses = 0;
            int diferenciaMeses = (int)Math.Truncate(DateTime.Now.Subtract(pFecVencimiento).Days / (365.25 / 12));
            double porcentajeInteresesPorMes = (pMontoCobro * pPorcentajeIntereses) / 100;
            double calculoMeses = porcentajeInteresesPorMes * diferenciaMeses;
            double diasTranscurridos;
            if (DateTime.Now.Day > pFecVencimiento.Day)
            {
                diasTranscurridos = Convert.ToDouble(Math.Abs(DateTime.Now.Day - pFecVencimiento.Day)) / pCalculo;
            }
            else
            {
                diasTranscurridos = Convert.ToDouble(Math.Abs(DateTime.Now.Day)) / pCalculo;
            }
            double calculoDias = diasTranscurridos * porcentajeInteresesPorMes;
            // double Temporal = calculoMeses + calculoDias;
            intereses = (int)Math.Truncate(calculoMeses + calculoDias + 0.01);
            return intereses;
        }

        /// <summary>
        /// Función que permite calcular la multa con la que cuenta un cobro, a partir del monto de cobro, fecha de vencimiento, porcentaje de multa y cantidad de días del mes
        /// </summary>
        /// <param name="pMontoCobro"></param>
        /// <param name="pFecVencimiento"></param>
        /// <param name="pPorcentajeMultas"></param>
        /// <param name="pCalculo"></param>
        /// <returns></returns>
        private double CalcularMultas(double pMontoCobro, DateTime pFecVencimiento, double pPorcentajeMultas, double pCalculo)
        {
            int multa = 0;
            int diferenciaMeses = (int)Math.Truncate(DateTime.Now.Subtract(pFecVencimiento).Days / (365.25 / 12));
            double porcentajeMultasPorMes = (pMontoCobro * pPorcentajeMultas) / 100;
            double calculoMeses = porcentajeMultasPorMes * diferenciaMeses;
            double diasTranscurridos;
            if (DateTime.Now.Day > pFecVencimiento.Day)
            {
                diasTranscurridos = Convert.ToDouble(Math.Abs(DateTime.Now.Day - pFecVencimiento.Day)) / pCalculo;
            }
            else
            {
                diasTranscurridos = Convert.ToDouble(Math.Abs(DateTime.Now.Day)) / pCalculo;
            }
            double calculoDias = diasTranscurridos * porcentajeMultasPorMes;
            // double Temporal = calculoMeses + calculoDias;
            multa = (int)Math.Truncate(calculoMeses + calculoDias + 0.01);


            if (multa > ((pMontoCobro * 20) / 100))
            {
                return (pMontoCobro * 20) / 100;
            }
            else
            {
                return multa;
            }
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
    }
}
