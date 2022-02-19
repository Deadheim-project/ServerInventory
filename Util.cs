using LitJson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ServerInventory
{
    class Util
    {        
        public static void SaveInventory(Inventory inventory)
        {
            var caller = Task.Factory.StartNew(() => SaveInventoryMethod(inventory));
        }

        private static void SaveInventoryMethod(Inventory __instance)
        {
            try
            {
                if (__instance.m_name != "Inventory") return;
                if (!Player.m_localPlayer) return;
                if (!ServerInventory.hasSpawned) return;
                if (ServerInventory.isDead) return;
                if (ServerInventory.isMovingAll) return;
                if (ServerInventory.isLoading) return;
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

                List<RootDTO.KnownTextDTO> knownTextDTOs = new List<RootDTO.KnownTextDTO>();

                if (Player.m_localPlayer.m_knownTexts.TryGetValue("playerLevel", out string value))
                {
                    var text = new RootDTO.KnownTextDTO();
                    text.Key = "playerLevel";
                    text.Value = value;
                    knownTextDTOs.Add(text);
                }

                if (Player.m_localPlayer.m_knownTexts.TryGetValue("playerExp", out string value2))
                {
                    var text = new RootDTO.KnownTextDTO();
                    text.Key = "playerExp";
                    text.Value = value2;
                    knownTextDTOs.Add(text);
                }

                if (Player.m_localPlayer.m_knownTexts.TryGetValue("playerAvailablePoints", out string value3))
                {
                    var text = new RootDTO.KnownTextDTO();
                    text.Key = "playerAvailablePoints";
                    text.Value = value3;
                    knownTextDTOs.Add(text);
                }

                if (Player.m_localPlayer.m_knownTexts.TryGetValue("playerAgility", out string value4))
                {
                    var text = new RootDTO.KnownTextDTO();
                    text.Key = "playerAgility";
                    text.Value = value4;
                    knownTextDTOs.Add(text);
                }

                if (Player.m_localPlayer.m_knownTexts.TryGetValue("playerStrength", out string value5))
                {
                    var text = new RootDTO.KnownTextDTO();
                    text.Key = "playerStrength";
                    text.Value = value5;
                    knownTextDTOs.Add(text);
                }

                if (Player.m_localPlayer.m_knownTexts.TryGetValue("playerIntelligence", out string value6))
                {
                    var text = new RootDTO.KnownTextDTO();
                    text.Key = "playerIntelligence";
                    text.Value = value6;
                    knownTextDTOs.Add(text);
                }

                if (Player.m_localPlayer.m_knownTexts.TryGetValue("playerFocus", out string value7))
                {
                    var text = new RootDTO.KnownTextDTO();
                    text.Key = "playerFocus";
                    text.Value = value7;
                    knownTextDTOs.Add(text);
                }
                

                if (Player.m_localPlayer.m_knownTexts.TryGetValue("playerConstitution", out string value8))
                {
                    var text = new RootDTO.KnownTextDTO();
                    text.Key = "playerConstitution";
                    text.Value = value8;
                    knownTextDTOs.Add(text);
                }

                if (Player.m_localPlayer.m_knownTexts.TryGetValue("dead_PlayerWards", out string value9))
                {
                    var text = new RootDTO.KnownTextDTO();
                    text.Key = "dead_PlayerWards";
                    text.Value = value9;
                    knownTextDTOs.Add(text);
                }

                RootDTO rootDTO = new RootDTO();
                rootDTO.SkillDTOList = skillDTOList;
                rootDTO.InventoryDTOList = inventoryDTOList;
                rootDTO.KnownTextDTOList = knownTextDTOs;

                ServerInventory.lastSavedInventoryCount = inventoryDTOList.Count;
                string json = JsonMapper.ToJson(rootDTO);
                ZPackage zpackge = new ZPackage();
                zpackge.Write(StringCompression.Compress(json));
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "SaveInventory", zpackge);
                Debug.Log("Inventory Saved: " + ServerInventory.lastSavedInventoryCount + " items");
            } catch (Exception e)
            {
                Debug.LogError(e.Message);
            }            
        }

        public static void LoadInventory(RootDTO rootDTO)
        {
            if (!Player.m_localPlayer) return;

            ServerInventory.isLoading = true;

            Player.m_localPlayer.m_inventory.RemoveAll();

            try
            {
                foreach (InventoryDTO item in rootDTO.InventoryDTOList)
                {
                    Player.m_localPlayer.m_inventory.AddItem(item.Name, item.Stack, item.Durability, new Vector2i(item.PosX, item.PosY), item.Equiped, item.Quality, item.Variant, item.CrafterID, item.CrafterName);
                }
            }
            catch
            {
                Debug.LogError("error loading items");
            }
            try
            {
                foreach (SkillDTO skillDTO in rootDTO.SkillDTOList)
                {
                    Player.m_localPlayer.m_skills.m_skillData[(Skills.SkillType)skillDTO.SkillType].m_accumulator = skillDTO.Accumulator;
                    Player.m_localPlayer.m_skills.m_skillData[(Skills.SkillType)skillDTO.SkillType].m_level = skillDTO.Level;
                }
            }
            catch
            {
                Debug.LogError("error loading skills");
            }
            try
            {
                foreach (RootDTO.KnownTextDTO knownTextDTO in rootDTO.KnownTextDTOList)
                {
                    Player.m_localPlayer.m_knownTexts[knownTextDTO.Key] = knownTextDTO.Value;
                }
            }
            catch
            {
                Debug.LogError("error loading knowTexts");
            }

            ServerInventory.isLoading = false;

            Debug.Log("Inventory Loaded: " + rootDTO.InventoryDTOList.Count + " items");
        }
    }
}
