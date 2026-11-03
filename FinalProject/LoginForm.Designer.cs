namespace FinalProject
{
    partial class LoginForm
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
            pnlBrand = new Panel();
            lblTagline = new Label();
            lblBrandName = new Label();
            picLogo = new PictureBox();
            lblWelcome = new Label();
            lblSubtitle = new Label();
            lblUsername = new Label();
            txtUsername = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            btnLogin = new Button();
            lblError = new Label();
            lblHint = new Label();
            pnlBrand.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            SuspendLayout();
            //
            // pnlBrand
            //
            pnlBrand.BackColor = Color.FromArgb(31, 36, 58);
            pnlBrand.Controls.Add(lblTagline);
            pnlBrand.Controls.Add(lblBrandName);
            pnlBrand.Controls.Add(picLogo);
            pnlBrand.Dock = DockStyle.Left;
            pnlBrand.Location = new Point(0, 0);
            pnlBrand.Name = "pnlBrand";
            pnlBrand.Size = new Size(380, 521);
            pnlBrand.TabIndex = 0;
            //
            // picLogo
            //
            picLogo.Location = new Point(100, 80);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(180, 180);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 0;
            picLogo.TabStop = false;
            //
            // lblBrandName
            //
            lblBrandName.Font = new Font("Georgia", 22F, FontStyle.Bold);
            lblBrandName.ForeColor = Color.White;
            lblBrandName.Location = new Point(0, 280);
            lblBrandName.Name = "lblBrandName";
            lblBrandName.Size = new Size(380, 50);
            lblBrandName.TabIndex = 1;
            lblBrandName.Text = "Himalayan Grill";
            lblBrandName.TextAlign = ContentAlignment.MiddleCenter;
            //
            // lblTagline
            //
            lblTagline.Font = new Font("Segoe UI", 11F);
            lblTagline.ForeColor = Color.FromArgb(240, 140, 40);
            lblTagline.Location = new Point(0, 330);
            lblTagline.Name = "lblTagline";
            lblTagline.Size = new Size(380, 30);
            lblTagline.TabIndex = 2;
            lblTagline.Text = "Order & Kitchen System";
            lblTagline.TextAlign = ContentAlignment.MiddleCenter;
            lblTagline.UseMnemonic = false;
            //
            // lblWelcome
            //
            lblWelcome.AutoSize = true;
            lblWelcome.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold);
            lblWelcome.Location = new Point(450, 100);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.TabIndex = 10;
            lblWelcome.Text = "Welcome back";
            //
            // lblSubtitle
            //
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 10F);
            lblSubtitle.ForeColor = Color.Gray;
            lblSubtitle.Location = new Point(453, 148);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.TabIndex = 11;
            lblSubtitle.Text = "Sign in to continue";
            //
            // lblUsername
            //
            lblUsername.AutoSize = true;
            lblUsername.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblUsername.Location = new Point(453, 200);
            lblUsername.Name = "lblUsername";
            lblUsername.TabIndex = 12;
            lblUsername.Text = "Username";
            //
            // txtUsername
            //
            txtUsername.Font = new Font("Segoe UI", 12F);
            txtUsername.Location = new Point(453, 225);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(380, 34);
            txtUsername.TabIndex = 1;
            //
            // lblPassword
            //
            lblPassword.AutoSize = true;
            lblPassword.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblPassword.Location = new Point(453, 280);
            lblPassword.Name = "lblPassword";
            lblPassword.TabIndex = 13;
            lblPassword.Text = "Password";
            //
            // txtPassword
            //
            txtPassword.Font = new Font("Segoe UI", 12F);
            txtPassword.Location = new Point(453, 305);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(380, 34);
            txtPassword.TabIndex = 2;
            txtPassword.UseSystemPasswordChar = true;
            //
            // btnLogin
            //
            btnLogin.Location = new Point(453, 365);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(380, 46);
            btnLogin.TabIndex = 3;
            btnLogin.Text = "Log in";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            //
            // lblError
            //
            lblError.ForeColor = Color.FromArgb(214, 69, 69);
            lblError.Location = new Point(453, 420);
            lblError.Name = "lblError";
            lblError.Size = new Size(380, 24);
            lblError.TabIndex = 14;
            //
            // lblHint
            //
            lblHint.AutoSize = true;
            lblHint.Font = new Font("Segoe UI", 8.5F);
            lblHint.ForeColor = Color.Gray;
            lblHint.Location = new Point(453, 470);
            lblHint.Name = "lblHint";
            lblHint.TabIndex = 15;
            lblHint.Text = "Demo: manager / server1 / cook1  -  password 1234";
            //
            // LoginForm
            //
            AcceptButton = btnLogin;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 247, 242);
            ClientSize = new Size(884, 521);
            Controls.Add(lblHint);
            Controls.Add(lblError);
            Controls.Add(btnLogin);
            Controls.Add(txtPassword);
            Controls.Add(lblPassword);
            Controls.Add(txtUsername);
            Controls.Add(lblUsername);
            Controls.Add(lblSubtitle);
            Controls.Add(lblWelcome);
            Controls.Add(pnlBrand);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Himalayan Grill - Login";
            pnlBrand.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlBrand;
        private PictureBox picLogo;
        private Label lblBrandName;
        private Label lblTagline;
        private Label lblWelcome;
        private Label lblSubtitle;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblPassword;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblError;
        private Label lblHint;
    }
}