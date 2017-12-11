using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class DirectorioItems
    {
        public int id;
        public string elementos;
        public string nombre;

        public DirectorioItems(int id, string elementos, string nombre)
        {
            this.id = id;
            this.elementos = elementos;
            this.nombre = nombre;
        }
    }
}