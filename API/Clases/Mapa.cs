using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class Mapa
    {
        public string nombre;
        public string alias;
        public string telefono;
        public string descripcion;
        public string imagen;
        public string latitud;
        public string longitud;
        public string categoria;
        public int idCategoria;


        public Mapa(string pNombre, string pTelefono, string pDescripcion, string pImagen, string pLatitud, string pLongitud, string pCategoria, int pIdCategoria, string pAlias)
        {
            this.nombre = pNombre;
            this.descripcion = pDescripcion;
            this.latitud = pLatitud;
            this.longitud = pLongitud;
            this.telefono = pTelefono;
            this.imagen = pImagen;
            this.categoria = pCategoria;
            this.idCategoria = pIdCategoria;
            this.alias = pAlias;
        }

        public Mapa() { }
    }
}