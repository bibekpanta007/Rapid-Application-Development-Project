using System.ComponentModel;
using System.Globalization;
using FinalProject.Data;
using FinalProject.UI;
using MenuItem = FinalProject.Data.MenuItem;

namespace FinalProject;

public partial class OrderForm : Form
{
    private readonly DiningTable _table;
    private readonly decimal _taxRate;
    private readonly string _taxLabel;
    private readonly List<CartLine> _cart = new();
    private readonly List<Button> _categoryButtons = new();
    private List<MenuItem> _allItems = new();

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int CreatedOrderId { get; private set; }

    public OrderForm(DiningTable table)
    {
        InitializeComponent();
        _table = table;

        if (Theme.AppIcon != null) Icon = Theme.AppIcon;
        Text = $"{Theme.RestaurantName} - Table {table.TableNumber}";

        headerBar.Title = $"New order  ·  Table {table.TableNumber}";
        headerBar.UserText = Session.CurrentUser!.FullName;
        headerBar.ShowLogout = false;

        lblCartTitle.Text = $"Table {table.TableNumber}  ·  {table.Seats} seats";
        Theme.StyleButton(btnSubmit, ButtonKind.Primary);
        Theme.StyleButton(btnCancel, ButtonKind.Secondary);

        // Tax rate comes from Settings, so the manager can change it
        try
        {
            _taxRate = SettingsRepo.TaxRate;
            _taxLabel = SettingsRepo.TaxLabel;
        }
        catch
        {
            _taxRate = 0.05m;
            _taxLabel = "GST";
        }
        lblTaxCaption.Text = $"{_taxLabel} ({_taxRate * 100:0.##}%)";

        SetupCartGrid();
        LoadMenu();
        RefreshCart();
    }

    // ---------------- Menu ----------------

    private void LoadMenu()
    {
        List<Category> categories;
        try
        {
            categories = MenuRepo.GetCategories();
            _allItems = MenuRepo.GetItems();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Could not load the menu:\n\n" + ex.Message, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        AddCategoryButton("All", null);
        foreach (Category c in categories)
            AddCategoryButton(c.Name, c.CategoryID);

        SelectCategory(null);
    }

    private void AddCategoryButton(string text, int? categoryId)
    {
        int width = Math.Max(80, TextRenderer.MeasureText(text, Theme.BodyBold).Width + 30);
        var b = new Button
        {
            Text = text,
            Size = new Size(width, 38),
            Margin = new Padding(4, 0, 4, 6),
            Tag = categoryId
        };
        b.Click += (s, e) => SelectCategory(categoryId);
        _categoryButtons.Add(b);
        flpCategories.Controls.Add(b);
    }

    private void SelectCategory(int? categoryId)
    {
        // Highlight the selected tab
        foreach (Button b in _categoryButtons)
            Theme.StyleButton(b, (int?)b.Tag == categoryId ? ButtonKind.Primary : ButtonKind.Secondary);

        flpMenu.SuspendLayout();
        var old = flpMenu.Controls.Cast<Control>().ToList();
        flpMenu.Controls.Clear();
        foreach (Control c in old) c.Dispose();

        foreach (MenuItem item in _allItems.Where(i => categoryId == null || i.CategoryID == categoryId))
        {
            var card = new MenuItemCard { Item = item };
            card.Click += MenuCard_Click;
            flpMenu.Controls.Add(card);
        }
        flpMenu.ResumeLayout();
    }

    private void MenuCard_Click(object? sender, EventArgs e)
    {
        if (sender is not MenuItemCard card) return;
        MenuItem item = card.Item;

        if (!item.IsAvailable)
        {
            MessageBox.Show($"{item.Name} is sold out today.", Theme.RestaurantName,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        CartLine? line = _cart.FirstOrDefault(l => l.Item.ItemID == item.ItemID);
        if (line == null)
            _cart.Add(new CartLine { Item = item, Quantity = 1 });
        else
            line.Quantity++;

        RefreshCart();
    }

    // ---------------- Cart ----------------

    private void SetupCartGrid()
    {
        dgvCart.AutoGenerateColumns = false;
        dgvCart.AllowUserToAddRows = false;
        dgvCart.AllowUserToDeleteRows = false;
        dgvCart.AllowUserToResizeRows = false;
        dgvCart.AllowUserToResizeColumns = false;
        dgvCart.ReadOnly = true;
        dgvCart.RowHeadersVisible = false;
        dgvCart.MultiSelect = false;
        dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvCart.BackgroundColor = Theme.Surface;
        dgvCart.BorderStyle = BorderStyle.None;
        dgvCart.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgvCart.GridColor = Theme.Border;
        dgvCart.EnableHeadersVisualStyles = false;
        dgvCart.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        dgvCart.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgvCart.ColumnHeadersHeight = 34;
        dgvCart.ColumnHeadersDefaultCellStyle.BackColor = Theme.Cream;
        dgvCart.ColumnHeadersDefaultCellStyle.Font = Theme.BodyBold;
        dgvCart.DefaultCellStyle.Font = Theme.Body;
        dgvCart.DefaultCellStyle.SelectionBackColor = Color.FromArgb(253, 236, 228);
        dgvCart.DefaultCellStyle.SelectionForeColor = Theme.TextDark;
        dgvCart.RowTemplate.Height = 38;

        dgvCart.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colItem",
            HeaderText = "Item",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });
        dgvCart.Columns.Add(MakeButtonColumn("colMinus", "−"));
        dgvCart.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colQty",
            HeaderText = "Qty",
            Width = 40,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
        });
        dgvCart.Columns.Add(MakeButtonColumn("colPlus", "+"));
        dgvCart.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colTotal",
            HeaderText = "Total",
            Width = 76,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight }
        });
        dgvCart.Columns.Add(MakeButtonColumn("colRemove", "✕"));

        dgvCart.CellContentClick += dgvCart_CellContentClick;
    }

    private static DataGridViewButtonColumn MakeButtonColumn(string name, string text) => new()
    {
        Name = name,
        HeaderText = "",
        Text = text,
        UseColumnTextForButtonValue = true,
        Width = 34,
        FlatStyle = FlatStyle.Flat
    };

    private void dgvCart_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _cart.Count) return;

        CartLine line = _cart[e.RowIndex];
        switch (dgvCart.Columns[e.ColumnIndex].Name)
        {
            case "colPlus":
                line.Quantity++;
                break;
            case "colMinus":
                line.Quantity--;
                if (line.Quantity <= 0) _cart.Remove(line);
                break;
            case "colRemove":
                _cart.Remove(line);
                break;
            default:
                return;
        }

        // Refresh after the click event finishes (the grid can't be rebuilt during its own event)
        BeginInvoke(new Action(RefreshCart));
    }

    private void RefreshCart()
    {
        dgvCart.Rows.Clear();
        foreach (CartLine l in _cart)
            dgvCart.Rows.Add(l.Item.Name, null, l.Quantity, null, Money(l.LineTotal), null);

        decimal subtotal = _cart.Sum(l => l.LineTotal);
        decimal tax = OrderRepo.CalcTax(subtotal, _taxRate);

        lblSubtotal.Text = Money(subtotal);
        lblTax.Text = Money(tax);
        lblTotal.Text = Money(subtotal + tax);

        int count = _cart.Sum(l => l.Quantity);
        btnSubmit.Text = count == 0 ? "Add items to start" : $"Send to kitchen  ({count} item{(count == 1 ? "" : "s")})";
        btnSubmit.Enabled = count > 0;
    }

    private static string Money(decimal value) => "$" + value.ToString("0.00", CultureInfo.InvariantCulture);

    // ---------------- Buttons ----------------

    private void btnSubmit_Click(object sender, EventArgs e)
    {
        if (_cart.Count == 0) return;

        try
        {
            CreatedOrderId = OrderRepo.CreateOrder(_table.TableID, Session.CurrentUser!.UserID,
                                                   _cart, txtNotes.Text, _taxRate);
            MessageBox.Show($"Order #{CreatedOrderId} for Table {_table.TableNumber} was sent to the kitchen.",
                Theme.RestaurantName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Could not save the order:\n\n" + ex.Message, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        if (_cart.Count > 0 &&
            MessageBox.Show("Discard this order?", Theme.RestaurantName,
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            return;

        DialogResult = DialogResult.Cancel;
    }
}