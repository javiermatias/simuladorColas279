using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP279
{
    public class DistUniforme : Distribucion
    {
        public double mediaA { get; set; }
        public double mediaB { get; set; }
        public DistUniforme(Double mediaA, Double mediaB)
        {
            this.mediaA = mediaA;
            this.mediaB = mediaB;

        }
        public double devolverTiempo(double _random)
        {

            return this.mediaA + _random * (this.mediaB - this.mediaA);

        }
    }
}
