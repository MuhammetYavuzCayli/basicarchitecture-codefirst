namespace BasicArchitecture.Core.Models;

// Single-level hierarchy: Register links to an existing Branch (does not create a new one).
public class RegisterDto
{
    public required int BranchId { get; set; }
    public int? RoleId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? PhoneNumber { get; set; }
}
