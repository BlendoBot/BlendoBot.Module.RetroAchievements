namespace BlendoBot.Module.RetroAchievements.Database;

internal class UserSetting {
	public ulong UserId { get; set; }

	public string RAUserName { get; set; } = default!;
}
