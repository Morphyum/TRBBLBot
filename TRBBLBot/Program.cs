using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using TRBBLBot.service;

namespace PPBot {
    public class Program {
        private DiscordSocketClient client;
        private CommandService commandService = new CommandService();
        private WelcomeService welcomeService = new WelcomeService();
        private ModalService modalService = new ModalService();
        
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync() {
            modalService.WelcomeService = welcomeService;
            var config = new DiscordSocketConfig() {
                GatewayIntents = GatewayIntents.All
            };
            client = new DiscordSocketClient(config);
            welcomeService.LoadWelcome();
            client.Ready += Ready;
            client.Log += Log;
            client.UserJoined += UserJoined;
            client.SlashCommandExecuted += SlashCommandExecuted;
            client.ModalSubmitted += ModalSubmitted;
            var token = Secret.token; // Remember to keep this private!
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        public async Task Ready() {
            await client.BulkOverwriteGlobalApplicationCommandsAsync(commandService.createCommands());
        }

        private async Task ModalSubmitted(SocketModal modal) {
            if(modal.Data.CustomId.Equals("welcome_message_modal")) {
                await modal.RespondAsync(embed: modalService.handleSetWelcomeMessage(modal));
            }

        }
        private async Task SlashCommandExecuted(SocketSlashCommand command) {
            switch(command.CommandName) {
                case "set-welcome":
                    await command.RespondWithModalAsync(commandService.handleSetWelcome());
                    break;
                case "schedule":
                    await command.RespondAsync(await commandService.handleScheduleAsync(command));
                    break;
                case "standings":
                    await command.RespondAsync(await commandService.handleStandings(command));
                    break;
                default:
                    break;
            }
        }

        private async Task UserJoined(SocketGuildUser arg) {
            SocketTextChannel channel = (SocketTextChannel)client.GetChannel(473036518454329365);
            var role = (arg as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name.ToLower() == "admin");
            await channel.SendMessageAsync("Welcome " + arg.Mention + ",\n" + welcomeService.WelcomeText);
        }

        private Task Log(LogMessage msg) {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
