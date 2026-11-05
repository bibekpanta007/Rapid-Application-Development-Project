using FinalProject.Data;
using FinalProject.UI;

namespace FinalProject;

public partial class ServerForm : Form
{
    // One tile per table, so a refresh just updates them instead of rebuilding
    private readonly Dictionary<int, TableTile> _tiles = new();

    public ServerForm()
    {
        InitializeComponent();
        if (Theme.AppIcon != null) Icon = Theme.AppIcon;
        Text = $"{Theme.RestaurantName} - Server";

        User user = Session.CurrentUser!;
        headerBar.Title = "Tables";
        headerBar.UserText = $"{user.FullName}  •  {user.Role}";
        headerBar.LogoutClicked += (s, e) =>
        {
            DialogResult = DialogResult.Retry;   // back to Login
            Close();
        };

        BuildLegend();
        LoadTables();
        refreshTimer.Start();   // live updates every 5 seconds
    }

    private void BuildLegend()
    {
        var items = new (string Text, Color Color)[]
        {
            ("Available", Theme.TableAvailable),
            ("Occupied",  Theme.TableOccupied),
            ("Reserved",  Theme.TableReserved)
        };

        foreach (var item in items)
        {
            flpLegend.Controls.Add(new Panel
            {
                Size = new Size(14, 14),
                BackColor = item.Color,
                Margin = new Padding(14, 7, 6, 0)
            });
            flpLegend.Controls.Add(new Label
            {
                Text = item.Text,
                AutoSize = true,
                ForeColor = Theme.TextMuted,
                Margin = new Padding(0, 5, 4, 0)
            });
        }
    }

    private void LoadTables()
    {
        List<DiningTable> tables;
        try
        {
            tables = TableRepo.GetAllWithActiveOrders();
        }
        catch (Exception ex)
        {
            refreshTimer.Stop();
            MessageBox.Show("Could not load tables:\n\n" + ex.Message, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        bool sameTables = tables.Count == _tiles.Count && tables.All(t => _tiles.ContainsKey(t.TableID));

        if (sameTables)
        {
            // Just update the data on the existing tiles (no flicker)
            foreach (DiningTable t in tables)
                _tiles[t.TableID].Table = t;
        }
        else
        {
            // Tables were added/removed by the manager: rebuild the grid
            flpTables.SuspendLayout();
            var old = flpTables.Controls.Cast<Control>().ToList();
            flpTables.Controls.Clear();
            foreach (Control c in old) c.Dispose();
            _tiles.Clear();

            foreach (DiningTable t in tables)
            {
                var tile = new TableTile { Table = t };
                tile.Click += Tile_Click;
                _tiles[t.TableID] = tile;
                flpTables.Controls.Add(tile);
            }
            flpTables.ResumeLayout();
        }

        int available = tables.Count(t => t.Status == "Available");
        int occupied = tables.Count(t => t.Status == "Occupied");
        int reserved = tables.Count(t => t.Status == "Reserved");
        int ready = tables.Count(t => t.ActiveOrderStatus == "Ready");

        lblSummary.Text = $"{available} available   ·   {occupied} occupied   ·   {reserved} reserved" +
                          (ready > 0 ? $"      |      {ready} order(s) ready to serve" : "");
    }

    private void Tile_Click(object? sender, EventArgs e)
    {
        if (sender is not TableTile tile) return;
        DiningTable t = tile.Table;

        refreshTimer.Stop();
        try
        {
            if (t.ActiveOrderID == null && t.Status == "Occupied")
            {
                // Food already served, guests still at the table
                var btnNewOrder = new TaskDialogButton("New order");
                var btnClear = new TaskDialogButton("Clear table");
                var page = new TaskDialogPage
                {
                    Caption = Theme.RestaurantName,
                    Heading = $"Table {t.TableNumber}",
                    Text = "The guests have been served.\n\n" +
                           "Take another order (drinks, dessert), or clear the table once the guests have left?",
                    Icon = TaskDialogIcon.Information,
                    Buttons = { btnNewOrder, btnClear, TaskDialogButton.Cancel }
                };

                TaskDialogButton result = TaskDialog.ShowDialog(this, page);
                if (result == btnNewOrder)
                {
                    using var form = new OrderForm(t);
                    form.ShowDialog(this);
                }
                else if (result == btnClear)
                {
                    TableRepo.SetStatus(t.TableID, "Available");
                }
            }
            else if (t.ActiveOrderID == null)
            {
                // Empty or reserved table: seat the guests and take their order
                using var form = new OrderForm(t);
                form.ShowDialog(this);
            }
            else if (t.ActiveOrderStatus == "Ready")
            {
                // Food is ready: serve it (table stays occupied while guests eat)
                var answer = MessageBox.Show(
                    $"Order #{t.ActiveOrderID} for Table {t.TableNumber} is ready.\n\n" +
                    "Mark it as served?",
                    "Serve order", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (answer == DialogResult.Yes)
                    OrderRepo.UpdateStatus(t.ActiveOrderID.Value, "Completed");
            }
            else
            {
                // Kitchen is still working: show what was ordered
                var lines = OrderRepo.GetLines(t.ActiveOrderID.Value);
                string items = string.Join("\n", lines.Select(l => $"   {l.Quantity} x {l.Name}"));
                string status = t.ActiveOrderStatus == "Preparing" ? "being prepared" : "waiting for the kitchen";

                MessageBox.Show($"Order #{t.ActiveOrderID} is {status} ({Gfx.Ago(t.ActiveOrderTime)}).\n\n{items}",
                    $"Table {t.TableNumber}", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Something went wrong:\n\n" + ex.Message, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            LoadTables();
            refreshTimer.Start();
        }
    }

    private void refreshTimer_Tick(object? sender, EventArgs e) => LoadTables();

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        refreshTimer.Stop();
        base.OnFormClosed(e);
    }
}