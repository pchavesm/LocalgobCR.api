using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class TarifasBasura
    {
        public string Codigo;
        public double Monto;

        public TarifasBasura()
        {
        }

        public TarifasBasura(string pCodigo, double pMonto)
        {
            this.Codigo = pCodigo;
            this.Monto = pMonto;
        }
    }
}