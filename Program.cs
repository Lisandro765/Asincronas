using System;
using System.Collections.Generic;

namespace SmartBusinessPro
{
    // 1. Clase Producto con sus atributos
    public class Producto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int CantidadDisponible { get; set; }

        public Producto(string codigo, string nombre, DateTime fechaIngreso, DateTime fechaVencimiento, int cantidadDisponible)
        {
            // Validación: evitar fechas de vencimiento anteriores a la fecha de ingreso
            if (fechaVencimiento < fechaIngreso)
            {
                throw new ArgumentException("La fecha de vencimiento no puede ser anterior a la fecha de ingreso.");
            }

            Codigo = codigo;
            Nombre = nombre;
            FechaIngreso = fechaIngreso;
            FechaVencimiento = fechaVencimiento;
            CantidadDisponible = cantidadDisponible;
        }

        // Calcula cuántos días permaneció almacenado desde su ingreso
        public int ObtenerDiasAlmacenado()
        {
            TimeSpan transcurrido = DateTime.Now - FechaIngreso;
            return transcurrido.Days;
        }

        // Calcula cuántos días faltan para el vencimiento (Subtract / TimeSpan)
        public int ObtenerDiasRestantes()
        {
            TimeSpan diferencia = FechaVencimiento.Subtract(DateTime.Now);
            return diferencia.Days;
        }

        // Determina si el producto ya está vencido
        public bool EstaVencido()
        {
            return FechaVencimiento.Date < DateTime.Now.Date;
        }

        // Determina si vencerá en los próximos 30 días (AddDays)
        public bool VenceEnProximos30Dias()
        {
            DateTime limite30Dias = DateTime.Now.AddDays(30);
            return !EstaVencido() && FechaVencimiento.Date <= limite30Dias.Date;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Obtener fecha actual
            DateTime fechaActual = DateTime.Now;

            Console.WriteLine("=========================================================================");
            Console.WriteLine("        SMARTBUSINESSPRO - CONTROL DE VENCIMIENTO DE PRODUCTOS");
            Console.WriteLine($"        Fecha Actual: {fechaActual.ToString("dd/MM/yyyy")}");
            Console.WriteLine("=========================================================================\n");

            // Registro de 5 productos con diferentes fechas
            List<Producto> productos = new List<Producto>();

            try
            {
                productos.Add(new Producto("P001", "Leche Entera 1L", fechaActual.AddDays(-20), fechaActual.AddDays(-2), 50));  // Vencido
                productos.Add(new Producto("P002", "Yogurt Frutilla", fechaActual.AddDays(-10), fechaActual.AddDays(5), 30));   // Vence en 5 días
                productos.Add(new Producto("P003", "Queso Gouda", fechaActual.AddDays(-15), fechaActual.AddDays(20), 40));     // Vence en 20 días
                productos.Add(new Producto("P004", "Avena 1kg", fechaActual.AddDays(-60), fechaActual.AddDays(120), 100));       // Vigente
                productos.Add(new Producto("P005", "Jugo Naranja 1L", fechaActual.AddDays(-30), fechaActual.AddDays(-5), 15));  // Vencido
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error de registro: {ex.Message}");
            }

            // Demostración de validación (intento de registrar fecha inválida)
            try
            {
                Producto prodErroneo = new Producto("P999", "Harina", fechaActual, fechaActual.AddDays(-10), 5);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[VALIDACIÓN CORRECTA]: {ex.Message}\n");
            }

            // TABLA GENERAL DE PRODUCTOS
            Console.WriteLine("--- LISTADO DE PRODUCTOS EN INVENTARIO ---");
            Console.WriteLine("---------------------------------------------------------------------------------------");
            Console.WriteLine("Código | Nombre          | F. Ingreso | F. Vencim. | Cant. | Días Alm. | Días Falt. | Estado");
            Console.WriteLine("---------------------------------------------------------------------------------------");

            foreach (var p in productos)
            {
                string estado = p.EstaVencido() ? "VENCIDO" : (p.VenceEnProximos30Dias() ? "POR VENCER" : "VIGENTE");

                Console.WriteLine($"{p.Codigo,-6} | {p.Nombre,-15} | {p.FechaIngreso.ToString("dd/MM/yyyy")} | {p.FechaVencimiento.ToString("dd/MM/yyyy")} | {p.CantidadDisponible,-5} | {p.ObtenerDiasAlmacenado(),-9} | {p.ObtenerDiasRestantes(),-10} | {estado}");
            }
            Console.WriteLine("---------------------------------------------------------------------------------------\n");

            // PRODUCTOS VENCIDOS
            Console.WriteLine("--- PRODUCTOS VENCIDOS ---");
            foreach (var p in productos)
            {
                if (p.EstaVencido())
                {
                    Console.WriteLine($"• [{p.Codigo}] {p.Nombre} - Venció el {p.FechaVencimiento.ToString("dd/MM/yyyy")} (Hace {Math.Abs(p.ObtenerDiasRestantes())} días)");
                }
            }

            // PRODUCTOS A VENCER EN PRÓXIMOS 30 DÍAS
            Console.WriteLine("\n--- PRODUCTOS QUE VENCEN EN LOS PRÓXIMOS 30 DÍAS ---");
            foreach (var p in productos)
            {
                if (p.VenceEnProximos30Dias())
                {
                    Console.WriteLine($"• [{p.Codigo}] {p.Nombre} - Vence el {p.FechaVencimiento.ToString("dd/MM/yyyy")} (Faltan {p.ObtenerDiasRestantes()} días)");
                }
            }

            // Pausa obligatoria para mantener abierta la consola en Visual Studio
            Console.WriteLine("\nPresione cualquier tecla para cerrar...");
            Console.ReadKey();
        }
    }
}