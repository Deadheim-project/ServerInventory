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
        public static bool hasSpawned = false;
        public static bool hasServerInventory = false;

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
