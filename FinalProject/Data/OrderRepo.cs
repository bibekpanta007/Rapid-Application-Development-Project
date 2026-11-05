namespace FinalProject.Data;

// One line in the server's cart (before the order is saved)
public class CartLine
{
    public MenuItem Item { get; set; } = new();
    public int Quantity { get; set; }
    public decimal LineTotal => Item.Price * Quantity;
}

// One line of a saved order (used by the kitchen and order details)
public class OrderLine
{
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Notes { get; set; }
}

public static class OrderRepo
{
    public static decimal CalcTax(decimal subtotal, decimal taxRate) =>
        Math.Round(subtotal * taxRate, 2, MidpointRounding.AwayFromZero);

    // Saves the order + its items and marks the table Occupied, all in one transaction
    public static int CreateOrder(int tableId, int userId, IReadOnlyList<CartLine> lines,
                                  string? notes, decimal taxRate)
    {
        decimal subtotal = lines.Sum(l => l.LineTotal);
        decimal tax = CalcTax(subtotal, taxRate);
        decimal total = subtotal + tax;

        using var conn = Db.Open();
        using var tx = conn.BeginTransaction();
        int orderId;

        using (var cmd = conn.CreateCommand())
        {
            cmd.Transaction = tx;
            cmd.CommandText = @"
                INSERT INTO Orders (TableID, UserID, Status, Subtotal, TaxRate, TaxAmount, Total, Notes)
                VALUES ($table, $user, 'NotStarted', $sub, $rate, $tax, $total, $notes);
                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$table", tableId);
            cmd.Parameters.AddWithValue("$user", userId);
            cmd.Parameters.AddWithValue("$sub", (double)subtotal);
            cmd.Parameters.AddWithValue("$rate", (double)taxRate);
            cmd.Parameters.AddWithValue("$tax", (double)tax);
            cmd.Parameters.AddWithValue("$total", (double)total);
            cmd.Parameters.AddWithValue("$notes",
                string.IsNullOrWhiteSpace(notes) ? DBNull.Value : notes.Trim());
            orderId = Convert.ToInt32(cmd.ExecuteScalar());
        }

        using (var cmd = conn.CreateCommand())
        {
            cmd.Transaction = tx;
            cmd.CommandText = @"INSERT INTO OrderItems (OrderID, ItemID, Quantity, UnitPrice)
                                VALUES ($order, $item, $qty, $price)";
            var pItem = cmd.Parameters.Add("$item", Microsoft.Data.Sqlite.SqliteType.Integer);
            var pQty = cmd.Parameters.Add("$qty", Microsoft.Data.Sqlite.SqliteType.Integer);
            var pPrice = cmd.Parameters.Add("$price", Microsoft.Data.Sqlite.SqliteType.Real);
            cmd.Parameters.AddWithValue("$order", orderId);

            foreach (CartLine line in lines)
            {
                pItem.Value = line.Item.ItemID;
                pQty.Value = line.Quantity;
                pPrice.Value = (double)line.Item.Price;   // price is copied, so menu changes don't affect old orders
                cmd.ExecuteNonQuery();
            }
        }

        using (var cmd = conn.CreateCommand())
        {
            cmd.Transaction = tx;
            cmd.CommandText = "UPDATE DiningTables SET Status = 'Occupied' WHERE TableID = $table";
            cmd.Parameters.AddWithValue("$table", tableId);
            cmd.ExecuteNonQuery();
        }

        tx.Commit();
        return orderId;
    }

    // NotStarted -> Preparing -> Ready -> Completed (also records the time of each step)
    public static void UpdateStatus(int orderId, string newStatus)
    {
        string stampColumn = newStatus switch
        {
            "Preparing" => "StartedAt",
            "Ready" => "ReadyAt",
            "Completed" => "CompletedAt",
            _ => ""
        };

        using var conn = Db.Open();
        using var tx = conn.BeginTransaction();

        using (var cmd = conn.CreateCommand())
        {
            cmd.Transaction = tx;
            cmd.CommandText = "UPDATE Orders SET Status = $status" +
                (stampColumn == "" ? "" : $", {stampColumn} = datetime('now', 'localtime')") +
                " WHERE OrderID = $id";
            cmd.Parameters.AddWithValue("$status", newStatus);
            cmd.Parameters.AddWithValue("$id", orderId);
            cmd.ExecuteNonQuery();
        }

        // Only a cancelled order frees the table right away.
        // A served (Completed) order keeps the table Occupied until the server clears it.
        if (newStatus == "Cancelled")
        {
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = @"UPDATE DiningTables SET Status = 'Available'
                                WHERE TableID = (SELECT TableID FROM Orders WHERE OrderID = $id)";
            cmd.Parameters.AddWithValue("$id", orderId);
            cmd.ExecuteNonQuery();
        }

        tx.Commit();
    }

    public static List<OrderLine> GetLines(int orderId)
    {
        var list = new List<OrderLine>();
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT m.Name, oi.Quantity, oi.UnitPrice, oi.Notes
                            FROM OrderItems oi
                            JOIN MenuItems m ON m.ItemID = oi.ItemID
                            WHERE oi.OrderID = $id
                            ORDER BY oi.OrderItemID";
        cmd.Parameters.AddWithValue("$id", orderId);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new OrderLine
            {
                Name = r.GetString(0),
                Quantity = r.GetInt32(1),
                UnitPrice = Math.Round((decimal)r.GetDouble(2), 2),
                Notes = r.IsDBNull(3) ? null : r.GetString(3)
            });
        }
        return list;
    }
}