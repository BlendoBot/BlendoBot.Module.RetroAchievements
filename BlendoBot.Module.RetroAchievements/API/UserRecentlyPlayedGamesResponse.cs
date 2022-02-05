using System.Text.Json.Serialization;

namespace BlendoBot.Module.RetroAchievements.API;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
internal class UserRecentlyPlayedGamesResponse {
	public ulong GameID { get; set; }
	public ulong ConsoleID { get; set; }
	public string ConsoleName { get; set; } = default!;
	public string Title { get; set; } = default!;
	public string ImageIcon { get; set; } = default!;
	public string LastPlayed { get; set; } = default!;
	public object? MyVote { get; set; } = default!;
	public int NumPossibleAchievements { get; set; }
	public int PossibleScore { get; set; }
	public int NumAchieved { get; set; }
	public int ScoreAchieved { get; set; }
}
