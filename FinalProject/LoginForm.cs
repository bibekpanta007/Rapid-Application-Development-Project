using System.ComponentModel;
using FinalProject.Data;
using FinalProject.UI;

namespace FinalProject;

public partial class LoginForm : Form
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public User? LoggedInUser { get; private set; }

    public LoginForm()
    {
        InitializeComponent();

        // Branding that comes from code / database
        if (Theme.AppIcon != null) Icon = Theme.AppIcon;
        picLogo.Image = Theme.Logo;
        lblBrandName.Text = Theme.RestaurantName;
        Theme.StyleButton(btnLogin, ButtonKind.Primary);
    }

    private void btnLogin_Click(object sender, EventArgs e)
    {
        lblError.Text = "";

        if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
        {
            lblError.Text = "Please enter your username and password.";
            return;
        }

        try
        {
            User? user = UserRepo.Authenticate(txtUsername.Text, txtPassword.Text);
            if (user == null)
            {
                lblError.Text = "Incorrect username or password.";
                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            LoggedInUser = user;
            DialogResult = DialogResult.OK;   // closes the form and tells Program it worked
        }
        catch (Exception ex)
        {
            lblError.Text = "Database error: " + ex.Message;
        }
    }
}