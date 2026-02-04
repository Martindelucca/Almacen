using Almacen.Business.Models;
using Almacen.Business.Services;
using Almacen.Core.Dtos;
using Almacen.Core.Entities;
using Almacen.Core.Interfaces; // Necesario si usas el repo de productos para búsquedas extra
using Almacen.Data.Repositories;
using Almacen.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Almacen.UI.Services.TicketService;

namespace Almacen.UI
{
    public partial class FormPrincipal : Form
    {
        // Dependencias inyectadas
        private readonly VentaService _ventaService;
        private readonly IProductoRepository _productoRepository;
        private readonly ProductoService _productoService;
        private List<Producto> _products = new List<Producto>(); // Bolsillo para el inventario
        private readonly TicketService _ticketService = new TicketService();
        private Usuario _usuarioActual;
        private readonly CajaService _cajaService;
        private readonly IServiceProvider _serviceProvider;

        // BindingSource actúa como intermediario entre la Lista y el DataGridView
        private readonly BindingSource _bindingSource = new BindingSource();

        // CONSTRUCTOR: Aquí recibimos el servicio gracias al Program.cs
        public FormPrincipal(VentaService ventaService, IProductoRepository productoRepo, ProductoService productoService, CajaService cajaService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _ventaService = ventaService;
            _productoRepository = productoRepo;
            _productoService = productoService;
            _cajaService = cajaService;
            _serviceProvider = serviceProvider;
        }

        private async void FormPrincipal_Load(object sender, EventArgs e)
        {
            // Configuramos la grilla al iniciar
            ConfigurarGrid();
            await CargarInventario();
            EstilizarGridModerno();
            ConfigurarBuscador();
            await CargarHistorial();
            ConfigurarGridInventario();
            EstilizarGridVentas();
            await VerificarEstadoCaja();

            // Esto hace que una fila sea blanca y la otra gris suave
            dgvVentas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            dgvVentas.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvVentas.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise; // Color de selección moderno
            dgvVentas.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            dgvVentas.BackgroundColor = Color.White; // Quita ese fondo gris de ventana vieja
        }
        // Variable a nivel de clase para buscar rápido el ID cuando eligen un nombre
        // Esto crea una lista vacía. Si el buscador falla, al menos no explota.
        private List<Producto> _listaProductosBuscador = new List<Producto>();

        private async Task VerificarEstadoCaja()
        {
            try
            {
                // 1. Preguntamos a la BD si hay sesión
                var sesion = await _cajaService.VerificarCajaAbierta(_usuarioActual.IdUsuario);

                if (sesion == null)
                {
                    // 1. El contenedor crea el form inyectando SOLO el servicio
                    using (var formApertura = _serviceProvider.GetRequiredService<FormAperturaCaja>())
                    {
                        // 2. NOSOTROS le pasamos el dato dinámico (el ID del usuario)
                        formApertura.AsignarUsuario(_usuarioActual.IdUsuario); // <--- NUEVA LÍNEA

                        var resultado = formApertura.ShowDialog();

                        if (resultado != DialogResult.OK)
                        {
                            MessageBox.Show("No se puede operar sin caja abierta.");
                            Application.Exit();
                        }
                    }
                }
                else
                {
                    // Ya estaba abierta de antes
                    // lblEstadoCaja.Text = "Caja: ABIERTA (Recuperada)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error verificando caja: {ex.Message}");
            }
        }


        private async void ConfigurarBuscador()
        {
            try
            {
                // 1. Traemos todo el catálogo
                // Nota: Si son 100.000 productos esto se hace diferente, pero para < 5.000 va perfecto.
                var productos = await _ventaService.ObtenerTodosLosProductos(); // O _ventaService.ObtenerTodosLosProductos()
                _listaProductosBuscador = productos.ToList();

                // 2. Configuramos el Autocompletado del TextBox
                var coleccionNombres = new AutoCompleteStringCollection();
                foreach (var p in _listaProductosBuscador)
                {
                    // Agregamos: "Coca Cola 2.25L"
                    coleccionNombres.Add(p.Nombre);
                }

                txtIdProducto.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtIdProducto.AutoCompleteSource = AutoCompleteSource.CustomSource;
                txtIdProducto.AutoCompleteCustomSource = coleccionNombres;
            }
            catch (Exception ex)
            {
                // Fallo silencioso (no rompemos la app, solo no habrá autocompletado)
                Debug.WriteLine("Error cargando buscador: " + ex.Message);
            }
        }
        private void ConfigurarGridInventario()
        {
            // 1. BLOQUEO TOTAL (Modo Lectura)
            dgvInventario.ReadOnly = true;       // Nadie puede editar nada
            dgvInventario.RowHeadersVisible = false; // Quita la barra gris fea de la izquierda
            dgvInventario.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Selecciona toda la fila
            dgvInventario.AllowUserToAddRows = false; // Quita la fila vacía del final

            // 2. Ocultar columnas técnicas
            if (dgvInventario.Columns["IdProducto"] != null) dgvInventario.Columns["IdProducto"].Visible = false;
            if (dgvInventario.Columns["IdCategoria"] != null) dgvInventario.Columns["IdCategoria"].Visible = false;

            // Opcional: Si no quieres ver la columna "Activo" en el inventario, ocúltala también:
            // if (dgvInventario.Columns["Activo"] != null) dgvInventario.Columns["Activo"].Visible = false;

            // 3. Renombrar encabezados (Asegúrate que los nombres coinciden con tu clase)
            if (dgvInventario.Columns["CategoriaNom"] != null) dgvInventario.Columns["CategoriaNom"].HeaderText = "Categoría";
            if (dgvInventario.Columns["Nombre"] != null) dgvInventario.Columns["Nombre"].HeaderText = "Producto";

            // ... (El resto de tu código de colores y formatos se queda igual) ...
            // Asegúrate de que el formato de moneda y stock siga aquí abajo
            if (dgvInventario.Columns["PrecioActual"] != null)
            {
                dgvInventario.Columns["PrecioActual"].DefaultCellStyle.Format = "C2";
                dgvInventario.Columns["PrecioActual"].HeaderText = "Precio Unit.";
                dgvInventario.Columns["PrecioActual"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dgvInventario.Columns["StockActual"] != null)
            {
                dgvInventario.Columns["StockActual"].DefaultCellStyle.Format = "N0"; // Sin decimales (80, no 80,0000)
                dgvInventario.Columns["StockActual"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvInventario.Columns["StockActual"].DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold); // ¡Destacado!
            }

            // 4. Orden de columnas (UX)
            // Es más lógico ver: Nombre -> Categoría -> Stock -> Precio
            if (dgvInventario.Columns["Nombre"] != null) dgvInventario.Columns["Nombre"].DisplayIndex = 0;
            if (dgvInventario.Columns["CategoriaNom"] != null) dgvInventario.Columns["CategoriaNom"].DisplayIndex = 1;
            if (dgvInventario.Columns["StockActual"] != null) dgvInventario.Columns["StockActual"].DisplayIndex = 2;
            if (dgvInventario.Columns["PrecioActual"] != null) dgvInventario.Columns["PrecioActual"].DisplayIndex = 3;
            // Esto obliga a las columnas a ocupar todo el ancho del control
            dgvInventario.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
                string textoInput = txtIdProducto.Text.Trim();
                if (string.IsNullOrEmpty(textoInput)) return;

                Producto productoSeleccionado = null;

                // ---------------------------------------------------------
                // PASO 1: BÚSQUEDA INTELIGENTE (Cascada) 🧠
                // ---------------------------------------------------------

                // A. Intentamos buscar por ID numérico exacto
                if (int.TryParse(textoInput, out int idNumerico))
                {
                    if (_listaProductosBuscador != null)
                    {
                        productoSeleccionado = _listaProductosBuscador
                            .FirstOrDefault(p => p.IdProducto == idNumerico);
                    }
                }

                // B. Si NO encontró por ID (o no era un número), buscamos por Nombre
                //    (Usamos 'Contains' para que si escribes "Coca" encuentre "Coca Cola")
                if (productoSeleccionado == null && _listaProductosBuscador != null)
                {
                    productoSeleccionado = _listaProductosBuscador.FirstOrDefault(p =>
                        p.Nombre.Contains(textoInput, StringComparison.OrdinalIgnoreCase));
                }

                // ---------------------------------------------------------
                // PASO 2: Validar si encontramos algo
                // ---------------------------------------------------------
                if (productoSeleccionado == null)
                {
                    MessageBox.Show("No se encontró un producto válido con ese código o nombre.",
                        "Producto No Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    // Truco de UX: Seleccionar el texto para que el usuario pueda borrar rápido
                    txtIdProducto.SelectAll();
                    txtIdProducto.Focus();
                    return;
                }

                // ---------------------------------------------------------
                // PASO 3: Validar Cantidad
                // ---------------------------------------------------------
                if (!decimal.TryParse(txtCantidad.Text, out decimal cantidadSolicitada) || cantidadSolicitada <= 0)
                {
                    MessageBox.Show("La cantidad debe ser un número mayor a 0.",
                        "Cantidad Inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ---------------------------------------------------------
                // PASO 4: Agregar al Carrito (El servicio valida el Stock)
                // ---------------------------------------------------------
                // NOTA: Para que esto no de error de stock, recuerda haber hecho el cambio
                // del LEFT JOIN en ProductoRepository.cs
                await _ventaService.AgregarProductoAlCarrito(productoSeleccionado.IdProducto, cantidadSolicitada);

                // Si llegamos aquí, todo salió bien
                ActualizarCarritoUI();

                // Limpieza para la siguiente venta
                txtIdProducto.Clear();
                txtCantidad.Text = "1";
                txtIdProducto.Focus();
            }
            catch (InvalidOperationException ex)
            {
                // Captura el error de stock del servicio
                MessageBox.Show(ex.Message, "Stock Insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar producto:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnConfirmar_Click(object sender, EventArgs e)
        {
            string textoTotal = lblTotal.Text.Replace("Total:", "").Replace("$", "").Trim();

            if (!decimal.TryParse(textoTotal, out decimal totalAEnviar) || totalAEnviar == 0)
            {
                MessageBox.Show("El total es 0 o no se pudo leer. Agrega productos primero.",
                    "Carrito Vacío", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var formCobro = new FormCobro(totalAEnviar))
            {
                var resultado = formCobro.ShowDialog();

                if (formCobro.VentaConfirmada)
                {
                    try
                    {
                        // ✅ CRÍTICO: USAR NOMBRES DE COLUMNAS, NO ÍNDICES
                        var listaParaTicket = new List<TicketDetalle>();

                        foreach (DataGridViewRow row in dgvCarrito.Rows)
                        {
                            // 1. Obtenemos el objeto real que está detrás de la fila
                            var item = row.DataBoundItem as DetalleVentaModel;

                            if (item != null)
                            {
                                // 2. Usamos sus propiedades numéricas directas (sin convertir a texto y volver a numero)
                                listaParaTicket.Add(new TicketDetalle
                                {
                                    Cantidad = item.Cantidad,
                                    Producto = item.NombreProducto,
                                    PrecioUnitario = item.PrecioUnitario, // El decimal puro
                                    Subtotal = item.Subtotal              // El decimal puro
                                });
                            }
                        }

                        // Guardar en BD
                        int? idCliente = null;
                        if (_usuarioActual == null) return;
                        int idVentaGenerada = await _ventaService.ConfirmarVenta(idCliente, _usuarioActual.IdUsuario);

                        // Imprimir ticket
                        try
                        {
                            var ticketService = new TicketService();

                            var datosTicket = new TicketData
                            {
                                NroTicket = idVentaGenerada,
                                Fecha = DateTime.Now,
                                ClienteNombre = string.IsNullOrWhiteSpace(txtCliente.Text)
                                    ? "Consumidor Final"
                                    : txtCliente.Text,
                                Total = formCobro.MontoTotal,
                                PagoCon = formCobro.MontoPagoCon,
                                Vuelto = formCobro.MontoPagoCon - formCobro.MontoTotal,
                                FormaPago = "Efectivo"
                            };

                            ticketService.GenerarTicket(datosTicket, listaParaTicket);
                        }
                        catch (Exception exTicket)
                        {
                            MessageBox.Show(
                                $"✅ Venta registrada con éxito (Ticket #{idVentaGenerada})\n\n" +
                                $"⚠️ Pero hubo un problema al imprimir:\n{exTicket.Message}",
                                "Advertencia de Impresión",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }

                        MessageBox.Show($"¡Venta registrada exitosamente!\n\nTicket Nro: {idVentaGenerada}",
                            "Venta Confirmada", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        ActualizarCarritoUI();
                        CargarHistorial(); // Actualizar historial automáticamente
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al guardar venta:\n\n{ex.Message}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
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

        private async Task CargarInventario()
        {
            try
            {
                // 1. Traemos la data cruda de SQL
                var listaProductos = await _productoRepository.GetAllAsync();

                // --- CAMBIO CLAVE 1: GUARDAR EN EL BOLSILLO ---
                // Convertimos a lista y guardamos en nuestra variable global
                _products = listaProductos.ToList();

                // 2. Llenamos la grilla usando la variable global (no la local)
                dgvInventario.DataSource = _products;

                // --- CAMBIO CLAVE 2: APLICAR MAQUILLAJE ---
                // Llamamos al método que oculta IDs, pone colores y formatos
                ConfigurarGridInventario();
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
            // Dentro de tu método de configurar la grilla del historial
            var dgv = dgvVentas; // O como se llame tu grilla

            // BLINDAJE: Usamos '?' para null check o un 'if' clásico

            // 1. ID de Venta
            if (dgv.Columns["IdVenta"] != null)
            {
                dgv.Columns["IdVenta"].HeaderText = "Nro. Ticket";
                // dgv.Columns["IdVenta"].Visible = false; // Descomenta si prefieres ocultarlo
            }
            else if (dgv.Columns["Id"] != null) // Por si en la clase se llama "Id"
            {
                dgv.Columns["Id"].HeaderText = "Nro. Ticket";
            }

            // 2. Nombre del Cliente
            if (dgv.Columns["ClienteNombre"] != null)
            {
                dgv.Columns["ClienteNombre"].HeaderText = "Cliente";
            }

            // 3. Total (Formato Moneda)
            if (dgv.Columns["Total"] != null)
            {
                dgv.Columns["Total"].HeaderText = "Total";
                dgv.Columns["Total"].DefaultCellStyle.Format = "C2"; // Pesos
                dgv.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            // 4. Fecha
            if (dgv.Columns["Fecha"] != null)
            {
                dgv.Columns["Fecha"].HeaderText = "Fecha y Hora";
                dgv.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            }

            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }
        private async void tabControlPrincipal_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reemplaza 'tabHistorial' por el nombre real de tu pestaña (tabPage2, etc.)
            if (tabControl1.SelectedTab == tabHistorial)
            {
                // Limpiamos la grilla primero para dar feedback visual (opcional)
                dgvVentas.DataSource = null;

                await CargarHistorial();
            }
        }

        // 1. Método para cargar la lista de arriba (Maestro)
        private async Task CargarHistorial()
        {
            try
            {
                // 1. Obtenemos la fecha del control visual (dtpFecha o dtpFechaHistorial)
                // Asegúrate de usar el nombre correcto de tu control
                DateTime fechaSeleccionada = dtpHistorial.Value;

                // 2. Se la enviamos al servicio
                var ventas = await _ventaService.ObtenerHistorialVentas(fechaSeleccionada);

                dgvVentas.DataSource = ventas;

                // Estética y configuraciones...
                EstilizarGridVentas(); // Si tienes este método, úsalo
                dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvVentas.MultiSelect = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar historial: " + ex.Message);
            }
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

        private async void txtIdProducto_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Si presiona ENTER
            if (e.KeyChar == (char)13)
            {
                e.Handled = true; // Quitar sonido bip

                string textoIngresado = txtIdProducto.Text.Trim();
                if (string.IsNullOrEmpty(textoIngresado)) return;

                int idEncontrado = 0;

                // CASO A: Es un número (Escáner o ID manual)
                if (int.TryParse(textoIngresado, out int idDirecto))
                {
                    idEncontrado = idDirecto;
                }
                // CASO B: Es Texto (Eligió del Autocomplete)
                else
                {
                    // Buscamos en nuestra lista en memoria cuál producto tiene ese nombre
                    var producto = _listaProductosBuscador
                        .FirstOrDefault(p => p.Nombre.Equals(textoIngresado, StringComparison.OrdinalIgnoreCase));

                    if (producto != null)
                    {
                        idEncontrado = producto.IdProducto;
                    }
                    else
                    {
                        MessageBox.Show("Producto no encontrado en el catálogo.");
                        return;
                    }
                }

                // Si encontramos un ID válido, procedemos a agregar
                if (idEncontrado > 0)
                {
                    // Pequeño truco: ponemos el ID numérico en el textbox para que btnAgregar funcione
                    txtIdProducto.Text = idEncontrado.ToString();
                    btnAgregar.PerformClick();
                }
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

        private void btnAdminProductos_Click(object sender, EventArgs e)
        {
            // Pedimos una instancia nueva del formulario al contenedor de inyección
            // Esto asegura que el FormProductos reciba su ProductoService automáticamente
            var form = _serviceProvider.GetRequiredService<FormProductos>();
            form.ShowDialog();
            CargarInventario();
        }
        private void FormPrincipal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                btnConfirmar.PerformClick(); // F5 para Cobrar
            }
            if (e.KeyCode == Keys.F10)
            {
                // Foco al buscador de productos para nueva venta
                txtIdProducto.Focus();
            }
        }

        private void dgvInventario_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Asegurarnos de que estamos en la fila correcta y no es el encabezado
            if (e.RowIndex >= 0 && dgvInventario.Columns[e.ColumnIndex].Name == "StockActual")
            {
                // Obtenemos el valor del stock
                if (e.Value != null && int.TryParse(e.Value.ToString(), out int stock))
                {
                    var fila = dgvInventario.Rows[e.RowIndex];

                    // REGLAS DE NEGOCIO (Puedes ajustarlas o traerlas de la BD "StockMinimo")
                    if (stock <= 5)
                    {
                        // CRÍTICO: Fondo Rojo suave, Texto Rojo Oscuro
                        fila.DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 230);
                        fila.DefaultCellStyle.ForeColor = Color.DarkRed;
                        fila.Cells["StockActual"].Style.ForeColor = Color.Red; // El número más fuerte
                    }
                    else if (stock <= 15)
                    {
                        // ADVERTENCIA: Amarillo
                        fila.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 220);
                        fila.DefaultCellStyle.ForeColor = Color.DarkGoldenrod;
                    }
                    else
                    {
                        // NORMAL: Blanco y Negro
                        fila.DefaultCellStyle.BackColor = Color.White;
                        fila.DefaultCellStyle.ForeColor = Color.Black;
                    }
                }
            }
        }

        private void txtBuscarInventario_KeyUp(object sender, KeyEventArgs e)
        {
            string textoBusqueda = txtBuscarInventario.Text.ToLower();

            // Si el bolsillo está vacío, no hacemos nada
            if (_products == null || _products.Count == 0) return;

            // LINQ AL RESCATE:
            // "De la lista completa, fíltrame los que el nombre contenga lo que escribí..."
            var listaFiltrada = _products
                                .Where(p => p.Nombre.Contains(textoBusqueda, StringComparison.CurrentCultureIgnoreCase) ||
                                            p.CategoriaNombre.ToLower().Contains(textoBusqueda)) // Opcional: buscar por categoría tmb
                                .ToList();

            // Actualizamos la grilla con la lista NUEVA y FILTRADA
            dgvInventario.DataSource = null; // A veces necesario para limpiar
            dgvInventario.DataSource = listaFiltrada;

            // IMPORTANTE: Como reseteamos el DataSource, quizás pierdas los formatos (colores, anchos).
            // Vuelve a aplicar el maquillaje visual:
            ConfigurarGridInventario();
        }

        private async void btnAnular_Click(object sender, EventArgs e)
        {
            // ✅ CRÍTICO: Validación segura de selección
            if (dgvVentas.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una venta primero.",
                    "Ninguna Venta Seleccionada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Validar que la celda tenga valor
            if (dgvVentas.CurrentRow.Cells["IdVenta"]?.Value == null)
            {
                MessageBox.Show("No se pudo obtener el ID de la venta.",
                    "Error de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Conversión segura
            if (!int.TryParse(dgvVentas.CurrentRow.Cells["IdVenta"].Value.ToString(), out int idVenta))
            {
                MessageBox.Show("El ID de venta no es válido.",
                    "Error de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validar estado actual
            string estadoActual = dgvVentas.CurrentRow.Cells["Estado"]?.Value?.ToString() ?? "";

            if (estadoActual.Equals("Anulada", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Esta venta ya se encuentra anulada.",
                    "Venta Ya Anulada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Confirmar anulación
            var confirm = MessageBox.Show(
                $"¿Está seguro que desea anular la venta #{idVenta}?\n\n" +
                "El stock de los productos será devuelto al inventario.",
                "Confirmar Anulación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    string motivoAnulacion = "Solicitud de cliente / Error en carga";

                    await _ventaService.AnularVenta(idVenta, motivoAnulacion);

                    MessageBox.Show("✅ Venta anulada correctamente.\n\nEl stock ha sido restaurado.",
                        "Anulación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    await CargarHistorial(); // Refrescar
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    MessageBox.Show($"Error de Base de Datos:\n\n{ex.Message}",
                        "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al anular:\n\n{ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        // ✅ CRÍTICO: Control de permisos por rol
        public void AsignarUsuario(Usuario usuario)
        {
            _usuarioActual = usuario ?? throw new ArgumentNullException(nameof(usuario));

            // Cambiar título de ventana
            this.Text = $"Sistema de Ventas - Usuario: {_usuarioActual.NombreCompleto} ({_usuarioActual.Rol})";

            // ✅ CRÍTICO: APLICAR PERMISOS POR ROL
            switch (_usuarioActual.Rol.ToUpper())
            {
                case "CAJERO":
                    // Cajero: Solo puede vender, no administrar
                    btnAdminProductos.Visible = false;
                    btnAnular.Visible = false;

                    // Quitar pestaña de inventario completamente
                    if (tabControl1.TabPages.Contains(tabInventario))
                    {
                        tabControl1.TabPages.Remove(tabInventario);
                    }
                    break;

                case "VENDEDOR":
                    // Vendedor: Puede vender y ver inventario, pero no anular ni administrar precios
                    btnAnular.Visible = false;
                    btnAdminProductos.Visible = false;
                    break;

                case "ADMIN":
                case "ADMINISTRADOR":
                    // Admin: Acceso total (sin cambios)
                    btnAdminProductos.Visible = true;
                    btnAnular.Visible = true;
                    break;

                default:
                    // Rol desconocido: acceso mínimo por seguridad
                    MessageBox.Show($"Rol '{_usuarioActual.Rol}' no reconocido. Acceso limitado aplicado.",
                        "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    btnAdminProductos.Visible = false;
                    btnAnular.Visible = false;

                    if (tabControl1.TabPages.Contains(tabInventario))
                    {
                        tabControl1.TabPages.Remove(tabInventario);
                    }
                    break;
            }
        }

        private async void btnBuscarVentas_Click(object sender, EventArgs e)
        {
            await CargarHistorial();
        }

        private void btnCerrarCaja_Click(object sender, EventArgs e)
        {
            using (var formCierre = _serviceProvider.GetRequiredService<FormCierreCaja>())
            {
                formCierre.Inicializar(_usuarioActual.IdUsuario);
                var resultado = formCierre.ShowDialog();

                if (resultado == DialogResult.OK)
                {
                    // Al cerrar caja, sacamos al usuario del sistema
                    Application.Exit();
                    // O podrías reiniciar la app para volver al Login:
                    // Application.Restart();
                }
            }
        }
    }
}