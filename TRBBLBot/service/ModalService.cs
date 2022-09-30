using Discord;
using Discord.WebSocket;
using System.Linq;

namespace TRBBLBot.service {
    public class ModalService {
        private WelcomeService welcomeService;

        public WelcomeService WelcomeService {
            get => welcomeService;
            set => welcomeService = value;
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
    }
}
