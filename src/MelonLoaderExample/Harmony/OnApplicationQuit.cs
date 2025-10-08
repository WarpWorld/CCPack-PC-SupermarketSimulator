using System.Net.Sockets;
using HarmonyLib;
using Il2Cpp;

namespace CrowdControl;

[HarmonyPatch(typeof(MusicPlayerManager))]
[HarmonyPatch("OnApplicationQuit")]
public class OnApplicationQuit
{
    static void Prefix()
    {
        try { CrowdControlMod.Instance.Client.Dispose(); }
        catch { /**/ }
    }
}