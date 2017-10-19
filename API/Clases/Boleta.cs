using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class Boleta
    {
        public string cod_ubicacion1;
        public string cod_ubicacion2;
        public string cod_ubicacion3;
        public string fecha;
        public string ind_estado;
        public string monto;
        public string num_boleta;
        public string num_placa;
        public string tip_placa;

        public Boleta()
        {
        }

        public Boleta(string pNum_boleta, string pTip_placa, string pNum_placa, string pInd_estado, string pCod_ubicacion1, string pCod_ubicacion2, string pCod_ubicacion3, string pMonto, string pFecha)
        {
            this.num_boleta = pNum_boleta;
            this.tip_placa = pTip_placa;
            this.num_placa = pNum_placa;
            this.cod_ubicacion1 = pCod_ubicacion1;
            this.cod_ubicacion2 = pCod_ubicacion2;
            this.cod_ubicacion3 = pCod_ubicacion3;
            this.ind_estado = pInd_estado;
            this.monto = pMonto;
            this.fecha = pFecha;
        }
    }
}