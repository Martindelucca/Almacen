using Almacen.Business.Services;
using Almacen.Core.Entities;
using System;
using System.Windows.Forms;

namespace Almacen.UI
{
    public partial class FormLogin : Form
    {
        private readonly LoginService _loginService;

        // ✅ CRÍTICO: Control de intentos fallidos
        private int _intentosFallidos = 0;
        private const int MAX_INTENTOS = 5;

        public Usuario UsuarioLogueado { get; private set; }

        public FormLogin(LoginService loginService)
        {
            InitializeComponent();
            _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
        }

        private async void btnIngresar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Validaciones básicas visuales
                if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtClave.Text))
                {
                    MessageBox.Show("Por favor complete todos los campos.",
                        "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Deshabilitar botón para evitar doble clic
                btnIngresar.Enabled = false;
                btnIngresar.Text = "Verificando...";
                this.Cursor = Cursors.WaitCursor;

                // 2. Llamamos al servicio (La lógica pesada)
                var usuario = await _loginService.ValidarUsuarioAsync(
                    txtUsuario.Text.Trim(),
                    txtClave.Text.Trim());

                if (usuario != null)
                {
                    // ✅ ÉXITO - Resetear intentos
                    _intentosFallidos = 0;
                    UsuarioLogueado = usuario;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    // ❌ FALLO - Incrementar contador
                    _intentosFallidos++;

                    // ✅ CRÍTICO: Bloquear después de X intentos
                    if (_intentosFallidos >= MAX_INTENTOS)
                    {
                        MessageBox.Show(
                            "Se han excedido los intentos permitidos.\n\n" +
                            "Por seguridad, la aplicación se cerrará.",
                            "Acceso Bloqueado",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Stop);

                        Application.Exit();
                        return;
                    }

                    // Mostrar intentos restantes
                    int intentosRestantes = MAX_INTENTOS - _intentosFallidos;
                    MessageBox.Show(
                        $"Usuario o contraseña incorrectos.\n\n" +
                        $"Intentos restantes: {intentosRestantes}",
                        "Error de Acceso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    txtClave.Clear();
                    txtClave.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al intentar conectarse:\n\n{ex.Message}",
                    "Error de Conexión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnIngresar.Enabled = true;
                btnIngresar.Text = "Ingresar";
                this.Cursor = Cursors.Default;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // ✅ Opcional: Resetear contador si cierra y vuelve a abrir
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _intentosFallidos = 0;
            txtUsuario.Focus();
        }
    }
}