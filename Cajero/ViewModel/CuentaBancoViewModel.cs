using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cajero.ViewModel
{
    public class CuentaBancoViewModel
    {
        public int Id { get; set; }
        public int Balance { get; set; }
        [Display(Name ="Numero de Cuenta")]
        public string NoCuenta { get; set; }
        public string PIN { get; set; }
        public string IdUser { get; set; }
        [Display(Name ="Depósito")]
        public int Deposito { get; set; }
        public int Retiro { get; set; }
        public string Cedula { get; set; }
        [Display(Name = "Tipo de cuenta")]
        public string typeAccount { get; set; }

        public bool? cien { get; set; }
        public bool? doscientos { get; set; }
        public bool? quinientos { get; set; }
        public bool? mil { get; set; }
        public bool? dosmil { get; set; }
    }

    public enum TipoCuenta
    {
        CORRIENTE,
        AHORRO
    }
}