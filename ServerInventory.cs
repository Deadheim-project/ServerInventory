using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

namespace ServerInventory
{
    [BepInPlugin("Detalhes.ServerInventory", "ServerInventory", "1.0.0")]
    public class ServerInventory : BaseUnityPlugin
    {
        public const string PluginGUID = "Detalhes.ServerInventory";
        Harmony harmony = new Harmony(PluginGUID);
        public static Assembly quickSlotsAssembly;
        public static string Version = "1";
        public static List<ZRpc> validatedUsers = new List<ZRpc>();
        public static bool hasSpawned = false;
        public static bool hasServerInventory = false;
        public static bool isDead = false;
        public static bool isLoadingInventory = false;
        public static bool isSynced = false;
        public static bool isMovingAll = false;
        public static int lastSavedInventoryCount = 0;

        private void Awake()
        {
            harmony.PatchAll();
        }

        private void Start()
        {
            if (Chainloader.PluginInfos.ContainsKey("randyknapp.mods.equipmentandquickslots"))
                quickSlotsAssembly = Chainloader.PluginInfos["randyknapp.mods.equipmentandquickslots"].Instance.GetType().Assembly;

        }
    }
}
