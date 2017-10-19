using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class Categoria
    {
        public int idCategoria;
        public string nombreCategoria;
        public Categoria() { }
        public Categoria(int pId, string pNombreCategoria)
        {
            this.idCategoria = pId;
            this.nombreCategoria = pNombreCategoria;
        }      
    }
}