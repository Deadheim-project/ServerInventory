using LitJson;
using System.Collections.Generic;
using UnityEngine;

namespace ServerInventory
{
    class Util
    {
        public static List<string> blacklist = new List<string> { "Neck_BiteAttack", "Greyling_attack", "boar_base_attack" };
        public static void SaveInventory(Inventory __instance)
        {
            if (__instance.m_name != "Inventory") return;
            if (!Player.m_localPlayer) return;
            if (!ServerInventory.hasSpawned) return;
            if (ServerInventory.isLoadingInventory) return;
            if (ServerInventory.isDead) return;
            if (ServerInventory.isMovingAll) return;
            if (!Player.m_localPlayer.m_skills) return;
            if (__instance != Player.m_localPlayer.m_inventory) return;            

            if (ServerInventory.lastSavedInventoryCount > 0)
            {
                int newInventoryCount = Player.m_localPlayer.m_inventory.m_inventory.Count;

                if (newInventoryCount == 0 && ServerInventory.lastSavedInventoryCount > 1) return;
            }

            List<SkillDTO> skillDTOList = new List<SkillDTO>();

            foreach (KeyValuePair<Skills.SkillType, Skills.Skill> entry in Player.m_localPlayer.m_skills.m_skillData)
            {
                SkillDTO dto = new SkillDTO();
                dto.SkillType = (int)entry.Key;
                dto.Accumulator = entry.Value.m_accumulator;
                dto.Level = entry.Value.m_level;
                skillDTOList.Add(dto);
            }  

            List<InventoryDTO> inventoryDTOList = new List<InventoryDTO>();

            foreach (ItemDrop.ItemData item in Player.m_localPlayer.m_inventory.m_inventory)
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
                dto.InventoryName = Player.m_localPlayer.m_inventory.m_name;

                inventoryDTOList.Add(dto);
            }

            RootDTO rootDTO = new RootDTO();
            rootDTO.SkillDTOList = skillDTOList;
            rootDTO.InventoryDTOList = inventoryDTOList;

            ServerInventory.lastSavedInventoryCount = inventoryDTOList.Count;
            string json = JsonMapper.ToJson(rootDTO);
            ZPackage zpackge = new ZPackage();
            zpackge.Write(StringCompression.Compress(json));
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "SaveInventory", zpackge);
            Debug.Log("Inventory Saved: " + ServerInventory.lastSavedInventoryCount + " items");
        }

        public static void LoadInventory(RootDTO rootDTO)
        {
            if (!Player.m_localPlayer) return;

            ServerInventory.isLoadingInventory = true;

            Player.m_localPlayer.m_inventory.RemoveAll();

            foreach (InventoryDTO item in rootDTO.InventoryDTOList)
            {
                Player.m_localPlayer.m_inventory.AddItem(item.Name, item.Stack, item.Durability, new Vector2i(item.PosX, item.PosY), item.Equiped, item.Quality, item.Variant, item.CrafterID, item.CrafterName);
            }

            foreach (SkillDTO skillDTO in rootDTO.SkillDTOList)
            {
                Skills.Skill skill;
                Player.m_localPlayer.m_skills.m_skillData.TryGetValue((Skills.SkillType)skillDTO.SkillType, out skill);
                if (skill == null) continue;

                skill.m_accumulator = skillDTO.Accumulator;
                skill.m_level = skillDTO.Level;
            }

            ServerInventory.isLoadingInventory = false;
            Debug.Log("Inventory Loaded: " + rootDTO.InventoryDTOList.Count + " items");
        }
    }
}
