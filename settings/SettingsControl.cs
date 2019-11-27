using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Discord.WebSocket;

namespace LPRankSyncBot {
    public class SettingsControl {

        private static string lpdbDataSource = "data source=" + GlobalVariables.BaseDirectory + "/luckperms/luckperms-sqlite.db";
        private static string userdictdbDataSource = "data source=" + GlobalVariables.BaseDirectory + "/LPRSB/UserDict.db";
        public static void loadSettings () {
            Util.Log ("Searching for LPRSB/Properties.json");
            if (!File.Exists (GlobalVariables.BaseDirectory + "/LPRSB/Properties.json")) //Check if the Properties file exists if not generate one
                GenerateProperties ();
            string PropertiesContent = File.ReadAllText (GlobalVariables.BaseDirectory + "/LPRSB/Properties.json"); //Read Contents of Properties File
            if (String.IsNullOrWhiteSpace (PropertiesContent)) //If properties file empty generate one
                GenerateProperties ();
            ESettings settings = JsonConvert.DeserializeObject<ESettings> (PropertiesContent); //JSON file to object
            Util.Log ("Loading LPRSB/Properties.json");
            GlobalVariables.DatabaseType = settings.DatabaseType; //Load all the Properties into GlobalVariables class
            GlobalVariables.UsernameChannel = settings.UsernameChannel;
            GlobalVariables.Token = settings.Token;
            GlobalVariables.SyncDelayMin = settings.SyncDelayMin;
            Util.Log ("LPRSB/Properties.json loaded sucessfully!");
        }

        public static void LoadRoleDict () {
            Util.Log ("Searching for LPRSB/RoleDict.json");
            if (!File.Exists (GlobalVariables.BaseDirectory + "/LPRSB/RoleDict.json")) //Check if the RoleDict file exists if not generate one
                GenerateRoleDict ();
            string RoleDictContent = File.ReadAllText (GlobalVariables.BaseDirectory + "/LPRSB/RoleDict.json"); //Read Contents of RoleDict File
            if (String.IsNullOrWhiteSpace (RoleDictContent)) //If RoleDict file found but empty, generate one
                GenerateRoleDict ();
            Util.Log ("Loading LPRSB/RoleDict.json");
            GlobalVariables.RoleDict = JsonConvert.DeserializeObject<Dictionary<string, ulong>> (RoleDictContent);
            Util.Log ("LPRSB/RoleDict.json Loaded Sucessfully!");
        }

        public static void LoadUserDict () {
            Util.Log ("Loading UserDict database");
            if (!File.Exists (GlobalVariables.BaseDirectory + "/LPRSB/UserDict.db")) //Check if the UserDict database exists if not generate one
                SQLiteConnection.CreateFile (GlobalVariables.BaseDirectory + "/LPRSB/UserDict.db");
            using (var connection = new SQLiteConnection (userdictdbDataSource)) {
                using (var command = new SQLiteCommand (connection)) {
                    Util.Log ("opening connection");
                    connection.Open ();
                    command.CommandText = @"CREATE TABLE IF NOT EXISTS [USERDICT] (
                                               [DCID] INTEGER  NULL,
                                               [MCUUID] TEXT(36)  NULL
                                               )";
                    Util.Log ("Creating table if not present");
                    command.ExecuteNonQuery ();

                    command.CommandText = "Select * FROM USERDICT";
                    Util.Log ("Reading UserDict");
                    using (var reader = command.ExecuteReader ()) {
                        while (reader.Read ()) {
                            GlobalVariables.UserDict.Add (UInt64.Parse (reader["DCID"].ToString ()), reader["MCUUID"].ToString ());
                        }
                    }
                    Util.Log ("UserDict Database Loaded sucessfully!");
                }
                connection.Close ();
            }
        }

        public static bool TryAddUser (ulong DiscordID, string MinecraftUsername) {
            Util.Log("Trying to add user to Db");
            string UUID = GetUUID (MinecraftUsername);
            if (String.IsNullOrWhiteSpace (UUID))
               return false;
            if(!GlobalVariables.UserDict.TryAdd (DiscordID, UUID))
                return false;
            using (var connection = new SQLiteConnection (userdictdbDataSource)) {
                using (var command = new SQLiteCommand (connection)) {
                    Util.Log ("opening Connection");
                    connection.Open ();
                    command.CommandText = $"INSERT INTO USERDICT (DCID,MCUUID) VALUES ('{DiscordID}','{UUID}')";
                    command.ExecuteNonQuery ();
                    Util.Log("User added sucessfully");
                    Program.Sync (DiscordID, UUID);
                
                }
                connection.Close();
            }
            return true;
        }

        public static List<string> GetUsersLPRanks (String MinecraftUUID) {
            List<string> Ranks = new List<string> ();
            using (var connection = new SQLiteConnection (lpdbDataSource)) {
                using (var command = new SQLiteCommand (connection)) {
                    connection.Open ();
                    command.CommandText = "SELECT uuid, permission FROM luckperms_user_permissions";
                    using (var reader = command.ExecuteReader ()) {
                        while (reader.Read ()) {
                            if (reader["uuid"].ToString () != MinecraftUUID)
                                continue;
                            if (!reader["permission"].ToString ().StartsWith ("group."))
                                continue;
                            Ranks.Add (reader["permission"].ToString ().Replace ("group.", String.Empty));
                        }
                    }
                    connection.Close();
                }
            }
            return Ranks;
        }
        private static string GetUUID (String MinecraftUsername) {
            string str = null;
            using (var connection = new SQLiteConnection (lpdbDataSource)) {
                using (var command = new SQLiteCommand (connection)) {
                    connection.Open ();
                    command.CommandText = "SELECT uuid, username FROM luckperms_players";
                    using (var reader = command.ExecuteReader ()) {
                        while (reader.Read ()) {
                            if (reader["username"].ToString () != MinecraftUsername)
                                continue;
                            str = reader["uuid"].ToString ();
                        }

                    }
                    connection.Close ();
                    return str;
                }
            }
        }

        private static void GenerateRoleDict () {
            Dictionary<string, ulong> RoleDict = new Dictionary<string, ulong> ();
            Util.Log ("LPRSB/RoleDict.json not found, generating");
            GetLPRanks ();
            Discord.Discord.GetRoles ();
            foreach (var rank in GlobalVariables.LPRanks) {
                foreach (var roleId in GlobalVariables.DCRanks) {
                    SocketRole role = Discord.Discord.GetRole(roleId);
                    if (rank.ToUpper ().Replace (" ", "") == role.Name.ToUpper ().Replace (" ", "") || rank.ToUpper ().Replace (" ", "").Contains(role.Name.ToUpper ().Replace (" ", "")) || role.Name.ToUpper ().Replace (" ", "").Contains(rank.ToUpper ().Replace (" ", ""))) {
                        Util.Log ($"Do you want to synchronize \"{rank}\" with \"{role.Name}\" Y/N", "Input", String.Empty);
                        if (Console.ReadLine () == "Y") {
                            RoleDict.Add (rank, roleId);
                            Util.Log ($"{rank} : {role.Name} added to RoleDict");
                        }
                    }
                }
            }
            Util.Log ("If your ranks weren't found automatically, add them Manually in LPRSB/RoleDict.json");
            string JSON = JsonConvert.SerializeObject (RoleDict, Formatting.Indented);
            File.WriteAllText (GlobalVariables.BaseDirectory + "/LPRSB/RoleDict.json", JSON);
        }

        private static void GetLPRanks () {
            // switch("SQLITE")
            switch (GlobalVariables.DatabaseType.ToUpper ()) {
                case "SQLITE":
                    using (var connection = new SQLiteConnection (lpdbDataSource)) {
                        using (var command = new SQLiteCommand (connection)) {
                            connection.Open ();

                            command.CommandText = "Select * From luckperms_groups";

                            using (var reader = command.ExecuteReader ()) {
                                while (reader.Read ()) {
                                    GlobalVariables.LPRanks.Add (reader["name"].ToString ());

                                }
                            }
                            connection.Close ();
                        }
                    }
                    break;
                default:
                    throw new Exception ("The current Luckperms database is either invalid or not yet supported!");
            }
        }
        private static void GenerateProperties () {
            ESettings settings = new ESettings ();
            Util.Log ("LPRSB/Properties.json not found, Generating");
            Util.Log ("Creating Directory");
            Directory.CreateDirectory (GlobalVariables.BaseDirectory + "/LPRSB/");
            Util.Log ("Searching for luckperms.conf");
            string LPConfFile = Directory.GetFiles (GlobalVariables.BaseDirectory, "luckperms.conf", SearchOption.AllDirectories).FirstOrDefault (); //Search for luckperms.conf in all sub-directories
            if (String.IsNullOrWhiteSpace (LPConfFile)) //If luckperms.conf not found throw exception
                throw new System.Exception ("luckperms.conf not found, make sure this program is in your sponge servers directory and that luckyperms is installed properly!");
            Util.Log ($"Found luckperms.conf at {LPConfFile}");
            Util.Log ("Searching for Database Type");
            settings.DatabaseType = GetDatabaseType (LPConfFile); // find the Database type in the luckperms.conf file
            Util.Log ($"Database type found: {settings.DatabaseType}");
            Util.Log ($"Please Enter Bot Token:", "Input", String.Empty);
            string pass = String.Empty;
            do { //get bot token from user, hide input
                ConsoleKeyInfo key = Console.ReadKey (true);
                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter) {
                    pass += key.KeyChar;
                    Console.Write ("*");
                } else {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0) {
                        pass = pass.Substring (0, (pass.Length - 1));
                        Console.Write ("\b \b");
                    } else if (key.Key == ConsoleKey.Enter) {
                        break;
                    }
                }

            } while (true);
            Console.WriteLine ();
            settings.Token = pass; // save user input token to settings object
            while (true) { // get the ID of the Username Input Channel from user
                Util.Log ($"Please Enter Username Input Channel ID:", "Input", String.Empty);
                string input = Console.ReadLine ();
                if (!UInt64.TryParse (input, out settings.UsernameChannel)) {
                    Util.Log ("Invalid ID, please try again!", "Input", String.Empty);
                } else
                    break;
            }
            Util.Log("Sync Delay(Min):");
            settings.SyncDelayMin = Console.Read();
            Util.Log ("LPRSB/Propertis.json generated Sucessfully!");
            string JSON = JsonConvert.SerializeObject (settings); // convert object to JSON
            System.IO.File.WriteAllText (GlobalVariables.BaseDirectory + "/LPRSB/Properties.json", JSON); // Create/Write JSON to Properties File
        }

        private static string GetDatabaseType (string LPConfFile) {
            string result = string.Empty;
            var lines = File.ReadAllLines (LPConfFile);
            foreach (var line in lines) { // look for a line that contains "storage-method" and doesn't contain "#"
                if (line.Contains ("storage-method") && !line.Contains ("#")) {
                    var text = line.Replace (" ", String.Empty); // remove all spaces
                    text = text.Replace ("storage-method=", String.Empty); // remove "storage-method=" from the string
                    text = text.Replace ("\"", String.Empty); //remove all '"' from the string
                    result = text.Trim (); //trim string and save to result
                }
            }
            if (String.IsNullOrEmpty (result)) //if result is empty, throw exception
                throw new System.Exception ("Database type not found, your luckperms configuration file (luckperms.conf) maybe broken");
            return result;
        }

    }
}