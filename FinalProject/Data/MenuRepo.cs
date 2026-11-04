using System.Globalization;

namespace FinalProject.Data;

public class Category
{
    public int CategoryID { get; set; }
    public string Name { get; set; } = "";
    public int SortOrder { get; set; }

    public override string ToString() => Name;
}

public class MenuItem
{
    public int ItemID { get; set; }
    public string Name { get; set; } = "";
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = "";
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImagePath { get; set; }
    public bool IsAvailable { get; set; } = true;   // false = sold out today
    public bool IsActive { get; set; } = true;      // false = removed from menu

    public override string ToString() => Name;
}

public static class MenuRepo
{
    public static List<Category> GetCategories()
    {
        var list = new List<Category>();
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT CategoryID, Name, SortOrder FROM Categories
                            WHERE IsActive = 1 ORDER BY SortOrder, Name";
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Category
            {
                CategoryID = r.GetInt32(0),
                Name = r.GetString(1),
                SortOrder = r.GetInt32(2)
            });
        }
        return list;
    }

    public static List<MenuItem> GetItems(bool includeRemoved = false)
    {
        var list = new List<MenuItem>();
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT m.ItemID, m.Name, m.CategoryID, c.Name, m.Description, m.Price,
                   m.ImagePath, m.IsAvailable, m.IsActive
            FROM MenuItems m
            JOIN Categories c ON c.CategoryID = m.CategoryID
            " + (includeRemoved ? "" : "WHERE m.IsActive = 1") + @"
            ORDER BY c.SortOrder, m.Name";
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new MenuItem
            {
                ItemID = r.GetInt32(0),
                Name = r.GetString(1),
                CategoryID = r.GetInt32(2),
                CategoryName = r.GetString(3),
                Description = r.IsDBNull(4) ? null : r.GetString(4),
                Price = Math.Round((decimal)r.GetDouble(5), 2),
                ImagePath = r.IsDBNull(6) ? null : r.GetString(6),
                IsAvailable = r.GetInt32(7) == 1,
                IsActive = r.GetInt32(8) == 1
            });
        }
        return list;
    }
}