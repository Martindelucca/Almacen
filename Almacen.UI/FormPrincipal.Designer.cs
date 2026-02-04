namespace Almacen.UI
{
    partial class FormPrincipal
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            Ventas = new TabPage();
            btnAdminProductos = new Button();
            panel1 = new Panel();
            lbl_producto = new Label();
            txtIdProducto = new TextBox();
            txtCliente = new TextBox();
            label2 = new Label();
            Cliente = new Label();
            txtCantidad = new TextBox();
            lblTotal = new Label();
            btnConfirmar = new Button();
            dgvCarrito = new DataGridView();
            btnAgregar = new Button();
            menuStrip1 = new MenuStrip();
            tabInventario = new TabPage();
            txtBuscarInventario = new TextBox();
            btnRefrescar = new Button();
            label1 = new Label();
            dgvInventario = new DataGridView();
            tabHistorial = new TabPage();
            btnBuscarVentas = new Button();
            dtpHistorial = new DateTimePicker();
            btnAnular = new Button();
            dgvDetalleVenta = new DataGridView();
            lblDetalle = new Label();
            dgvVentas = new DataGridView();
            btnCerrarCaja = new Button();
            tabControl1.SuspendLayout();
            Ventas.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCarrito).BeginInit();
            tabInventario.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvInventario).BeginInit();
            tabHistorial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDetalleVenta).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvVentas).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(Ventas);
            tabControl1.Controls.Add(tabInventario);
            tabControl1.Controls.Add(tabHistorial);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(906, 632);
            tabControl1.TabIndex = 12;
            tabControl1.SelectedIndexChanged += FormPrincipal_Load;
            // 
            // Ventas
            // 
            Ventas.Controls.Add(btnCerrarCaja);
            Ventas.Controls.Add(btnAdminProductos);
            Ventas.Controls.Add(panel1);
            Ventas.Controls.Add(lblTotal);
            Ventas.Controls.Add(btnConfirmar);
            Ventas.Controls.Add(dgvCarrito);
            Ventas.Controls.Add(btnAgregar);
            Ventas.Controls.Add(menuStrip1);
            Ventas.Location = new Point(4, 29);
            Ventas.Name = "Ventas";
            Ventas.Padding = new Padding(3);
            Ventas.Size = new Size(898, 599);
            Ventas.TabIndex = 0;
            Ventas.Text = "Ventas";
            Ventas.UseVisualStyleBackColor = true;
            // 
            // btnAdminProductos
            // 
            btnAdminProductos.Cursor = Cursors.Hand;
            btnAdminProductos.Location = new Point(827, 567);
            btnAdminProductos.Name = "btnAdminProductos";
            btnAdminProductos.Size = new Size(65, 29);
            btnAdminProductos.TabIndex = 21;
            btnAdminProductos.Text = "⚙️ Admin Productos";
            btnAdminProductos.UseVisualStyleBackColor = true;
            btnAdminProductos.Click += btnAdminProductos_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(lbl_producto);
            panel1.Controls.Add(txtIdProducto);
            panel1.Controls.Add(txtCliente);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(Cliente);
            panel1.Controls.Add(txtCantidad);
            panel1.Location = new Point(18, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(835, 92);
            panel1.TabIndex = 22;
            // 
            // lbl_producto
            // 
            lbl_producto.AutoSize = true;
            lbl_producto.Location = new Point(62, 11);
            lbl_producto.Name = "lbl_producto";
            lbl_producto.Size = new Size(69, 20);
            lbl_producto.TabIndex = 12;
            lbl_producto.Text = "Producto";
            // 
            // txtIdProducto
            // 
            txtIdProducto.Location = new Point(31, 34);
            txtIdProducto.Name = "txtIdProducto";
            txtIdProducto.Size = new Size(156, 27);
            txtIdProducto.TabIndex = 13;
            txtIdProducto.KeyPress += txtIdProducto_KeyPress;
            // 
            // txtCliente
            // 
            txtCliente.Location = new Point(693, 6);
            txtCliente.Name = "txtCliente";
            txtCliente.Size = new Size(125, 27);
            txtCliente.TabIndex = 20;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(298, 11);
            label2.Name = "label2";
            label2.Size = new Size(69, 20);
            label2.TabIndex = 14;
            label2.Text = "Cantidad";
            // 
            // Cliente
            // 
            Cliente.AutoSize = true;
            Cliente.Location = new Point(612, 11);
            Cliente.Name = "Cliente";
            Cliente.Size = new Size(55, 20);
            Cliente.TabIndex = 19;
            Cliente.Text = "Cliente";
            // 
            // txtCantidad
            // 
            txtCantidad.Location = new Point(254, 34);
            txtCantidad.Name = "txtCantidad";
            txtCantidad.Size = new Size(156, 27);
            txtCantidad.TabIndex = 15;
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Font = new Font("Segoe UI", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotal.ForeColor = SystemColors.HotTrack;
            lblTotal.Location = new Point(35, 518);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(206, 46);
            lblTotal.TabIndex = 21;
            lblTotal.Text = "Total: $0.00";
            // 
            // btnConfirmar
            // 
            btnConfirmar.BackColor = Color.FromArgb(40, 167, 69);
            btnConfirmar.Cursor = Cursors.Hand;
            btnConfirmar.FlatAppearance.BorderSize = 0;
            btnConfirmar.FlatStyle = FlatStyle.Flat;
            btnConfirmar.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnConfirmar.ForeColor = SystemColors.ButtonHighlight;
            btnConfirmar.Location = new Point(371, 530);
            btnConfirmar.Name = "btnConfirmar";
            btnConfirmar.Size = new Size(158, 35);
            btnConfirmar.TabIndex = 18;
            btnConfirmar.Text = "Confirmar Venta";
            btnConfirmar.UseVisualStyleBackColor = false;
            btnConfirmar.Click += btnConfirmar_Click;
            // 
            // dgvCarrito
            // 
            dgvCarrito.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvCarrito.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCarrito.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCarrito.Location = new Point(35, 141);
            dgvCarrito.Name = "dgvCarrito";
            dgvCarrito.ReadOnly = true;
            dgvCarrito.RowHeadersWidth = 51;
            dgvCarrito.Size = new Size(818, 345);
            dgvCarrito.TabIndex = 17;
            dgvCarrito.CellContentClick += dgvCarrito_CellContentClick;
            // 
            // btnAgregar
            // 
            btnAgregar.BackColor = Color.DodgerBlue;
            btnAgregar.Cursor = Cursors.Hand;
            btnAgregar.FlatAppearance.BorderSize = 0;
            btnAgregar.FlatStyle = FlatStyle.Flat;
            btnAgregar.Font = new Font("Segoe UI Semibold", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAgregar.ForeColor = SystemColors.ButtonHighlight;
            btnAgregar.Location = new Point(655, 101);
            btnAgregar.Name = "btnAgregar";
            btnAgregar.Size = new Size(170, 34);
            btnAgregar.TabIndex = 16;
            btnAgregar.Text = "Agregar a Carrito";
            btnAgregar.UseVisualStyleBackColor = false;
            btnAgregar.Click += btnAgregar_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Location = new Point(3, 3);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(892, 24);
            menuStrip1.TabIndex = 23;
            menuStrip1.Text = "menuStrip1";
            // 
            // tabInventario
            // 
            tabInventario.Controls.Add(txtBuscarInventario);
            tabInventario.Controls.Add(btnRefrescar);
            tabInventario.Controls.Add(label1);
            tabInventario.Controls.Add(dgvInventario);
            tabInventario.Location = new Point(4, 29);
            tabInventario.Name = "tabInventario";
            tabInventario.Padding = new Padding(3);
            tabInventario.Size = new Size(898, 599);
            tabInventario.TabIndex = 1;
            tabInventario.Text = "Inventario";
            tabInventario.UseVisualStyleBackColor = true;
            // 
            // txtBuscarInventario
            // 
            txtBuscarInventario.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtBuscarInventario.Location = new Point(268, 26);
            txtBuscarInventario.Name = "txtBuscarInventario";
            txtBuscarInventario.Size = new Size(276, 27);
            txtBuscarInventario.TabIndex = 3;
            txtBuscarInventario.KeyUp += txtBuscarInventario_KeyUp;
            // 
            // btnRefrescar
            // 
            btnRefrescar.Location = new Point(693, 15);
            btnRefrescar.Name = "btnRefrescar";
            btnRefrescar.Size = new Size(131, 29);
            btnRefrescar.TabIndex = 1;
            btnRefrescar.Text = "Actualizar Stock";
            btnRefrescar.UseVisualStyleBackColor = true;
            btnRefrescar.Click += btnRefrescar_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(347, 3);
            label1.Name = "label1";
            label1.Size = new Size(55, 20);
            label1.TabIndex = 2;
            label1.Text = "Buscar:";
            // 
            // dgvInventario
            // 
            dgvInventario.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvInventario.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvInventario.Location = new Point(3, 59);
            dgvInventario.Name = "dgvInventario";
            dgvInventario.RowHeadersWidth = 51;
            dgvInventario.Size = new Size(889, 534);
            dgvInventario.TabIndex = 0;
            dgvInventario.CellFormatting += dgvInventario_CellFormatting;
            // 
            // tabHistorial
            // 
            tabHistorial.Controls.Add(btnBuscarVentas);
            tabHistorial.Controls.Add(dtpHistorial);
            tabHistorial.Controls.Add(btnAnular);
            tabHistorial.Controls.Add(dgvDetalleVenta);
            tabHistorial.Controls.Add(lblDetalle);
            tabHistorial.Controls.Add(dgvVentas);
            tabHistorial.Location = new Point(4, 29);
            tabHistorial.Name = "tabHistorial";
            tabHistorial.Padding = new Padding(3);
            tabHistorial.Size = new Size(898, 599);
            tabHistorial.TabIndex = 2;
            tabHistorial.Text = "Historial";
            tabHistorial.UseVisualStyleBackColor = true;
            // 
            // btnBuscarVentas
            // 
            btnBuscarVentas.Location = new Point(568, 16);
            btnBuscarVentas.Name = "btnBuscarVentas";
            btnBuscarVentas.Size = new Size(94, 29);
            btnBuscarVentas.TabIndex = 6;
            btnBuscarVentas.Text = "Buscar";
            btnBuscarVentas.UseVisualStyleBackColor = true;
            btnBuscarVentas.Click += btnBuscarVentas_Click;
            // 
            // dtpHistorial
            // 
            dtpHistorial.Location = new Point(287, 15);
            dtpHistorial.Name = "dtpHistorial";
            dtpHistorial.Size = new Size(250, 27);
            dtpHistorial.TabIndex = 5;
            // 
            // btnAnular
            // 
            btnAnular.Location = new Point(798, 567);
            btnAnular.Name = "btnAnular";
            btnAnular.Size = new Size(94, 29);
            btnAnular.TabIndex = 4;
            btnAnular.Text = "Anular";
            btnAnular.UseVisualStyleBackColor = true;
            btnAnular.Click += btnAnular_Click;
            // 
            // dgvDetalleVenta
            // 
            dgvDetalleVenta.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDetalleVenta.Location = new Point(6, 397);
            dgvDetalleVenta.Name = "dgvDetalleVenta";
            dgvDetalleVenta.RowHeadersWidth = 51;
            dgvDetalleVenta.Size = new Size(855, 154);
            dgvDetalleVenta.TabIndex = 2;
            // 
            // lblDetalle
            // 
            lblDetalle.AutoSize = true;
            lblDetalle.Location = new Point(329, 374);
            lblDetalle.Name = "lblDetalle";
            lblDetalle.Size = new Size(208, 20);
            lblDetalle.TabIndex = 1;
            lblDetalle.Text = "Detalle de venta seleccionada";
            // 
            // dgvVentas
            // 
            dgvVentas.AllowUserToAddRows = false;
            dgvVentas.AllowUserToDeleteRows = false;
            dgvVentas.AllowUserToResizeColumns = false;
            dgvVentas.AllowUserToResizeRows = false;
            dgvVentas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvVentas.Location = new Point(6, 60);
            dgvVentas.Name = "dgvVentas";
            dgvVentas.RowHeadersWidth = 51;
            dgvVentas.Size = new Size(855, 295);
            dgvVentas.TabIndex = 0;
            dgvVentas.SelectionChanged += dgvVentas_SelectionChanged;
            // 
            // btnCerrarCaja
            // 
            btnCerrarCaja.Location = new Point(711, 567);
            btnCerrarCaja.Name = "btnCerrarCaja";
            btnCerrarCaja.Size = new Size(92, 29);
            btnCerrarCaja.TabIndex = 24;
            btnCerrarCaja.Text = "Cerrar Caja";
            btnCerrarCaja.UseVisualStyleBackColor = true;
            btnCerrarCaja.Click += btnCerrarCaja_Click;
            // 
            // FormPrincipal
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(937, 656);
            Controls.Add(tabControl1);
            KeyPreview = true;
            MainMenuStrip = menuStrip1;
            Name = "FormPrincipal";
            Text = "Registro de ventas";
            Load += FormPrincipal_Load;
            KeyPress += txtIdProducto_KeyPress;
            tabControl1.ResumeLayout(false);
            Ventas.ResumeLayout(false);
            Ventas.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCarrito).EndInit();
            tabInventario.ResumeLayout(false);
            tabInventario.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvInventario).EndInit();
            tabHistorial.ResumeLayout(false);
            tabHistorial.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDetalleVenta).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvVentas).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private TabControl tabControl1;
        private TabPage Ventas;
        private TabPage tabInventario;
        private Label lblTotal;
        private TextBox txtCliente;
        private Label Cliente;
        private Button btnConfirmar;
        private DataGridView dgvCarrito;
        private Button btnAgregar;
        private TextBox txtCantidad;
        private Label label2;
        private TextBox txtIdProducto;
        private Label lbl_producto;
        private Button btnRefrescar;
        private DataGridView dgvInventario;
        private TabPage tabHistorial;
        private DataGridView dgvVentas;
        private DataGridView dgvDetalleVenta;
        private Label lblDetalle;
        private Panel panel1;
        private MenuStrip menuStrip1;
        private Button btnAdminProductos;
        private TextBox txtBuscarInventario;
        private Label label1;
        private Button btnAnular;
        private DateTimePicker dtpHistorial;
        private Button btnBuscarVentas;
        private Button btnCerrarCaja;
    }
}
