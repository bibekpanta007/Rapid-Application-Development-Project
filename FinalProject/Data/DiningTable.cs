namespace FinalProject.Data;

public class DiningTable
{
    public int TableID { get; set; }
    public int TableNumber { get; set; }
    public int Seats { get; set; }
    public string Status { get; set; } = "Available";   // Available, Occupied, Reserved
    public bool IsActive { get; set; } = true;

    // The open order on this table right now (null if none)
    public int? ActiveOrderID { get; set; }
    public string? ActiveOrderStatus { get; set; }      // NotStarted, Preparing, Ready
    public DateTime? ActiveOrderTime { get; set; }

    public override string ToString() => $"Table {TableNumber}";
}