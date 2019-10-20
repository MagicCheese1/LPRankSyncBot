using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;

namespace LPRankSyncBot {
    class Program {
        public static DiscordSocketClient Client;
        public static CommandService Commands;

        private static void Main (string[] args) => new Program ().MainAsync ().GetAwaiter ().GetResult ();

        private async Task MainAsync () {
            SettingsControl.load ();
        }
    }
}