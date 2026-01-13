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
            //// --- BORRAR ESTO DESPUÉS DE OBTENER EL HASH ---
            //// Esto generará un hash válido para la contraseña "1234"
            //string miHashSecreto = BCrypt.Net.BCrypt.HashPassword("1234");

            //// Lo copiamos al portapapeles para que puedas hacer CTRL+V en SQL
            //System.Windows.Forms.Clipboard.SetText(miHashSecreto);

            //System.Windows.Forms.MessageBox.Show("Hash generado y copiado al portapapeles:\n\n" + miHashSecreto);
            //// ---------------------------------------------
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 1. Configurar AppSettings
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            // 2. Configurar Inyección de Dependencias
            var services = new ServiceCollection();
            ConfigureServices(services);

            // 3. Construir el proveedor de servicios
            ServiceProvider = services.BuildServiceProvider();

            // -------------------------------------------------------------
            // 4. NUEVA LÓGICA DE ARRANQUE (LOGIN -> PRINCIPAL)
            // -------------------------------------------------------------
            try
            {
                // A. Pedimos el Form de Login a la fábrica
                var formLogin = ServiceProvider.GetRequiredService<FormLogin>();

                // B. Lo mostramos como una ventana de diálogo (Modal)
                // El código se detiene aquí hasta que el usuario cierre el Login o entre.
                DialogResult resultado = formLogin.ShowDialog();

                // C. Si el resultado fue OK (El usuario entró correctamente)
                if (resultado == DialogResult.OK)
                {
                    // 1. Pedimos el FormPrincipal
                    var mainForm = ServiceProvider.GetRequiredService<FormPrincipal>();

                    // 2. IMPORTANTE: Le pasamos el usuario que se logueó
                    // (Asegúrate de haber creado el método AsignarUsuario en FormPrincipal como vimos en el paso anterior)
                    mainForm.AsignarUsuario(formLogin.UsuarioLogueado);

                    // 3. Arrancamos la aplicación principal
                    Application.Run(mainForm);
                }
                else
                {
                    // Si el usuario cerró la ventana o canceló, la app se cierra aquí.
                    // No hacemos nada más.
                }
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
            services.AddScoped<IUsuarioRepository>(provider => new UsuarioRepository(connectionString));

            // C. Registramos los Servicios de Negocio (Capa Lógica)
            // VentaService se creará automáticamente recibiendo los repositorios que necesita.
            services.AddScoped<VentaService>();
            services.AddScoped<ProductoService>();
            services.AddScoped<LoginService>();


            // D. Registramos los Formularios (Capa Visual)
            // Es vital registrar el Form para poder inyectarle cosas en su constructor.
            services.AddTransient<FormPrincipal>();
            services.AddTransient<FormProductos>();
            services.AddTransient<FormLogin>();
        }

        // ... (Toda la configuración de servicios que ya arreglamos antes) ...

    }
}