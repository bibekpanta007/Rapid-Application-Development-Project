using System.Globalization;

namespace FinalProject.Data;

public static class SettingsRepo
{
    public static string Get(string key, string fallback = "")
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Value FROM Settings WHERE Key = $key";
        cmd.Parameters.AddWithValue("$key", key);
        return cmd.ExecuteScalar() as string ?? fallback;
    }

    public static void Set(string key, string value)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"INSERT INTO Settings (Key, Value) VALUES ($key, $value)
                            ON CONFLICT(Key) DO UPDATE SET Value = excluded.Value";
        cmd.Parameters.AddWithValue("$key", key);
        cmd.Parameters.AddWithValue("$value", value);
        cmd.ExecuteNonQuery();
    }

    // Tax rate stored as a decimal, e.g. 0.05 = 5%
    public static decimal TaxRate
    {
        get => decimal.Parse(Get("TaxRate", "0.05"), CultureInfo.InvariantCulture);
        set => Set("TaxRate", value.ToString(CultureInfo.InvariantCulture));
    }

    public static string TaxLabel => Get("TaxLabel", "GST");
}