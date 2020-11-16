﻿using Discord;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PPBot
{
    public class Program
    {
        private static DiscordSocketClient client = new DiscordSocketClient();
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync() {
            client.Log += Log;
            client.UserJoined += UserJoined;
            string token = Secret.token; // Remember to keep this private!
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();


            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task UserJoined(SocketGuildUser arg) {
                SocketTextChannel channel = (SocketTextChannel) client.GetChannel(473036518454329365);
            var role = (arg as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name.ToLower() == "admin");
            await channel.SendMessageAsync( arg.Mention + "First things firsts, please make sure you check out the <#572155952078258228> and the pinned messages in <#473044083330514944>. These will hopefully let you work out if it's the league for you. We're just heading into S7 and sorting out leagues at the moment.  If you're all happy with the rules etc... then you can find the sign up sheet in both <#473733975509041163> and pinned in <#753677775591702590>." +
                    "\nAs well as the league we also run side competitions.The main one is the perpetual ladder which is great for finding random games.They're just for fun but do allow you to play more between league games. Help on joining the ladder can be found at <#479662400640122890>. " +
                    "\nOther than that just look around and get involved.We've got plenty of opportunity to chat outside of just scheduling games so please feel free to get involved.If you've got any questions feel free to ask but make sure you include the"+role.Mention+" tag so we know the question is there." +
                    "\n\nAlso check out the new coach primer pinned in this channel.https://docs.google.com/document/d/1");
        }

        private Task Log(LogMessage msg) {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
