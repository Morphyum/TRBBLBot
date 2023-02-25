using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using TRBBLBot.repository;

namespace TRBBLBot.service {
    public class ModalService {
        private WelcomeService welcomeService;
        private IFixMatchRepository fixMatchRepository;
        public WelcomeService WelcomeService {
            get => welcomeService;
            set => welcomeService = value;
        }
        internal IFixMatchRepository FixMatchRepository {
            get => fixMatchRepository;
            set => fixMatchRepository = value;
        }

        public Embed handleSetWelcomeMessage(SocketModal modal) {

            WelcomeService.WelcomeText = modal.Data.Components.ToList().First(x => x.CustomId == "welcome_message_text").Value;
            WelcomeService.SaveWelcome();
            var embeded = new EmbedBuilder()
                .WithTitle("New Welcome Message was Set")
                .WithDescription(WelcomeService.WelcomeText)
                .WithAuthor(modal.User)
                .WithCurrentTimestamp();
            return embeded.Build();
        }

        internal string handleFixMatch(SocketModal modal) {
          /*  .AddTextInput("ScheduleId(DO NOT CHANGE)", "schedule_id", TextInputStyle.Short, "new score", 0, 99, false, match.ScheduleOriginId)
                     .AddTextInput(match.TeamNameHome, "home_score", TextInputStyle.Short, "new score", 1, 2, true)
                     .AddTextInput(match.TeamNameAway, "away_score", TextInputStyle.Short, "new score", 1, 2, true);*/


            FixMatchRepository.AddFixMatch();
        }
    }
}
