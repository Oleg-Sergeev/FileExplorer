namespace FileExplorer.Data;

public sealed class UserFile
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public string Name { get; set; }

    public DateTime CreatedAt { get; set; }


    public ApplicationUser? User { get; set; }


    public UserFile(string userId, string name)
    {
        UserId = userId;
        Name = name;
    }
}