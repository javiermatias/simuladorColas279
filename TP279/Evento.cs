using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP279
{
    public class Evento
    {
        public Evento(string evento, string objeto, double rnd, double tiempoEvento, double horaEvento)
        {
            this.evento = evento;
            this.objeto = objeto;
            this.rnd = rnd;
            this.tiempoEvento = tiempoEvento;
            this.horaEvento = horaEvento;
        }

        public string evento { get; set; }
        public string objeto { get; set; }
        public double rnd { get; set; } = 0;
        public double tiempoEvento { get; set; }
        public double horaEvento { get; set; }





    }
}
