using System.Drawing.Drawing2D;
using FinalProject.Data;

namespace FinalProject.UI;

public enum ButtonKind { Primary, Secondary, Ghost, Success, Danger }

// One place for all colors, fonts and styling so every screen looks the same
public static class Theme
{
    public static string RestaurantName { get; private set; } = "Himalayan Grill";

    // ---- Brand colors ----
    public static readonly Color Charcoal = Color.FromArgb(31, 36, 58);    // logo navy
    public static readonly Color CharcoalLight = Color.FromArgb(52, 58, 84);
    public static readonly Color Ember = Color.FromArgb(228, 87, 46);
    public static readonly Color EmberDark = Color.FromArgb(194, 65, 12);
    public static readonly Color Saffron = Color.FromArgb(240, 140, 40);  // logo ring
    public static readonly Color Cream = Color.FromArgb(250, 247, 242);
    public static readonly Color Surface = Color.White;
    public static readonly Color Border = Color.FromArgb(226, 221, 213);
    public static readonly Color TextDark = Color.FromArgb(43, 43, 43);
    public static readonly Color TextMuted = Color.FromArgb(110, 110, 110);

    // ---- Order status colors ----
    public static readonly Color StatusNotStarted = Color.FromArgb(138, 143, 152);
    public static readonly Color StatusPreparing = Color.FromArgb(242, 169, 59);
    public static readonly Color StatusReady = Color.FromArgb(46, 158, 91);
    public static readonly Color StatusLate = Color.FromArgb(214, 69, 69);

    // ---- Table status colors ----
    public static readonly Color TableAvailable = Color.FromArgb(46, 158, 91);
    public static readonly Color TableOccupied = Color.FromArgb(228, 87, 46);
    public static readonly Color TableReserved = Color.FromArgb(59, 125, 216);

    // ---- Fonts ----
    public static readonly Font Body = new("Segoe UI", 10f);
    public static readonly Font BodyBold = new("Segoe UI Semibold", 10f);
    public static readonly Font Small = new("Segoe UI", 8.5f);
    public static readonly Font SubHeading = new("Segoe UI Semibold", 12f);
    public static readonly Font Heading = new("Segoe UI Semibold", 16f);
    public static readonly Font Brand = new("Georgia", 15f, FontStyle.Bold);

    // ---- Logo ----
    private static readonly string AssetsDir = Path.Combine(AppContext.BaseDirectory, "Assets");
    public static readonly Image? Logo =
        File.Exists(Path.Combine(AssetsDir, "logo.png")) ? Image.FromFile(Path.Combine(AssetsDir, "logo.png")) : null;
    public static readonly Icon? AppIcon =
        File.Exists(Path.Combine(AssetsDir, "logo.ico")) ? new Icon(Path.Combine(AssetsDir, "logo.ico")) : null;

    // Call once at startup, after the database is ready
    public static void LoadFromDb()
    {
        RestaurantName = SettingsRepo.Get("RestaurantName", RestaurantName);
    }

    // Apply the standard look to any form
    public static void ApplyForm(Form form)
    {
        form.BackColor = Cream;
        form.ForeColor = TextDark;
        form.Font = Body;
        form.StartPosition = FormStartPosition.CenterScreen;
        if (AppIcon != null) form.Icon = AppIcon;
    }

    // Flat, colored buttons
    public static void StyleButton(Button b, ButtonKind kind = ButtonKind.Primary)
    {
        (Color back, Color fore, Color hover) = kind switch
        {
            ButtonKind.Primary => (Ember, Color.White, EmberDark),
            ButtonKind.Secondary => (Surface, TextDark, Color.FromArgb(240, 236, 230)),
            ButtonKind.Ghost => (CharcoalLight, Color.White, Color.FromArgb(72, 80, 112)),
            ButtonKind.Success => (StatusReady, Color.White, Color.FromArgb(34, 128, 72)),
            ButtonKind.Danger => (StatusLate, Color.White, Color.FromArgb(176, 50, 50)),
            _ => (Ember, Color.White, EmberDark)
        };

        b.FlatStyle = FlatStyle.Flat;
        b.UseVisualStyleBackColor = false;
        b.BackColor = back;
        b.ForeColor = fore;
        b.Font = BodyBold;
        b.Cursor = Cursors.Hand;
        b.UseMnemonic = false;   // so "&" shows as "&"
        b.FlatAppearance.BorderSize = kind == ButtonKind.Secondary ? 1 : 0;
        b.FlatAppearance.BorderColor = Border;
        b.FlatAppearance.MouseOverBackColor = hover;
        b.FlatAppearance.MouseDownBackColor = hover;
    }

    // Draws the logo (falls back to an orange circle if the file is missing)
    public static void DrawLogo(Graphics g, Rectangle r)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        if (Logo != null)
        {
            g.DrawImage(Logo, r);
            return;
        }
        using var brush = new SolidBrush(Ember);
        g.FillEllipse(brush, r);
    }

    public static Color ForOrderStatus(string status) => status switch
    {
        "NotStarted" => StatusNotStarted,
        "Preparing" => StatusPreparing,
        "Ready" => StatusReady,
        _ => TextMuted
    };

    public static Color ForTableStatus(string status) => status switch
    {
        "Available" => TableAvailable,
        "Occupied" => TableOccupied,
        "Reserved" => TableReserved,
        _ => TextMuted
    };
}