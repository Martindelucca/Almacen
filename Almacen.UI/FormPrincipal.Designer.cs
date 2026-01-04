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
            lblTotal = new Label();
            txtCliente = new TextBox();
            Cliente = new Label();
            btnConfirmar = new Button();
            dgvCarrito = new DataGridView();
            btnAgregar = new Button();
            txtCantidad = new TextBox();
            label2 = new Label();
            txtIdProducto = new TextBox();
            lbl_producto = new Label();
            tabInventario = new TabPage();
            btnRefrescar = new Button();
            dgvInventario = new DataGridView();
            tabControl1.SuspendLayout();
            Ventas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCarrito).BeginInit();
            tabInventario.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvInventario).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(Ventas);
            tabControl1.Controls.Add(tabInventario);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(978, 501);
            tabControl1.TabIndex = 12;
            // 
            // Ventas
            // 
            Ventas.Controls.Add(lblTotal);
            Ventas.Controls.Add(txtCliente);
            Ventas.Controls.Add(Cliente);
            Ventas.Controls.Add(btnConfirmar);
            Ventas.Controls.Add(dgvCarrito);
            Ventas.Controls.Add(btnAgregar);
            Ventas.Controls.Add(txtCantidad);
            Ventas.Controls.Add(label2);
            Ventas.Controls.Add(txtIdProducto);
            Ventas.Controls.Add(lbl_producto);
            Ventas.Location = new Point(4, 29);
            Ventas.Name = "Ventas";
            Ventas.Padding = new Padding(3);
            Ventas.Size = new Size(970, 468);
            Ventas.TabIndex = 0;
            Ventas.Text = "Ventas";
            Ventas.UseVisualStyleBackColor = true;
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTotal.Location = new Point(424, 423);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(127, 31);
            lblTotal.TabIndex = 21;
            lblTotal.Text = "Total: $0.00";
            // 
            // txtCliente
            // 
            txtCliente.Location = new Point(811, 12);
            txtCliente.Name = "txtCliente";
            txtCliente.Size = new Size(125, 27);
            txtCliente.TabIndex = 20;
            // 
            // Cliente
            // 
            Cliente.AutoSize = true;
            Cliente.Location = new Point(725, 15);
            Cliente.Name = "Cliente";
            Cliente.Size = new Size(55, 20);
            Cliente.TabIndex = 19;
            Cliente.Text = "Cliente";
            // 
            // btnConfirmar
            // 
            btnConfirmar.BackColor = Color.LimeGreen;
            btnConfirmar.Location = new Point(740, 425);
            btnConfirmar.Name = "btnConfirmar";
            btnConfirmar.Size = new Size(143, 34);
            btnConfirmar.TabIndex = 18;
            btnConfirmar.Text = "Confirmar Venta";
            btnConfirmar.UseVisualStyleBackColor = false;
            btnConfirmar.Click += btnConfirmar_Click;
            // 
            // dgvCarrito
            // 
            dgvCarrito.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCarrito.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCarrito.Location = new Point(34, 119);
            dgvCarrito.Name = "dgvCarrito";
            dgvCarrito.ReadOnly = true;
            dgvCarrito.RowHeadersWidth = 51;
            dgvCarrito.Size = new Size(672, 278);
            dgvCarrito.TabIndex = 17;
            // 
            // btnAgregar
            // 
            btnAgregar.Location = new Point(490, 65);
            btnAgregar.Name = "btnAgregar";
            btnAgregar.Size = new Size(170, 29);
            btnAgregar.TabIndex = 16;
            btnAgregar.Text = "Agregar a Carrito";
            btnAgregar.UseVisualStyleBackColor = true;
            btnAgregar.Click += btnAgregar_Click;
            // 
            // txtCantidad
            // 
            txtCantidad.Location = new Point(256, 67);
            txtCantidad.Name = "txtCantidad";
            txtCantidad.Size = new Size(156, 27);
            txtCantidad.TabIndex = 15;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(296, 44);
            label2.Name = "label2";
            label2.Size = new Size(69, 20);
            label2.TabIndex = 14;
            label2.Text = "Cantidad";
            // 
            // txtIdProducto
            // 
            txtIdProducto.Location = new Point(34, 67);
            txtIdProducto.Name = "txtIdProducto";
            txtIdProducto.Size = new Size(156, 27);
            txtIdProducto.TabIndex = 13;
            // 
            // lbl_producto
            // 
            lbl_producto.AutoSize = true;
            lbl_producto.Location = new Point(71, 44);
            lbl_producto.Name = "lbl_producto";
            lbl_producto.Size = new Size(69, 20);
            lbl_producto.TabIndex = 12;
            lbl_producto.Text = "Producto";
            // 
            // tabInventario
            // 
            tabInventario.Controls.Add(btnRefrescar);
            tabInventario.Controls.Add(dgvInventario);
            tabInventario.Location = new Point(4, 29);
            tabInventario.Name = "tabInventario";
            tabInventario.Padding = new Padding(3);
            tabInventario.Size = new Size(970, 468);
            tabInventario.TabIndex = 1;
            tabInventario.Text = "Inventario";
            tabInventario.UseVisualStyleBackColor = true;
            // 
            // btnRefrescar
            // 
            btnRefrescar.Location = new Point(822, 433);
            btnRefrescar.Name = "btnRefrescar";
            btnRefrescar.Size = new Size(131, 29);
            btnRefrescar.TabIndex = 1;
            btnRefrescar.Text = "Actualizar Stock";
            btnRefrescar.UseVisualStyleBackColor = true;
            btnRefrescar.Click += btnRefrescar_Click;
            // 
            // dgvInventario
            // 
            dgvInventario.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvInventario.Dock = DockStyle.Left;
            dgvInventario.Location = new Point(3, 3);
            dgvInventario.Name = "dgvInventario";
            dgvInventario.RowHeadersWidth = 51;
            dgvInventario.Size = new Size(971, 462);
            dgvInventario.TabIndex = 0;
            // 
            // FormPrincipal
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1002, 525);
            Controls.Add(tabControl1);
            Name = "FormPrincipal";
            Text = "Registro de ventas";
            Load += FormPrincipal_Load;
            tabControl1.ResumeLayout(false);
            Ventas.ResumeLayout(false);
            Ventas.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCarrito).EndInit();
            tabInventario.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvInventario).EndInit();
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
    }
}
