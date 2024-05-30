using Newtonsoft.Json.Linq;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("Playtime Command", "Papa", "1.1.0")]
    [Description("Shows playtime of a player on a specific server using BattleMetrics API.")]
    public class PlaytimeCommand : CovalencePlugin
    {
        private string apiKey;
        private string serverId;
        private string serverName;

        #region Configuration
        protected override void LoadDefaultConfig()
        {
            Config["BattleMetricsAPIKey"] = "YOUR_BATTLEMETRICS_API_KEY"; // Replace with your API key
            Config["ServerID"] = "YOUR_SERVER_ID"; // Replace with your server ID
            Config["ServerName"] = "YOUR_SERVER_NAME"; // Replace with your server name
            SaveConfig();
        }

        private void Init()
        {
            LoadConfig();
            apiKey = Config["BattleMetricsAPIKey"]?.ToString();
            serverId = Config["ServerID"]?.ToString();
            serverName = Config["ServerName"]?.ToString();
            
            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(serverId) || string.IsNullOrEmpty(serverName))
            {
                PrintError("Configuration is not set properly. Please check your configuration file.");
                return;
            }
        }
        #endregion

        #region Localization
        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["PlaytimeMessage"] = "Your playtime on {0}: {1} hours",
                ["ErrorBattleMetricsID"] = "Error: Unable to find BattleMetrics PlayerID.",
                ["ErrorPlaytimeRetrieval"] = "Error: Unable to retrieve playtime. Status code: {0}",
                ["ErrorProcessingData"] = "Error processing your playtime data."
            }, this);
        }
        #endregion

        [Command("playtime")]
        private void PlaytimeCommandMethod(IPlayer player, string command, string[] args)
        {
            GetPlayerIdFromSteamId(player.Id, playerId =>
            {
                if (playerId != null)
                {
                    QueryBattleMetrics(playerId, message => player.Reply(message));
                }
                else
                {
                    player.Reply(lang.GetMessage("ErrorBattleMetricsID", this, player.Id));
                }
            });
        }

        private void GetPlayerIdFromSteamId(string steamId, Action<string> callback)
        {
            var url = $"https://api.battlemetrics.com/players?filter[search]={steamId}";

            webrequest.Enqueue(url, null, (code, response) =>
            {
                if (code != 200 || response == null)
                {
                    callback(null);
                    return;
                }

                try
                {
                    var jsonResponse = JObject.Parse(response);
                    var playerId = jsonResponse["data"][0]["id"].ToString();
                    callback(playerId);
                }
                catch (Exception)
                {
                    callback(null);
                }
            }, this, RequestMethod.GET, new Dictionary<string, string> { { "Authorization", $"Bearer {apiKey}" } });
        }

        private void QueryBattleMetrics(string playerId, Action<string> callback)
        {
            var url = $"https://api.battlemetrics.com/players/{playerId}/servers/{serverId}";

            webrequest.Enqueue(url, null, (code, response) =>
            {
                if (code != 200 || response == null)
                {
                    callback(string.Format(lang.GetMessage("ErrorPlaytimeRetrieval", this), code));
                    return;
                }

                try
                {
                    var playtime = ParsePlaytimeFromResponse(response);
                    callback(string.Format(lang.GetMessage("PlaytimeMessage", this), serverName, playtime));
                }
                catch (Exception)
                {
                    callback(lang.GetMessage("ErrorProcessingData", this));
                }
            }, this, RequestMethod.GET, new Dictionary<string, string> { { "Authorization", $"Bearer {apiKey}" } });
        }

        private string ParsePlaytimeFromResponse(string response)
        {
            try
            {
                var jsonResponse = JObject.Parse(response);

                if (jsonResponse["data"] == null || jsonResponse["data"]["attributes"] == null)
                {
                    return "Error: No playtime data available.";
                }

                var playtime = jsonResponse["data"]["attributes"]["timePlayed"].ToObject<int>();
                var playtimeHours = playtime / 3600; // Convert seconds to hours
                return playtimeHours.ToString();
            }
            catch (Exception)
            {
                return "Error parsing playtime.";
            }
        }
    }
}
