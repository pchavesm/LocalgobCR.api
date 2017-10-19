
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class Informacion
    {

        public string titulo;
        public string texto;

        public Informacion() { }

        public Informacion(string _titulo, string _texto)
        {
            this.titulo = _titulo;
            this.texto = _texto;
        }

    }
}