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
    public partial class FormProductos : Form
    {
        // Inyección del servicio
        private readonly ProductoService _productoService;

        // Variable para recordar qué producto estamos editando (si es null, es uno nuevo)
        private int? _idProductoSeleccionado = null;

        // Constructor que recibe el servicio (gracias a Program.cs)
        public FormProductos(ProductoService productoService)
        {
            InitializeComponent();
            _productoService = productoService;
        }

        private async void FormProductos_Load(object sender, EventArgs e)
        {
            // Configuración inicial visual
            ConfigurarGrid();

            // Cargar datos
            await CargarCategorias();
            await CargarListaProductos();

            // Empezar en modo "Limpieza"
            LimpiarFormulario();
        }

        private void ConfigurarGrid()
        {
            // Estética básica (puedes copiar el estilo moderno del otro form aquí también)
            dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.MultiSelect = false;
            dgvProductos.ReadOnly = true;
            dgvProductos.RowHeadersVisible = false;
        }

        private async Task CargarCategorias()
        {
            try
            {
                var categorias = await _productoService.ObtenerCategorias();
                var lista = categorias.ToList();

                // --- DIAGNÓSTICO ---
                if (lista.Count == 0)
                {
                    MessageBox.Show("ERROR: La lista está vacía (Count = 0). Revisa tu Base de Datos.");
                    return;
                }

                var primera = lista[0];
                if (string.IsNullOrEmpty(primera.Nombre))
                {
                    MessageBox.Show($"ERROR CRÍTICO DE MAPEO:\n" +
                                    $"Se encontraron {lista.Count} categorías.\n" +
                                    $"Pero el Nombre es NULL.\n" +
                                    $"ID de la primera: {primera.IdCategoria}\n" +
                                    $"Nombre de la primera: '{primera.Nombre}' (Vacío)");
                    // Si sale esto, el problema es que tu clase C# no coincide con la columna SQL
                }
                // -------------------

                // CORRECCIÓN DE ORDEN (A veces ayuda poner esto antes del DataSource)
                cboCategoria.DisplayMember = "Nombre";
                cboCategoria.ValueMember = "IdCategoria";
                cboCategoria.DataSource = lista;

                cboCategoria.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Excepción: " + ex.Message);
            }
        }

        private async Task CargarListaProductos()
        {
            try
            {
                var productos = await _productoService.ObtenerTodos();
                // Usamos ToList() para evitar problemas de binding
                dgvProductos.DataSource = productos.ToList();

                // Ocultamos columnas técnicas que no le importan al usuario
                if (dgvProductos.Columns["IdCategoria"] != null) dgvProductos.Columns["IdCategoria"].Visible = false;
                if (dgvProductos.Columns["IdProducto"] != null) dgvProductos.Columns["IdProducto"].Visible = false; // Opcional
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando productos: " + ex.Message);
            }
        }

        // --- EVENTOS DE LA GRILLA (Selección) ---
        private void dgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            // Si hay una fila seleccionada, llenamos el formulario para editar
            if (dgvProductos.SelectedRows.Count > 0)
            {
                var producto = (Producto)dgvProductos.SelectedRows[0].DataBoundItem;

                _idProductoSeleccionado = producto.IdProducto; // Guardamos el ID en memoria

                txtNombre.Text = producto.Nombre;
                txtPrecio.Text = producto.PrecioActual.ToString("N2"); // Formato número
                chkActivo.Checked = producto.Activo;

                // Seleccionar la categoría correcta en el combo
                cboCategoria.SelectedValue = producto.IdCategoria;

                // Cambiar texto del botón para dar feedback visual
                btnGuardar.Text = "Modificar Producto";
                btnGuardar.BackColor = System.Drawing.Color.Orange; // Feedback visual opcional
            }
        }

        // --- BOTONES DE ACCIÓN ---

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Recolectar datos del formulario
                if (cboCategoria.SelectedValue == null)
                {
                    MessageBox.Show("Por favor seleccione una categoría.");
                    return;
                }

                // Validar precio numérico
                if (!decimal.TryParse(txtPrecio.Text, out decimal precio))
                {
                    MessageBox.Show("El precio debe ser un número válido.");
                    return;
                }

                var producto = new Producto
                {
                    IdProducto = _idProductoSeleccionado ?? 0, // Si es null, mandamos 0
                    Nombre = txtNombre.Text.Trim(),
                    PrecioActual = precio,
                    IdCategoria = (int)cboCategoria.SelectedValue,
                    Activo = chkActivo.Checked
                };

                // 2. Decidir si es Crear o Editar
                if (_idProductoSeleccionado == null || _idProductoSeleccionado == 0)
                {
                    // CREAR NUEVO
                    await _productoService.CrearProducto(producto);
                    MessageBox.Show("¡Producto creado con éxito!");
                }
                else
                {
                    // EDITAR EXISTENTE
                    await _productoService.ModificarProducto(producto);
                    MessageBox.Show("¡Producto modificado con éxito!");
                }

                // 3. Refrescar todo
                LimpiarFormulario();
                await CargarListaProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void btnEliminar_Click(object sender, EventArgs e)
        {
            // Solo podemos borrar si hay algo seleccionado
            if (_idProductoSeleccionado == null) return;

            var confirmacion = MessageBox.Show(
                "¿Está seguro de eliminar este producto? (Se ocultará de las ventas nuevas)",
                "Confirmar Eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                try
                {
                    await _productoService.EliminarProducto(_idProductoSeleccionado.Value);
                    LimpiarFormulario();
                    await CargarListaProductos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message);
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            var confirmacion = MessageBox.Show(
                "Seguro desea salir?",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirmacion == DialogResult.Yes)
            {
                this.Close();
            }
           
        }

        // Método auxiliar para resetear la pantalla
        private void LimpiarFormulario()
        {
            _idProductoSeleccionado = null; // Modo "Nuevo"

            txtNombre.Clear();
            txtPrecio.Clear();
            cboCategoria.SelectedIndex = -1;
            chkActivo.Checked = true; // Por defecto activo

            // Deseleccionar grilla para que no se vuelva a llenar solo
            dgvProductos.ClearSelection();

            // Restaurar estética botón
            btnGuardar.Text = "Guardar Nuevo";
            btnGuardar.BackColor = System.Drawing.Color.DodgerBlue; // O tu color default

            txtNombre.Focus();
        }
        private async void btnSumarStock_Click(object sender, EventArgs e)
        {
            if (_idProductoSeleccionado == null)
            {
                MessageBox.Show("Selecciona un producto primero.");
                return;
            }

            // 1. Pedir cantidad (Usando un form modal rápido creado por código)
            string input = ShowInputDialog("Ingreso de Stock", "Cantidad a sumar:");

            if (string.IsNullOrEmpty(input)) return; // Canceló

            if (decimal.TryParse(input, out decimal cantidad) && cantidad > 0)
            {
                try
                {
                    // 2. Llamar al servicio
                    await _productoService.SumarStock(_idProductoSeleccionado.Value, cantidad);

                    MessageBox.Show($"Se agregaron {cantidad} unidades correctamente.");

                    // 3. Refrescar la grilla (para ver si mostramos columna stock)
                    // Nota: Si tu grilla de FormProductos no muestra stock, no verás el cambio visualmente ahí,
                    // pero sí en la pestaña Inventario del FormPrincipal.
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Cantidad inválida.");
            }
        }

        // Método Helper para crear un input box (Copia y pega esto al final de tu clase)
        public static string ShowInputDialog(string title, string promptText)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new System.Drawing.Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new System.Drawing.Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            return dialogResult == DialogResult.OK ? textBox.Text : "";
        }

    }
}
