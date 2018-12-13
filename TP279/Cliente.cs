using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP279
{
    public class Cliente
    {
        public Cliente(double horaLLegada)
        {
            this.horaLLegada = horaLLegada;
        }

        public double horaLLegada { get; set; }
        public double horaAtendido { get; set; }
        public string estado { get; set; }



    }
}
