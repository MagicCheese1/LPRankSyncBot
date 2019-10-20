using System.Runtime.Serialization.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace LPRankSyncBot {
    public class SettingsControl {
        public static void load () {
            Util.Log ("Set BaseDirectory");
            GlobalSettings.BaseDirectory = System.IO.Path.GetDirectoryName (Process.GetCurrentProcess ().MainModule.FileName);
            Util.Log ("Searching for LPRSBProperties.json");
            if (!File.Exists (GlobalSettings.BaseDirectory + "/LPRSBProperties.json"))
                CreateProperties ();
            if (File.ReadAllText (GlobalSettings.BaseDirectory + "/LPRSBProperties.json").Trim () == String.Empty)
                CreateProperties ();
        }

        public static void CreateProperties () {
            ESettings settings = new ESettings ();
            Util.Log ("LPRSBProperties.json not found, Creating");
            Util.Log ("Searching for luckperms.conf");
            settings.LPConfFile = Directory.GetFiles (GlobalSettings.BaseDirectory, "luckperms.conf", SearchOption.AllDirectories).FirstOrDefault();
            if (String.IsNullOrWhiteSpace(settings.LPConfFile))
                throw new System.Exception ("luckperms.conf not found, make sure this program is in your sponge servers directory and that luckyperms is installed properly!");
            Util.Log ($"Found luckperms.conf at {settings.LPConfFile}");
            Util.Log ("Searching for Database Type");
            settings.DatabaseType = GetDatabaseType (settings.LPConfFile);
            Util.Log ($"Database type found: {settings.DatabaseType}");
            while (true) {
                Util.Log ($"Please Enter Username Input Channel ID:", "Input");
                string input = Console.ReadLine ();
                if (!Int64.TryParse (input, out settings.UsernameChannel)) {
                    Util.Log("Invalid ID, please try again!", "Input");
                }else
                    break;
            }
            Util.Log("Generating LPRSBProperties.json");
            string JSON = JsonConvert.SerializeObject(settings);
            System.IO.File.WriteAllText(GlobalSettings.BaseDirectory + "/LPRSBProperties.json", JSON);
        }

        public static string GetDatabaseType (string LPConfFile) {
            string result = string.Empty;
            var lines = File.ReadAllLines (LPConfFile);
            foreach (var line in lines) {
                if (line.Contains ("storage-method") && !line.Contains ("#")) {
                    var text = line.Replace (" ", String.Empty);
                    text = text.Replace ("storage-method=", String.Empty);
                    text = text.Replace ("\"", String.Empty);
                    result = text.Trim ();
                }
            }
            if (String.IsNullOrEmpty(result))
                throw new System.Exception ("Database type not found, your luckyperms.conf maybe broken");
            return result;
        }
    }
}