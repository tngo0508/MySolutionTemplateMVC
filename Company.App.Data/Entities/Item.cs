namespace Company.App.Data.Entities;

/// <summary>
/// Database persistence entity representing an item record.
/// Isolated inside Company.App.Data to preserve separation of concerns.
/// </summary>
public class Item
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}