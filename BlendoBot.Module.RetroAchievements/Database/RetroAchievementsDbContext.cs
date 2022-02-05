using Microsoft.EntityFrameworkCore;
using System.IO;

namespace BlendoBot.Module.RetroAchievements.Database;

internal class RetroAchievementsDbContext : DbContext {
	private RetroAchievementsDbContext(DbContextOptions<RetroAchievementsDbContext> options) : base(options) { }
	public DbSet<UserSetting> UserSettings { get; set; } = default!;

	public static RetroAchievementsDbContext Get(RetroAchievements module) {
		DbContextOptionsBuilder<RetroAchievementsDbContext> optionsBuilder = new();
		optionsBuilder.UseSqlite($"Data Source={Path.Combine(module.FilePathProvider.GetDataDirectoryPath(module), "retroachievements.db")}");
		RetroAchievementsDbContext dbContext = new(optionsBuilder.Options);
		dbContext.Database.EnsureCreated();
		return dbContext;
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<UserSetting>().HasKey(s => new { s.UserId });
	}
}
