
using LitJson;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ServerInventory
{
    public class Rpc
    {
        public static void RPC_LoadInventory(long sender, ZPackage pkg)
        {
            Debug.LogError("RPC_LoadInventory");
            Util.LoadInventory(JsonMapper.ToObject<List<InventoryDTO>>(StringCompression.Decompress(pkg.ReadString())));
        }

        public static void RPC_SaveInventory(long sender, ZPackage pkg)
        {
            Debug.LogError("RPC_SaveInventory");

            ZNetPeer peer = ZNet.instance.GetPeer(sender);

            if (peer != null)
            {
                Debug.LogError("RPC_SaveInventory");
                Directory.CreateDirectory(Utils.GetSaveDataPath() + "/inventories");
                string steamId = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString();
                string playerName = peer.m_playerName;
                string directory = Utils.GetSaveDataPath() + "/inventories/" + playerName + "-" + steamId + ".json";
                File.WriteAllText(directory, StringCompression.Decompress(pkg.ReadString()));
            }
        }

        public static void RPC_InventorySync(long sender, ZPackage pkg)
        {
            Debug.LogError("RPC_InventorySync");

            if (!ZNet.instance.IsServer()) return;

            ZNetPeer peer = ZNet.instance.GetPeer(sender);

            if (peer != null)
            {
                Debug.LogError("RPC_InventorySync");
                string steamId = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString();
                string playerName = peer.m_playerName;
                string directory = Utils.GetSaveDataPath() + "/inventories/" + playerName + "-" + steamId + ".json";

                if (File.Exists(directory))
                {
                    ZPackage inventory = new ZPackage();
                    inventory.Write(StringCompression.Compress(File.ReadAllText(directory)));
                    ZRoutedRpc.instance.InvokeRoutedRPC(sender, "LoadInventory", inventory);
                }
            }
        }
    }
}