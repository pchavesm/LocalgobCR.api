using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class Propiedad
    {
        public double area;
        public double area_comun;
        public double area_construida;
        public double area_libre;
        public double area_privada;
        public string canton;
        public string colindandes;
        public string direccion;
        public string distrito;
        public string folio;
        public double fondo;
        public double frente;
        public double frente2;
        public string GIS;
        public double metros_limpieza;
        public string paga_basura;
        public string paga_impuestos;
        public string plano;
        public string provincia;
        public string tipo_zona;

        public Propiedad()
        {
        }

        public Propiedad(string pFolio, string pGIS, string pPlano, double pFrente, double pFrente2, double pFondo, double pArea, double pArea_privada, double pArea_comun, double pArea_construida, double pArea_libre, string pDireccion, string pPaga_impuestos, string pPaga_basura, double pMetros_limpieza, string pTipo_zona, string pConlindantes, string pProvincia, string pCanton, string pDistrito)
        {
            this.folio = pFolio;
            this.GIS = pGIS;
            this.plano = pPlano;
            this.frente = pFrente;
            this.frente2 = pFrente2;
            this.fondo = pFondo;
            this.area = pArea;
            this.area_privada = pArea_privada;
            this.area_comun = pArea_comun;
            this.area_construida = pArea_construida;
            this.area_libre = pArea_libre;
            this.direccion = pDireccion;
            this.paga_impuestos = pPaga_impuestos;
            this.paga_basura = pPaga_basura;
            this.metros_limpieza = pMetros_limpieza;
            this.tipo_zona = pTipo_zona;
            this.colindandes = pConlindantes;
            this.provincia = pProvincia;
            this.canton = pCanton;
            this.distrito = pDistrito;
        }
    }
}