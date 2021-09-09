using LitJson;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ServerInventory
{
    class Util
    {
        public static List<string> blacklist = new List<string> { "Neck_BiteAttack" };
        public static void SaveInventory(Inventory __instance)
        {
            if (!Player.m_localPlayer) return;
            if (!ServerInventory.hasSpawned) return;
            if (ServerInventory.isLoadingInventory) return;
            if (ServerInventory.isDead) return;
            if (ServerInventory.isMovingAll) return;
            if (__instance.m_name != "Inventory") return;

            List<Inventory> invetoryList = GetInventoryList(__instance);

            if (ServerInventory.lastSavedInventoryCount > 0)
            {
                int newInventoryCount = invetoryList.Select(x => x.m_inventory.Count).ToList().Sum();

                if (newInventoryCount == 0 && ServerInventory.lastSavedInventoryCount > 1) return;
            }

            Debug.Log("SaveInventory");

            List<InventoryDTO> inventoryDTOList = new List<InventoryDTO>();

            foreach (Inventory inventory in invetoryList)
            {
                foreach (ItemDrop.ItemData item in inventory.m_inventory)
                {
                    if (blacklist.Contains(item.m_dropPrefab.name)) continue;

                    InventoryDTO dto = new InventoryDTO();
                    dto.Name = item.m_dropPrefab.name;
                    dto.Stack = item.m_stack;
                    dto.Quality = item.m_quality;
                    dto.Variant = item.m_variant;
                    dto.CrafterID = item.m_crafterID;
                    dto.CrafterName = item.m_crafterName;
                    dto.Equiped = item.m_equiped;
                    dto.PosX = item.m_gridPos.x;
                    dto.PosY = item.m_gridPos.y;
                    dto.Durability = item.m_durability;
                    dto.InventoryName = inventory.m_name;

                    inventoryDTOList.Add(dto);
                }
            }

            ServerInventory.lastSavedInventoryCount = inventoryDTOList.Count;
            string json = JsonMapper.ToJson(inventoryDTOList);
            ZPackage zpackge = new ZPackage();
            zpackge.Write(StringCompression.Compress(json));
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "SaveInventory", zpackge);

        }

        public static void LoadInventory(List<InventoryDTO> inventoryDTOs)
        {
            if (!Player.m_localPlayer) return;

            List<Inventory> inventoryList = GetInventoryList(Player.m_localPlayer.m_inventory);

            if (inventoryList.Count == 0) return;

            ServerInventory.isLoadingInventory = true;

            inventoryList.ForEach(x => x.RemoveAll());
            Inventory playerInventory = inventoryList.FirstOrDefault(x => x.m_name == "Inventory");
            Inventory quickSlot = inventoryList.FirstOrDefault(x => x.m_name == "QuickSlotInventory");
            Inventory equipmentInventory = inventoryList.FirstOrDefault(x => x.m_name == "EquipmentSlotInventory");

            foreach (InventoryDTO item in inventoryDTOs)
            {
                if (item.InventoryName == "EquipmentSlotInventory") equipmentInventory.AddItem(item.Name, item.Stack, item.Durability, new Vector2i(item.PosX, item.PosY), item.Equiped, item.Quality, item.Variant, item.CrafterID, item.CrafterName);
                if (item.InventoryName == "Inventory") playerInventory.AddItem(item.Name, item.Stack, item.Durability, new Vector2i(item.PosX, item.PosY), item.Equiped, item.Quality, item.Variant, item.CrafterID, item.CrafterName);
                if (item.InventoryName == "QuickSlotInventory") quickSlot.AddItem(item.Name, item.Stack, item.Durability, new Vector2i(item.PosX, item.PosY), item.Equiped, item.Quality, item.Variant, item.CrafterID, item.CrafterName);
            }

            ServerInventory.isLoadingInventory = false;
            ServerInventory.isSynced = true;
        }

        public static List<Inventory> GetInventoryList(Inventory __instance)
        {
            List<Inventory> inventoryList = new List<Inventory>();
            if (ServerInventory.quickSlotsAssembly != null)
            {
                var extendedInventory = ServerInventory.quickSlotsAssembly.GetType("EquipmentAndQuickSlots.InventoryExtensions").GetMethod("Extended", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { __instance });
                if (extendedInventory == null) return inventoryList;

                inventoryList = (List<Inventory>)ServerInventory.quickSlotsAssembly.GetType("EquipmentAndQuickSlots.ExtendedInventory").GetField("_inventories", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(extendedInventory);
            }
            else
            {
                inventoryList.Add(__instance);
            }

            return inventoryList;
        }    
    }
}
