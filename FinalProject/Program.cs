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

            // Each role gets its own screen (Kitchen and Manager come in Steps 7-8)
            using Form main = Session.CurrentUser!.Role switch
            {
                "Server" => new ServerForm(),
                _ => new Form1()
            };

            if (main.ShowDialog() != DialogResult.Retry) break;   // Retry = logged out
            Session.CurrentUser = null;
        }
    }
}