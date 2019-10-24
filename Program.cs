using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LPRankSyncBot {
    class Program {
        private static void Main (string[] args) => new Program ().MainAsync ().GetAwaiter ().GetResult ();

        private async Task MainAsync () {
            Util.Log ("Set BaseDirectory");
            GlobalVariables.BaseDirectory = System.IO.Path.GetDirectoryName (Process.GetCurrentProcess ().MainModule.FileName); // Finds and saves the Directory, this program is running in 
            SettingsControl.loadSettings (); //Load all the settings like bot token etc.
            await Discord.Discord.DiscordAsync (); //connect to Discords API 
        }

        public static void DiscordReady () {
            SettingsControl.LoadRoleDict (); // Load the role Dictonary
            SettingsControl.LoadUserDict ();
            SyncAll();
        }

        public static async Task SyncAll () {
            while (true) {
                foreach (var entry in GlobalVariables.UserDict) {
                    Task.Run (() => Sync (entry.Key, entry.Value));
                }
                await Task.Delay (600000);
            }
        }

        public static void Sync (ulong DiscordID, string MinecraftUUID) {
            foreach (var Rank in SettingsControl.GetUsersLPRanks (MinecraftUUID)) {
                foreach (var entry in GlobalVariables.RoleDict) {
                    if (Rank == entry.Value) {
                        Discord.Discord.GiveRole (DiscordID, entry.Key);
                    }
                }
            }
        }
    }
}