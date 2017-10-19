using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class PermisoConstruccion
    {
        public string can_areaconstruccion;
        public string can_metrosdefrente;
        public string ced_contribuyente;
        public string ced_ingeniero;
        public string dsc_usoobra;
        public string fec_solicitud;
        public string ind_estadopermiso;
        public string nom_contribuyente;
        public string nom_ingeniero;
        public string num_asociado;
        public string num_folioreal;
        public string num_permiso;
        public string tip_obra;

        public PermisoConstruccion()
        {
        }

        public PermisoConstruccion(string Pnum_permiso, string Pced_contribuyente, string Pnum_folioreal, string Pfec_solicitud, string Pcan_metrosdefrente, string Ptip_obra, string Pdsc_usoobra, string Pcan_areaconstruccion, string Pnom_ingeniero, string Pced_ingeniero, string Pnum_asociado, string Pind_estadopermiso, string Pnom_contribuyente)
        {
            this.num_permiso = Pnum_permiso;
            this.ced_contribuyente = Pced_contribuyente;
            this.num_folioreal = Pnum_folioreal;
            this.fec_solicitud = Pfec_solicitud;
            this.can_metrosdefrente = Pcan_metrosdefrente;
            this.tip_obra = Ptip_obra;
            this.dsc_usoobra = Pdsc_usoobra;
            this.can_areaconstruccion = Pcan_areaconstruccion;
            this.nom_ingeniero = Pnom_ingeniero;
            this.ced_ingeniero = Pced_ingeniero;
            this.num_asociado = Pnum_asociado;
            this.ind_estadopermiso = Pind_estadopermiso;
            this.nom_contribuyente = Pnom_contribuyente;
        }
    }
}