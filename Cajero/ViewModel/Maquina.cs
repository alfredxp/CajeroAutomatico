using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cajero.ViewModel
{
    public class Maquina
    {
        public int idATM { get; set; }
        public int idAdmin { get; set; }
        public string direccion { get; set; }
        public string billete { get; set; }
        public int cantidad { get; set; }
        public int balance { get; set; }

        public List<MonedaDetalle> monedaDetalles { get; set; }

        public partial class MonedaDetalle
        {
            public int idMonedaDetalle { get; set; }
            public int Billete { get; set; }
            public int Cantidad { get; set; }
            public int idATM { get; set; }
        }

    }

    public enum Billetes
    {
        CIEN,
        DOSCIENTOS,
        QUINIENTOS,
        MIL,
        DOSMIL
    }
}