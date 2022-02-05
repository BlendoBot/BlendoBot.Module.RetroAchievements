using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlendoBot.Module.RetroAchievements.API;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
internal class UserSummaryResponse {
	public int RecentlyPlayedCount { get; set; }
	public List<RecentlyPlayedGame> RecentlyPlayed { get; set; } = default!;
	public string MemberSince { get; set; } = default!;
	public LastActivityDetails LastActivity { get; set; } = default!;
	public string RichPresenceMsg { get; set; } = default!;
	public ulong LastGameID { get; set; }
	public LastGameDetails LastGame { get; set; } = default!;
	public int ContribCount { get; set; }
	public int ContribYield { get; set; }
	public int TotalPoints { get; set; }
	public int? TotalTruePoints { get; set; }
	public ulong Permissions { get; set; }
	public int Untracked { get; set; }
	public ulong ID { get; set; }
	public int UserWallActive { get; set; }
	public string Motto { get; set; } = default!;
	public int Rank { get; set; }
	public Dictionary<string, GameAchievementDetails> Awarded { get; set; } = default!;
	public Dictionary<string, Dictionary<string, RecentAchievementDetails>>? RecentAchievements { get; set; } = default!;
	public int Points { get; set; }
	public string UserPic { get; set; } = default!;
	public int TotalRanked { get; set; }
	public string Status { get; set; } = default!;


	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	internal class RecentlyPlayedGame {
		public ulong GameID { get; set; }
		public ulong ConsoleID { get; set; }
		public string ConsoleName { get; set; } = default!;
		public string Title { get; set; } = default!;
		public string ImageIcon { get; set; } = default!;
		[JsonConverter(typeof(DateTimeConverter))]
		public DateTime LastPlayed { get; set; }
		public object MyVote { get; set; } = default!;
	}

	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	internal class LastActivityDetails {
		public ulong ID { get; set; }
		[JsonConverter(typeof(DateTimeConverter))]
		public DateTime Timestamp { get; set; }
		[JsonConverter(typeof(DateTimeConverter))]
		public DateTime LastUpdate { get; set; }
		public ulong ActivityType { get; set; }
		public string User { get; set; } = default!;
		public int Data { get; set; }
		public int Data2 { get; set; }
	}

	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	internal class LastGameDetails {
		public ulong ID { get; set; }
		public string Title { get; set; } = default!;
		public ulong ConsoleID { get; set; }
		public ulong ForumTopicID { get; set; }
		public ulong Flags { get; set; }
		public string ImageIcon { get; set; } = default!;
		public string ImageTitle { get; set; } = default!;
		public string ImageIngame { get; set; } = default!;
		public string ImageBoxArt { get; set; } = default!;
		public string Publisher { get; set; } = default!;
		public string Developer { get; set; } = default!;
		public string Genre { get; set; } = default!;
		public string Released { get; set; } = default!;
		public bool IsFinal { get; set; }
		public string ConsoleName { get; set; } = default!;
		public string RichPresencePatch { get; set; } = default!;
	}

	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	internal class GameAchievementDetails {
		public int NumPossibleAchievements { get; set; }
		public int PossibleScore { get; set; }
		public int NumAchieved { get; set; }
		public int ScoreAchieved { get; set; }
		public int NumAchievedHardcore { get; set; }
		public int ScoreAchievedHardcore { get; set; }
	}

	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
	internal class RecentAchievementDetails {
		public ulong ID { get; set; }
		public ulong GameID { get; set; }
		public string GameTitle { get; set; } = default!;
		public string Title { get; set; } = default!;
		public string Description { get; set; } = default!;
		public int Points { get; set; }
		public ulong BadgeName { get; set; }
		public int IsAwarded { get; set; }
		[JsonConverter(typeof(DateTimeConverter))]
		public DateTime? DateAwarded { get; set; } = default!;
		public int? HardcoreAchieved { get; set; }
	}
}
