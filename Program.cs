using System;

namespace EstacionamientoPremium
{
    internal class Program
    {
        // Constantes de capacidad
        private const int MAX_MOTOS = 20;
        private const int MAX_GRANDES = 30;

        // Tipos de vehículo
        private const int TIPO_MOTO = 1;
        private const int TIPO_AUTO = 2;
        private const int TIPO_CAMIONETA = 3;

        // Precios por salida
        private const int PRECIO_MOTO = 800;
        private const int PRECIO_AUTO = 1500;
        private const int PRECIO_CAMIONETA = 1800;

        // Estructuras de datos
        private static readonly string[] EspaciosMotos = new string[MAX_MOTOS];
        private static readonly int[] TipoVehiculoMotos = new int[MAX_MOTOS];

        private static readonly string[] EspaciosGrandes = new string[MAX_GRANDES];
        private static readonly int[] TipoVehiculoGrandes = new int[MAX_GRANDES];

        // Contadores de ocupación
        private static int MotosOcupadas;
        private static int GrandesOcupados;

        // Caja acumulada
        private static int CajaTotal;
        private static int CajaMotos;
        private static int CajaAutos;
        private static int CajaCamionetas;

        private static bool SeguirAbierto = true;

        static void Main()
        {
            InicializarArrays();

            while (SeguirAbierto)
            {
                Console.Clear();
                MostrarMenu();
                int opcion = LeerEntero("Elige una opción: ");

                switch (opcion)
                {
                    case 1:
                        MostrarEstado();
                        break;
                    case 2:
                        RegistrarEntrada();
                        break;
                    case 3:
                        LiberarLugarYCobrar();
                        break;
                    case 4:
                        MostrarCaja();
                        break;
                    case 5:
                        ReubicarVehiculo();
                        break;
                    case 6:
                        Console.WriteLine("Gracias por usar nuestro sistema PREMIUM.");
                        SeguirAbierto = false;
                        Pausa();
                        break;
                    default:
                        Console.WriteLine("Por favor, elige un número del 1 al 6.");
                        Pausa();
                        break;
                }
            }
        }

        #region Menú y utilidades

        private static void MostrarMenu()
        {
            Console.WriteLine("----|           °°°°°°°°°°°°°°°°°°°°°°°°°          |-----");
            Console.WriteLine("           SISTEMA   ESTACIONAMIENTO  quokka111");
            Console.WriteLine("1) Estado del estacionamiento");
            Console.WriteLine("2) Registrar entrada");
            Console.WriteLine("3) Liberar lugar y cobrar");
            Console.WriteLine("4) Mostrar caja");
            Console.WriteLine("5) Reubicar vehículo");
            Console.WriteLine("6) Salir");
            Console.WriteLine("----------------------------------------------------------");
        }

        private static int LeerEntero(string mensaje)
        {
            int valor;
            do
            {
                Console.Write(mensaje);
                if (int.TryParse(Console.ReadLine(), out valor))
                    return valor;
                Console.WriteLine("Entrada no válida. Intenta de nuevo.");
            } while (true);
        }

        private static string LeerCadena(string mensaje)
        {
            Console.Write(mensaje);
            return Console.ReadLine() ?? string.Empty;
        }

        private static void Pausa()
        {
            Console.WriteLine("Presiona una tecla para continuar...");
            Console.ReadKey();
        }

        private static void InicializarArrays()
        {
            for (int i = 0; i < MAX_MOTOS; i++)
            {
                EspaciosMotos[i] = string.Empty;
                TipoVehiculoMotos[i] = 0;
            }

            for (int i = 0; i < MAX_GRANDES; i++)
            {
                EspaciosGrandes[i] = string.Empty;
                TipoVehiculoGrandes[i] = 0;
            }
        }

        private static bool PatenteValida(string patente)
        {
            if (patente.Length != 3) return false;
            foreach (char c in patente)
            {
                if (!char.IsDigit(c)) return false;
            }
            return true;
        }

        private static string TipoVehiculoTexto(int tipo)
        {
            return tipo switch
            {
                TIPO_MOTO => "Moto",
                TIPO_AUTO => "Auto",
                TIPO_CAMIONETA => "Camioneta",
                _ => "Desconocido"
            };
        }

        #endregion

        #region Estado

        private static void MostrarEstado()
        {
            Console.Clear();
            Console.WriteLine("----------------------- ESTADO DEL ESTACIONAMIENTO --------------\n");
            MostrarEstadoArea("MOTOS", MAX_MOTOS, EspaciosMotos, TipoVehiculoMotos);
            Console.WriteLine();
            MostrarEstadoArea("AUTOS y CAMIONETAS", MAX_GRANDES, EspaciosGrandes, TipoVehiculoGrandes);
            Pausa();
        }

        private static void MostrarEstadoArea(string titulo, int maxEspacios, string[] espacios, int[] tipos)
        {
            Console.WriteLine($"-------- Área de {titulo} (1-{maxEspacios}) --------\n");
            Console.WriteLine("Libres:");
            int columnas = 5;
            int cont = 0;
            string linea = string.Empty;
            for (int i = 0; i < maxEspacios; i++)
            {
                if (string.IsNullOrEmpty(espacios[i]))
                {
                    linea += $"{i + 1,3} ";
                    cont++;
                    if (cont % columnas == 0)
                    {
                        Console.WriteLine(linea);
                        linea = string.Empty;
                    }
                }
            }
            if (!string.IsNullOrEmpty(linea)) Console.WriteLine(linea);

            Console.WriteLine("\nOcupados:");
            for (int i = 0; i < maxEspacios; i++)
            {
                if (!string.IsNullOrEmpty(espacios[i]))
                {
                    Console.WriteLine($"Lugar {i + 1}: {espacios[i]} ({TipoVehiculoTexto(tipos[i])})");
                }
            }
            Console.WriteLine("---------------------------------------------------------------");
        }

        #endregion

        #region Registrar entrada

        private static void RegistrarEntrada()
        {
            Console.Clear();
            Console.WriteLine("--- REGISTRANDO ENTRADA ---\n");
            int tipo = SeleccionarTipoVehiculo();
            if (tipo == 0) return;

            string patente = LeerPatente();
            if (string.IsNullOrEmpty(patente)) return;

            if (PatenteYaExiste(patente))
            {
                Console.WriteLine("Esa patente ya está dentro.");
                Pausa();
                return;
            }

            switch (tipo)
            {
                case TIPO_MOTO:
                    RegistrarEnArea(patente, tipo, EspaciosMotos, TipoVehiculoMotos, MAX_MOTOS, ref MotosOcupadas);
                    break;
                case TIPO_AUTO:
                case TIPO_CAMIONETA:
                    RegistrarEnArea(patente, tipo, EspaciosGrandes, TipoVehiculoGrandes, MAX_GRANDES, ref GrandesOcupados);
                    break;
            }
        }

        private static int SeleccionarTipoVehiculo()
        {
            while (true)
            {
                int tipo = LeerEntero("Tipo de vehículo (1:Moto 2:Auto 3:Camioneta 0:Cancelar): ");
                if (tipo is >= 0 and <= 3) return tipo;
                Console.WriteLine("Tipo inválido.");
            }
        }

        private static string LeerPatente()
        {
            while (true)
            {
                string p = LeerCadena("Patente numérica de 3 dígitos (0 para cancelar): ").Trim();
                if (p == "0") return string.Empty;
                if (PatenteValida(p)) return p;
                Console.WriteLine("Patente inválida.");
            }
        }

        private static bool PatenteYaExiste(string patente)
        {
            foreach (var p in EspaciosMotos) if (p == patente) return true;
            foreach (var p in EspaciosGrandes) if (p == patente) return true;
            return false;
        }

        private static void RegistrarEnArea(string patente, int tipoVehiculo, string[] espacios, int[] tipos, int capacidad, ref int ocupados)
        {
            if (ocupados >= capacidad)
            {
                Console.WriteLine("El área está llena.");
                Pausa();
                return;
            }

            int lugar;
            do
            {
                lugar = LeerEntero($"Lugar (1-{capacidad}): ") - 1;
                if (lugar < 0 || lugar >= capacidad)
                {
                    Console.WriteLine("Lugar inexistente.");
                }
                else if (!string.IsNullOrEmpty(espacios[lugar]))
                {
                    Console.WriteLine("Lugar ocupado.");
                    lugar = -1; // fuerza repetir
                }
            } while (lugar < 0 || lugar >= capacidad);

            espacios[lugar] = patente;
            tipos[lugar] = tipoVehiculo;
            ocupados++;
            Console.WriteLine($"Vehículo registrado en lugar {lugar + 1}.");
            Pausa();
        }

        #endregion

        #region Liberar y cobrar

        private static void LiberarLugarYCobrar()
        {
            Console.Clear();
            Console.WriteLine("--- SALIDA Y COBRO ---\n");
            string patente = LeerCadena("Ingrese patente para salir (0 cancelar): ").Trim();
            if (patente == "0") return;

            if (EliminarDeArea(patente, EspaciosMotos, TipoVehiculoMotos, MAX_MOTOS, ref MotosOcupadas,PRECIO_MOTO)) return;
            if (EliminarDeArea(patente, EspaciosGrandes, TipoVehiculoGrandes, MAX_GRANDES, ref GrandesOcupados, null)) return; // precio dentro

            Console.WriteLine("Patente no encontrada.");
            Pausa();
        }

                private static bool EliminarDeArea(string patente, string[] espacios, int[] tipos, int capacidad, ref int ocupados, int? precioFijo)
        {
            for (int i = 0; i < capacidad; i++)
            {
                if (espacios[i] == patente)
                {
                    int tipo = tipos[i];
                    int monto = precioFijo ?? tipo switch
                    {
                        TIPO_AUTO => PRECIO_AUTO,
                        TIPO_CAMIONETA => PRECIO_CAMIONETA,
                        _ => 0
                    };

                    espacios[i] = string.Empty;
                    tipos[i] = 0;
                    ocupados--;

                    CajaTotal += monto;
                    if (tipo == TIPO_MOTO) CajaMotos += monto;
                    if (tipo == TIPO_AUTO) CajaAutos += monto;
                    if (tipo == TIPO_CAMIONETA) CajaCamionetas += monto;

                    Console.WriteLine($"Vehículo retirado. Se cobró: ${monto}.");
                    Pausa();
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Caja

        private static void MostrarCaja()
        {
            Console.Clear();
            Console.WriteLine("---- CAJA ----\n");
            Console.WriteLine($"Total motos: ${CajaMotos}");
            Console.WriteLine($"Total autos: ${CajaAutos}");
            Console.WriteLine($"Total camionetas: ${CajaCamionetas}");
            Console.WriteLine("---------------------");
            Console.WriteLine($"Total general: ${CajaTotal}");
            Pausa();
        }

        #endregion

        #region Reubicar

        private static void ReubicarVehiculo()
        {
            Console.Clear();
            Console.WriteLine("--- REUBICAR VEHÍCULO ---\n");
            string patente = LeerCadena("Ingrese la patente a mover (0 cancelar): ").Trim();
            if (patente == "0") return;

            if (!PatenteYaExiste(patente))
            {
                Console.WriteLine("Esa patente no está registrada.");
                Pausa();
                return;
            }

            // Buscar en motos
            if (BuscarYReubicar(patente, EspaciosMotos, TipoVehiculoMotos, MAX_MOTOS)) return;
            // Buscar en grandes
            if (BuscarYReubicar(patente, EspaciosGrandes, TipoVehiculoGrandes, MAX_GRANDES)) return;
        }

        private static bool BuscarYReubicar(string patente, string[] espacios, int[] tipos, int capacidad)
        {
            for (int i = 0; i < capacidad; i++)
            {
                if (espacios[i] == patente)
                {
                    int tipo = tipos[i];
                    Console.WriteLine($"Patente encontrada en lugar {i + 1} ({TipoVehiculoTexto(tipo)}).");

                    int nuevoLugar;
                    do
                    {
                        nuevoLugar = LeerEntero($"Nuevo lugar (1-{capacidad}): ") - 1;
                        if (nuevoLugar < 0 || nuevoLugar >= capacidad)
                        {
                            Console.WriteLine("Lugar inexistente.");
                        }
                        else if (!string.IsNullOrEmpty(espacios[nuevoLugar]))
                        {
                            Console.WriteLine("Ese lugar está ocupado.");
                            nuevoLugar = -1;
                        }
                    } while (nuevoLugar < 0 || nuevoLugar >= capacidad);

                    espacios[i] = string.Empty;
                    tipos[i] = 0;
                    espacios[nuevoLugar] = patente;
                    tipos[nuevoLugar] = tipo;

                    Console.WriteLine($"Vehículo movido al lugar {nuevoLugar + 1}.");
                    Pausa();
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}

