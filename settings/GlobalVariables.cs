using System;
using System.Collections.Generic;

namespace LPRankSyncBot {
    public static class GlobalVariables {
        public static string BaseDirectory;
        public static string DatabaseType;
        public static ulong UsernameChannel;
        public static string Token;
        public static int SyncDelayMin;
        public static List<string> LPRanks = new List<string> ();
        public static List<ulong> DCRanks = new List<ulong> ();
        public static Dictionary<string, ulong> RoleDict = new Dictionary<string, ulong> ();
        public static Dictionary<ulong, String> UserDict = new Dictionary<ulong, string> ();
    }
}