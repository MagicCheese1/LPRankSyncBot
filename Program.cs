using System;
using System.Diagnostics;
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
            Util.Log ("Set BaseDirectory");
            GlobalVariables.BaseDirectory = System.IO.Path.GetDirectoryName (Process.GetCurrentProcess ().MainModule.FileName); // Finds and saves the Directory, this program is running in 
            SettingsControl.loadSettings (); //Load all the settings like bot token etc.
            SettingsControl.LoadRoleDict (); // Load the role Dictonary
        }
    }
}