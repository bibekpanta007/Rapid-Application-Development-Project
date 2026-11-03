using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace FinalProject.UI;

// Dark bar with logo, restaurant name, screen title, user and Log out button.
// After building, it appears in the Toolbox and can be dragged onto any form.
public class HeaderBar : Panel
{
    private const TextFormatFlags Flags = TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;

    private readonly Button _logout = new();
    private string _title = "";
    private string _userText = "";

    public event EventHandler? LogoutClicked;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Title { get => _title; set { _title = value; Invalidate(); } }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string UserText { get => _userText; set { _userText = value; Invalidate(); } }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ShowLogout { get => _logout.Visible; set { _logout.Visible = value; Invalidate(); } }

    public HeaderBar()
    {
        Dock = DockStyle.Top;
        Height = 64;
        BackColor = Theme.Charcoal;
        DoubleBuffered = true;
        ResizeRedraw = true;

        _logout.Text = "Log out";
        _logout.Size = new Size(96, 36);
        Theme.StyleButton(_logout, ButtonKind.Ghost);
        _logout.Click += (s, e) => LogoutClicked?.Invoke(this, EventArgs.Empty);
        Controls.Add(_logout);
    }

    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        _logout.Location = new Point(Width - _logout.Width - 16, (Height - _logout.Height) / 2);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        // Logo + restaurant name
        Theme.DrawLogo(g, new Rectangle(12, 6, 52, 52));
        Size nameSize = TextRenderer.MeasureText(Theme.RestaurantName, Theme.Brand, Size.Empty, Flags);
        TextRenderer.DrawText(g, Theme.RestaurantName, Theme.Brand,
            new Point(72, (Height - nameSize.Height) / 2), Color.White, Flags);

        // Divider + screen title (e.g. "Kitchen")
        int x = 72 + nameSize.Width + 10;
        if (!string.IsNullOrEmpty(Title))
        {
            using var pen = new Pen(Color.FromArgb(90, 98, 130));
            g.DrawLine(pen, x, 20, x, Height - 20);
            x += 14;
            Size titleSize = TextRenderer.MeasureText(Title, Theme.SubHeading, Size.Empty, Flags);
            TextRenderer.DrawText(g, Title, Theme.SubHeading,
                new Point(x, (Height - titleSize.Height) / 2), Theme.Saffron, Flags);
        }

        // Logged-in user on the right
        if (!string.IsNullOrEmpty(UserText))
        {
            int right = _logout.Visible ? _logout.Left - 16 : Width - 16;
            Size userSize = TextRenderer.MeasureText(UserText, Theme.Body, Size.Empty, Flags);
            TextRenderer.DrawText(g, UserText, Theme.Body,
                new Point(right - userSize.Width, (Height - userSize.Height) / 2),
                Color.FromArgb(210, 214, 228), Flags);
        }
    }
}