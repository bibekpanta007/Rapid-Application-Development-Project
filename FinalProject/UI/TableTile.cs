using System.ComponentModel;
using System.Drawing.Drawing2D;
using FinalProject.Data;

namespace FinalProject.UI;

public class TableTile : Control
{
    private DiningTable _table = new();
    private bool _hover;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DiningTable Table { get => _table; set { _table = value; Invalidate(); } }

    public TableTile()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        Size = new Size(210, 150);
        Margin = new Padding(10);
        Cursor = Cursors.Hand;
    }

    protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent?.BackColor ?? Theme.Cream);

        Color accent = Theme.ForTableStatus(_table.Status);
        var card = new Rectangle(0, 0, Width - 1, Height - 1);
        using GraphicsPath path = Gfx.RoundedRect(card, 12);

        // Card background
        using (var fill = new SolidBrush(_hover ? Color.FromArgb(255, 251, 245) : Theme.Surface))
            g.FillPath(fill, path);

        // Colored strip on top + order banner on the bottom (clipped to the rounded card)
        g.SetClip(path);
        using (var strip = new SolidBrush(accent))
            g.FillRectangle(strip, 0, 0, Width, 8);

        if (_table.ActiveOrderID != null)
        {
            string status = _table.ActiveOrderStatus ?? "";
            string banner = status switch
            {
                "Ready" => "READY TO SERVE",
                "Preparing" => "Kitchen: preparing",
                _ => "Kitchen: not started"
            };
            var bannerRect = new Rectangle(0, Height - 30, Width, 30);
            using (var b = new SolidBrush(Theme.ForOrderStatus(status)))
                g.FillRectangle(b, bannerRect);
            TextRenderer.DrawText(g, banner, Theme.BodyBold, bannerRect, Color.White,
                Gfx.Flags | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
        g.ResetClip();

        // Border (highlighted on hover)
        using (var border = new Pen(_hover ? accent : Theme.Border, _hover ? 2f : 1f))
            g.DrawPath(border, path);

        // Table number and seats
        TextRenderer.DrawText(g, $"Table {_table.TableNumber}", Theme.Heading,
            new Point(12, 16), Theme.TextDark, Gfx.Flags);
        string seats = $"{_table.Seats} seats";
        Size seatsSize = TextRenderer.MeasureText(seats, Theme.Small, Size.Empty, Gfx.Flags);
        TextRenderer.DrawText(g, seats, Theme.Small,
            new Point(Width - seatsSize.Width - 12, 26), Theme.TextMuted, Gfx.Flags);

        // Table status pill
        Gfx.DrawPill(g, _table.Status, accent, new Point(14, 56));

        // Order number and how long ago
        if (_table.ActiveOrderID != null)
        {
            string info = $"Order #{_table.ActiveOrderID}  ·  {Gfx.Ago(_table.ActiveOrderTime)}";
            TextRenderer.DrawText(g, info, Theme.Small, new Point(14, 92), Theme.TextMuted, Gfx.Flags);
        }
        else if (_table.Status == "Occupied")
        {
            // Food served, guests still eating
            TextRenderer.DrawText(g, "Served · guests dining", Theme.Small,
                new Point(14, 92), Theme.TextMuted, Gfx.Flags);
        }
    }
}