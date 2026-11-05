using System.ComponentModel;
using System.Drawing.Drawing2D;
using FinalProject.Data;

namespace FinalProject.UI;

public class OrderCard : Control
{
    private const int CardWidth = 300;
    private const int HeaderHeight = 64;
    private const int LineHeight = 26;
    private const int Pad = 14;
    private const int ButtonHeight = 44;

    private static readonly Font QtyFont = new("Segoe UI", 11f, FontStyle.Bold);
    private static readonly Font ItemFont = new("Segoe UI", 11f);
    private static readonly Font NoteFont = new("Segoe UI", 9f, FontStyle.Italic);

    private readonly Button _action = new();
    private KitchenOrder _order = new();

    public event EventHandler? ActionClicked;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public KitchenOrder Order { get => _order; set { _order = value; ApplyOrder(); } }

    public OrderCard()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        Width = CardWidth;
        Margin = new Padding(10);

        _action.Click += (s, e) => ActionClicked?.Invoke(this, EventArgs.Empty);
        Controls.Add(_action);
        ApplyOrder();
    }

    // Sets the button text/color and the card height for the current order
    private void ApplyOrder()
    {
        switch (_order.Status)
        {
            case "NotStarted":
                _action.Text = "Start preparing";
                Theme.StyleButton(_action, ButtonKind.Primary);
                _action.Visible = true;
                break;
            case "Preparing":
                _action.Text = "Mark ready";
                Theme.StyleButton(_action, ButtonKind.Success);
                _action.Visible = true;
                break;
            default:
                _action.Visible = false;   // Ready: waiting for the server
                break;
        }

        Height = CalcHeight();
        _action.SetBounds(Pad, Height - ButtonHeight - Pad, Width - Pad * 2, ButtonHeight);
        Invalidate();
    }

    private int ItemsTop => HeaderHeight + 12;

    private int NotesHeight() =>
        TextRenderer.MeasureText("Note: " + _order.Notes, NoteFont,
            new Size(Width - Pad * 2 - 16, int.MaxValue),
            TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix).Height + 12;

    private int CalcHeight()
    {
        int h = ItemsTop;
        foreach (OrderLine l in _order.Lines)
            h += LineHeight + (string.IsNullOrWhiteSpace(l.Notes) ? 0 : 18);
        if (!string.IsNullOrWhiteSpace(_order.Notes))
            h += NotesHeight() + 12;
        h += 10 + ButtonHeight + Pad;
        return h;
    }

    private static string FirstName(string fullName) =>
        fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? fullName;

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent?.BackColor ?? Theme.Cream);

        Color accent = _order.IsLate ? Theme.StatusLate : Theme.ForOrderStatus(_order.Status);
        var card = new Rectangle(0, 0, Width - 1, Height - 1);
        using GraphicsPath path = Gfx.RoundedRect(card, 12);

        using (var fill = new SolidBrush(Theme.Surface))
            g.FillPath(fill, path);

        // Colored header band
        g.SetClip(path);
        using (var b = new SolidBrush(accent))
            g.FillRectangle(b, 0, 0, Width, HeaderHeight);
        g.ResetClip();

        // Left: table + order number + server
        TextRenderer.DrawText(g, $"Table {_order.TableNumber}", Theme.Heading,
            new Point(Pad, 8), Color.White, Gfx.Flags);
        TextRenderer.DrawText(g, $"#{_order.OrderID}  ·  {FirstName(_order.ServerName)}", Theme.Small,
            new Point(Pad + 2, 40), Color.White, Gfx.Flags);

        // Right: time + status label
        string time = _order.Status == "Ready" ? "Ready " + Gfx.Ago(_order.ReadyAt) : Gfx.Ago(_order.OrderDate);
        string label = _order.IsLate ? "LATE" : _order.Status switch
        {
            "NotStarted" => "NEW",
            "Preparing" => "PREPARING",
            _ => "READY"
        };
        TextRenderer.DrawText(g, time, Theme.SubHeading, new Rectangle(0, 10, Width - Pad, 26),
            Color.White, Gfx.Flags | TextFormatFlags.Right);
        TextRenderer.DrawText(g, label, Theme.BodyBold, new Rectangle(0, 38, Width - Pad, 20),
            Color.White, Gfx.Flags | TextFormatFlags.Right);

        // Items
        int y = ItemsTop;
        foreach (OrderLine l in _order.Lines)
        {
            TextRenderer.DrawText(g, $"{l.Quantity}×", QtyFont, new Point(Pad, y), accent, Gfx.Flags);
            TextRenderer.DrawText(g, l.Name, ItemFont,
                new Rectangle(Pad + 38, y, Width - Pad * 2 - 38, LineHeight),
                Theme.TextDark, Gfx.Flags | TextFormatFlags.EndEllipsis);
            y += LineHeight;

            if (!string.IsNullOrWhiteSpace(l.Notes))
            {
                TextRenderer.DrawText(g, "→ " + l.Notes, NoteFont, new Point(Pad + 38, y - 4),
                    Theme.StatusLate, Gfx.Flags);
                y += 18;
            }
        }

        // Order note from the server (yellow box)
        if (!string.IsNullOrWhiteSpace(_order.Notes))
        {
            y += 4;
            int nh = NotesHeight();
            var noteRect = new Rectangle(Pad, y, Width - Pad * 2, nh);
            using (var nb = new SolidBrush(Color.FromArgb(255, 244, 214)))
                g.FillRectangle(nb, noteRect);
            TextRenderer.DrawText(g, "Note: " + _order.Notes, NoteFont, Rectangle.Inflate(noteRect, -8, -6),
                Theme.TextDark, TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix);
        }

        // Ready orders have no button, just a message
        if (_order.Status == "Ready")
        {
            TextRenderer.DrawText(g, "Waiting for server to pick up", Theme.BodyBold,
                new Rectangle(Pad, Height - ButtonHeight - Pad, Width - Pad * 2, ButtonHeight),
                Theme.StatusReady, Gfx.Flags | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        using (var border = new Pen(_order.IsLate ? accent : Theme.Border, _order.IsLate ? 2f : 1f))
            g.DrawPath(border, path);
    }
}