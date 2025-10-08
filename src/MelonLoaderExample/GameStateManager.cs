using System.Runtime.CompilerServices;
using ConnectorLib.JSON;
using Il2Cpp;
using Il2CppPG;
using UnityEngine;

namespace CrowdControl;

public class GameStateManager
{
    //Everything in the game-specific region will need to be changed for each game
    
    #region Game-Specific Code
    
    public static bool isFocused = true;
    public static bool ForceUseCash = false;
    public static bool ForceUseCredit = false;
    public static bool ForceRequireChange = false;
    public static bool ForceMath = false;
    public static bool ForceExactChange = false;
    public static bool AllowMischarge = false;
    public static bool ForceLargeBills = false;
    public static int CurrentLanguage = 0;

    public static int OrgLanguage = 0;
    public static int NewLanguage = 0;
    
    public static string currentHeldItem;
    
    public static string NameOverride = "";
    public static List<GameObject> nameplates = new List<GameObject>();

    /// <summary>Checks if the game is in a state where effects can be applied.</summary>
    /// <param name="code">The effect codename the caller is intending to apply.</param>
    /// <returns>True if the game is in a state where the effect can be applied, false otherwise.</returns>
    /// <remarks>
    /// The <paramref name="code"/> parameter is not normally checked.
    /// Use this is you want to exempt certain effects from checks (e.g. debug or "fix-it" effects).
    /// </remarks>
    public bool IsReady(string code = "") => GetGameState() == ConnectorLib.JSON.GameState.Ready;

    /// <summary>Gets the current game state as it pertains to the firing of effects.</summary>
    /// <returns>The current game state.</returns>
    public GameState GetGameState()
    {
        try
        {
            //make sure the game is in focus otherwise don't let effects trigger
            if (!isFocused)
                return GameState.Paused;
            
            //add check for whether the game is in a state it can accept effects
            PlayerInteraction player = Singleton<PlayerInteraction>.Instance;
            if (player == null) return GameState.BadPlayerState;
            
            bool isPaused = (bool)CrowdDelegates.getProperty(player, "m_Paused");
            if (isPaused) return GameState.Paused;
            
            return GameState.Ready;
        }
        catch (Exception e)
        {
            CrowdControlMod.Instance.Logger.Error($"GameStateManager Error: {e}");
            return GameState.Error;
        }
    }

    #endregion

    //Everything from here down is the same for every game - you probably don't need to change it

    #region General Code

    /// <summary>Reports the updated game state to the Crowd Control client.</summary>
    /// <param name="force">True to force the report to be sent, even if the state is the same as the previous state, false to only report the state if it has changed.</param>
    /// <returns>True if the data was sent successfully, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool UpdateGameState(bool force = false) => UpdateGameState(GetGameState(), force);

    /// <summary>Reports the updated game state to the Crowd Control client.</summary>
    /// <param name="newState">The new game state to report.</param>
    /// <param name="force">True to force the report to be sent, even if the state is the same as the previous state, false to only report the state if it has changed.</param>
    /// <returns>True if the data was sent successfully, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool UpdateGameState(ConnectorLib.JSON.GameState newState, bool force) => UpdateGameState(newState, null, force);

    private ConnectorLib.JSON.GameState? _last_game_state;
    private readonly CrowdControlMod m_mod;
    public GameStateManager(CrowdControlMod mod)
    {
        m_mod = mod;
    }

    /// <summary>Reports the updated game state to the Crowd Control client.</summary>
    /// <param name="newState">The new game state to report.</param>
    /// <param name="message">The message to attach to the state report.</param>
    /// <param name="force">True to force the report to be sent, even if the state is the same as the previous state, false to only report the state if it has changed.</param>
    /// <returns>True if the data was sent successfully, false otherwise.</returns>
    public bool UpdateGameState(ConnectorLib.JSON.GameState newState, string? message = null, bool force = false)
    {
        if (force || (_last_game_state != newState))
        {
            _last_game_state = newState;
            return m_mod.Client.Send(new GameUpdate(newState, message));
        }

        return true;
    }

    #endregion
}