namespace App;

using Microsoft.EntityFrameworkCore;

/// Database management for the app.
public class AppDbContext : DbContext {
	/// We need this constructor for configuration via `appsetting.json`
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	/// Declare all models for LinQ query translation
	public DbSet<UserModel> users { get; set; }

	public DbSet<BingoClientModel> bingoClients { get; set; }
	public DbSet<BingoModel> bingos { get; set; }

	/// Construct model + Seeding data
	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		UserModelBuilder.OnModelCreating(modelBuilder);

		BingoModelBuilder.OnModelCreating(modelBuilder);
		BingoClientModelBuilder.OnModelCreating(modelBuilder);
	}

	/// @Override
	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
		var modifiedEntries = this.ChangeTracker.Entries()
			.Where(x => x.State == EntityState.Modified)
			.Select(x => x.Entity)
		;
		foreach (var modifiedEntry in modifiedEntries) {
			var entity = modifiedEntry as AutoGenerateUpdateTime;
			if (entity != null) {
				entity.updated_at = DateTime.UtcNow;
			}
		}

		return base.SaveChangesAsync(cancellationToken);
	}
}
