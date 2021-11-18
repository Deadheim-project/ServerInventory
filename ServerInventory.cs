using BepInEx;
using HarmonyLib;
using System.Collections.Generic;

namespace ServerInventory
{
    [BepInPlugin("Detalhes.ServerInventory", "ServerInventory", "1.0.1")]
    public class ServerInventory : BaseUnityPlugin
    {
        public const string PluginGUID = "Detalhes.ServerInventory";
        Harmony harmony = new Harmony(PluginGUID);
        public static string Version = "1.2";
        public static List<ZRpc> validatedUsers = new List<ZRpc>();
        public static bool hasSpawned = false;
        public static bool hasServerInventory = false;
        public static bool isDead = false;
        public static bool isSynced = false;
        public static bool isMovingAll = false;
        public static bool isLoading = false;
        public static int lastSavedInventoryCount = 0;

        private void Awake()
        {
            harmony.PatchAll();
        }
    }
}

