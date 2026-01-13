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
    public partial class FormCobro : Form
    {// Propiedades para recibir y devolver datos
        public decimal MontoTotal { get; set; }
        public bool VentaConfirmada { get; private set; } = false;

        public FormCobro(decimal montoTotal)
        {
            InitializeComponent();
            MontoTotal = montoTotal;
        }
        // En FormCobro.cs agrega esto:
        public decimal MontoPagoCon
        {
            get
            {
                decimal.TryParse(txtPagaCon.Text, out decimal val);
                return val;
            }
        }

        private void FormCobro_Load(object sender, EventArgs e)
        {
            // 1. Mostrar el total a cobrar
            lblTotal.Text = MontoTotal.ToString("C2");

            // 2. Limpiar campos
            txtPagaCon.Text = "";

            // 3. Enfocar el input para escribir directo sin usar mouse
            txtPagaCon.Focus();
        }

        private void txtPagaCon_TextChanged(object sender, EventArgs e)
        {
            CalcularVuelto();
        }

        private void CalcularVuelto()
        {
            if (decimal.TryParse(txtPagaCon.Text, out decimal pagaCon))
            {
                decimal vuelto = pagaCon - MontoTotal;
                lblVuelto.Text = vuelto.ToString("C2");

                if (vuelto >= 0)
                {
                    // --- TODO EN ORDEN (VERDE) ---
                    lblVuelto.ForeColor = Color.SeaGreen; // Color dinero
                    lblTitulo.Text = "Su Vuelto:"; // Texto normal

                    btnFinalizar.Enabled = true;
                    btnFinalizar.BackColor = Color.SeaGreen;
                    btnFinalizar.Text = "CONFIRMAR VENTA"; // Texto en mayúsculas queda bien
                }
                else
                {
                    // --- FALTA DINERO (ROJO) ---
                    lblVuelto.ForeColor = Color.IndianRed; // Rojo suave moderno
                    lblTitulo.Text = "Faltan:"; // Cambiamos el título para ser claros

                    btnFinalizar.Enabled = false;
                    btnFinalizar.BackColor = Color.LightGray; // Gris apagado
                    btnFinalizar.Text = "Monto Insuficiente";
                }
            }
            else
            {
                lblVuelto.Text = "$ 0.00";
                lblVuelto.ForeColor = Color.Gray;
                btnFinalizar.Enabled = false;
                btnFinalizar.BackColor = Color.LightGray;
            }
        }

        // Evento para finalizar (Clic en botón)
        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            Confirmar();
        }

        // Evento para detectar ENTER en el TextBox
        private void txtPagaCon_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir solo números, coma/punto y borrar
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // Si presiona ENTER y el botón está habilitado (pago suficiente)
            if (e.KeyChar == (char)13 && btnFinalizar.Enabled)
            {
                Confirmar();
            }
        }

        // Cierre con escape (UX)
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Confirmar()
        {
            VentaConfirmada = true;
            this.Close(); // Cerramos el formulario devolviendo el control al Principal
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            VentaConfirmada = false;
            this.Close();
        }

    }
}
