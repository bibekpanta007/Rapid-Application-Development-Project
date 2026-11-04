using System.Globalization;

namespace FinalProject.Data;

public static class TableRepo
{
    // All active tables, each with its current open order (if any)
    public static List<DiningTable> GetAllWithActiveOrders()
    {
        var list = new List<DiningTable>();

        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT t.TableID, t.TableNumber, t.Seats, t.Status,
                   o.OrderID, o.Status, o.OrderDate
            FROM DiningTables t
            LEFT JOIN Orders o ON o.OrderID = (
                SELECT OrderID FROM Orders
                WHERE TableID = t.TableID
                  AND Status IN ('NotStarted', 'Preparing', 'Ready')
                ORDER BY OrderDate DESC
                LIMIT 1)
            WHERE t.IsActive = 1
            ORDER BY t.TableNumber";

        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new DiningTable
            {
                TableID = r.GetInt32(0),
                TableNumber = r.GetInt32(1),
                Seats = r.GetInt32(2),
                Status = r.GetString(3),
                ActiveOrderID = r.IsDBNull(4) ? null : r.GetInt32(4),
                ActiveOrderStatus = r.IsDBNull(5) ? null : r.GetString(5),
                ActiveOrderTime = r.IsDBNull(6) ? null
                    : DateTime.Parse(r.GetString(6), CultureInfo.InvariantCulture)
            });
        }
        return list;
    }

    public static void SetStatus(int tableId, string status)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE DiningTables SET Status = $s WHERE TableID = $id";
        cmd.Parameters.AddWithValue("$s", status);
        cmd.Parameters.AddWithValue("$id", tableId);
        cmd.ExecuteNonQuery();
    }
}