using AssetManagementSystem.Domain.Common;
using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.Interfaces;
using AssetManagementSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace AssetManagementSystem.Infrastructure;
public class ApplicationDbContext
    : IdentityDbContext<User, UserRole, Guid>
{
    private readonly IScopeContext _scope;
    private readonly CurrentUserAccessor _currentUser;
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IScopeContext scope,
        CurrentUserAccessor currentUser)
        : base(options)
    {
        _scope = scope;
        _currentUser = currentUser;
    }
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<User>(b => b.ToTable("Users"));
        builder.Entity<UserRole>(b => b.ToTable("Roles"));
        builder.Entity<IdentityUserRole<Guid>>(b => b.ToTable("UserRoles"));
        builder.Entity<IdentityUserClaim<Guid>>(b => b.ToTable("UserClaims"));
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        // Global query filter: zbatohet VETVETIU ne cdo query mbi Assets — edhe ne ato
        // qe do te shkruhen vite me vone. Korrektesia behet ajo qe ndodh kur harron.
        //
        // Shprehja lexon fushen _scope, pra EF-i e vlereson per cdo query, jo nje here.
        // Perjashtimi i qarte, kur duhet vertet gjithcka: .IgnoreQueryFilters()
        //
        // Vetem te Asset: nje filter mbi Employee do te krijonte varesi rrethore, sepse
        // vete zgjidhja e scope-it lexon tabelen Employees para se scope-i te dihet.
        builder.Entity<Asset>().HasQueryFilter(
            asset => _scope.IsGlobal || asset.DepartmentId == _scope.DepartmentId);
    }
   public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }
    private void ApplyAuditFields()
    {
        var now = DateTime.UtcNow;
        var userId = _currentUser.UserId;
        Guid? currentUserId = userId == Guid.Empty ? null : userId;
        // IAuditable dhe jo BaseEntity: User nuk e trashegon dot BaseEntity, por e mban
        // shenjuesin. Trupi mbetet i njejti, sepse vetite kerkohen me EMER, jo me tip.
        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(nameof(BaseEntity.CreatedAt)).CurrentValue = now;
                    entry.Property(nameof(BaseEntity.CreatedBy)).CurrentValue = currentUserId;
                    break;
                case EntityState.Modified:
                    // 
                    entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                    entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
                    entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = now;
                    entry.Property(nameof(BaseEntity.UpdatedBy)).CurrentValue = currentUserId;
                   break;
            }
        }
    }
}