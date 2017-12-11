using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class Preguntas_Item
    {
        public int id;
        public string pregunta;
        public string respuesta;

        public Preguntas_Item(int id, string pregunta, string respuesta)
        {
            this.id = id;
            this.pregunta = pregunta;
            this.respuesta = respuesta;
        }
    }
}