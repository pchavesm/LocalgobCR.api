using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class Categoria_Preguntas
    {
        public int id;
        public string titulo;
        public string descripcion;
        public string imagen;

        public Categoria_Preguntas(int id, string titulo, string descripcion, string imagen)
        {
            this.id = id;
            this.titulo = titulo;
            this.descripcion = descripcion;
            this.imagen = imagen;
        }
    }
}