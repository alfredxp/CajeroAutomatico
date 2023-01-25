using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cajero.Utils
{
    public static class Constantes
    {
        public const string TIPO_TRANS_DEPOSIT = "DEPOSITO";
        public const string TIPO_TRANS_RETIRO = "RETIRO";
    }

    public static class Utilidades
    {
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}