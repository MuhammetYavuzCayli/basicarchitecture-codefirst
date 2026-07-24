namespace BasicArchitecture.Core.Dtos;

public partial class BranchDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public int? CreatedUserId { get; set; }
    public int? UpdatedUserId { get; set; }
    public virtual ICollection<UserDto> Users { get; set; } = new List<UserDto>();
}
