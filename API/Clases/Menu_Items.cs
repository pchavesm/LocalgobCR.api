using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class Menu_Items
    {
        public int id;
        public string titulo;
        public string imagen;
        public string navegar_a;


        public Menu_Items() { }

        public Menu_Items(int id, string imagen, string titulo, string navegar_a)
        {
            this.id = id;
            this.titulo = titulo;
            this.imagen = imagen;
            this.navegar_a = navegar_a;
        }
    }
}