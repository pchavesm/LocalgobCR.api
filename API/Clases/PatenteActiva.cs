using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class PatenteActiva
    {
        public string Ced_Contribuyente;
        public string Descripcion;
        public string Modalidad;
        public string Direccion;
        public string Nombre;
        public string Telefono;
        public string Fax;
        public string Email;
        public string Apartado;
        public string Distrito;
        public string Nom_Contribuyente;

        public PatenteActiva() { }

        public PatenteActiva(string pCedula, string pDescripcion, string pModalidad, string pDireccion, string pNombre, string pTelefono, string pFax, string pEmail, string pApartado, string pDistrito, string pNombreContribuyente)
        {
            this.Ced_Contribuyente = pCedula;
            this.Descripcion = pDescripcion;
            this.Modalidad = pModalidad;
            this.Direccion = pDireccion;
            this.Nombre = pNombre;
            this.Telefono = pTelefono;
            this.Fax = pFax;
            this.Email = pEmail;
            this.Apartado = pApartado;
            this.Distrito = pDistrito;
            this.Nom_Contribuyente = pNombreContribuyente;
        }
    }
}