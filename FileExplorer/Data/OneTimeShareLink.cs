namespace FileExplorer.Data;

public sealed class OneTimeShareLink
{
    public required Guid Id { get; set; }

    public required string UserId { get; set; }

    public required string Url { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UsedAt { get; set; }

    public bool IsUsed { get; set; }

    public required IEnumerable<int> FileIds { get; set; }

    public ApplicationUser? User { get; set; }
}
