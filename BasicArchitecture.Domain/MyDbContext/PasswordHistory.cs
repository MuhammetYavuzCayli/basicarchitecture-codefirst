namespace BasicArchitecture.Domain.MyDbContext;

// Sensitive table: no repository/service/controller is generated for it — only AuthService
// manages it directly through its injected DbContext.
public partial class PasswordHistory
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string PasswordHash { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTime CreateDate { get; set; }

    public virtual User User { get; set; } = null!;
}
