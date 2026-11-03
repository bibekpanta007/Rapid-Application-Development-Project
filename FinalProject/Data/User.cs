namespace FinalProject.Data;

public class User
{
    public int UserID { get; set; }
    public string Username { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Role { get; set; } = "";   // Manager, Server, Cook
    public bool IsActive { get; set; } = true;

    public override string ToString() => $"{FullName} ({Role})";
}

// Remembers who is logged in, so any screen can use it
public static class Session
{
    public static User? CurrentUser { get; set; }
}