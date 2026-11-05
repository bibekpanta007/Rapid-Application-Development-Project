using FinalProject.Data;
using FinalProject.UI;

namespace FinalProject;

public partial class KitchenForm : Form
{
    private readonly Dictionary<int, OrderCard> _cards = new();
    private HashSet<int> _knownOrderIds = new();
    private string _signature = "";
    private bool _firstLoad = true;

    public KitchenForm()
    {
        InitializeComponent();
        if (Theme.AppIcon != null) Icon = Theme.AppIcon;
        Text = $"{Theme.RestaurantName} - Kitchen";

        User user = Session.CurrentUser!;
        headerBar.Title = "Kitchen";
        headerBar.UserText = $"{user.FullName}  •  {user.Role}";
        headerBar.LogoutClicked += (s, e) =>
        {
            DialogResult = DialogResult.Retry;   // back to Login
            Close();
        };

        BuildLegend();
        LoadOrders();
        refreshTimer.Start();   // live updates every 3 seconds
    }

    private void BuildLegend()
    {
        var items = new (string Text, Color Color)[]
        {
            ("New",       Theme.StatusNotStarted),
            ("Preparing", Theme.StatusPreparing),
            ("Ready",     Theme.StatusReady),
            ($"Late ({KitchenRepo.LateAfterMinutes}+ min)", Theme.StatusLate)
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

    private void LoadOrders()
    {
        List<KitchenOrder> orders;
        try
        {
            orders = KitchenRepo.GetOpenOrders();
        }
        catch (Exception ex)
        {
            refreshTimer.Stop();
            MessageBox.Show("Could not load orders:\n\n" + ex.Message, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Play a sound when a new order comes in
        var ids = orders.Select(o => o.OrderID).ToHashSet();
        if (!_firstLoad && ids.Any(id => !_knownOrderIds.Contains(id)))
            System.Media.SystemSounds.Asterisk.Play();
        _knownOrderIds = ids;
        _firstLoad = false;

        // Only rebuild the cards when orders or statuses changed
        string signature = string.Join(",", orders.Select(o => $"{o.OrderID}:{o.Status}"));
        if (signature == _signature)
        {
            foreach (KitchenOrder o in orders)
                _cards[o.OrderID].Order = o;   // just refreshes the timers
        }
        else
        {
            Point scroll = flpOrders.AutoScrollPosition;
            flpOrders.SuspendLayout();

            var old = flpOrders.Controls.Cast<Control>().ToList();
            flpOrders.Controls.Clear();
            foreach (Control c in old) c.Dispose();
            _cards.Clear();

            if (orders.Count == 0)
            {
                flpOrders.Controls.Add(new Label
                {
                    Text = "No open orders right now. All caught up!",
                    AutoSize = true,
                    Font = Theme.SubHeading,
                    ForeColor = Theme.TextMuted,
                    Margin = new Padding(16)
                });
            }

            foreach (KitchenOrder o in orders)
            {
                var card = new OrderCard { Order = o };
                card.ActionClicked += Card_ActionClicked;
                _cards[o.OrderID] = card;
                flpOrders.Controls.Add(card);
            }

            flpOrders.ResumeLayout();
            flpOrders.AutoScrollPosition = new Point(-scroll.X, -scroll.Y);
            _signature = signature;
        }

        int newCount = orders.Count(o => o.Status == "NotStarted");
        int preparing = orders.Count(o => o.Status == "Preparing");
        int ready = orders.Count(o => o.Status == "Ready");
        int late = orders.Count(o => o.IsLate);

        lblSummary.Text = $"{newCount} new   ·   {preparing} preparing   ·   {ready} ready" +
                          (late > 0 ? $"      |      {late} late" : "") +
                          $"      |      updated {DateTime.Now:h:mm:ss tt}";
    }

    private void Card_ActionClicked(object? sender, EventArgs e)
    {
        if (sender is not OrderCard card) return;

        string next = card.Order.Status == "NotStarted" ? "Preparing" : "Ready";
        try
        {
            OrderRepo.UpdateStatus(card.Order.OrderID, next);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Could not update the order:\n\n" + ex.Message, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Rebuild after the click finishes (the clicked button gets replaced)
        BeginInvoke(new Action(LoadOrders));
    }

    private void refreshTimer_Tick(object? sender, EventArgs e) => LoadOrders();

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        refreshTimer.Stop();
        base.OnFormClosed(e);
    }
}