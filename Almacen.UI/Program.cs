using Almacen.Business.Services;
using Almacen.Core.Interfaces;
using Almacen.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Almacen.UI
{
    static class Program
    {
        public static IConfiguration Configuration { get; private set; }
        public static IServiceProvider ServiceProvider { get; private set; }

        [STAThread]
        static void Main()
        {
            // ✅ CRÍTICO: MANEJADORES GLOBALES DE EXCEPCIONES
            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
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

                // 4. Iniciar con Login
                var formLogin = ServiceProvider.GetRequiredService<FormLogin>();
                DialogResult result = formLogin.ShowDialog();
                if (result == DialogResult.OK)
                {
                    var mainForm = ServiceProvider.GetRequiredService<FormPrincipal>();
                    mainForm.AsignarUsuario(formLogin.UsuarioLogueado);
                    Application.Run(mainForm);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                MessageBox.Show(
                    $"Error crítico al iniciar la aplicación:\n\n{ex.Message}\n\n" +
                    "Revise el archivo errors.log para más detalles.",
                    "Error Fatal",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            // Repositorios
            services.AddScoped<IProductoRepository>(provider => new ProductoRepository(connectionString));
            services.AddScoped<IVentaRepository>(provider => new VentaRepository(connectionString));
            services.AddScoped<IUsuarioRepository>(provider => new UsuarioRepository(connectionString));
            services.AddScoped<ICajaRepository>(provider => new CajaRepository(connectionString));

            // Servicios
            services.AddScoped<VentaService>();
            services.AddScoped<ProductoService>();
            services.AddScoped<LoginService>();
            services.AddScoped<CajaService>();
            // Si vas a vender mañana, cambias MockFacturacionService por AfipFacturacionService
            services.AddScoped<IFacturacionService, MockFacturacionService>();

            // Formularios
            services.AddTransient<FormPrincipal>();
            services.AddTransient<FormProductos>();
            services.AddTransient<FormLogin>();
            services.AddTransient<FormAperturaCaja>();
            services.AddTransient<FormCierreCaja>();
        }

        // ✅ CRÍTICO: Manejadores de errores no capturados
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            LogError(e.Exception);
            MessageBox.Show(
                $"Ha ocurrido un error inesperado:\n\n{e.Exception.Message}\n\n" +
                "La aplicación continuará ejecutándose, pero revise el log de errores.",
                "Error de Aplicación",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            LogError(ex);

            MessageBox.Show(
                $"Error crítico no recuperable:\n\n{ex.Message}\n\n" +
                "La aplicación se cerrará.",
                "Error Fatal",
                MessageBoxButtons.OK,
                MessageBoxIcon.Stop);

            Application.Exit();
        }

        // ✅ CRÍTICO: Sistema de logging
        private static void LogError(Exception ex)
        {
            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "errors.log");
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex.GetType().Name}: {ex.Message}\n" +
                                  $"StackTrace: {ex.StackTrace}\n" +
                                  $"{"".PadLeft(80, '-')}\n";

                File.AppendAllText(logPath, logEntry);
            }
            catch
            {
                // Si falla el logging, no hacer nada (evitar error al loguear error)
            }
        }
    }
}


              