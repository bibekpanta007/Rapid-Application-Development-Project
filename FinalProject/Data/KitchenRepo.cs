using System.Globalization;

namespace FinalProject.Data;

public class KitchenOrder
{
    public int OrderID { get; set; }
    public int TableNumber { get; set; }
    public string ServerName { get; set; } = "";
    public string Status { get; set; } = "NotStarted";
    public DateTime OrderDate { get; set; }
    public DateTime? ReadyAt { get; set; }
    public string? Notes { get; set; }
    public List<OrderLine> Lines { get; } = new();

    public int MinutesWaiting => Math.Max(0, (int)(DateTime.Now - OrderDate).TotalMinutes);
    public bool IsLate => Status != "Ready" && MinutesWaiting >= KitchenRepo.LateAfterMinutes;
}

public static class KitchenRepo
{
    public const int LateAfterMinutes = 15;

    // All open orders with their items: new/preparing first (oldest first), ready ones last
    public static List<KitchenOrder> GetOpenOrders()
    {
        var result = new List<KitchenOrder>();
        var byId = new Dictionary<int, KitchenOrder>();

        using var conn = Db.Open();

        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
                SELECT o.OrderID, t.TableNumber, u.FullName, o.Status, o.OrderDate, o.ReadyAt, o.Notes
                FROM Orders o
                JOIN DiningTables t ON t.TableID = o.TableID
                JOIN Users u ON u.UserID = o.UserID
                WHERE o.Status IN ('NotStarted', 'Preparing', 'Ready')
                ORDER BY CASE o.Status WHEN 'Ready' THEN 1 ELSE 0 END, o.OrderDate";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var o = new KitchenOrder
                {
                    OrderID = r.GetInt32(0),
                    TableNumber = r.GetInt32(1),
                    ServerName = r.GetString(2),
                    Status = r.GetString(3),
                    OrderDate = ParseDate(r.GetString(4)),
                    ReadyAt = r.IsDBNull(5) ? null : ParseDate(r.GetString(5)),
                    Notes = r.IsDBNull(6) ? null : r.GetString(6)
                };
                result.Add(o);
                byId[o.OrderID] = o;
            }
        }

        if (result.Count == 0) return result;

        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
                SELECT oi.OrderID, m.Name, oi.Quantity, oi.UnitPrice, oi.Notes
                FROM OrderItems oi
                JOIN MenuItems m ON m.ItemID = oi.ItemID
                JOIN Orders o ON o.OrderID = oi.OrderID
                WHERE o.Status IN ('NotStarted', 'Preparing', 'Ready')
                ORDER BY oi.OrderID, oi.OrderItemID";
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                if (!byId.TryGetValue(r.GetInt32(0), out KitchenOrder? order)) continue;
                order.Lines.Add(new OrderLine
                {
                    Name = r.GetString(1),
                    Quantity = r.GetInt32(2),
                    UnitPrice = Math.Round((decimal)r.GetDouble(3), 2),
                    Notes = r.IsDBNull(4) ? null : r.GetString(4)
                });
            }
        }

        return result;
    }

    private static DateTime ParseDate(string s) => DateTime.Parse(s, CultureInfo.InvariantCulture);
}