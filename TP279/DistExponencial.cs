using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP279
{
    public class DistExponencial : Distribucion
    {

        public DistExponencial(double _media)
        {

            this.media = _media;
        }

        public double media { get; set; }
        public double devolverTiempo(double _random)
        {
            if (_random == 1)
            {
                _random = 0.99;
            }
            if (_random == 0)
            {
                _random = 0.01;
            }
            return (-this.media) * Math.Log(1 - _random);

        }
    }
}
