using Almacen.Business.Services;
using Almacen.Core.Interfaces; // Necesario si usas el repo de productos para búsquedas extra
using Almacen.Data.Repositories;
using System;
using System.Windows.Forms;

namespace Almacen.UI
{
    public partial class FormPrincipal : Form
    {
        // Dependencias inyectadas
        private readonly VentaService _ventaService;
        private readonly IProductoRepository _productoRepository;

        // BindingSource actúa como intermediario entre la Lista y el DataGridView
        private readonly BindingSource _bindingSource = new BindingSource();

        // CONSTRUCTOR: Aquí recibimos el servicio gracias al Program.cs
        public FormPrincipal(VentaService ventaService, IProductoRepository productoRepo)
        {
            InitializeComponent();
            _ventaService = ventaService;
            _productoRepository = productoRepo;
            
        }

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            // Configuramos la grilla al iniciar
            ConfigurarGrid();
            CargarInventario();
        }

        private void ConfigurarGrid()
        {
            // 1. Evitar que genere columnas basura automáticamente (opcional, pero profesional)
            // Por ahora dejémoslo en true para no complicarte con diseño manual, 
            // pero vamos a manipular las columnas generadas.
            dgvCarrito.DataSource = _bindingSource;

            // Ejecutamos esto SOLO si hay fuente de datos o forzamos la estructura
            // Un truco es agregar un item dummy y borrarlo, o configurar después del primer bind.
            // Pero lo mejor es definir el estilo genérico aquí:

            dgvCarrito.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCarrito.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCarrito.MultiSelect = false;
            dgvCarrito.AllowUserToAddRows = false; // Importante para que no salga la fila vacía abajo

            // Ocultar ID si no quieres que se vea (aunque para debug sirve)
            // dgvCarrito.Columns["IdProducto"].Visible = false; 
        }

        // Agrega este método y llámalo al final de ActualizarCarritoUI()
        private void FormatearColumnas()
        {
            if (dgvCarrito.Columns.Count == 0) return;

            // Nombres amigables en las cabeceras
            if (dgvCarrito.Columns["NombreProducto"] != null)
            {
                dgvCarrito.Columns["NombreProducto"].HeaderText = "Producto";
                dgvCarrito.Columns["NombreProducto"].FillWeight = 200; // Más ancho
            }

            if (dgvCarrito.Columns["PrecioUnitario"] != null)
            {
                dgvCarrito.Columns["PrecioUnitario"].HeaderText = "Precio Unit.";
                dgvCarrito.Columns["PrecioUnitario"].DefaultCellStyle.Format = "C2"; // Formato Moneda ($1,200.00)
                dgvCarrito.Columns["PrecioUnitario"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dgvCarrito.Columns["Cantidad"] != null)
            {
                dgvCarrito.Columns["Cantidad"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgvCarrito.Columns["Subtotal"] != null)
            {
                dgvCarrito.Columns["Subtotal"].DefaultCellStyle.Format = "C2";
                dgvCarrito.Columns["Subtotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvCarrito.Columns["Subtotal"].DefaultCellStyle.Font = new System.Drawing.Font(dgvCarrito.Font, System.Drawing.FontStyle.Bold);
            }
        }

        private async void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Validaciones de entrada básicas
                if (!int.TryParse(txtIdProducto.Text, out int idProducto))
                {
                    MessageBox.Show("El ID del producto debe ser un número.");
                    return;
                }

                if (!decimal.TryParse(txtCantidad.Text, out decimal cantidad))
                {
                    MessageBox.Show("La cantidad debe ser un número válido.");
                    return;
                }

                // 2. Llamada al Servicio (Negocio)
                // Usamos 'await' para no congelar la pantalla mientras va a la BD
                await _ventaService.AgregarProductoAlCarrito(idProducto, cantidad);

                // 3. Refrescar la UI
                ActualizarCarritoUI();

                // 4. Limpiar para el siguiente item (UX Rápida)
                txtIdProducto.Clear();
                txtCantidad.Text = "1";
                txtIdProducto.Focus();
            }
            catch (Exception ex)
            {
                // Aquí atrapamos errores de negocio (ej: "Stock insuficiente")
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar cliente opcional
                int? idCliente = null;
                if (!string.IsNullOrWhiteSpace(txtCliente.Text))
                {
                    if (int.TryParse(txtCliente.Text, out int id)) idCliente = id;
                    else
                    {
                        MessageBox.Show("ID de Cliente inválido.");
                        return;
                    }
                }

                // Llamar al servicio para confirmar
                int idVenta = await _ventaService.ConfirmarVenta(idCliente);

                // Éxito
                MessageBox.Show($"¡Venta registrada con éxito!\nNro Ticket: {idVenta}", "Venta Confirmada", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpiar UI
                ActualizarCarritoUI();
                txtIdProducto.Focus();
                CargarInventario();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar la venta: {ex.Message}", "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarCarritoUI()
        {
            // Obtenemos la lista actual del servicio
            var listaItems = _ventaService.ObtenerCarrito();

            // Truco para refrescar el BindingSource:
            _bindingSource.DataSource = null; // Desconectar
            _bindingSource.DataSource = listaItems; // Reconectar

            // Actualizar Label de Total
            lblTotal.Text = $"Total: {_ventaService.ObtenerTotal():C2}"; // Formato Moneda
            FormatearColumnas();
        }

        private async void CargarInventario()
        {
            try
            {
                // Usamos el repositorio para traer la lista fresca desde SQL
                var listaProductos = await _productoRepository.GetAllAsync();

                // Llenamos la grilla de la segunda pestaña
                dgvInventario.DataSource = listaProductos;
                dgvInventario.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al traer stock: " + ex.Message);
            }
        }

        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            CargarInventario();
        }
    }
}