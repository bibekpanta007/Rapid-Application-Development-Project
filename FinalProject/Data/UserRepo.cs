using System.Security.Cryptography;
using System.Text;

namespace FinalProject.Data;

public static class UserRepo
{
    // Passwords are never stored as plain text, only their SHA-256 hash
    public static string HashPassword(string password) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(password)));

    // Returns the user if the username and password match, otherwise null
    public static User? Authenticate(string username, string password)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT UserID, Username, FullName, Role, IsActive
                            FROM Users
                            WHERE Username = $u AND PasswordHash = $p AND IsActive = 1";
        cmd.Parameters.AddWithValue("$u", username.Trim());
        cmd.Parameters.AddWithValue("$p", HashPassword(password));

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;

        return new User
        {
            UserID = r.GetInt32(0),
            Username = r.GetString(1),
            FullName = r.GetString(2),
            Role = r.GetString(3),
            IsActive = r.GetInt32(4) == 1
        };
    }
}