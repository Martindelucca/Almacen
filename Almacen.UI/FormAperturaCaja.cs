using Almacen.Business.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Almacen.UI
{
    public partial class FormAperturaCaja : Form
    {
        private readonly CajaService _cajaService;
        private int _idUsuario;

        public bool CajaAbiertaExitosamente { get; private set; } = false;

        public FormAperturaCaja(CajaService cajaService)
        {
            InitializeComponent();
            _cajaService = cajaService;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Text = "Apertura de Turno";
        }


        public void AsignarUsuario(int idUsuario)
        {
            _idUsuario = idUsuario;
        }

        private async void btnAbrir_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Validar que sea número
                if (!decimal.TryParse(txtMontoInicial.Text, out decimal montoInicial) || montoInicial < 0)
                {
                    MessageBox.Show("Por favor, ingrese un monto válido (puede ser 0).");
                    return;
                }

                // 2. Llamar al servicio para abrir
                await _cajaService.AbrirCaja(_idUsuario, montoInicial);
                this.DialogResult = DialogResult.OK;

                MessageBox.Show("¡Caja abierta correctamente! Ya puede realizar ventas.");

                CajaAbiertaExitosamente = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir caja: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    
}
