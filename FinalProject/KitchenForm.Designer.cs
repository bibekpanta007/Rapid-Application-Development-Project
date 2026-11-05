namespace FinalProject
{
    partial class KitchenForm
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
            components = new System.ComponentModel.Container();
            headerBar = new FinalProject.UI.HeaderBar();
            pnlTop = new Panel();
            flpLegend = new FlowLayoutPanel();
            lblSummary = new Label();
            lblHeading = new Label();
            flpOrders = new FlowLayoutPanel();
            refreshTimer = new System.Windows.Forms.Timer(components);
            pnlTop.SuspendLayout();
            SuspendLayout();
            //
            // headerBar
            //
            headerBar.BackColor = Color.FromArgb(31, 36, 58);
            headerBar.Dock = DockStyle.Top;
            headerBar.Location = new Point(0, 0);
            headerBar.Name = "headerBar";
            headerBar.Size = new Size(1184, 64);
            headerBar.TabIndex = 0;
            //
            // pnlTop
            //
            pnlTop.Controls.Add(flpLegend);
            pnlTop.Controls.Add(lblSummary);
            pnlTop.Controls.Add(lblHeading);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 64);
            pnlTop.Name = "pnlTop";
            pnlTop.Size = new Size(1184, 84);
            pnlTop.TabIndex = 1;
            //
            // lblHeading
            //
            lblHeading.AutoSize = true;
            lblHeading.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold);
            lblHeading.Location = new Point(24, 12);
            lblHeading.Name = "lblHeading";
            lblHeading.TabIndex = 0;
            lblHeading.Text = "Live orders";
            //
            // lblSummary
            //
            lblSummary.AutoSize = true;
            lblSummary.Font = new Font("Segoe UI", 10F);
            lblSummary.ForeColor = Color.Gray;
            lblSummary.Location = new Point(27, 50);
            lblSummary.Name = "lblSummary";
            lblSummary.TabIndex = 1;
            lblSummary.Text = "Loading orders...";
            //
            // flpLegend
            //
            flpLegend.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            flpLegend.Location = new Point(684, 30);
            flpLegend.Name = "flpLegend";
            flpLegend.Size = new Size(480, 30);
            flpLegend.TabIndex = 2;
            flpLegend.WrapContents = false;
            //
            // flpOrders
            //
            flpOrders.AutoScroll = true;
            flpOrders.Dock = DockStyle.Fill;
            flpOrders.Location = new Point(0, 148);
            flpOrders.Name = "flpOrders";
            flpOrders.Padding = new Padding(14, 4, 14, 14);
            flpOrders.Size = new Size(1184, 613);
            flpOrders.TabIndex = 2;
            //
            // refreshTimer
            //
            refreshTimer.Interval = 3000;
            refreshTimer.Tick += refreshTimer_Tick;
            //
            // KitchenForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 247, 242);
            ClientSize = new Size(1184, 761);
            Controls.Add(flpOrders);
            Controls.Add(pnlTop);
            Controls.Add(headerBar);
            MinimumSize = new Size(1000, 700);
            Name = "KitchenForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Himalayan Grill - Kitchen";
            WindowState = FormWindowState.Maximized;
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private FinalProject.UI.HeaderBar headerBar;
        private Panel pnlTop;
        private Label lblHeading;
        private Label lblSummary;
        private FlowLayoutPanel flpLegend;
        private FlowLayoutPanel flpOrders;
        private System.Windows.Forms.Timer refreshTimer;
    }
}