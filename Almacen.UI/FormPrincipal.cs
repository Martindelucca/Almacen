using Almacen.Business.Models;
using Almacen.Business.Services;
using Almacen.Core.Dtos;
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
            EstilizarGridModerno();
            // Esto hace que una fila sea blanca y la otra gris suave
            dgvVentas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            dgvVentas.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvVentas.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise; // Color de selección moderno
            dgvVentas.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            dgvVentas.BackgroundColor = Color.White; // Quita ese fondo gris de ventana vieja
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
            if (!dgvCarrito.Columns.Contains("btnEliminar"))
            {
                var botonEliminar = new DataGridViewButtonColumn();
                botonEliminar.Name = "btnEliminar";
                botonEliminar.HeaderText = "";
                botonEliminar.Text = "🗑️";
                botonEliminar.UseColumnTextForButtonValue = true; // Para que se vea la X
                botonEliminar.DefaultCellStyle.BackColor = Color.White;
                botonEliminar.DefaultCellStyle.ForeColor = Color.Red;
                botonEliminar.FlatStyle = FlatStyle.Flat;
                botonEliminar.DefaultCellStyle.SelectionBackColor = Color.LightCoral;
                dgvCarrito.Columns.Add(botonEliminar);
            }

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
        private void EstilizarGridModerno()
        {
            // 1. Quitar bordes feos del contenedor
            dgvCarrito.BackgroundColor = Color.White;
            dgvCarrito.BorderStyle = BorderStyle.None;
            dgvCarrito.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvCarrito.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // 2. Estilo de Cabecera (Header) - Un color sólido y moderno
            dgvCarrito.EnableHeadersVisualStyles = false; // Importante para que tome el color
            dgvCarrito.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 37, 41); // Gris Oscuro (Estilo Bootstrap)
            dgvCarrito.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCarrito.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvCarrito.ColumnHeadersHeight = 40; // Más alto para que respire

            // 3. Estilo de Filas
            dgvCarrito.DefaultCellStyle.SelectionBackColor = Color.FromArgb(232, 234, 237); // Gris muy suave al seleccionar
            dgvCarrito.DefaultCellStyle.SelectionForeColor = Color.Black; // Texto negro al seleccionar
            dgvCarrito.RowTemplate.Height = 35; // Filas más altas (mejor lectura)
            dgvCarrito.DefaultCellStyle.Font = new Font("Segoe UI", 10);

            // 4. Alinear columnas específicas (ajusta los nombres si cambian)
            if (dgvCarrito.Columns["PrecioUnitario"] != null)
                dgvCarrito.Columns["PrecioUnitario"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            if (dgvCarrito.Columns["Subtotal"] != null)
                dgvCarrito.Columns["Subtotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            if (dgvCarrito.Columns["Cantidad"] != null)
                dgvCarrito.Columns["Cantidad"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void EstilizarGridVentas()
        {
            // Ocultar ID si no es relevante para el usuario final (opcional)
            // dgvVentas.Columns["IdVenta"].Visible = false;

            // 1. Encabezados Bonitos (Espacios y Mayúsculas)
            dgvVentas.Columns["IdVenta"].HeaderText = "Nro. Ticket";
            dgvVentas.Columns["ClienteNombre"].HeaderText = "Cliente";
            dgvVentas.Columns["CantidadItems"].HeaderText = "Items";
            dgvVentas.Columns["Total"].HeaderText = "Total";

            // 2. Formato de Moneda (C2 = Currency 2 decimals -> $ 1.200,50)
            dgvVentas.Columns["Total"].DefaultCellStyle.Format = "C2";

            // 3. Alineación (Números a la derecha, Texto a la izquierda)
            dgvVentas.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvVentas.Columns["CantidadItems"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvVentas.Columns["IdVenta"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // 4. Formato de Fecha (dd/MM/yyyy HH:mm)
            dgvVentas.Columns["FechaHora"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
        }

        // 1. Método para cargar la lista de arriba (Maestro)
        private async void CargarHistorial()
        {
            try
            {
                var ventas = await _ventaService.ObtenerHistorialVentas();
                dgvVentas.DataSource = ventas;
                EstilizarGridVentas();

                // Estética
                dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvVentas.MultiSelect = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar historial: " + ex.Message);
            }
        }

        // 2. Evento del Botón Refrescar
        private void btnRefrescarHistorial_Click(object sender, EventArgs e)
        {
            CargarHistorial();
        }

        // 3. EL EVENTO CLAVE: Cuando tocas una fila de arriba, se llena la de abajo
        private async void dgvVentas_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvVentas.SelectedRows.Count > 0)
            {
                var venta = (VentaResumenDto)dgvVentas.SelectedRows[0].DataBoundItem;

                // Feedback visual
                lblDetalle.Text = $"Detalle del Ticket #{venta.IdVenta} - Cliente: {venta.ClienteNombre}";

                await CargarDetalleDeVenta(venta.IdVenta);
            }
        }

        private async Task CargarDetalleDeVenta(int idVenta)
        {
            var detalles = await _ventaService.ObtenerDetalleVenta(idVenta);
            dgvDetalleVenta.DataSource = detalles;
            dgvDetalleVenta.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void txtIdProducto_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Si presiona ENTER (char 13)
            if (e.KeyChar == (char)13)
            {
                e.Handled = true; // Evita el sonido "bip" de Windows

                // Llamamos a la misma lógica del botón agregar
                btnAgregar.PerformClick();
            }
        }

        private void dgvCarrito_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Si hizo clic en la columna del botón eliminar
            if (dgvCarrito.Columns[e.ColumnIndex].Name == "btnEliminar" && e.RowIndex >= 0)
            {
                // Obtener el objeto de la fila
                var item = (DetalleVentaModel)dgvCarrito.Rows[e.RowIndex].DataBoundItem;

                // Llamar al servicio para quitarlo
                _ventaService.QuitarProducto(item.IdProducto);

                // Refrescar UI
                ActualizarCarritoUI();
            }
        }

    }
}