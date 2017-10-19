using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class Patente
    {
        public string descripcion;
        public string direccion;
        public string distrito;
        public string estado;
        public string nombre;
        public string num_folio;
        public int num_patente;

        public Patente()
        {
        }

        public Patente(int pNum_patente, string pDescripcion, string pNombre, string pDireccion, string pFolio, string pDistrito, string pEstado)
        {
            this.num_patente = pNum_patente;
            this.descripcion = pDescripcion;
            this.nombre = pNombre;
            this.direccion = pDireccion;
            this.num_folio = pFolio;
            this.distrito = pDistrito;
            this.estado = pEstado;
        }
    }
}