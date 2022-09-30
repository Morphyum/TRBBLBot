using Discord;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TRBBLBot.service {
    public class CommandService {
        private ScheduleService scheduleService = new ScheduleService();

        public ApplicationCommandProperties[] createCommands() {
            List<ApplicationCommandProperties> applicationCommandProperties = new List<ApplicationCommandProperties>();

            var globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("set-welcome");
            globalCommand.WithDescription("Change the welcome message");
            var command = globalCommand.Build();
            applicationCommandProperties.Add(command);

            globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("schedule");
            globalCommand.WithDescription("Show the schedule for a competition");
            globalCommand.AddOption("competition", ApplicationCommandOptionType.String, "The name of the competition", true);
            command = globalCommand.Build();
            applicationCommandProperties.Add(command);

            globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("standings");
            globalCommand.WithDescription("Show the standings for a competition");
            globalCommand.AddOption("competition", ApplicationCommandOptionType.String, "The name of the competition", true);
            command = globalCommand.Build();
            applicationCommandProperties.Add(command);

            return applicationCommandProperties.ToArray();
        }

        internal Modal handleSetWelcome() {
            var mb = new ModalBuilder()
                    .WithTitle("Welcome Message")
                    .WithCustomId("welcome_message_modal")
                    .AddTextInput("New Welcome Message", "welcome_message_text", TextInputStyle.Paragraph);
            return mb.Build();
        }

        public async Task<string> handleScheduleAsync(SocketSlashCommand command) {
            var url = string.Format("https://www.mordrek.com:666/api/comp/id/schedule");
            var comp = (string)command.Data.Options.ToList().First().Value;
            url = url.Replace("id", comp);
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var jsonString = await client.GetStringAsync(url);
            var token = JToken.Parse(jsonString);
            var schedules = scheduleService.convertToSchedules(token);
            var filtered = scheduleService.filterCurrentSeason(schedules);

            var newestRound = 99999;
            var longestHomeCoach = 0;
            var longestHomeTeam = 0;
            var longestAwayCoach = 0;
            var longestAwayTeam = 0;

            foreach(var schedule in filtered) {
                if(schedule.ScoreHome.Length == 0) {
                    if(int.Parse(schedule.Round) < newestRound) {
                        newestRound = int.Parse(schedule.Round);
                    }
                }
            }
            foreach(var schedule in filtered) {
                if(int.Parse(schedule.Round) == newestRound) {
                    var homeCoach = schedule.CoachNameHome.Length;
                    var awayCoach = schedule.CoachNameAway.Length;
                    var homeTeam = schedule.TeamNameHome.Length;
                    var awayTeam = schedule.TeamNameAway.Length;
                    if(homeCoach > longestHomeCoach) {
                        longestHomeCoach = homeCoach;
                    }
                    if(homeTeam > longestHomeTeam) {
                        longestHomeTeam = homeTeam;
                    }
                    if(awayCoach > longestAwayCoach) {
                        longestAwayCoach = awayCoach;
                    }
                    if(awayTeam > longestAwayTeam) {
                        longestAwayTeam = awayTeam;
                    }
                }
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Schedule - " + comp + " - Day " + newestRound);
            sb.AppendLine();
            foreach(var schedule in filtered) {
                var scoreHome = schedule.ScoreHome;
                var rowRound = schedule.Round;
                if(int.Parse(rowRound) == newestRound) {
                    sb.AppendLine(String.Format("{0," + longestHomeCoach + "} {1," + longestHomeTeam + "} {2,1} vs {3,1} {4," + longestAwayTeam + "} {5," + longestAwayCoach + "}", schedule.CoachNameHome, schedule.TeamNameHome, schedule.ScoreHome, schedule.ScoreAway, schedule.TeamNameAway, schedule.CoachNameAway));
                }
            }
            return Format.Code(sb.ToString());
        }
    }

}
