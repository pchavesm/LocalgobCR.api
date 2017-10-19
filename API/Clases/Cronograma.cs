using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class Cronograma
    {
        public string codigo;
        public string fecha1;
        public string fecha10;
        public string fecha11;
        public string fecha12;
        public string fecha2;
        public string fecha3;
        public string fecha4;
        public string fecha5;
        public string fecha6;
        public string fecha7;
        public string fecha8;
        public string fecha9;

        public Cronograma()
        {
        }

        public Cronograma(string codigo, string fecha1, string fecha2 = "Vacio", string fecha3 = "Vacio", string fecha4 = "Vacio", string fecha5 = "Vacio", string fecha6 = "Vacio", string fecha7 = "Vacio", string fecha8 = "Vacio", string fecha9 = "Vacio", string fecha10 = "Vacio", string fecha11 = "Vacio", string fecha12 = "Vacio")
        {
            this.codigo = codigo;
            this.fecha1 = fecha1;
            this.fecha2 = fecha2;
            this.fecha3 = fecha3;
            this.fecha4 = fecha4;
            this.fecha5 = fecha5;
            this.fecha6 = fecha6;
            this.fecha7 = fecha7;
            this.fecha8 = fecha8;
            this.fecha9 = fecha9;
            this.fecha10 = fecha10;
            this.fecha11 = fecha11;
            this.fecha12 = fecha12;
        }
    }
}