using System.ComponentModel.DataAnnotations;

namespace Thomas.Api.Domain.Entities;

public enum UserRole { Admin = 0, Candidate = 1 }

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(255)]
    public string Email { get; set; } = null!;

    [MaxLength(512)]
    public string PasswordHash { get; set; } = null!;

    [MaxLength(200)]
    public string FullName { get; set; } = null!;

    public UserRole Role { get; set; } = UserRole.Candidate;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    public ICollection<Attempt> Attempts { get; set; } = new List<Attempt>();
}
