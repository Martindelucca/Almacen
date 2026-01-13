using Almacen.Business.Services;
using Almacen.Core.Interfaces;
using Almacen.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; // Requiere instalar NuGet: Microsoft.Extensions.Hosting
using System;
using System.IO;
using System.Windows.Forms;

namespace Almacen.UI
{
    static class Program
    {
        // Propiedad pública para acceder a la configuración si hiciera falta desde otros lados
        public static IConfiguration Configuration { get; private set; }

        // El contenedor de servicios (nuestra "caja de herramientas" global)
        public static IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        ///  Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 1. Configurar AppSettings (Leer el archivo json)
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            // 2. Configurar Inyección de Dependencias
            var services = new ServiceCollection();
            ConfigureServices(services);

            // 3. Construir el proveedor de servicios
            ServiceProvider = services.BuildServiceProvider();

            // 4. Arrancar el formulario principal
            // YA NO hacemos "new FormPrincipal()". Se lo pedimos al ServiceProvider.
            // Esto permite que el Form reciba el VentaService automáticamente.
            try
            {
                var mainForm = ServiceProvider.GetRequiredService<FormPrincipal>();
                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error grave al iniciar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Aquí "registramos" qué clases cumplen qué funciones.
        /// </summary>
        private static void ConfigureServices(IServiceCollection services)
        {
            // A. Leemos la cadena de conexión de la configuración
            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            // B. Registramos los Repositorios (Capa de Datos)
            // "Cuando alguien pida IProductoRepository, dale un ProductoRepository conectado a SQL"
            services.AddScoped<IProductoRepository>(provider => new ProductoRepository(connectionString));
            services.AddScoped<IVentaRepository>(provider => new VentaRepository(connectionString));

            // C. Registramos los Servicios de Negocio (Capa Lógica)
            // VentaService se creará automáticamente recibiendo los repositorios que necesita.
            services.AddScoped<VentaService>();
            services.AddScoped<ProductoService>();

            // D. Registramos los Formularios (Capa Visual)
            // Es vital registrar el Form para poder inyectarle cosas en su constructor.
            services.AddTransient<FormPrincipal>();
            services.AddTransient<FormProductos>();
        }
    }
}