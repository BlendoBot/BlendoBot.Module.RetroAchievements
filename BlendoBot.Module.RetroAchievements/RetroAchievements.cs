using BlendoBot.Core.Entities;
using BlendoBot.Core.Module;
using BlendoBot.Core.Services;
using BlendoBot.Module.RetroAchievements.API;
using BlendoBot.Module.RetroAchievements.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BlendoBot.Module.RetroAchievements;

[Module(Guid = "com.biendeo.blendobot.module.retroachievements", Name = "Retro Achievements", Author = "Biendeo", Version = "2.0.0", Url = "https://github.com/BlendoBot/BlendoBot.Module.RetroAchievements")]
public class RetroAchievements : IModule, IDisposable {
	public RetroAchievements(IConfig config, IDiscordInteractor discordInteractor, IFilePathProvider filePathProvider, ILogger logger, IModuleManager moduleManager) {
		Config = config;
		DiscordInteractor = discordInteractor;
		FilePathProvider = filePathProvider;
		Logger = logger;
		ModuleManager = moduleManager;

		RetroAchievementsCommand = new(this);

		ApiClient = new(string.Empty, string.Empty);
	}

	internal ulong GuildId { get; private set; }

	internal readonly RetroAchievementsCommand RetroAchievementsCommand;

	internal readonly IConfig Config;
	internal readonly IDiscordInteractor DiscordInteractor;
	internal readonly IFilePathProvider FilePathProvider;
	internal readonly ILogger Logger;
	internal readonly IModuleManager ModuleManager;

	internal RetroAchievementsClient ApiClient { get; private set; }

	public Task<bool> Startup(ulong guildId) {
		GuildId = guildId;
		bool configPopulated = true;
		string user = Config.ReadConfig(this, "Retro Achievements", "User");
		if (user == null) {
			Config.WriteConfig(this, "Retro Achievements", "User", "PLEASE ADD USERNAME");
			Logger.Log(this, new LogEventArgs {
				Type = LogType.Error,
				Message = $"BlendoBot Retro Achievements has not been supplied a valid username! Please acquire a key from https://retroachievements.org/controlpanel.php and add it in the config under the [Retro Achievements] section."
			});
			configPopulated = false;
		}
		string apiKey = Config.ReadConfig(this, "Retro Achievements", "ApiKey");
		if (apiKey == null) {
			Config.WriteConfig(this, "Retro Achievements", "ApiKey", "PLEASE ADD API KEY");
			Logger.Log(this, new LogEventArgs {
				Type = LogType.Error,
				Message = $"BlendoBot Retro Achievements has not been supplied a valid API key! Please acquire a key from https://retroachievements.org/controlpanel.php and add it in the config under the [Retro Achievements] section."
			});
			configPopulated = false;
		}
		if (!configPopulated) {
			return Task.FromResult(false);
		} else {
			ApiClient = new(user!, apiKey!);
			return Task.FromResult(ModuleManager.RegisterCommand(this, RetroAchievementsCommand, out _));
		}
	}
	public void Dispose() {
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing) {
		if (disposing) {
			ApiClient?.Dispose();
		}
	}

	internal string? GetUserProfileName(ulong userId) {
		using RetroAchievementsDbContext dbContext = RetroAchievementsDbContext.Get(this);
		UserSetting? setting = dbContext.UserSettings.FirstOrDefault(s => s.UserId == userId);
		if (setting == null) {
			return null;
		} else {
			return setting.RAUserName;
		}
	}

	internal async Task<bool> SetUserProfileName(ulong userId, string raUserName) {
		UserRankScoreResponse profileResponse = await ApiClient.GetUserRankAndScore(raUserName);
		if (profileResponse.Score == null) {
			return false;
		}
		using RetroAchievementsDbContext dbContext = RetroAchievementsDbContext.Get(this);
		UserSetting? setting = dbContext.UserSettings.FirstOrDefault(s => s.UserId == userId);
		if (setting == null) {
			setting = new UserSetting { UserId = userId, RAUserName = raUserName };
			dbContext.UserSettings.Add(setting);
		} else {
			setting.RAUserName = raUserName;
		}
		await dbContext.SaveChangesAsync();
		return true;
	}
}
