using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class ArchivoDesc
    {
        public int idArchivo;
        public string nombreArchivo;
        public string titulo;
        public string fecha;
        public string tamanno;

        public ArchivoDesc(int x_idArchivo, string x_nombreArchivo, string x_titulo, string x_fecha, string x_tamanno)
        {
            this.idArchivo = x_idArchivo;
            this.nombreArchivo = x_nombreArchivo;
            this.titulo = x_titulo;
            this.fecha = x_fecha;
            this.tamanno = x_tamanno;
        }
    }
}