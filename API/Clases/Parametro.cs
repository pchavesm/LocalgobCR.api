using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class Parametro
    {
        public string clave;
        public string valor;

        public Parametro() { }

        public Parametro(string clave, string valor)
        {
            this.clave = clave;
            this.valor = valor;
        }
    }
}