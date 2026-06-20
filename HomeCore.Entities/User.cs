using System.Data;

namespace HomeCore.Entities;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; } = "admin";
    public DateTime CreatedAt { get; set; }
}
