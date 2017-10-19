using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class DirectorioItems
    {
        public string elementos;
        public string nombre;

        public DirectorioItems(string elementos, string nombre)
        {
            this.elementos = elementos;
            this.nombre = nombre;
        }
    }
}