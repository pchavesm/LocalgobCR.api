using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPZ_API.Clases
{
    public class RespuestaPendientes
    {
        public int estado;

        public RespuestaPendientes(int pEstado)
        {
            this.estado = pEstado;
        }
    }
}