using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlendoBot.Module.RetroAchievements.API;

internal class RetroAchievementsClient : IDisposable {
	public RetroAchievementsClient(string user, string apiKey) {
		this.user = user;
		this.apiKey = apiKey;
		httpClient = new();
	}

	private readonly string user;
	private readonly string apiKey;
	private readonly HttpClient httpClient;

	public void Dispose() {
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing) {
		if (disposing) {
			httpClient?.Dispose();
		}
	}

	private string CreateRequestUrl(string targetPath, Dictionary<string, string> parameters) => $"https://retroachievements.org/API/{targetPath}?z={user}&y={apiKey}{string.Concat(parameters.Select(p => $"&{p.Key}={p.Value}"))}";

	private async Task<T> Get<T>(string targetPath, Dictionary<string, string> parameters) {
		string requestUrl = CreateRequestUrl(targetPath, parameters);
		using Stream stream = await httpClient.GetStreamAsync(requestUrl);
		T response = await JsonSerializer.DeserializeAsync<T>(stream) ?? throw new JsonException($"Could not deserialize {nameof(T)}");
		return response;
	}

	public string GetGameUrl(ulong gameId) => $"https://retroachievements.org/game/{gameId}";
	public string GetUserUrl(string user) => $"https://retroachievements.org/user/{user}";
	public string GetAchievementUrl(ulong achievementId) => $"https://retroachievements.org/achievement/{achievementId}";

	public string GetImageFullPath(string partialImagePath) => $"https://retroachievements.org/{partialImagePath}";
	public string GetBadgeNamePath(ulong badgeName) => $"https://s3-eu-west-1.amazonaws.com/i.retroachievements.org/Badge/{badgeName}.png";

	public async Task<List<TopUserResponse>> GetTopTenUsers() => await Get<List<TopUserResponse>>("API_GetTopTenUsers.php", new());

	public async Task<GameInfoResponse> GetGameInfo(ulong gameId) => await Get<GameInfoResponse>("API_GetGame.php", new() { { "i", gameId.ToString() } });

	public async Task<GameInfoExtendedResponse> GetGameInfoExtended(ulong gameId) => await Get<GameInfoExtendedResponse>("API_GetGameExtended.php", new() { { "i", gameId.ToString() } });

	public async Task<ulong?> GetGameIDByName(string gameName) {
		string response = await httpClient.GetStringAsync($"https://www.google.com/search?q=site:retroachievements.org+{string.Join('+', gameName.Split(' '))}");
		MatchCollection matches = Regex.Matches(response, @"https:\/\/retroachievements\.org\/game\/(\d+)");
		if (matches.Count > 0) {
			return ulong.Parse(matches[0].Groups[1].Value);
		} else {
			return null;
		}
	}

	public async Task<List<ConsoleIDResponse>> GetConsoleIDs() => await Get<List<ConsoleIDResponse>>("API_GetConsoleIDs.php", new());

	public async Task<List<GameListResponse>> GetGameList(ulong consoleId) => await Get<List<GameListResponse>>("API_GetGameList.php", new() { { "i", consoleId.ToString() } });

	//public async Task<> GetFeedFor(string user, int count, int offset) => await Get<List<GameListResponse>>("API_GetFeed.php", new() { { "u", user }, { "c", count.ToString() }, { "o", offset.ToString() } });

	public async Task<UserRankScoreResponse> GetUserRankAndScore(string user) => await Get<UserRankScoreResponse>("API_GetUserRankAndScore.php", new() { { "u", user } });

	//public async Task<> GetUserProgress(string user, IEnumerable<ulong> gameIds) => await Get<List<GameListResponse>>("API_GetUserProgress.php", new() { { "u", user }, { "i", string.Join(',', gameIds) } });

	public async Task<List<UserRecentlyPlayedGamesResponse>> GetUserRecentlyPlayedGames(string user, int count, int offset) => await Get<List<UserRecentlyPlayedGamesResponse>>("API_GetUserRecentlyPlayedGames.php", new() { { "u", user }, { "c", count.ToString() }, { "o", offset.ToString() } });

	public async Task<UserSummaryResponse?> GetUserSummary(string user, int numRecentGames, int numAchievements) {
		try {
			return await Get<UserSummaryResponse>("API_GetUserSummary.php", new() { { "u", user }, { "g", numRecentGames.ToString() }, { "a", numAchievements.ToString() } });
		} catch (HttpRequestException) {
			return null;
		}
	}

	public async Task<GameInfoAndUserProgressResponse> GetGameInfoAndUserProgress(string user, ulong gameId) => await Get<GameInfoAndUserProgressResponse>("API_GetGameInfoAndUserProgress.php", new() { { "u", user }, { "g", gameId.ToString() } });

	public async Task<AchievementOfTheWeekResponse> GetAchievementOfTheWeek() => await Get<AchievementOfTheWeekResponse>("API_GetAchievementOfTheWeek.php", new());

	public async Task<AchievementUnlocksResponse> GetAchievementUnlocks(ulong achievementId) => await Get<AchievementUnlocksResponse>("API_GetAchievementUnlocks.php", new() { { "a", achievementId.ToString() } });
}
