namespace BasicArchitecture.Core.Dtos;

public partial class UserDto
{
    public int Id { get; set; }
    public int? BranchId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public int? CreatedUserId { get; set; }
    public int? UpdatedUserId { get; set; }
    public virtual BranchDto? Branch { get; set; }
}
