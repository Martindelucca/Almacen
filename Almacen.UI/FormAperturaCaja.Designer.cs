namespace Almacen.UI
{
    partial class FormAperturaCaja
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
            btnAbrir = new Button();
            label1 = new Label();
            label2 = new Label();
            txtMontoInicial = new TextBox();
            SuspendLayout();
            // 
            // btnAbrir
            // 
            btnAbrir.Location = new Point(188, 241);
            btnAbrir.Name = "btnAbrir";
            btnAbrir.Size = new Size(94, 29);
            btnAbrir.TabIndex = 0;
            btnAbrir.Text = "Abrir Caja";
            btnAbrir.UseVisualStyleBackColor = true;
            btnAbrir.Click += btnAbrir_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(90, 30);
            label1.Name = "label1";
            label1.Size = new Size(280, 41);
            label1.TabIndex = 1;
            label1.Text = "APERTURA DE CAJA";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(65, 141);
            label2.Name = "label2";
            label2.Size = new Size(99, 20);
            label2.TabIndex = 2;
            label2.Text = "Monto Inicial:";
            // 
            // txtMontoInicial
            // 
            txtMontoInicial.Location = new Point(217, 138);
            txtMontoInicial.Name = "txtMontoInicial";
            txtMontoInicial.Size = new Size(125, 27);
            txtMontoInicial.TabIndex = 3;
            // 
            // FormAperturaCaja
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(471, 450);
            Controls.Add(txtMontoInicial);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnAbrir);
            Name = "FormAperturaCaja";
            Text = "FormAperturaCaja";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnAbrir;
        private Label label1;
        private Label label2;
        private TextBox txtMontoInicial;
    }
}