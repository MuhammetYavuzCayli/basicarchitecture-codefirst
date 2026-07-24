using BasicArchitecture.Core.Statics;

namespace BasicArchitecture.Domain.MyDbContext;

public partial class BasicArchitecturedbContext : DbContext
{
    public BasicArchitecturedbContext()
    {
    }

    public BasicArchitecturedbContext(DbContextOptions<BasicArchitecturedbContext> options) : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; } = null!;
    public virtual DbSet<Branch> Branches { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<PasswordHistory> PasswordHistories { get; set; } = null!;
    public virtual DbSet<UserToken> UserTokens { get; set; } = null!;

    // No connection string here: it is always supplied from the outside — via DI's
    // AddDbContext (at runtime) or via BasicArchitecturedbContextFactory (at design
    // time, for the dotnet ef CLI).

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Role_pkey");
            entity.ToTable("Role");
            entity.Property(e => e.Id).UseIdentityByDefaultColumn();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreateDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Branch_pkey");
            entity.ToTable("Branch");
            entity.Property(e => e.Id).UseIdentityByDefaultColumn();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreateDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");
            entity.ToTable("User");
            entity.Property(e => e.Id).UseIdentityByDefaultColumn();
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
            entity.Property(e => e.PhoneNumber).HasMaxLength(30);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreateDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasOne(e => e.Branch)
                .WithMany(b => b.Users)
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade),
                    j => j.ToTable("UserRole"));
        });

        modelBuilder.Entity<PasswordHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PasswordHistory_pkey");
            entity.ToTable("PasswordHistory");
            entity.Property(e => e.Id).UseIdentityByDefaultColumn();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreateDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.User)
                .WithMany(u => u.PasswordHistories)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserToken_pkey");
            entity.ToTable("UserToken");
            entity.Property(e => e.Id).UseIdentityByDefaultColumn();
            entity.Property(e => e.RefreshToken).IsRequired();
            entity.Property(e => e.CreateDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.User)
                .WithMany(u => u.UserTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = Constants.Roles.Admin, CreateDate = new DateTime(2026, 1, 1) },
            new Role { Id = 2, Name = Constants.Roles.SuperAdmin, CreateDate = new DateTime(2026, 1, 1) },
            new Role { Id = 3, Name = Constants.Roles.User, CreateDate = new DateTime(2026, 1, 1) }
        );

        modelBuilder.Entity<Branch>().HasData(
            new Branch { Id = 1, Name = "Head Office", Code = "HQ", CreateDate = new DateTime(2026, 1, 1) }
        );

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
