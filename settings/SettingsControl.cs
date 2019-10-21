using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using Newtonsoft.Json;

namespace LPRankSyncBot
{
    public class SettingsControl {
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
            Util.Log ("LPRSB/Properties.json loaded sucessfully!");
        }

        public static void LoadRoleDict()
        {
            Util.Log ("Searching for LPRSB/RoleDict.json");
            if (!File.Exists (GlobalVariables.BaseDirectory + "/LPRSB/RoleDict.json")) //Check if the RoleDict file exists if not generate one
                GenerateRoleDict ();
             string RoleDictContent = File.ReadAllText (GlobalVariables.BaseDirectory + "/LPRSB/RoleDict.json"); //Read Contents of RoleDict File
            if (String.IsNullOrWhiteSpace (RoleDictContent)) //If RoleDict file found but empty, generate one
                GenerateRoleDict ();
            Util.Log ("Loading LPRSB/RoleDict.json");
            GlobalVariables.RoleDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(RoleDictContent);
        }

        private static void GenerateRoleDict()
        {
            Dictionary<string, string> RoleDict = new Dictionary<string, string>();
            Util.Log ("LPRSB/RoleDict.json not found, generating");
            ReadLPDB();
        }

        private static void ReadLPDB()
        {
            // switch("SQLITE")
            switch(GlobalVariables.DatabaseType.ToUpper())
            {
                case "SQLITE":
                    using(var connection = new SQLiteConnection("data source=" + GlobalVariables.BaseDirectory + "/luckperms/luckperms-sqlite.db;mode=ReadOnly"))
                    {
                        using(var command = new SQLiteCommand(connection))
                        {
                            connection.Open();

                            command.CommandText = "Select * From luckperms_groups";

                            using(var reader = command.ExecuteReader())
                            {
                                while(reader.Read())
                                {
                                    GlobalVariables.LPRanks.Add(reader["name"].ToString());
                                    
                                }
                            }
                        }   
                    }
                    Util.Log("LP ranks found");
                    foreach(var rank in GlobalVariables.LPRanks)
                    {
                        Console.Write(rank+", ");
                    }
                    break;
                default:
                    throw new Exception("The current Luckperms database is either invalid or not yet supported!");
            }
        }
        private static void GenerateProperties () {
            ESettings settings = new ESettings ();
            Util.Log ("LPRSB/Properties.json not found, Generating");
            Util.Log ("Creating Directory");
            Directory.CreateDirectory(GlobalVariables.BaseDirectory + "/LPRSB/");
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
                if (!Int64.TryParse (input, out settings.UsernameChannel)) {
                    Util.Log ("Invalid ID, please try again!", "Input", String.Empty);
                } else
                    break;
            }
            Util.Log ("LPRSB/Propertis.json generated Sucessfully!");
            string JSON = JsonConvert.SerializeObject (settings); // convert object to JSON
            System.IO.File.WriteAllText (GlobalVariables.BaseDirectory + "/LPRSB/Properties.json", JSON); // Create/Write JSON to Properties File
        }

        public static string GetDatabaseType (string LPConfFile) {
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