using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP279
{
    public class ComparadorEvento : IComparer<Evento>
    {
        public int Compare(Evento x, Evento y)
        {

            int comparacion = x.horaEvento.CompareTo(y.horaEvento);

            //     Número con signo que indica los valores relativos de esta instancia y value.Valor
            //     Descripción Un entero negativo Esta instancia es menor que value. Zero Esta instancia
            //     es igual a value. Un entero positivo. Esta instancia es mayor que value.
            //if (comparacion == 0)
            //{
            //    return 1;
            //}

            return comparacion;
        }
    }
}
