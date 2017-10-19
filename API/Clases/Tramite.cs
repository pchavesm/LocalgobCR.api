using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class Tramite
    {
        public string ced_contribuyente;
        public string estado;
        public string fec_fin;
        public string fec_inicio;
        public string fec_notificacion;
        public string fec_solicitud;
        public string nom_contribuyente;
        public string num_tramite;
        public string tipo;

        public Tramite()
        {
        }

        public Tramite(string pNum_tramite, string pFec_solic, string pFec_inicio, string fec_fin, string pFec_notificacion, string pTipo, string pEstado, string pNom_contribuyente, string pCed_contribuyente)
        {
            this.num_tramite = pNum_tramite;
            this.fec_solicitud = pFec_solic;
            this.fec_inicio = pFec_inicio;
            this.fec_fin = fec_fin;
            this.fec_notificacion = pFec_notificacion;
            this.tipo = pTipo;
            this.estado = pEstado;
            this.nom_contribuyente = pNom_contribuyente;
            this.ced_contribuyente = pCed_contribuyente;
        }
    }
}