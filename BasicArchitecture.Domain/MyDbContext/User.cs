namespace BasicArchitecture.Domain.MyDbContext;

public partial class User
{
    public int Id { get; set; }
    public int? BranchId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public int? CreatedUserId { get; set; }
    public int? UpdatedUserId { get; set; }

    public virtual Branch? Branch { get; set; }
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    public virtual ICollection<PasswordHistory> PasswordHistories { get; set; } = new List<PasswordHistory>();
    public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();
}
