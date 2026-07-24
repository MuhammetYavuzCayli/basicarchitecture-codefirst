namespace BasicArchitecture.Domain.MyDbContext;

// Sensitive table: no repository/service/controller is generated for it — only AuthService
// manages it directly through its injected DbContext.
public partial class UserToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string RefreshToken { get; set; } = null!;
    public DateTime ExpireDate { get; set; }
    public DateTime CreateDate { get; set; }

    public virtual User User { get; set; } = null!;
}
