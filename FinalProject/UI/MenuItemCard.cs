using System.ComponentModel;
using System.Drawing.Drawing2D;
using MenuItem = FinalProject.Data.MenuItem;

namespace FinalProject.UI;

public class MenuItemCard : Control
{
    // One color per category, used when an item has no photo yet
    private static readonly Color[] Palette =
    {
        Color.FromArgb(228, 87, 46), Color.FromArgb(240, 140, 40), Color.FromArgb(46, 158, 91),
        Color.FromArgb(59, 125, 216), Color.FromArgb(142, 68, 173), Color.FromArgb(22, 160, 133),
        Color.FromArgb(214, 69, 69), Color.FromArgb(52, 58, 84)
    };
    private static readonly Font InitialsFont = new("Segoe UI Semibold", 24f);
    private static readonly Dictionary<string, Image?> ImageCache = new();

    private MenuItem _item = new();
    private bool _hover;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public MenuItem Item { get => _item; set { _item = value; Invalidate(); } }

    public MenuItemCard()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        Size = new Size(180, 176);
        Margin = new Padding(8);
        Cursor = Cursors.Hand;
    }

    public static Color ColorFor(int categoryId) => Palette[Math.Abs(categoryId) % Palette.Length];

    // Loads Images/xxx.png if it exists (photos can be added later)
    private static Image? LoadImage(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return null;
        if (ImageCache.TryGetValue(relativePath, out Image? cached)) return cached;

        Image? img = null;
        try
        {
            string full = Path.Combine(AppContext.BaseDirectory, relativePath);
            if (File.Exists(full)) img = Image.FromFile(full);
        }
        catch { img = null; }

        ImageCache[relativePath] = img;
        return img;
    }

    private static string Initials(string name)
    {
        var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Where(w => char.IsLetter(w[0])).Take(2);
        return string.Concat(words.Select(w => char.ToUpper(w[0])));
    }

    protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.Clear(Parent?.BackColor ?? Theme.Cream);

        var card = new Rectangle(0, 0, Width - 1, Height - 1);
        using GraphicsPath path = Gfx.RoundedRect(card, 10);

        using (var fill = new SolidBrush(Theme.Surface))
            g.FillPath(fill, path);

        // Picture area (photo if available, otherwise a colored tile with initials)
        var pic = new Rectangle(0, 0, Width, 100);
        g.SetClip(path);
        g.SetClip(pic, CombineMode.Intersect);
        Image? img = LoadImage(_item.ImagePath);
        if (img != null)
        {
            float scale = Math.Max((float)pic.Width / img.Width, (float)pic.Height / img.Height);
            int w = (int)(img.Width * scale), h = (int)(img.Height * scale);
            g.DrawImage(img, pic.X + (pic.Width - w) / 2, pic.Y + (pic.Height - h) / 2, w, h);
        }
        else
        {
            using (var b = new SolidBrush(ColorFor(_item.CategoryID)))
                g.FillRectangle(b, pic);
            TextRenderer.DrawText(g, Initials(_item.Name), InitialsFont, pic, Color.White,
                Gfx.Flags | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
        g.ResetClip();

        // Name (up to 2 lines) and price
        var nameRect = new Rectangle(10, 106, Width - 20, 40);
        TextRenderer.DrawText(g, _item.Name, Theme.BodyBold, nameRect, Theme.TextDark,
            TextFormatFlags.NoPrefix | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
        TextRenderer.DrawText(g, $"${_item.Price:0.00}", Theme.SubHeading,
            new Point(10, Height - 32), Theme.Ember, Gfx.Flags);

        // Sold out: fade the card
        if (!_item.IsAvailable)
        {
            using (var shade = new SolidBrush(Color.FromArgb(150, 255, 255, 255)))
                g.FillPath(shade, path);
            Gfx.DrawPill(g, "Sold out", Theme.StatusLate, new Point(Width - 92, Height - 34));
        }

        using (var border = new Pen(_hover && _item.IsAvailable ? Theme.Ember : Theme.Border,
                                    _hover && _item.IsAvailable ? 2f : 1f))
            g.DrawPath(border, path);
    }
}