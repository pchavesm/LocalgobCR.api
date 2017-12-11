using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class Pendiente
    {
        public string num_cobro;
        public string anno;
        public string cedula;
        public string codigo;
        public string codigo_estandar;
        public string contribuyente;
        public string descuento;
        public string estado;
        public string fecha_vencimiento;
        public double intereses;
        public double monto;
        public string periodo;
        public double multa;
        public double timbres;
        public string cod_conceptoCobro;
        public bool tienePendientes;


        public Pendiente()
        {
        }

        public Pendiente(string num_cobro, string pCodigo, string pPeriodo, string pAnno, string pEstado, double pMonto, string pCodigo_Estandar, string pVencimiento, string pCedula, string pContribuyente, string pDescuento, double pMulta)
        {
            this.num_cobro = num_cobro;
            this.codigo = pCodigo;
            this.periodo = pPeriodo;
            this.anno = pAnno;
            this.estado = pEstado;
            this.monto = pMonto;
            this.codigo_estandar = pCodigo_Estandar;
            this.fecha_vencimiento = pVencimiento;
            this.cedula = pCedula;
            this.contribuyente = pContribuyente;
            this.descuento = pDescuento;
            this.multa = pMulta;
        }

        public Pendiente(string num_cobro, string pCodigo, string pPeriodo, string pAnno, string pEstado, double pMonto, string pCodigo_Estandar, string pVencimiento, double pIntereses, string pCedula, string pContribuyente, string pDescuento, double pMulta, string cod_conceptoCobro, double pTimbres)
        {
            this.num_cobro = num_cobro;
            this.codigo = pCodigo;
            this.periodo = pPeriodo;
            this.anno = pAnno;
            this.estado = pEstado;
            this.monto = pMonto;
            this.codigo_estandar = pCodigo_Estandar;
            this.fecha_vencimiento = pVencimiento;
            this.intereses = pIntereses;
            this.cedula = pCedula;
            this.contribuyente = pContribuyente;
            this.descuento = pDescuento;
            this.multa = pMulta;
            this.cod_conceptoCobro = cod_conceptoCobro;
            this.timbres = pTimbres;
        }

        public Pendiente(bool pTienePendiente)
        {
            this.tienePendientes = pTienePendiente;
        }
    }
}