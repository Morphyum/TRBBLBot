using Discord;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PPBot
{
    public class Program
    {
        private static System.Timers.Timer ppTimer;
        private static DiscordSocketClient client = new DiscordSocketClient();
        private static SocketGuild server = null;
        private static SocketTextChannel channel = null;
        private static Dictionary<string, string> players = new Dictionary<string, string>();
        private static string APIURL = "https://new.scoresaber.com/api/";
        private static string BASEURL = "https://scoresaber.com/";
        private static int UPDATEMINUTES = 3;
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync() {
            LoadRegisteredPlayers();
            ppTimer = new System.Timers.Timer(1000 * 60 * UPDATEMINUTES);
            ppTimer.Elapsed += Test;
            ppTimer.Enabled = true;
            client.Log += Log;
            client.MessageReceived += MessageReceived;
            string token = Secret.token; // Remember to keep this private!
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();


            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task MessageReceived(SocketMessage message) {
            foreach (var i in client.Guilds) {
                if (i.Id == 736932523699077162) {
                    server = i;
                }
            }
            if (server != null) {
                channel = server.GetTextChannel(766215958213296148);
            }

            if (message.Channel.Id == 766215958213296148) {
                if (message.Content.StartsWith("!register")) {
                    var user = message.Content.Substring(10);
                    string html = string.Empty;
                    string url = APIURL + "players/by-name/" + user;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ServerCertificateValidationCallback = (sender, certificate, chain, policyErrors) => { return true; };
                    request.AutomaticDecompression = DecompressionMethods.GZip;

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream)) {
                        html = reader.ReadToEnd();
                    }
                    JObject a = JObject.Parse(html);
                    JArray jPlayers = (JArray)a["players"];
                    foreach (JObject jPlayer in jPlayers) {
                        if ((string)jPlayer["playerName"] == user) {
                            if (!players.ContainsKey(user)) {
                                players.Add(user, (string)jPlayer["playerId"]);
                                await channel.SendMessageAsync(BASEURL + "u/" + (string)jPlayer["playerId"] + " was added to the watchlist");
                                SaveRegisteredPlayers();
                                break;
                            }
                            else {
                                await channel.SendMessageAsync(BASEURL + "u/" + (string)jPlayer["playerId"] + " already is registered");
                            }
                        }
                    }
                }
            }
        }

        private static void Test(Object source, System.Timers.ElapsedEventArgs e) {
            foreach (var i in client.Guilds) {
                if (i.Id == 736932523699077162) {
                    server = i;
                }
            }
            if (server != null) {
                channel = server.GetTextChannel(766215958213296148);
            }

            foreach (KeyValuePair<string, string> player in players) {
                string html = string.Empty;
                string url = APIURL + "player/" + player.Value + "/scores/recent/1";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ServerCertificateValidationCallback = (sender, certificate, chain, policyErrors) => { return true; };
                request.AutomaticDecompression = DecompressionMethods.GZip;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream)) {
                    html = reader.ReadToEnd();
                }
                JObject a = JObject.Parse(html);
                DateTime timeSet = (DateTime)a["scores"][0]["timeSet"];
                timeSet = timeSet.ToLocalTime();
                if (timeSet.AddMinutes(UPDATEMINUTES) > DateTime.Now && (float)a["scores"][0]["pp"] > 0) {
                    channel.SendMessageAsync(BASEURL + "u/" + player.Value + " just set a new personal best on " + BASEURL + "leaderboard/" + a["scores"][0]["leaderboardId"] +
                        "\nEffective Worth: " + (float)a["scores"][0]["pp"] * (float)a["scores"][0]["weight"] + "pp");
                }
            }
        }

        private Task Log(LogMessage msg) {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
        private void LoadRegisteredPlayers() {
            string load = System.IO.File.ReadAllText("Saves/Players.sav");
            players = ((JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(load)).ToObject<Dictionary<string, string>>();
            if (players == null) {
                players = new Dictionary<string, string>();
            }
        }

        private bool SaveRegisteredPlayers() {
            System.IO.File.WriteAllText("Saves/Players.sav", Newtonsoft.Json.JsonConvert.SerializeObject(players));
            return true;
        }
    }
}
