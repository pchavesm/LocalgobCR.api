using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class TarifasCementerio
    {
        public double Monto;
        public string Tip_Tarifa;

        public TarifasCementerio()
        {
        }

        public TarifasCementerio(string pTipo, double pMonto)
        {
            this.Tip_Tarifa = pTipo;
            this.Monto = pMonto;
        }
    }
}