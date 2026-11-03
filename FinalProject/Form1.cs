using FinalProject.Data;
using FinalProject.UI;

namespace FinalProject;

// Temporary placeholder screen - will be replaced by the Server, Kitchen
// and Manager screens in the next steps
public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        Theme.ApplyForm(this);
        Text = Theme.RestaurantName;
        ClientSize = new Size(1000, 500);

        User user = Session.CurrentUser!;

        var header = new HeaderBar
        {
            Title = user.Role,
            UserText = user.FullName
        };
        header.LogoutClicked += (s, e) =>
        {
            DialogResult = DialogResult.Retry;   // tells Program to show Login again
            Close();
        };

        var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(24) };

        flow.Controls.Add(new Label
        {
            Text = $"Logged in as {user.FullName} ({user.Role}). " +
                   "This screen will be replaced in the next steps.",
            AutoSize = true,
            Font = Theme.SubHeading,
            Margin = new Padding(8, 8, 8, 24)
        });
        flow.SetFlowBreak(flow.Controls[^1], true);

        foreach (ButtonKind kind in Enum.GetValues<ButtonKind>())
        {
            var b = new Button { Text = kind.ToString(), Size = new Size(140, 44), Margin = new Padding(8) };
            Theme.StyleButton(b, kind);
            flow.Controls.Add(b);
        }
        flow.SetFlowBreak(flow.Controls[^1], true);

        foreach (string status in new[] { "NotStarted", "Preparing", "Ready" })
        {
            flow.Controls.Add(new Label
            {
                Text = status,
                AutoSize = false,
                Size = new Size(140, 36),
                Margin = new Padding(8),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                BackColor = Theme.ForOrderStatus(status),
                Font = Theme.BodyBold
            });
        }

        Controls.Add(flow);
        Controls.Add(header);   // add the header last so it docks on top
    }
}