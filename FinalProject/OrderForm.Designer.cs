namespace FinalProject
{
    partial class OrderForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            headerBar = new FinalProject.UI.HeaderBar();
            pnlCart = new Panel();
            lblCartTitle = new Label();
            dgvCart = new DataGridView();
            lblNotes = new Label();
            txtNotes = new TextBox();
            lblSubtotalCaption = new Label();
            lblSubtotal = new Label();
            lblTaxCaption = new Label();
            lblTax = new Label();
            pnlLine = new Panel();
            lblTotalCaption = new Label();
            lblTotal = new Label();
            btnSubmit = new Button();
            btnCancel = new Button();
            flpCategories = new FlowLayoutPanel();
            flpMenu = new FlowLayoutPanel();
            pnlCart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCart).BeginInit();
            SuspendLayout();
            //
            // headerBar
            //
            headerBar.BackColor = Color.FromArgb(31, 36, 58);
            headerBar.Dock = DockStyle.Top;
            headerBar.Location = new Point(0, 0);
            headerBar.Name = "headerBar";
            headerBar.Size = new Size(1280, 64);
            headerBar.TabIndex = 0;
            //
            // pnlCart
            //
            pnlCart.BackColor = Color.White;
            pnlCart.Controls.Add(lblCartTitle);
            pnlCart.Controls.Add(dgvCart);
            pnlCart.Controls.Add(lblNotes);
            pnlCart.Controls.Add(txtNotes);
            pnlCart.Controls.Add(lblSubtotalCaption);
            pnlCart.Controls.Add(lblSubtotal);
            pnlCart.Controls.Add(lblTaxCaption);
            pnlCart.Controls.Add(lblTax);
            pnlCart.Controls.Add(pnlLine);
            pnlCart.Controls.Add(lblTotalCaption);
            pnlCart.Controls.Add(lblTotal);
            pnlCart.Controls.Add(btnSubmit);
            pnlCart.Controls.Add(btnCancel);
            pnlCart.Dock = DockStyle.Right;
            pnlCart.Location = new Point(860, 64);
            pnlCart.Name = "pnlCart";
            pnlCart.Size = new Size(420, 696);
            pnlCart.TabIndex = 1;
            //
            // lblCartTitle
            //
            lblCartTitle.AutoSize = true;
            lblCartTitle.Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold);
            lblCartTitle.Location = new Point(16, 16);
            lblCartTitle.Name = "lblCartTitle";
            lblCartTitle.TabIndex = 0;
            lblCartTitle.Text = "Current order";
            //
            // dgvCart
            //
            dgvCart.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvCart.Location = new Point(16, 52);
            dgvCart.Name = "dgvCart";
            dgvCart.Size = new Size(388, 330);
            dgvCart.TabIndex = 1;
            //
            // lblNotes
            //
            lblNotes.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblNotes.AutoSize = true;
            lblNotes.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblNotes.Location = new Point(16, 394);
            lblNotes.Name = "lblNotes";
            lblNotes.TabIndex = 2;
            lblNotes.Text = "Notes for the kitchen (optional)";
            //
            // txtNotes
            //
            txtNotes.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtNotes.Location = new Point(16, 416);
            txtNotes.Multiline = true;
            txtNotes.Name = "txtNotes";
            txtNotes.Size = new Size(388, 54);
            txtNotes.TabIndex = 3;
            //
            // lblSubtotalCaption
            //
            lblSubtotalCaption.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblSubtotalCaption.AutoSize = true;
            lblSubtotalCaption.Location = new Point(16, 486);
            lblSubtotalCaption.Name = "lblSubtotalCaption";
            lblSubtotalCaption.TabIndex = 4;
            lblSubtotalCaption.Text = "Subtotal";
            //
            // lblSubtotal
            //
            lblSubtotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblSubtotal.Location = new Point(254, 484);
            lblSubtotal.Name = "lblSubtotal";
            lblSubtotal.Size = new Size(150, 22);
            lblSubtotal.TabIndex = 5;
            lblSubtotal.Text = "$0.00";
            lblSubtotal.TextAlign = ContentAlignment.MiddleRight;
            //
            // lblTaxCaption
            //
            lblTaxCaption.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblTaxCaption.AutoSize = true;
            lblTaxCaption.Location = new Point(16, 512);
            lblTaxCaption.Name = "lblTaxCaption";
            lblTaxCaption.TabIndex = 6;
            lblTaxCaption.Text = "GST";
            //
            // lblTax
            //
            lblTax.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblTax.Location = new Point(254, 510);
            lblTax.Name = "lblTax";
            lblTax.Size = new Size(150, 22);
            lblTax.TabIndex = 7;
            lblTax.Text = "$0.00";
            lblTax.TextAlign = ContentAlignment.MiddleRight;
            //
            // pnlLine
            //
            pnlLine.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlLine.BackColor = Color.FromArgb(226, 221, 213);
            pnlLine.Location = new Point(16, 538);
            pnlLine.Name = "pnlLine";
            pnlLine.Size = new Size(388, 1);
            pnlLine.TabIndex = 8;
            //
            // lblTotalCaption
            //
            lblTotalCaption.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblTotalCaption.AutoSize = true;
            lblTotalCaption.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
            lblTotalCaption.Location = new Point(16, 546);
            lblTotalCaption.Name = "lblTotalCaption";
            lblTotalCaption.TabIndex = 9;
            lblTotalCaption.Text = "Total";
            //
            // lblTotal
            //
            lblTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblTotal.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold);
            lblTotal.ForeColor = Color.FromArgb(228, 87, 46);
            lblTotal.Location = new Point(204, 542);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(200, 34);
            lblTotal.TabIndex = 10;
            lblTotal.Text = "$0.00";
            lblTotal.TextAlign = ContentAlignment.MiddleRight;
            //
            // btnSubmit
            //
            btnSubmit.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnSubmit.Location = new Point(16, 588);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new Size(388, 48);
            btnSubmit.TabIndex = 11;
            btnSubmit.Text = "Send to kitchen";
            btnSubmit.UseVisualStyleBackColor = true;
            btnSubmit.Click += btnSubmit_Click;
            //
            // btnCancel
            //
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnCancel.Location = new Point(16, 644);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(388, 38);
            btnCancel.TabIndex = 12;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            //
            // flpCategories
            //
            flpCategories.AutoSize = true;
            flpCategories.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flpCategories.Dock = DockStyle.Top;
            flpCategories.Location = new Point(0, 64);
            flpCategories.Name = "flpCategories";
            flpCategories.Padding = new Padding(16, 14, 16, 8);
            flpCategories.Size = new Size(860, 22);
            flpCategories.TabIndex = 2;
            //
            // flpMenu
            //
            flpMenu.AutoScroll = true;
            flpMenu.Dock = DockStyle.Fill;
            flpMenu.Location = new Point(0, 86);
            flpMenu.Name = "flpMenu";
            flpMenu.Padding = new Padding(12, 4, 12, 12);
            flpMenu.Size = new Size(860, 674);
            flpMenu.TabIndex = 3;
            //
            // OrderForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 247, 242);
            ClientSize = new Size(1280, 760);
            Controls.Add(flpMenu);
            Controls.Add(flpCategories);
            Controls.Add(pnlCart);
            Controls.Add(headerBar);
            MinimumSize = new Size(1100, 720);
            Name = "OrderForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Himalayan Grill - New Order";
            WindowState = FormWindowState.Maximized;
            pnlCart.ResumeLayout(false);
            pnlCart.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCart).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FinalProject.UI.HeaderBar headerBar;
        private Panel pnlCart;
        private Label lblCartTitle;
        private DataGridView dgvCart;
        private Label lblNotes;
        private TextBox txtNotes;
        private Label lblSubtotalCaption;
        private Label lblSubtotal;
        private Label lblTaxCaption;
        private Label lblTax;
        private Panel pnlLine;
        private Label lblTotalCaption;
        private Label lblTotal;
        private Button btnSubmit;
        private Button btnCancel;
        private FlowLayoutPanel flpCategories;
        private FlowLayoutPanel flpMenu;
    }
}