using Almacen.Business.Services;
using Almacen.Core.Dtos; // Asegúrate de tener el DTO ResumenCierreDto
using System;
using System.Windows.Forms;

namespace Almacen.UI
{
    public partial class FormCierreCaja : Form
    {
        private readonly CajaService _cajaService;
        private int _idUsuario;
        private decimal _montoSistema;

        public FormCierreCaja(CajaService cajaService)
        {
            InitializeComponent();
            _cajaService = cajaService;
        }

        // Método para pasar el ID (igual que hicimos con Apertura)
        public void Inicializar(int idUsuario)
        {
            _idUsuario = idUsuario;
        }

        private async void FormCierreCaja_Load(object sender, EventArgs e)
        {
            try
            {
                // 1. Obtenemos los datos del servicio
                var resumen = await _cajaService.ObtenerResumenCierre(_idUsuario);

                // 2. Llenamos el label de resumen detallado
                lblInfo.Text = $"Monto Inicial: {resumen.MontoInicial:C2}\n" +
                                  $"Ventas del Turno: {resumen.TotalVentasEfectivo:C2}";

                // 3. Llenamos el label del TOTAL que el sistema espera ver
                lblSistema.Text = resumen.TotalEsperado.ToString("C2");

                // Guardamos este valor en una variable de clase para comparar al final
                _montoSistema = resumen.TotalEsperado;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos de cierre: " + ex.Message);
                this.Close();
            }
        }

        private async void btnCerrar_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtReal.Text, out decimal realEnCaja))
            {
                MessageBox.Show("Ingrese un monto válido.");
                return;
            }

            try
            {
                // 2. Ejecutamos el cierre
                await _cajaService.CerrarCaja(_idUsuario, realEnCaja);

                decimal diferencia = realEnCaja - _montoSistema;
                string mensaje = diferencia == 0 ? "Caja Perfecta." :
                                 diferencia > 0 ? $"Sobra dinero: {diferencia:C2}" :
                                 $"Falta dinero: {diferencia:C2}";

                MessageBox.Show($"¡Caja Cerrada!\n\n{mensaje}", "Fin del Turno", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cerrando: " + ex.Message);
            }
        }
    }
}