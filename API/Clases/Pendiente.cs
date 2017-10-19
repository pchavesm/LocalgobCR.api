using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class Pendiente
    {
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
        public bool tienePendientes;

        public Pendiente()
        {
        }

        public Pendiente(string pCodigo, string pPeriodo, string pAnno, string pEstado, double pMonto, string pCodigo_Estandar, string pVencimiento, string pCedula, string pContribuyente, string pDescuento, double pMulta)
        {
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

        public Pendiente(string pCodigo, string pPeriodo, string pAnno, string pEstado, double pMonto, string pCodigo_Estandar, string pVencimiento, double pIntereses, string pCedula, string pContribuyente, string pDescuento, double pMulta)
        {
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
        }

        public Pendiente(bool pTienePendiente)
        {
            this.tienePendientes = pTienePendiente;
        }
    }
}