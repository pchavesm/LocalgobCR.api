using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class TipoPatente
    {
        public int id;
        public string descripcion;

        public TipoPatente() { }

        public TipoPatente(int id, string descripcion)
        {
            this.id = id;
            this.descripcion = descripcion;
        }
    }
}