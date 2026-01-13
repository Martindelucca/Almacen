using Almacen.Business.Services;
using Almacen.Core.Entities;
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
    public partial class FormLogin : Form
    {
        private readonly LoginService _loginService;

        // Esta es la "Cajita Feliz" donde guardaremos al usuario si logra entrar.
        // Program.cs leerá esto después.
        public Usuario UsuarioLogueado { get; private set; }

        // Inyección de Dependencias en el Constructor
        public FormLogin(LoginService loginService)
        {
            InitializeComponent();
            _loginService = loginService;
        }

        private async void btnIngresar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Validaciones básicas visuales
                if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtClave.Text))
                {
                    MessageBox.Show("Por favor complete todos los campos.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Deshabilitar botón para evitar doble clic
                btnIngresar.Enabled = false;
                btnIngresar.Text = "Verificando...";

                // 2. Llamamos al servicio (La lógica pesada)
                var usuario = await _loginService.ValidarUsuarioAsync(txtUsuario.Text.Trim(), txtClave.Text.Trim());

                if (usuario != null)
                {
                    // ¡ÉXITO! 🎉
                    UsuarioLogueado = usuario;
                    this.DialogResult = DialogResult.OK; // Le dice a Program.cs que todo salió bien
                    this.Close();
                }
                else
                {
                    // FALLO ❌
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Error de Acceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtClave.Clear();
                    txtClave.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar loguearse: " + ex.Message);
            }
            finally
            {
                btnIngresar.Enabled = true;
                btnIngresar.Text = "Ingresar";
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
