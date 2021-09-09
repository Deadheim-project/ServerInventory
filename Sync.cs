using HarmonyLib;
using System;

namespace ServerInventory
{

    public static class Sync
    {
        public static void RPC_InventoryVersionValidator(ZRpc rpc, ZPackage pkg)
        {
            if (!ZNet.instance.IsServer()) return;

            var version = pkg.ReadString();
            if (version != ServerInventory.Version)
            {
                rpc.Invoke("Error", (object)3);
            }
            else
            {
                ServerInventory.validatedUsers.Add(rpc);
            }
        }

        [HarmonyPatch(typeof(ZNet), "OnNewConnection")]
        public static class OnNewConnection
        {
            private static void Prefix(ZNetPeer peer, ref ZNet __instance)
            {
                peer.m_rpc.Register<ZPackage>("InventoryVersionValidator", new Action<ZRpc, ZPackage>(RPC_InventoryVersionValidator));
                ZPackage zpackage = new ZPackage();
                zpackage.Write(ServerInventory.Version);
                peer.m_rpc.Invoke("InventoryVersionValidator", zpackage);
            }
        }

        [HarmonyPatch(typeof(ZNet), "RPC_PeerInfo")]
        public static class RPC_PeerInfo
        {
            private static bool Prefix(ZRpc rpc, ref ZNet __instance)
            {
                if (!__instance.IsServer()) return true;

                if (!ServerInventory.validatedUsers.Contains(rpc))
                {
                    rpc.Invoke("Error", 3);
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(ZNet), "Disconnect")]
        public static class Disconnect
        {
            private static void Prefix(ZNetPeer peer, ref ZNet __instance)
            {
                if (__instance.IsServer()) return;

                ServerInventory.validatedUsers.Remove(peer.m_rpc);
            }
        }
    }
}