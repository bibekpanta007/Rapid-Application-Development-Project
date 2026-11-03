using Microsoft.Data.Sqlite;

namespace FinalProject.Data;

public static class Db
{
    // The database file is created next to the .exe (bin\Debug\net10.0-windows\)
    private static readonly string DbPath =
        Path.Combine(AppContext.BaseDirectory, "restaurant.db");

    public static string ConnectionString => $"Data Source={DbPath};Foreign Keys=True";

    // Opens a connection; always use it with "using" so it closes automatically
    public static SqliteConnection Open()
    {
        var conn = new SqliteConnection(ConnectionString);
        conn.Open();
        return conn;
    }

    // Creates the database the first time the app runs
    public static void Initialize()
    {
        if (File.Exists(DbPath)) return;
        RunScript("schema.sql");
        RunScript("seed.sql");
    }

    // Runs a .sql file from the Database folder
    public static void RunScript(string fileName)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Database", fileName);
        string sql = File.ReadAllText(path);

        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    // Deletes the database so it gets rebuilt next run (handy while developing)
    public static void Reset()
    {
        SqliteConnection.ClearAllPools();
        if (File.Exists(DbPath)) File.Delete(DbPath);
        Initialize();
    }
}