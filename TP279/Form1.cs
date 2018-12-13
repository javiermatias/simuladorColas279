using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TP279
{
    public partial class Form1 : Form
    {
        #region parametrosInciales
        //Distribuciones
        DistExponencial llegada_cliente;
        DistExponencial fin_inflado;
        DistUniforme fin_atencion;

        //Parametros Iniciales
        Int32 cantidadSim=0;
        Int32  horasSim = 0;
        DataTable dtGrilla;
        Double reloj_sistema=0;
        int cantidadColumnas = 0;
        Int32 cantidadClientes = 0;
        int cantMaxClientes = 100;
        double sumatoriaTiempo = 0;
        double promedio = 0;
        Int32 clientesSalieron = 0;
        Random rnd;
        Vector vector;
        object[] fila;
        List<Cliente> lista_clientes;


        //Objetos
        enum Objetos
        {
            Cliente,Surtidor1,Surtidor2,Neumatico

        }
        #region eventos
        //Eventos
        enum Eventos
        {
            Inicio, LlegadaCliente, FinAtencion1, FinAtencion2, FinInflado
                
        }

        //Estados 
        enum EstadosClientes
        {
            EnColaSurtidor1, EnColaSurtidor2, SiendoAtendido1, SiendoAtendido2, Inflando, Fin

        }
        enum EstadosSurtidores
        {
            Libre, Ocupado

        }

        enum EstadosNeumaticos
        {
            Libre, Ocupado

        }
        #endregion


        #region colas
        //Colas
        //Cola de eventos, Ordena Automáticamente los eventos 
        SortedSet<Evento> colaEventos = new SortedSet<Evento>(new ComparadorEvento());
        //En las Colas
        Queue<Cliente> colaSurtidor1 = new Queue<Cliente>();
        Queue<Cliente> colaSurtidor2 = new Queue<Cliente>();
        //Guarda el cliente que esta siendo atendido en el surtidor
        Queue<Cliente> clienteSiendoAtendido1 = new Queue<Cliente>();
        Queue<Cliente> clienteSiendoAtendido2 = new Queue<Cliente>();
        //Guarda el cliente que esta siendo atendido en los neumaticos
        Queue<Cliente> clienteSiendoAtendidoNeumatico = new Queue<Cliente>();

        #endregion
        #endregion

        public Form1()
        {
            InitializeComponent();
            borrarResumen();
        }


        private void btn_simular_Click(object sender, EventArgs e)
        {
            inicio();
            if (chkSim.Checked)
            {
                simularPorIteraciones();
            }else
            {
                simularPorHoras();
            }
            
            mostrarGrilla();
            generarResumen();
        }



        #region inicio
        private void inicio()
        {
            tomarDatosIniciales();
            armarGrilla();
            generarPrimerCliente();
            agregarFila();
        }

        private void tomarDatosIniciales()
        {
            llegada_cliente = new DistExponencial(_media: double.Parse(npMediaExpClientes.Value.ToString()));

            fin_inflado = new DistExponencial(_media: double.Parse(npMediaNeumaticos.Value.ToString()));

            fin_atencion = new DistUniforme(double.Parse(npMediaA.Value.ToString()), double.Parse(npMediaB.Value.ToString()));
            reloj_sistema = 0;
            cantidadSim = Int32.Parse(npIteraciones.Value.ToString());
            horasSim = Int32.Parse(npHoras.Value.ToString());
            dtGrilla = new DataTable("Tabla");
            lista_clientes = new List<Cliente>();
            vector = new Vector();
            rnd = new Random();
            cantidadClientes = 0;
            sumatoriaTiempo = 0;
            clientesSalieron = 0;
             promedio = 0;
            cantMaxClientes = Int32.Parse(npClientes.Value.ToString());
            borrarResumen();
        }

        private void borrarResumen()
        {
            TimeSpan pSurtidor1 = new TimeSpan(0, 0, 0);
            TimeSpan pSurtidor2 = new TimeSpan(0, 0, 0);
            TimeSpan pEspera = new TimeSpan(0, 0, 0);
            lblpromediosurtidores1.Text = pSurtidor1.ToString();
            lblpromediosurtidores2.Text = pSurtidor2.ToString();
            lblpromedioespera.Text = pEspera.ToString();
            lblneumatico.Text = 0.ToString();
        }

        private void generarPrimerCliente()
        {

            Evento generarProxLlegada = eventollegadaCliente();
            agregarCola(generarProxLlegada);
            limpiarVector();
            vector.Evento = Eventos.LlegadaCliente.ToString();
            MostrarLlegadaCliente(generarProxLlegada);



        }

        #endregion

        #region simulacion
        private void simularPorIteraciones()
        {
            for (int i = 0; i < (Int32)npIteraciones.Value; i++)
            {
                simulacion();
            }


        }


        private void simularPorHoras()
        {
            while (reloj_sistema <= (Int32)npHoras.Value)
            {
                simulacion();
            }
        }

        private void simulacion()
        {

            Evento eventoActual = colaEventos.First();
            eliminarDeCola(eventoActual);
            //Actualizo el reloj a la hora del evento que llego
            reloj_sistema = eventoActual.horaEvento;
            limpiarVector();
            //LlegadaCliente, FinAtencion1, FinAtencion2, FinInflado
            switch (eventoActual.evento)
            {
                case "LlegadaCliente":
                    vector.Evento = Eventos.LlegadaCliente.ToString();
                    llegadaCliente();
                    break;

                case "FinAtencion1":
                    vector.Evento = Eventos.FinAtencion1.ToString();
                    finAtencion1();
                    break;
                case "FinAtencion2":
                    vector.Evento = Eventos.FinAtencion2.ToString();
                    finAtencion2();
                    break;
                case "FinInflado":
                    vector.Evento = Eventos.FinInflado.ToString();
                    finInflado();

                    break;


                default:
                    Console.WriteLine("Error en el Tipo Evento");
                    break;
            }

            vector.ID = vector.ID + 1;
            vector.Reloj = reloj_sistema;
            agregarFila();


        }


        private void llegadaCliente()
        {
            Cliente cliente = new Cliente(horaLLegada:reloj_sistema);
            this.cantidadClientes++;    
            Evento proxLlegada = eventollegadaCliente();            
            agregarCola(proxLlegada);
            MostrarLlegadaCliente(proxLlegada);
            if (vector.Estado1 == EstadosSurtidores.Libre.ToString())
            {
                clienteAtendidoSurtidor1(cliente);
                vector.Acumulador1= vector.Acumulador1 + (reloj_sistema - vector.HoraInicioLibre1);
            }
            else if (vector.Estado2 == EstadosSurtidores.Libre.ToString())
            {
                clienteAtendidoSurtidor2(cliente);
                vector.Acumulador2 = vector.Acumulador2 + (reloj_sistema - vector.HoraInicioLibre2);
            }
            else
            {
                if (vector.Cola1 < vector.Cola2)
                {
                    cliente.estado = EstadosClientes.EnColaSurtidor1.ToString();
                    colaSurtidor1.Enqueue(cliente);
                    vector.Cola1 = vector.Cola1 + 1;
                }
                else
                {
                    cliente.estado = EstadosClientes.EnColaSurtidor2.ToString();
                    colaSurtidor2.Enqueue(cliente);
                    vector.Cola2 = vector.Cola2 + 1;
                }
            }
            
            lista_clientes.Add(cliente);
            //Cantidad Max Clientes que vamos a mostrar
            if (cantidadClientes <= cantMaxClientes)
            {
                agregarColumna(cliente, cantidadClientes);
            }
            
        }


        private void finAtencion1()
        {
            Cliente cliente = clienteSiendoAtendido1.Dequeue();
            seraAtendidoNeumatico(cliente);


            
                if (vector.Cola1 > 0)
                {
                    Cliente cliente2 = colaSurtidor1.Dequeue();
                    clienteAtendidoSurtidor1(cliente2);
                    vector.Cola1 = vector.Cola1 - 1;

                }else
                {
                    vector.Estado1 = EstadosSurtidores.Libre.ToString();
                    vector.HoraInicioLibre1 = reloj_sistema;
                }


            







        }
        private void finAtencion2()
        {
            Cliente cliente = clienteSiendoAtendido2.Dequeue();           
            seraAtendidoNeumatico(cliente);

          
                if (vector.Cola2 > 0)
                {
                    Cliente cliente2 = colaSurtidor2.Dequeue();
                    clienteAtendidoSurtidor2(cliente2);
                    vector.Cola2 = vector.Cola2 - 1;

                }
                else
                {
                    vector.Estado2 = EstadosSurtidores.Libre.ToString();
                    vector.HoraInicioLibre2 = reloj_sistema;
            }


            







        }

        private void finInflado()
        {
            Cliente cliente = clienteSiendoAtendidoNeumatico.Dequeue();
            clienteDejaSistema(cliente);
            vector.EstadoNeumatico = EstadosNeumaticos.Libre.ToString();
        }


        private void seraAtendidoNeumatico(Cliente cliente)
        {

            if (atenderNeumaticos())
            {
                if (vector.EstadoNeumatico != EstadosNeumaticos.Ocupado.ToString())
                {
                    vector.EstadoNeumatico = EstadosNeumaticos.Ocupado.ToString();
                    clienteAtendidoNeumatico(cliente);
                }
                else
                {
                    
                    clienteDejaSistema(cliente);
                    vector.NoCargo = vector.NoCargo + 1;

                }

            }
            else{

                clienteDejaSistema(cliente);
            }


        }


        #endregion

        #region eventos
        //Cliente
        private Evento eventollegadaCliente()
        {
            Double random = rnd.NextDouble();
            Double tiempoLlegada = llegada_cliente.devolverTiempo(random);
            Double proxLlegadaCliente = reloj_sistema + tiempoLlegada;

            Evento generarProxLlegada = new Evento(evento: Eventos.LlegadaCliente.ToString(), objeto: Objetos.Cliente.ToString(), rnd: random, tiempoEvento: tiempoLlegada, horaEvento: proxLlegadaCliente);

            return generarProxLlegada;
        }

        private Evento eventoFinAtencionCliente(string evento, string objeto)
        {
            Double random = rnd.NextDouble();
            Double tiempoLlegada = fin_atencion.devolverTiempo(random);
            Double FinAtencion = reloj_sistema + tiempoLlegada;

            Evento generarFinAtencion = new Evento(evento: evento, objeto: objeto.ToString(), rnd: random, tiempoEvento: tiempoLlegada, horaEvento: FinAtencion);
           
            return generarFinAtencion;

        }

        private bool atenderNeumaticos()
        {
            double random = rnd.NextDouble();
            vector.RndCargaNeumatico = random;
           
            if (random >= (double)(npPorcentaje.Value/100))
            {
                vector.CargaNeumatico = "NO";
                return false;


            }
            vector.CargaNeumatico = "SI";
            return true;
        }

        private Evento eventoFinNeumatico()
        {
            Double random = rnd.NextDouble();
            Double tiempoLlegada = fin_inflado.devolverTiempo(random);
            Double proxLlegadaCliente = reloj_sistema + tiempoLlegada;

            Evento generarProxLlegada = new Evento(evento: Eventos.FinInflado.ToString(), objeto: Objetos.Neumatico.ToString(), rnd: random, tiempoEvento: tiempoLlegada, horaEvento: proxLlegadaCliente);

            return generarProxLlegada;
        }

        private void clienteDejaSistema(Cliente cliente)
        {
            cliente.estado = EstadosClientes.Fin.ToString();
            clientesSalieron++;
            double horaLLegada = cliente.horaLLegada;
            double horaAtendido = cliente.horaAtendido;
            sumatoriaTiempo = sumatoriaTiempo + (horaAtendido - horaLLegada);
            promedio = (sumatoriaTiempo / clientesSalieron);
        }

        private void clienteAtendidoNeumatico(Cliente cliente)
        {
            Evento fin_neumatico = eventoFinNeumatico();
            agregarCola(fin_neumatico);
            MostrarFinNeumatico(fin_neumatico);
            cliente.estado = EstadosClientes.Inflando.ToString();
            clienteSiendoAtendidoNeumatico.Enqueue(cliente);
        }


    

        private void clienteAtendidoSurtidor1(Cliente cliente)
        {
            Evento fin_atencion = eventoFinAtencionCliente(Eventos.FinAtencion1.ToString(),
          Objetos.Surtidor1.ToString());
            agregarCola(fin_atencion);
            vector.Estado1 = EstadosSurtidores.Ocupado.ToString();
            cliente.estado = EstadosClientes.SiendoAtendido1.ToString();
            cliente.horaAtendido = reloj_sistema;
            clienteSiendoAtendido1.Enqueue(cliente);
            MostrarFinAtencion(fin_atencion, Objetos.Surtidor1.ToString());
        }
        private void clienteAtendidoSurtidor2(Cliente cliente)
        {
            Evento fin_atencion = eventoFinAtencionCliente(Eventos.FinAtencion2.ToString(),
                Objetos.Surtidor2.ToString());
            agregarCola(fin_atencion);
            vector.Estado2 = EstadosSurtidores.Ocupado.ToString();
            cliente.estado = EstadosClientes.SiendoAtendido2.ToString();
            cliente.horaAtendido = reloj_sistema;
            clienteSiendoAtendido2.Enqueue(cliente);
            MostrarFinAtencion(fin_atencion, Objetos.Surtidor2.ToString());
        }

        #endregion

        #region mostrarVector
        private void limpiarVector()
        {

            vector.Rnd = -1;
            vector.TiempoLlegadaCliente = -1;
            vector.RndSurtidor1 = -1;
            vector.TiempoAtencion1 = -1;
            vector.RndSurtidor2 = -1;
            vector.TiempoAtencion2 = -1;
            vector.RndCargaNeumatico = -1;
            vector.CargaNeumatico = "";
            vector.RndNeumatico = -1;
            vector.TiempoNeumatico = -1;




        }

        private void MostrarLlegadaCliente(Evento ProxLlegada)
        {
           
            vector.Rnd = ProxLlegada.rnd;
            vector.TiempoLlegadaCliente = ProxLlegada.tiempoEvento;
            vector.ProxLlegada = ProxLlegada.horaEvento;

        }
        private void MostrarFinAtencion(Evento ProxLlegada, string objeto)
        {
            switch (objeto)
            {
                case "Surtidor1":
                    vector.RndSurtidor1 = ProxLlegada.rnd;
                    vector.TiempoAtencion1 = ProxLlegada.tiempoEvento;
                    vector.FinAtencion1 = ProxLlegada.horaEvento;
                    break;
                case "Surtidor2":
                    vector.RndSurtidor2 = ProxLlegada.rnd;
                    vector.TiempoAtencion2 = ProxLlegada.tiempoEvento;
                    vector.FinAtencion2 = ProxLlegada.horaEvento;
                    break;
         
            }
 

        }

        private void MostrarFinNeumatico(Evento ProxLlegada)
        {
            vector.RndNeumatico= ProxLlegada.rnd;
            vector.TiempoNeumatico = ProxLlegada.tiempoEvento;
            vector.FinNeumatico = ProxLlegada.horaEvento;
        }
        #endregion

        #region grilla
        private void armarGrilla()
        {
            dtGrilla = new DataTable("Tabla");
           
            int columnas = 0;
            foreach (PropertyInfo property in vector.GetType().GetProperties())
            {
                columnas++;
                dtGrilla.Columns.Add(property.Name.ToString());
                //Console.WriteLine(property.Name.ToString());
            }
            cantidadColumnas = columnas;
        }


        private void agregarFila()
        {

            int iteracion = 0;

            if (lista_clientes.Count * 3 >= cantMaxClientes * 3)
            {
                fila = new Object[cantidadColumnas + cantMaxClientes * 3];
            }else
            {
                fila = new Object[cantidadColumnas + lista_clientes.Count * 3];
            }
            
            foreach (PropertyInfo property in vector.GetType().GetProperties())
            {

                if (property.PropertyType.Name == "Double")
                {
                    String mostrar;
                    double tiempo = (double)property.GetValue(vector, null);
                    if (tiempo == -1)
                    {
                        mostrar = "";
                    }else
                    {
                        mostrar = Math.Round(tiempo, 2).ToString();
                    }
                    
                    fila[iteracion] = mostrar;
                    //Console.WriteLine("Soy un TimeSpan");
                }
                else
                {
                    fila[iteracion] = property.GetValue(vector, null).ToString();
                }


                iteracion++;

            }
            iteracion = 0;
            int contador = cantidadColumnas;
            foreach (var cliente in lista_clientes)
            {
                if (iteracion >= cantMaxClientes)
                {
                    break;
                }

                fila[contador] = cliente.estado;
                contador++;
                fila[contador] = cliente.horaLLegada;
                contador++;
                fila[contador] = cliente.horaAtendido;
                contador++;
                iteracion++;
            }
            


            dtGrilla.Rows.Add(fila);




        }

        private void mostrarGrilla()
        {

            grilla.DataSource = dtGrilla;
            grilla.AllowUserToAddRows = false;

        }

        private void agregarColumna(Cliente _cliente, Int32 cantidad)
        {

            dtGrilla.Columns.Add("Camion - " + cantidad);
            dtGrilla.Columns.Add("HorarioLlegada-" + cantidad);
            dtGrilla.Columns.Add("HorarioSalida-" + cantidad);


        }


        #endregion

        #region colaEventos
        private void agregarCola(Evento _evento)
        {
            
            if (colaEventos.Add(_evento) != true)
            {
                Double nuevaHora = _evento.horaEvento;
                nuevaHora = nuevaHora + 0.002133123;
                _evento.horaEvento = nuevaHora;

                //aplicando la recursividad a full.
                agregarCola(_evento);
                
            }
       

        }
        
        private void eliminarDeCola(Evento _evento)
        {
            if (!colaEventos.Remove(_evento))
            {
                Console.WriteLine("Error Al borrar el primer elemento");
            }

        }



        #endregion

        #region resumen

        private void generarResumen()
        {

            TimeSpan promedioSurtidor1 = TimeSpan.FromMinutes((vector.Acumulador1 / reloj_sistema));
            TimeSpan promedioSurtidor2 = TimeSpan.FromMinutes(vector.Acumulador2 / reloj_sistema);
            TimeSpan promedioEspera= TimeSpan.FromMinutes(promedio);
            TimeSpan pSurtidor1 = new TimeSpan(0, promedioSurtidor1.Minutes, promedioSurtidor1.Seconds);
            TimeSpan pSurtidor2 = new TimeSpan(0, promedioSurtidor2.Minutes, promedioSurtidor2.Seconds);
            TimeSpan pEspera = new TimeSpan(0, promedioEspera.Minutes, promedioEspera.Seconds);
            lblpromediosurtidores1.Text = pSurtidor1.ToString();
            lblpromediosurtidores2.Text = pSurtidor2.ToString();
            lblpromedioespera.Text = pEspera.ToString();
            lblneumatico.Text = vector.NoCargo.ToString();

        
    }





        #endregion

        private void lblneumatico_Click(object sender, EventArgs e)
        {

        }
    }

}