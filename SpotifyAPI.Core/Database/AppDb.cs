using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;

using SpotifyAPI.Core.Identity.Entities;

namespace SpotifyAPI.Core.Database;


public class AppDb : IdentityDbContext<User, Role, Guid>, IPersistedGrantDbContext
{
    private readonly IOptions<OperationalStoreOptions> _operationalStoreOptions;

    public AppDb (
        DbContextOptions options,
        IOptions<OperationalStoreOptions> operationalStoreOptions
    )
        : base(options)
    {
        _operationalStoreOptions = operationalStoreOptions;
    }

    public DbSet<PersistedGrant>? PersistedGrants { get; set; }
    public DbSet<DeviceFlowCodes>? DeviceFlowCodes { get; set; }

    Task<int> IPersistedGrantDbContext.SaveChangesAsync ()
    {
        return base.SaveChangesAsync();
    }

    protected override void OnModelCreating (ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ConfigurePersistedGrantContext(_operationalStoreOptions.Value);

        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public override int SaveChanges ()
    {
        return this.SaveChangesWithTriggers(base.SaveChanges);
    }

    public override int SaveChanges (bool acceptAllChangesOnSuccess)
    {
        return this.SaveChangesWithTriggers(base.SaveChanges, acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync (CancellationToken cancellationToken = default)
    {
        return this.SaveChangesWithTriggersAsync(base.SaveChangesAsync, true, cancellationToken);
    }

    public override Task<int> SaveChangesAsync (
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default
    )
    {
        return this.SaveChangesWithTriggersAsync(
            base.SaveChangesAsync,
            acceptAllChangesOnSuccess,
            cancellationToken
        );
    }
}