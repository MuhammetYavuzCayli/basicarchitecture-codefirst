namespace BasicArchitecture.Domain.MyDbContext;

public partial class Branch
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public int? CreatedUserId { get; set; }
    public int? UpdatedUserId { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
