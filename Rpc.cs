using LitJson;
using System.IO;
using UnityEngine;

namespace ServerInventory
{
    public class Rpc
    {
        public static void RPC_LoadInventory(long sender, ZPackage pkg)
        {
            Util.LoadInventory(JsonMapper.ToObject<RootDTO>(StringCompression.Decompress(pkg.ReadString())));
        }

        public static void RPC_SaveInventory(long sender, ZPackage pkg)
        {
            ZNetPeer peer = ZNet.instance.GetPeer(sender);

            if (peer != null)
            {
                Directory.CreateDirectory(Utils.GetSaveDataPath() + "/inventories");
                string steamId = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString();
                string playerName = peer.m_playerName;
                string directory = Utils.GetSaveDataPath() + "/inventories/" + playerName + "-" + steamId + ".json";
                File.WriteAllText(directory, StringCompression.Decompress(pkg.ReadString()));
            }
        }

        public static void RPC_InventorySync(long sender, ZPackage pkg)
        {
            if (!ZNet.instance.IsServer()) return;

            ZNetPeer peer = ZNet.instance.GetPeer(sender);

            if (peer != null)
            {
                string steamId = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString();
                string playerName = peer.m_playerName;
                string directory = Utils.GetSaveDataPath() + "/inventories/" + playerName + "-" + steamId + ".json";

                ZPackage inventory = new ZPackage();

                if (File.Exists(directory))
                {
                    inventory.Write(StringCompression.Compress(File.ReadAllText(directory)));
                }
                else
                {
                    inventory.Write(StringCompression.Compress(File.ReadAllText(Utils.GetSaveDataPath() + "/inventories/default.json")));
                }

                ZRoutedRpc.instance.InvokeRoutedRPC(sender, "LoadInventory", inventory);
                Debug.LogError("RPC_InventorySync: " + playerName + " - " + steamId);
            }
        }
    }
}