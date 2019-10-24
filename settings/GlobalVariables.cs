using System;
using System.Collections.Generic;

namespace LPRankSyncBot {
    public static class GlobalVariables {
        public static string BaseDirectory;
        public static string DatabaseType;
        public static ulong UsernameChannel;
        public static string Token;
        public static List<string> LPRanks = new List<string> ();
        public static List<string> DCRanks = new List<string> ();
        public static Dictionary<string, string> RoleDict = new Dictionary<string, string> ();
        public static Dictionary<ulong, String> UserDict = new Dictionary<ulong, string> ();
    }
}