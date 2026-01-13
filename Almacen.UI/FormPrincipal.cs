using Almacen.Business.Models;
using Almacen.Business.Services;
using Almacen.Core.Dtos;
using Almacen.Core.Entities;
using Almacen.Core.Interfaces; // Necesario si usas el repo de productos para búsquedas extra
using Almacen.Data.Repositories;
using Almacen.UI.Services;
using System;
using System.Diagnostics;
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

        // BindingSource actúa como intermediario entre la Lista y el DataGridView
        private readonly BindingSource _bindingSource = new BindingSource();

        // CONSTRUCTOR: Aquí recibimos el servicio gracias al Program.cs
        public FormPrincipal(VentaService ventaService, IProductoRepository productoRepo, ProductoService productoService)
        {
            InitializeComponent();
            _ventaService = ventaService;
            _productoRepository = productoRepo;
            _productoService = productoService;
        }

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            // Configuramos la grilla al iniciar
            ConfigurarGrid();
            CargarInventario();
            EstilizarGridModerno();
            ConfigurarBuscador();
            CargarHistorial();
            ConfigurarGridInventario();
            EstilizarGridVentas();

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
                int idProductoParaAgregar = 0;

                // PASO 1: Determinar el ID (Sea por número o por nombre)

                // ¿Es un número directo? (Ej: Escáner de barras o escribió "1")
                if (int.TryParse(textoInput, out int idNumerico))
                {
                    idProductoParaAgregar = idNumerico;
                }
                // ¿Es texto? (Ej: Seleccionó "Coca Cola" del buscador)
                else
                {
                    // Buscamos en nuestra lista en memoria
                    // Usamos _listaProductosBuscador que cargamos en el Load
                    if (_listaProductosBuscador != null)
                    {
                        var productoEncontrado = _listaProductosBuscador
                            .FirstOrDefault(p => p.Nombre.Equals(textoInput, StringComparison.OrdinalIgnoreCase));

                        if (productoEncontrado != null)
                        {
                            idProductoParaAgregar = productoEncontrado.IdProducto;
                        }
                    }
                }

                // PASO 2: Validar si encontramos algo
                if (idProductoParaAgregar == 0)
                {
                    MessageBox.Show("No se encontró un producto válido con ese código o nombre.");
                    return;
                }

                // PASO 3: Validar Cantidad
                if (!decimal.TryParse(txtCantidad.Text, out decimal cantidad))
                {
                    MessageBox.Show("La cantidad debe ser un número válido.");
                    return;
                }

                // PASO 4: Llamar al Servicio (Negocio)
                await _ventaService.AgregarProductoAlCarrito(idProductoParaAgregar, cantidad);

                // PASO 5: Refrescar UI y Limpiar
                ActualizarCarritoUI();

                txtIdProducto.Clear();
                txtCantidad.Text = "1";
                txtIdProducto.Focus(); // Volver el foco para seguir vendiendo rápido
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void btnConfirmar_Click(object sender, EventArgs e)
        {
            // ESTRATEGIA SEGURA: Leer el total del Label que ya se ve en pantalla
            // Quitamos el signo $ y espacios para obtener solo el número
            string textoTotal = lblTotal.Text.Replace("Total:", "").Replace("$", "").Trim();

            decimal totalAEnviar = 0;

            if (!decimal.TryParse(textoTotal, out totalAEnviar) || totalAEnviar == 0)
            {
                MessageBox.Show("El total es 0 o no se pudo leer. Agrega productos primero.");
                return;
            }

            // 2. ABRIR EL FORMULARIO DE COBRO (MODAL)
            // Pasamos el total en el constructor
            using (var formCobro = new FormCobro(totalAEnviar))
            {
                var resultado = formCobro.ShowDialog(); // Espera aquí hasta que se cierre

                // 3. VERIFICAR SI PAGÓ
                if (formCobro.VentaConfirmada)
                {
                    // ¡AHORA SÍ GUARDAMOS EN BASE DE DATOS!
                    try
                    {
                        // Aquí va tu lógica original de guardar venta
                        int? idCliente = null; // O la lógica que tengas para cliente

                        // NOTA: Ajusta esto a tu método real de CrearVenta
                        int idVentaGenerada = await _ventaService.ConfirmarVenta(idCliente);
                        // --- NUEVO: GENERAR TICKET ---
                        // 2. GENERAR TICKET
                        try
                        {
                            var ticketService = new TicketService();

                            // A. Preparamos la CABECERA del ticket
                            var datosTicket = new TicketData
                            {
                                NroTicket = idVentaGenerada, // <--- TODO: Cambiar esto cuando tu Repo devuelva el ID real
                                Fecha = DateTime.Now,
                                Cliente = "Consumidor Final",
                                Total = formCobro.MontoTotal,
                                PagoCon = formCobro.MontoPagoCon,
                                Vuelto = formCobro.MontoPagoCon - formCobro.MontoTotal
                            };

                            // B. Preparamos la LISTA DE PRODUCTOS
                            var listaProductos = new List<TicketDetalle>();

                            foreach (DataGridViewRow row in dgvCarrito.Rows)
                            {
                                // Ignorar fila vacía
                                if (row.IsNewRow) continue;
                                if (row.Cells[3].Value == null) continue;

                                listaProductos.Add(new TicketDetalle
                                {
                                    Cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value),
                                    Producto = row.Cells["3"].Value.ToString(), // <--- Nombre correcto
                                    Subtotal = Convert.ToDecimal(row.Cells["Subtotal"].Value)
                                });
                            }

                            // C. IMPRIMIR
                            ticketService.GenerarTicket(datosTicket, listaProductos);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Venta OK, pero error al imprimir: " + ex.Message);
                        }

                        MessageBox.Show("¡Venta registrada exitosamente!");

                        // Limpieza post-venta
                        ActualizarCarritoUI();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al guardar venta: " + ex.Message);
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
            var formProductos = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<FormProductos>(Program.ServiceProvider);

            // Lo abrimos como MODAL (bloquea la ventana de atrás hasta que cierres esta)
            // Esto evita que vendas un producto mientras le cambias el precio al mismo tiempo.
            formProductos.ShowDialog();

            // Al volver, refrescamos el inventario por si cambiaste algo
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
                                .Where(p => p.Nombre.ToLower().Contains(textoBusqueda) ||
                                            p.CategoriaNombre.ToLower().Contains(textoBusqueda)) // Opcional: buscar por categoría tmb
                                .ToList();

            // Actualizamos la grilla con la lista NUEVA y FILTRADA
            dgvInventario.DataSource = null; // A veces necesario para limpiar
            dgvInventario.DataSource = listaFiltrada;

            // IMPORTANTE: Como reseteamos el DataSource, quizás pierdas los formatos (colores, anchos).
            // Vuelve a aplicar el maquillaje visual:
            ConfigurarGridInventario();
        }

    }
}