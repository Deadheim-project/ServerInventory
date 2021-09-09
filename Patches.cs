using HarmonyLib;
using System;
using System.Collections.Generic;
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

            if (!ServerInventory.isSynced)
            {
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "InventorySync", new ZPackage());
            }

            ServerInventory.hasSpawned = true;
            ServerInventory.isDead = false;
            Debug.LogError("OnSpawned");
        }
    }

    [HarmonyPatch(typeof(Player), "OnDeath")]
    internal class OnDeath
    {
        private static void Postfix(Player __instance)
        {
            Debug.LogError("OnDeath");
            Util.SaveInventory(Player.m_localPlayer.m_inventory);
            ServerInventory.isDead = true;
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

    [HarmonyPatch(typeof(Inventory), "RemoveItem", new Type[] { typeof(ItemDrop.ItemData) })]
    public static class InventoryRemoveItem1
    {
        private static void Postfix(Inventory __instance)
        {
            if (!Player.m_localPlayer) return;

            Debug.LogError("InventoryRemoveItem1");
            Util.SaveInventory(__instance);
        }
    }

    [HarmonyPatch(typeof(Inventory), "RemoveItem", new Type[] { typeof(int) })]
    public static class InventoryRemoveItem2
    {
        private static void Postfix(Inventory __instance)
        {
            if (!Player.m_localPlayer) return;

            Debug.LogError("InventoryRemoveItem2");
            Util.SaveInventory(__instance);
        }
    }

    [HarmonyPatch(typeof(Inventory), "RemoveItem", new Type[] { typeof(string), typeof(int) })]
    public static class InventoryRemoveItem3
    {
        private static void Postfix(Inventory __instance)
        {
            if (!Player.m_localPlayer) return;

            Debug.LogError("InventoryRemoveItem3");
            Util.SaveInventory(__instance);
        }
    }


    [HarmonyPatch(typeof(Inventory), "RemoveItem", new Type[] { typeof(ItemDrop.ItemData), typeof(int) })]
    public static class InventoryRemoveItem4
    {
        private static void Postfix(Inventory __instance)
        {
            if (!Player.m_localPlayer) return;

            Debug.LogError("InventoryRemoveItem4");
            Util.SaveInventory(__instance);
        }
    }

    [HarmonyPatch(typeof(Inventory), "AddItem", new Type[] { typeof(string), typeof(int), typeof(int), typeof(int), typeof(long), typeof(string) })]
    public static class InventoryAddItem
    {
        private static void Postfix(Inventory __instance)
        {
            if (!Player.m_localPlayer) return;

            Debug.LogError("InventoryAddItem");
            Util.SaveInventory(__instance);
        }
    }

    [HarmonyPatch(typeof(Inventory), "AddItem", new Type[] { typeof(ItemDrop.ItemData) })]
    public static class InventoryAddItem1
    {
        private static void Postfix(Inventory __instance)
        {
            if (!Player.m_localPlayer) return;

            Debug.LogError("InventoryAddItem1");
            Util.SaveInventory(__instance);
        }
    }    
     
    [HarmonyPatch(typeof(Inventory), "AddItem", new Type[] { typeof(ItemDrop.ItemData), typeof(int), typeof(int), typeof(int) })]
    public static class InventoryAddItem2
    {
        private static void Postfix(Inventory __instance)
        {
            if (!Player.m_localPlayer) return;

            Debug.LogError("InventoryAddItem2");
            Util.SaveInventory(__instance);
        }
    }

    [HarmonyPatch(typeof(Inventory), "RemoveOneItem")]
    public static class RemoveOneItem
    {
        private static void Postfix(Inventory __instance)
        {
            if (!Player.m_localPlayer) return;

            Debug.LogError("RemoveOneItem");
            Util.SaveInventory(__instance);
        }
    }

    [HarmonyPatch(typeof(Inventory), "MoveAll")]
    public static class MoveAllPrefix
    {
        private static void Prefix(Inventory __instance)
        {
            ServerInventory.isMovingAll = true;
            Debug.LogError("isMovingAll true");
        }

        private static void Postfix(Inventory __instance)
        {
            ServerInventory.isMovingAll = false;
            Debug.LogError("isMovingAll false");
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
