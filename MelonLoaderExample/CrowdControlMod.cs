using CrowdControl;
using CrowdControl.Delegates.Effects;
using MelonLoader;
using UnityEngine;

[assembly: MelonGame("Ved", "Megabonk")]
[assembly: MelonInfo(typeof(CrowdControlMod), CrowdControlMod.MOD_NAME, "1.0.3", CrowdControlMod.MOD_DEVELOPER, "https://crowdcontrol.live/")]

namespace CrowdControl;

/// <summary>
/// The main Crowd Control mod class.
/// </summary>
public class CrowdControlMod : MelonMod
{
    // Mod Details
    public const string MOD_GUID = "WarpWorld.CrowdControl";
    public const string MOD_DEVELOPER = "Warp World";
    public const string MOD_NAME = "Crowd Control";
    public const string MOD_VERSION = "1.0.3.0";
    
    public static float DeltaTime => Time.fixedDeltaTime; //change this to Time.deltaTime if using Update instead of FixedUpdate

    private readonly HarmonyLib.Harmony harmony = new(MOD_GUID);

    /// <summary>The logger for the mod.</summary>
    public MelonLogger.Instance Logger => LoggerInstance;

    /// <summary>The singleton instance of the game mod.</summary>
    internal static CrowdControlMod Instance { get; private set; } = null!;

    /// <summary>The game state manager object.</summary>
    public GameStateManager GameStateManager { get; private set; } = null!;
    
    /// <summary>The effect class loader.</summary>
    public EffectLoader EffectLoader { get; private set; } = null!;

    /// <summary>
    /// Gets a value indicating whether the client is connected.
    /// </summary>
    public bool ClientConnected => Client.Connected;

    public NetworkClient Client { get; private set; }
    
    public Scheduler Scheduler { get; private set; }
    
    private const float GAME_STATUS_UPDATE_INTERVAL = 1f;
    private float m_gameStatusUpdateTimer;

    /// <summary>
    /// Called when the mod is created.
    /// </summary>
    public override void OnInitializeMelon()
    {
        Instance = this;

        Logger.Msg($"Loaded {MOD_GUID}. Patching.");
        harmony.PatchAll();

        Logger.Msg("Initializing Crowd Control");

        try
        {
            GameStateManager = new(this);
            Client = new(this);
            EffectLoader = new(this, Client);
            Scheduler = new(this, Client);
        }
        catch (Exception e)
        {
            Logger.Error($"Crowd Control Init Error: {e}");
        }

        Logger.Msg("Crowd Control Initialized");
    }

    /// <summary>Called every fixed frame (physics) update.</summary>
    /// <remarks>This function is called on the main game thread. Blocking here may cause lag or crash the game entirely.</remarks>
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        m_gameStatusUpdateTimer += Time.fixedDeltaTime;
        if (m_gameStatusUpdateTimer >= GAME_STATUS_UPDATE_INTERVAL)
        {
            GameStateManager.UpdateGameState();
            m_gameStatusUpdateTimer = 0f;
        }

        Scheduler?.Tick();
    }

    /***** == ONLY USE THIS IF FixedUpdate() ISN'T ALREADY BEING CALLED EVERY TICK == *****/
    //attach this to some game class with a function that runs every frame like the player's Update()
    //[HarmonyPatch(typeof(PlayerMovement), nameof(PlayerMovement.FixedUpdate))]
    //private class PlayerMovement_FixedUpdate { static void Prefix() => Instance.FixedUpdate(); }
}
