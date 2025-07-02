using System;
using System.Linq; // Necesario para el método .Any() que se usa en PatenteYaExisteEnArea

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
            Console.WriteLine("----|           °°°°°°°°°°°°°°°°°°°°°°°°°           |-----");
            Console.WriteLine("              SISTEMA   ESTACIONAMIENTO PREMIUM");
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
            // La patente debe ser de 3 dígitos numéricos
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
            int tipo = SeleccionarTipoVehiculo(); // Obtiene el tipo de vehículo (Moto, Auto, Camioneta)
            if (tipo == 0) return; // Si el usuario cancela la selección de tipo

            string patente = LeerPatente(); // Obtiene la patente
            if (string.IsNullOrEmpty(patente)) return; // Si el usuario cancela la entrada de patente

            // La verificación de si la patente ya existe para ESE TIPO de vehículo
            // se realizará dentro de RegistrarEnArea para cada área específica.

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

        // Nueva función para leer la patente
        private static string LeerPatente()
        {
            while (true)
            {
                string p = LeerCadena("Patente numérica de 3 dígitos (0 para cancelar): ").Trim();
                if (p == "0") return string.Empty;
                if (PatenteValida(p)) return p;
                Console.WriteLine("Patente inválida. Debe ser de 3 dígitos numéricos.");
            }
        }

        // NUEVA FUNCIÓN: Verifica si una patente YA EXISTE para un tipo específico en un área dada.
        // Esto permite que "123" (Moto) y "123" (Auto) coexistan.
        private static bool PatenteYaExisteEnArea(string patente, string[] espacios, int[] tipos, int tipoBuscado)
        {
            for (int i = 0; i < espacios.Length; i++)
            {
                // Comprueba si el espacio está ocupado por la patente DADA Y coincide con el TIPO de vehículo BUSCADO
                if (espacios[i] == patente && tipos[i] == tipoBuscado)
                {
                    return true;
                }
            }
            return false;
        }

        private static void RegistrarEnArea(string patente, int tipoVehiculo, string[] espacios, int[] tipos, int capacidad, ref int ocupados)
        {
            if (ocupados >= capacidad)
            {
                Console.WriteLine($"El área de {TipoVehiculoTexto(tipoVehiculo)} está llena.");
                Pausa();
                return;
            }

            // AHORA: Verificamos si esta combinación específica (patente + tipo de vehículo)
            // ya está registrada en esta área.
            if (PatenteYaExisteEnArea(patente, espacios, tipos, tipoVehiculo))
            {
                Console.WriteLine($"Error: La patente '{patente}' para un '{TipoVehiculoTexto(tipoVehiculo)}' ya está registrada en esta área.");
                Console.WriteLine("Si desea registrar la misma patente con un tipo de vehículo diferente, elija la opción correspondiente.");
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
                    Console.WriteLine($"Lugar {lugar + 1} ocupado por {espacios[lugar]} ({TipoVehiculoTexto(tipos[lugar])}).");
                    lugar = -1; // fuerza repetir
                }
            } while (lugar < 0 || lugar >= capacidad);

            espacios[lugar] = patente;
            tipos[lugar] = tipoVehiculo;
            ocupados++;
            Console.WriteLine($"Vehículo '{patente}' ({TipoVehiculoTexto(tipoVehiculo)}) registrado en lugar {lugar + 1}.");
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

            // Solicitar el tipo de vehículo para diferenciar patentes iguales
            int tipo = SeleccionarTipoVehiculoParaSalida();
            if (tipo == 0) return; // Si el usuario cancela

            bool encontrado = false;

            // Intentar eliminar de la zona de motos si el tipo es moto
            if (tipo == TIPO_MOTO)
            {
                encontrado = EliminarDeArea(patente, tipo, EspaciosMotos, TipoVehiculoMotos, MAX_MOTOS, ref MotosOcupadas, PRECIO_MOTO);
            }
            // Intentar eliminar de la zona de grandes si el tipo es auto o camioneta
            else if (tipo == TIPO_AUTO || tipo == TIPO_CAMIONETA)
            {
                encontrado = EliminarDeArea(patente, tipo, EspaciosGrandes, TipoVehiculoGrandes, MAX_GRANDES, ref GrandesOcupados, null); // precio se calcula dentro
            }

            if (!encontrado)
            {
                Console.WriteLine($"Patente '{patente}' ({TipoVehiculoTexto(tipo)}) no encontrada o no coincide con el tipo de vehículo especificado.");
            }
            Pausa();
        }

        // Nueva función para seleccionar el tipo de vehículo al salir/reubicar
        private static int SeleccionarTipoVehiculoParaSalida()
        {
            while (true)
            {
                int tipo = LeerEntero("Tipo de vehículo a salir/reubicar (1:Moto 2:Auto 3:Camioneta 0:Cancelar): ");
                if (tipo is >= 0 and <= 3) return tipo;
                Console.WriteLine("Tipo inválido.");
            }
        }

        // Modificada para buscar por patente Y tipo de vehículo
        private static bool EliminarDeArea(string patente, int tipoVehiculoBuscado, string[] espacios, int[] tipos, int capacidad, ref int ocupados, int? precioFijo)
        {
            for (int i = 0; i < capacidad; i++)
            {
                // Ahora verificamos tanto la patente como el tipo de vehículo
                if (espacios[i] == patente && tipos[i] == tipoVehiculoBuscado)
                {
                    int tipoEncontrado = tipos[i]; // El tipo real del vehículo encontrado
                    int monto = precioFijo ?? tipoEncontrado switch // Usa el tipo encontrado para el precio
                    {
                        TIPO_AUTO => PRECIO_AUTO,
                        TIPO_CAMIONETA => PRECIO_CAMIONETA,
                        _ => 0
                    };

                    espacios[i] = string.Empty;
                    tipos[i] = 0;
                    ocupados--;

                    CajaTotal += monto;
                    if (tipoEncontrado == TIPO_MOTO) CajaMotos += monto;
                    if (tipoEncontrado == TIPO_AUTO) CajaAutos += monto;
                    if (tipoEncontrado == TIPO_CAMIONETA) CajaCamionetas += monto;

                    Console.WriteLine($"Vehículo '{patente}' ({TipoVehiculoTexto(tipoEncontrado)}) retirado del lugar {i + 1}. Se cobró: ${monto}.");
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

            // Solicitar el tipo de vehículo para diferenciar patentes iguales
            int tipo = SeleccionarTipoVehiculoParaSalida();
            if (tipo == 0) return; // Si el usuario cancela

            bool encontrado = false;

            // Buscar y reubicar en la zona de motos si el tipo es moto
            if (tipo == TIPO_MOTO)
            {
                encontrado = BuscarYReubicar(patente, tipo, EspaciosMotos, TipoVehiculoMotos, MAX_MOTOS);
            }
            // Buscar y reubicar en la zona de grandes si el tipo es auto o camioneta
            else if (tipo == TIPO_AUTO || tipo == TIPO_CAMIONETA)
            {
                encontrado = BuscarYReubicar(patente, tipo, EspaciosGrandes, TipoVehiculoGrandes, MAX_GRANDES);
            }

            if (!encontrado)
            {
                Console.WriteLine($"Patente '{patente}' ({TipoVehiculoTexto(tipo)}) no encontrada para el tipo de vehículo especificado en ninguna área.");
            }
            Pausa();
        }

        // Modificada para buscar por patente Y tipo de vehículo
        private static bool BuscarYReubicar(string patente, int tipoVehiculoBuscado, string[] espacios, int[] tipos, int capacidad)
        {
            for (int i = 0; i < capacidad; i++)
            {
                // Ahora verificamos tanto la patente como el tipo de vehículo
                if (espacios[i] == patente && tipos[i] == tipoVehiculoBuscado)
                {
                    int tipoEncontrado = tipos[i]; // El tipo real del vehículo encontrado
                    Console.WriteLine($"Patente '{patente}' ({TipoVehiculoTexto(tipoEncontrado)}) encontrada en lugar {i + 1}.");

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
                            Console.WriteLine($"Ese lugar {nuevoLugar + 1} está ocupado por {espacios[nuevoLugar]} ({TipoVehiculoTexto(tipos[nuevoLugar])}).");
                            nuevoLugar = -1; // fuerza repetir
                        }
                    } while (nuevoLugar < 0 || nuevoLugar >= capacidad);

                    // Mover el vehículo
                    espacios[i] = string.Empty; // Vaciar el lugar original
                    tipos[i] = 0; // Resetear el tipo del lugar original
                    espacios[nuevoLugar] = patente; // Ocupar el nuevo lugar con la patente
                    tipos[nuevoLugar] = tipoEncontrado; // Asignar el tipo al nuevo lugar

                    Console.WriteLine($"Vehículo '{patente}' ({TipoVehiculoTexto(tipoEncontrado)}) movido al lugar {nuevoLugar + 1}.");
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}