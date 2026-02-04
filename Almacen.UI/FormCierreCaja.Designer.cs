namespace Almacen.UI
{
    partial class FormCierreCaja
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
            lblSistema = new Label();
            txtReal = new TextBox();
            btnCerrar = new Button();
            lblInfo = new Label();
            SuspendLayout();
            // 
            // lblSistema
            // 
            lblSistema.AutoSize = true;
            lblSistema.Location = new Point(123, 78);
            lblSistema.Name = "lblSistema";
            lblSistema.Size = new Size(50, 20);
            lblSistema.TabIndex = 0;
            lblSistema.Text = "label1";
            // 
            // txtReal
            // 
            txtReal.Location = new Point(96, 135);
            txtReal.Name = "txtReal";
            txtReal.Size = new Size(125, 27);
            txtReal.TabIndex = 1;
            // 
            // btnCerrar
            // 
            btnCerrar.Location = new Point(107, 200);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(94, 29);
            btnCerrar.TabIndex = 2;
            btnCerrar.Text = "Cerrar";
            btnCerrar.UseVisualStyleBackColor = true;
            btnCerrar.Click += btnCerrar_Click;
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(38, 22);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(50, 20);
            lblInfo.TabIndex = 3;
            lblInfo.Text = "label1";
            // 
            // FormCierreCaja
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(351, 353);
            Controls.Add(lblInfo);
            Controls.Add(btnCerrar);
            Controls.Add(txtReal);
            Controls.Add(lblSistema);
            Name = "FormCierreCaja";
            Text = "FormCierreCaja";
            Load += FormCierreCaja_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblSistema;
        private TextBox txtReal;
        private Button btnCerrar;
        private Label lblInfo;
    }
}