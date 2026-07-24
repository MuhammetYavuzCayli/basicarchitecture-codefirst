namespace BasicArchitecture.Core.Models;

public class AccountDto
{
    public int Id { get; set; }
    public int? BranchId { get; set; }
    public string? BranchName { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public List<string> Roles { get; set; } = new();
}
