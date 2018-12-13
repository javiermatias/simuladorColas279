using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP279
{
    public class Vector
    {

        public Int32 ID { get; set; } = 0;
        public string Evento { get; set; } = "Inicio";
        public double Reloj { get; set; } = 0;
        public double Rnd { get; set; } = 0;
        public double TiempoLlegadaCliente { get; set; } = 0;
        public double ProxLlegada { get; set; } = 0;

        public double RndSurtidor1 { get; set; } = 0;

        public double TiempoAtencion1 { get; set; } = 0;

        public double FinAtencion1 { get; set; } = 0;

        public Int32  Cola1 { get; set; } = 0;
      
        public string Estado1 { get; set; } = "Libre";
        public double HoraInicioLibre1 { get; set; } = 0;

        public double Acumulador1 { get; set; } = 0;

        public double RndSurtidor2 { get; set; } = 0;

        public double TiempoAtencion2 { get; set; } = 0;

        public double FinAtencion2 { get; set; } = 0;

        public Int32 Cola2 { get; set; } = 0;

        public string Estado2 { get; set; } = "Libre";

        public double HoraInicioLibre2 { get; set; } = 0;

        public double Acumulador2 { get; set; } = 0;

        public double RndCargaNeumatico { get; set; } = 0;

        public string CargaNeumatico { get; set; } = "";

        public double RndNeumatico { get; set; } = 0;

        public double TiempoNeumatico { get; set; } = 0;

        public double FinNeumatico { get; set; } = 0;

        public string EstadoNeumatico { get; set; } = "Libre";

        public Int32 NoCargo { get; set; } = 0;
    }
}
