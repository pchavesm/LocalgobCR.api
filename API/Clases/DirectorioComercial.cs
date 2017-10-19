using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class DirectorioComercial
    {
        public int id;
        public string nombre;

        public DirectorioComercial(int x_id, string x_nombre)
        {
            this.id = x_id;
            this.nombre = x_nombre;
        }
    }
}