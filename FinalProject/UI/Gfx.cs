using System.Drawing.Drawing2D;

namespace FinalProject.UI;

public static class Gfx
{
    public const TextFormatFlags Flags = TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine;

    public static GraphicsPath RoundedRect(Rectangle r, int radius)
    {
        int d = radius * 2;
        var p = new GraphicsPath();
        p.AddArc(r.X, r.Y, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }

    // Small rounded "pill" badge, e.g. [ Occupied ]
    public static void DrawPill(Graphics g, string text, Color color, Point at)
    {
        Size s = TextRenderer.MeasureText(text, Theme.BodyBold, Size.Empty, Flags);
        var r = new Rectangle(at.X, at.Y, s.Width + 18, s.Height + 6);
        using (GraphicsPath path = RoundedRect(r, r.Height / 2))
        using (var b = new SolidBrush(Color.FromArgb(38, color)))
            g.FillPath(b, path);
        TextRenderer.DrawText(g, text, Theme.BodyBold, r, color,
            Flags | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    // "5 min" or "1h 20m"
    public static string Ago(DateTime? time)
    {
        if (time == null) return "";
        int mins = Math.Max(0, (int)(DateTime.Now - time.Value).TotalMinutes);
        return mins < 60 ? $"{mins} min" : $"{mins / 60}h {mins % 60}m";
    }
}