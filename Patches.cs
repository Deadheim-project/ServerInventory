using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using UnityEngine;

namespace ServerInventory
{
    [HarmonyPatch(typeof(Player), "OnSpawned")]
    internal class OnSpawned
    {
        private static void Postfix(Player __instance)
        {
            ServerInventory.hasSpawned = true;
            ServerInventory.isDead = false;

            if (ServerInventory.isSynced) return;
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "InventorySync", new ZPackage());
            ServerInventory.isSynced = true;
        }
    }

    [HarmonyPatch(typeof(Player), "OnDeath")]
    internal class OnDeath
    {
        private static void Postfix(Player __instance)
        {
            Util.SaveInventory(Player.m_localPlayer.m_inventory);
            ServerInventory.isDead = true;
        }
    }

    [HarmonyPatch(typeof(ZNet), "Shutdown")]
    internal class Disconnect
    {
        private static void Prefix()
        {
            ServerInventory.hasSpawned = false;
            ServerInventory.isSynced = false;
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

            Util.SaveInventory(__instance);
        }
    }

    [HarmonyPatch(typeof(Inventory), "RemoveItem", new Type[] { typeof(int) })]
    public static class InventoryRemoveItem2
    {
        private static void Postfix(Inventory __instance)
        {
            if (!Player.m_localPlayer) return;

            Util.SaveInventory(__instance);
        }
    }

    [HarmonyPatch(typeof(Inventory), "RemoveItem", new Type[] { typeof(string), typeof(int) })]
    public static class InventoryRemoveItem3
    {
        private static void Postfix(Inventory __instance)
        {
            if (!Player.m_localPlayer) return;

            Util.SaveInventory(__instance);
        }
    }

    [HarmonyPatch(typeof(Inventory), "RemoveItem", new Type[] { typeof(ItemDrop.ItemData), typeof(int) })]
    public static class InventoryRemoveItem4
    {
        private static void Postfix(Inventory __instance)
        {
            if (!Player.m_localPlayer) return;

            Util.SaveInventory(__instance);
        }
    }

    [HarmonyPatch(typeof(Inventory), "AddItem", new Type[] { typeof(string), typeof(int), typeof(int), typeof(int), typeof(long), typeof(string) })]
    public static class InventoryAddItem
    {
        private static void Postfix(Inventory __instance)
        {
            if (!Player.m_localPlayer) return;

            Util.SaveInventory(__instance);
        }
    }

    [HarmonyPatch(typeof(Inventory), "AddItem", new Type[] { typeof(ItemDrop.ItemData) })]
    public static class InventoryAddItem1
    {
        private static void Postfix(Inventory __instance)
        {
            if (!Player.m_localPlayer) return;

            Util.SaveInventory(__instance);
        }
    }

    [HarmonyPatch(typeof(Inventory), "AddItem", new Type[] { typeof(ItemDrop.ItemData), typeof(int), typeof(int), typeof(int) })]
    public static class InventoryAddItem2
    {
        private static void Postfix(Inventory __instance)
        {
            if (!Player.m_localPlayer) return;

            Util.SaveInventory(__instance);
        }
    }

    [HarmonyPatch(typeof(Inventory), "RemoveOneItem")]
    public static class RemoveOneItem
    {
        private static void Postfix(Inventory __instance)
        {
            if (!Player.m_localPlayer) return;

            Util.SaveInventory(__instance);
        }
    }

    [HarmonyPatch(typeof(Inventory), "MoveAll")]
    public static class MoveAllPrefix
    {
        private static void Prefix(Inventory __instance)
        {
            ServerInventory.isMovingAll = true;
        }

        private static void Postfix(Inventory __instance)
        {
            ServerInventory.isMovingAll = false;
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
        }
    }

    [HarmonyPatch(typeof(World), nameof(World.SaveWorldMetaData))]
    public static class SaveWorldMetaData
    {
        private static void Postfix()
        {
            if (ZNet.instance?.IsServer() == true)
            {
                string backupPath = Utils.GetSaveDataPath() + "/inventoriesBackup/";
                string inventoriesPath = Utils.GetSaveDataPath() + "/inventories/";
                Directory.CreateDirectory(backupPath);

                string fileName = backupPath + "inventories-" + DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss") + ".zip";
                ZipFile.CreateFromDirectory(inventoriesPath, fileName);
            }
        }
    }
}
