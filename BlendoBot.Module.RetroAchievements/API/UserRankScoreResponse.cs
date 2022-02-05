namespace BlendoBot.Module.RetroAchievements.API;

internal class UserRankScoreResponse {
	public int? Score { get; set; }
	public int Rank { get; set; }
	public string TotalRanked { get; set; } = default!;
}
