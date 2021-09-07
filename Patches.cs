using HarmonyLib;
using System;
using UnityEngine;

namespace ServerInventory
{
    [HarmonyPatch(typeof(Player), "OnSpawned")]
    internal class OnSpawned
    {
        private static void Postfix(Player __instance)
        {
            if (ZRoutedRpc.instance == null)
                return;

            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "InventorySync", new ZPackage());
            ServerInventory.hasSpawned = true;
            Debug.LogError("OnSpawned");
        }
    }

    [HarmonyPatch(typeof(ItemDrop), "Interact")]
    internal class ItemDropInteract
    {
        private static void Postfix()
        {
            Util.SaveInventory(Player.m_localPlayer.m_inventory);
        }
    }

    [HarmonyPatch(typeof(TombStone), "Interact")]
    internal class TombStoneInteract
    {
        private static void Postfix()
        {
            Util.SaveInventory(Player.m_localPlayer.m_inventory);
        }
    }

    [HarmonyPatch(typeof(CraftingStation), "Interact")]
    internal class CraftingStationInteract
    {
        private static void Postfix()
        {
            Util.SaveInventory(Player.m_localPlayer.m_inventory);
        }
    }

    [HarmonyPatch(typeof(Inventory), "Changed")]
    public static class AddItem
    {
        private static void Postfix(Inventory __instance)
        {
            Debug.LogError("Changed");
            Util.SaveInventory(__instance);
        }
    }

    [HarmonyPatch(typeof(Game), "Start")]
    public static class GameStart
    {
        public static void Postfix()
        {
            if (ZRoutedRpc.instance == null)
                return;

            ZRoutedRpc.instance.Register<ZPackage>("InventorySync", new Action<long, ZPackage>(Rpc.RPC_InventorySync));
            ZRoutedRpc.instance.Register<ZPackage>("LoadInventory", new Action<long, ZPackage>(Rpc.RPC_LoadInventory));
            ZRoutedRpc.instance.Register<ZPackage>("SaveInventory", new Action<long, ZPackage>(Rpc.RPC_SaveInventory));
            Debug.LogError("Routed");
        }
    }
}
