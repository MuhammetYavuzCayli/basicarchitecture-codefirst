namespace BasicArchitecture.Core.Dtos;

public partial class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public int? CreatedUserId { get; set; }
    public int? UpdatedUserId { get; set; }
}
