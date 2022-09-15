﻿using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPBot {
    public class Program {
        private string welcomeText = "First things firsts, please make sure you check out the<#572155952078258228> and the pinned messages in <#473044083330514944>. These will hopefully let you work out if it's the league for you. We're just heading into S7 and sorting out leagues at the moment.  If you're all happy with the rules etc... then you can find the sign up sheet in both <#473733975509041163> and pinned in <#753677775591702590>." +
                    "\nAs well as the league we also run side competitions.The main one is the perpetual ladder which is great for finding random games.They're just for fun but do allow you to play more between league games. Help on joining the ladder can be found at <#479662400640122890>. " +
                    "\nOther than that just look around and get involved.We've got plenty of opportunity to chat outside of just scheduling games so please feel free to get involved.If you've got any questions feel free to ask but make sure you include the <@&473036877524369459> tag so we know the question is there." +
                    "\n\nAlso check out the new coach primer pinned in this channel.https://docs.google.com/document/d/1";
        private DiscordSocketClient client;
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync() {
            var config = new DiscordSocketConfig() {
                GatewayIntents = GatewayIntents.All
            };
            client = new DiscordSocketClient(config);
            LoadWelcome();
            client.Ready += Ready;
            client.Log += Log;
            client.UserJoined += UserJoined;
            client.MessageReceived += MessageReceived;
            client.SlashCommandExecuted += SlashCommandExecuted;
            client.ModalSubmitted += ModalSubmitted;
            var token = Secret.token; // Remember to keep this private!
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        public async Task Ready() {
            List<ApplicationCommandProperties> applicationCommandProperties = new List<ApplicationCommandProperties>();
            var globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("set-welcome");
            globalCommand.WithDescription("Change the welcome message");
            var command = globalCommand.Build();
            applicationCommandProperties.Add(command);

            await client.BulkOverwriteGlobalApplicationCommandsAsync(applicationCommandProperties.ToArray());
        }
        private async Task MessageReceived(SocketMessage arg) {
            //Normal command go here

            //Admin commands go here
            if(isAdmin(arg.Author as IGuildUser)) {
                if(arg.Content.StartsWith("!setwelcome ")) {
                    welcomeText = arg.Content.Replace("!setwelcome ", "");
                    SaveWelcome();
                    await arg.Channel.SendMessageAsync(welcomeText);

                }
            }
        }
        private async Task ModalSubmitted(SocketModal modal) {
            if(modal.Data.CustomId.Equals("welcome_message_modal")) {
                welcomeText = modal.Data.Components.ToList().First(x => x.CustomId == "welcome_message_text").Value;
                SaveWelcome();
                var embeded = new EmbedBuilder()
                    .WithTitle("New Welcome Message was Set")
                    .WithDescription(welcomeText)
                    .WithAuthor(modal.User)
                    .WithCurrentTimestamp();

                await modal.RespondAsync(embed: embeded.Build());
            }

        }
        private async Task SlashCommandExecuted(SocketSlashCommand command) {
            if(command.CommandName.Equals("set-welcome")) {
                var mb = new ModalBuilder()
                    .WithTitle("Welcome Message")
                    .WithCustomId("welcome_message_modal")
                    .AddTextInput("New Welcome Message", "welcome_message_text", TextInputStyle.Paragraph);
                await command.RespondWithModalAsync(mb.Build());
            }

        }

        private async Task UserJoined(SocketGuildUser arg) {
            SocketTextChannel channel = (SocketTextChannel)client.GetChannel(473036518454329365);
            var role = (arg as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name.ToLower() == "admin");
            await channel.SendMessageAsync("Welcome " + arg.Mention + ",\n" + welcomeText);
        }

        private Task Log(LogMessage msg) {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private bool isAdmin(IGuildUser user) {
            if(user.RoleIds.Any(r => r == 473036877524369459)) {
                return true;
            }
            return false;
        }

        private void LoadWelcome() {
            welcomeText = System.IO.File.ReadAllText("Saves/Welcome.txt");
        }

        private void SaveWelcome() {
            System.IO.File.WriteAllText("Saves/Welcome.txt", welcomeText);
        }
    }
}
