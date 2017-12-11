using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class Agenda_Eventos
    {
        public int id;
        public string titulo;
        public string imagen;
        public string fechaInicio;
        public string fechaFin;
        public string email;
        public string lugar;
        public string telefono;
        public string ciudad;
        public string direccion;
        public string latitud;
        public string longitud;
        public string descripCorta;
        public string descripcion;
        public string categoria;


        public Agenda_Eventos() { }

        public Agenda_Eventos(int id, string titulo, string imagen, string fechaInicio, string email, string lugar, string telefono, string ciudad, string direccion, string latitud,
            string longitud, string descripCorta, string descripcion, string categoria)
        {
            this.id = id;
            this.titulo = titulo;
            this.imagen = imagen;
            this.fechaInicio = fechaInicio;
            this.email = email;
            this.lugar = lugar;
            this.telefono = telefono;
            this.ciudad = ciudad;
            this.direccion = direccion;
            this.latitud = latitud;
            this.longitud = longitud;
            this.descripCorta = descripCorta;
            this.descripcion = descripcion;
            this.categoria = categoria;
        }
    }
}