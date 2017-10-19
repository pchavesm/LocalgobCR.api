using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class TarifasLimpieza
    {
        public string construccion;
        public double pendiente;
        public string zonas;

        public TarifasLimpieza()
        {
        }

        public TarifasLimpieza(double pPendiente, string pZonas, string pConstruccion)
        {
            this.pendiente = pPendiente;
            this.zonas = pZonas;
            this.construccion = pConstruccion;
        }
    }
}