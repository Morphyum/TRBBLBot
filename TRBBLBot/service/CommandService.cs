using Discord;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TRBBLBot.entity;

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
            var comp = (string)command.Data.Options.ToList().First().Value;
            var filtered = scheduleService.filterCurrentSeason(await scheduleService.getSchedulesAsync(comp));

            var newestRound = scheduleService.getNewestRound(filtered);
            var longestHomeCoach = 0;
            var longestHomeTeam = 0;
            var longestAwayCoach = 0;
            var longestAwayTeam = 0;

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
                    sb.AppendLine(String.Format("{0," + longestHomeCoach + "}  {1," + longestHomeTeam + "}  {2,1} vs {3,1}  {4," + longestAwayTeam + "}  {5," + longestAwayCoach + "}", schedule.CoachNameHome, schedule.TeamNameHome, schedule.ScoreHome, schedule.ScoreAway, schedule.TeamNameAway, schedule.CoachNameAway));
                }
            }
            return Format.Code(sb.ToString());
        }

        public async Task<string> handleStandings(SocketSlashCommand command) {
            var standings = new Standings();
            var comp = (string)command.Data.Options.ToList().First().Value;
            var filtered = scheduleService.filterCurrentSeason(await scheduleService.getSchedulesAsync(comp));

            foreach(var schedule in filtered) {
                var homeTeam = standings.Teams.FirstOrDefault(t => t.Name.Equals(schedule.TeamNameHome));
                var awayTeam = standings.Teams.FirstOrDefault(t => t.Name.Equals(schedule.TeamNameAway));
                if(homeTeam == null) {
                    homeTeam = standings.CreateTeam(schedule.TeamNameHome, schedule.CoachNameHome);
                }
                if(awayTeam == null) {
                    awayTeam = standings.CreateTeam(schedule.TeamNameAway, schedule.CoachNameAway);
                }
                if(schedule.ScoreHome.Length > 0) {
                    var matchHomeScore = int.Parse(schedule.ScoreHome);
                    var matchAwayScore = int.Parse(schedule.ScoreAway);
                    homeTeam.Score += matchHomeScore;
                    awayTeam.Score += matchAwayScore;
                    if(matchHomeScore > matchAwayScore) {
                        homeTeam.Wins++;
                        awayTeam.Loses++;
                    } else if(matchHomeScore == matchAwayScore) {
                        homeTeam.Draws++;
                        awayTeam.Draws++;
                    } else if(matchHomeScore < matchAwayScore) {
                        homeTeam.Loses++;
                        awayTeam.Wins++;
                    }
                }
            }
            standings.Teams = standings.Teams.OrderByDescending(t => t.Points).ThenByDescending(t => t.TDD).ToList();

            var longestCoach = "Coach".Length;
            var longestTeam = "Team".Length;
            foreach(var team in standings.Teams) {
                if(team.Coach.Length > longestCoach) {
                    longestCoach = team.Coach.Length;
                }
                if(team.Name.Length > longestTeam) {
                    longestTeam = team.Name.Length;
                }
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Standings - " + comp + " - Day " + scheduleService.getNewestRound(filtered));
            sb.AppendLine();
            sb.AppendLine(String.Format("{0,5} | {1," + longestTeam + "} | {2," + longestCoach + "} | {3,6} | {4,5} | {5,4} | {6,5} | {7,6} | {8,3}", "Place", "Team", "Coach", "Points", "Games", "Wins", "Draws", "Losses", "TDD"));
            sb.AppendLine();
            int place = 1;
            foreach(var team in standings.Teams) {
                sb.AppendLine(String.Format("{0,5} | {1," + longestTeam + "} | {2," + longestCoach + "} | {3,6} | {4,5} | {5,4} | {6,5} | {7,6} | {8,3}", place, team.Name, team.Coach, team.Points, team.Games, team.Wins, team.Draws, team.Loses, team.TDD));
                place++;
            }
            return Format.Code(sb.ToString());
        }

    }
}
