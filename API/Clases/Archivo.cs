using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class Archivo
    {
        public int idArchivo;
        public string nombre;

        public Archivo(int x_idArchivo, string x_nombre)
        {
            this.idArchivo = x_idArchivo;
            this.nombre = x_nombre;
        }
    }
}