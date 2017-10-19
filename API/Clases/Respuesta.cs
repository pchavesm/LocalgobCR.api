using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class Respuesta
    {
        public string mensaje;
        public int numParte;

        public Respuesta () { }

        public Respuesta(string pMensaje)
        {
            this.mensaje = pMensaje;
        }

        public Respuesta(string pMensaje, int pParte)
        {
            this.mensaje = pMensaje;
            this.numParte = pParte;
        }
    }
}