using FinalProject.Data;
using FinalProject.UI;

namespace FinalProject;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            Db.Initialize();
            Theme.LoadFromDb();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Database setup failed:\n\n" + ex.Message, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Login -> role screen -> Log out returns to Login. Closing a window exits.
        while (true)
        {
            using var login = new LoginForm();
            if (login.ShowDialog() != DialogResult.OK) break;

            Session.CurrentUser = login.LoggedInUser;

            // Placeholder until we build each role's screen
            using Form main = new Form1();
            if (main.ShowDialog() != DialogResult.Retry) break;   // Retry = logged out
            Session.CurrentUser = null;
        }
    }
}