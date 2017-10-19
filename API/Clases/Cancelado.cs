using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class Cancelado
    {
        public string anno_cobro;
        public string ced_contribuyente;
        public string cod_estandar;
        public string codigo_usuario;
        public string concepto;
        public string fecha;
        public double monto;
        public string nom_contribuyente;
        public int num_periodo;
        public string num_recibo;

        public Cancelado()
        {
        }

        public Cancelado(double pMonto, string pFecha, string pRecibo, string pCod_Usuario, string pCod_estandar, int pNum_periodo, string pAnno, string pConcepto, string pNom_contribuyente, string pCed_contribuyente)
        {
            this.monto = pMonto;
            this.fecha = pFecha;
            this.num_recibo = pRecibo;
            this.codigo_usuario = pCod_Usuario;
            this.cod_estandar = pCod_estandar;
            this.num_periodo = pNum_periodo;
            this.anno_cobro = pAnno;
            this.concepto = pConcepto;
            this.nom_contribuyente = pNom_contribuyente;
            this.ced_contribuyente = pCed_contribuyente;
        }
    }
}