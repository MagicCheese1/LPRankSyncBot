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
            SyncAll ();
        }

        public static async Task SyncAll () {
            while (true) {
                Util.Log ("AutoSync Started");
                foreach (var entry in GlobalVariables.UserDict) {
                    Sync (entry.Key, entry.Value);
                }
                await Task.Delay (60000 * GlobalVariables.SyncDelayMin);
            }
        }

        public static void Sync (ulong DiscordID, string MinecraftUUID) {
            Util.Log ($"Syncing {DiscordID} with {MinecraftUUID}");
            foreach (var Rank in SettingsControl.GetUsersLPRanks (MinecraftUUID)) {
                foreach (var entry in GlobalVariables.RoleDict) {
                    if (Rank == entry.Key) {
                        Discord.Discord.GiveRole (DiscordID, entry.Value);
                    }
                }
            }
            foreach (var Role in Discord.Discord.GetUserDCRoles (DiscordID)) {
                foreach (var entry in GlobalVariables.RoleDict) {
                    if(Role.Id == entry.Value)
                    {
                        if(!SettingsControl.GetUsersLPRanks(MinecraftUUID).Contains(entry.Key))
                        {
                            Discord.Discord.RemoveRole(DiscordID, entry.Value);
                        }
                    }
                }
            }
        }
    }
}