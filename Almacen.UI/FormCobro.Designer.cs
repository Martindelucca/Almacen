namespace Almacen.UI
{
    partial class FormCobro
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblTotal = new Label();
            lblTitulo = new Label();
            txtPagaCon = new TextBox();
            btnFinalizar = new Button();
            btnCancelar = new Button();
            lblVuelto = new Label();
            lblTituloTotal = new Label();
            lblTituloPago = new Label();
            panel1 = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Font = new Font("Segoe UI", 25.8000011F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTotal.Location = new Point(128, 46);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(143, 60);
            lblTotal.TabIndex = 0;
            lblTotal.Text = "$ 0.00";
            lblTotal.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblTitulo
            // 
            lblTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitulo.ForeColor = SystemColors.ControlDarkDark;
            lblTitulo.Location = new Point(146, 27);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(107, 30);
            lblTitulo.TabIndex = 1;
            lblTitulo.Text = "Su vuelto:";
            // 
            // txtPagaCon
            // 
            txtPagaCon.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPagaCon.Location = new Point(128, 148);
            txtPagaCon.Name = "txtPagaCon";
            txtPagaCon.Size = new Size(133, 47);
            txtPagaCon.TabIndex = 2;
            txtPagaCon.TextAlign = HorizontalAlignment.Right;
            txtPagaCon.TextChanged += txtPagaCon_TextChanged;
            // 
            // btnFinalizar
            // 
            btnFinalizar.AutoSize = true;
            btnFinalizar.BackColor = Color.SeaGreen;
            btnFinalizar.Cursor = Cursors.Hand;
            btnFinalizar.Enabled = false;
            btnFinalizar.FlatAppearance.BorderSize = 0;
            btnFinalizar.FlatStyle = FlatStyle.Flat;
            btnFinalizar.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnFinalizar.ForeColor = Color.White;
            btnFinalizar.Location = new Point(156, 155);
            btnFinalizar.Name = "btnFinalizar";
            btnFinalizar.Size = new Size(94, 38);
            btnFinalizar.TabIndex = 3;
            btnFinalizar.Text = "Finalizar";
            btnFinalizar.UseVisualStyleBackColor = false;
            btnFinalizar.Click += btnFinalizar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.BackColor = Color.White;
            btnCancelar.Cursor = Cursors.Hand;
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnCancelar.ForeColor = Color.DimGray;
            btnCancelar.Location = new Point(343, 210);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(94, 29);
            btnCancelar.TabIndex = 4;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // lblVuelto
            // 
            lblVuelto.AutoSize = true;
            lblVuelto.Font = new Font("Segoe UI", 25.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblVuelto.ForeColor = Color.DimGray;
            lblVuelto.Location = new Point(122, 70);
            lblVuelto.Name = "lblVuelto";
            lblVuelto.Size = new Size(149, 60);
            lblVuelto.TabIndex = 5;
            lblVuelto.Text = "$ 0.00";
            // 
            // lblTituloTotal
            // 
            lblTituloTotal.AutoSize = true;
            lblTituloTotal.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTituloTotal.ForeColor = SystemColors.ControlDarkDark;
            lblTituloTotal.Location = new Point(128, 23);
            lblTituloTotal.Name = "lblTituloTotal";
            lblTituloTotal.Size = new Size(131, 23);
            lblTituloTotal.TabIndex = 6;
            lblTituloTotal.Text = "TOTAL A PAGAR";
            // 
            // lblTituloPago
            // 
            lblTituloPago.AutoSize = true;
            lblTituloPago.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTituloPago.ForeColor = SystemColors.ControlDarkDark;
            lblTituloPago.Location = new Point(146, 122);
            lblTituloPago.Name = "lblTituloPago";
            lblTituloPago.Size = new Size(95, 23);
            lblTituloPago.TabIndex = 7;
            lblTituloPago.Text = "PAGA CON";
            // 
            // panel1
            // 
            panel1.BackColor = Color.LightGray;
            panel1.Controls.Add(lblTitulo);
            panel1.Controls.Add(lblVuelto);
            panel1.Controls.Add(btnFinalizar);
            panel1.Controls.Add(btnCancelar);
            panel1.Location = new Point(0, 201);
            panel1.Name = "panel1";
            panel1.Size = new Size(440, 242);
            panel1.TabIndex = 8;
            // 
            // FormCobro
            // 
            AcceptButton = btnFinalizar;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(437, 447);
            ControlBox = false;
            Controls.Add(panel1);
            Controls.Add(lblTituloPago);
            Controls.Add(lblTituloTotal);
            Controls.Add(txtPagaCon);
            Controls.Add(lblTotal);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormCobro";
            StartPosition = FormStartPosition.CenterParent;
            Text = "FormCobro";
            Load += FormCobro_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTotal;
        private Label lblTitulo;
        private TextBox txtPagaCon;
        private Button btnFinalizar;
        private Button btnCancelar;
        private Label lblVuelto;
        private Label lblTituloTotal;
        private Label lblTituloPago;
        private Panel panel1;
    }
}