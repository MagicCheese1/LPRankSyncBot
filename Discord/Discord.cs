using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace LPRankSyncBot.Discord {
    public class Discord {
        private static DiscordSocketClient Client;
        public static async Task DiscordAsync () {
            Client = new DiscordSocketClient (new DiscordSocketConfig {
                LogLevel = LogSeverity.Debug
            });

            Client.Ready += Client_Ready;
            Client.Log += Client_Log;
            Client.MessageReceived += Client_MessageReceived;
            await Client.LoginAsync (TokenType.Bot, GlobalVariables.Token);
            await Client.StartAsync ();
            await Task.Delay (-1);
        }

        private static async Task Client_Ready () {
            Task.Run (() => Program.DiscordReady ());
        }

        private static async Task Client_Log (LogMessage message) {
            Util.Log (message.Message, message.Source, "Discord");
        }

        private static async Task Client_MessageReceived (SocketMessage message) {
            try {
                var Message = message as SocketUserMessage;
                var Context = new SocketCommandContext (Client, Message);

                if (Context.Message.Channel.Id != GlobalVariables.UsernameChannel)
                    return;
                if (SettingsControl.TryAddUser (Context.Message.Author.Id, Context.Message.Content))
                    await Context.Message.AddReactionAsync (new Emoji ("✔️"));
                else
                    await Context.Message.AddReactionAsync (new Emoji ("❌"));
            } catch (Exception e) {
                System.Console.WriteLine(e.Message);
            }
        }
        public static void GetRoles () {
            foreach (var role in Client.Guilds.FirstOrDefault ().Roles) {
                GlobalVariables.DCRanks.Add (role.Name);
            }
        }

        public static void GiveRole (ulong DiscordID, string role) {
            Client.Guilds.FirstOrDefault ().GetUser (DiscordID).AddRoleAsync (Client.Guilds.FirstOrDefault ().Roles.FirstOrDefault (r => r.Name == role));

        }
    }
}