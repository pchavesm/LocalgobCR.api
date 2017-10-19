using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class Contribuyente
    {
        public string Cedula;
        public string Nombre;

        public Contribuyente()
        {
        }

        public Contribuyente(string pNombre, string pCedula)
        {
            this.Nombre = pNombre;
            this.Cedula = pCedula;
        }
    }
}