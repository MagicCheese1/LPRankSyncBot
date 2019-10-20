using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LPRankSyncBot {
    public class Util {

        public static void Log (string Message, [CallerFilePath] string Location = "") { { Console.WriteLine ($"{ DateTime.Now } at {Location.Split('/').Last()}] {Message}"); }
        }
    }
}