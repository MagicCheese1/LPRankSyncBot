using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LPRankSyncBot {
    public class Util {
        public static void Log (string Message, [CallerFilePath] string CallerFile = "", [CallerMemberName] string CallerMethod = "") { //Function to make Console Logs look better
            Console.WriteLine ($"{ DateTime.Now } at {CallerFile.Split('/').Last() + "/" + CallerMethod}] {Message}");
        }
    }
}