using System.Runtime.CompilerServices;
using ConnectorLib.JSON;
using Il2Cpp;
using Il2CppAssets.Scripts.Actors.Player;
using UnityEngine;

namespace CrowdControl;

public class GameStateManager
{
    //Everything in the game-specific region will need to be changed for each game
    
    #region Game-Specific Code

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
            // Application isn't even focused, game has probably autopaused or is about to
            if (!Application.isFocused)
                return GameState.Paused;

            // No player, probably in main menu or loading screen
            if (!MyPlayer.Instance)
                return GameState.Menu;
            
            // No game manager, probably in main menu or loading screen
            if (!GameManager.Instance)
                return GameState.Menu;
            
            // Game reports it's in a cutscene
            if (GameManager.Instance.cutscene)
                return GameState.Cutscene;

            // The level time is very low so we're probably loading in or just loaded in
            if (GameManager.Instance.totalStageTime <= 1)
                return GameState.Loading;

            // Game is over so player is presumably dead or has won (can you win in this game?)
            if (GameManager.Instance.isGameOver)
                return GameState.BadPlayerState;
            
            // Game explicitly reports not playing (main menu, level select, etc)
            if (!GameManager.Instance.isPlaying)
                return GameState.Menu;

            // Catch unexpected null refs inside IsDead
            try
            {
                // Player is dead
                if (MyPlayer.Instance.IsDead())
                    return GameState.BadPlayerState;
            }
            catch { /**/ }
            
            // Game is paused, or one of several UI screens (which also pause the action) is open
            if (Il2CppAssets.Scripts.Utility.MyTime.paused || isPausedChecker() || chestOpen() || levelUpOpen())
                return GameState.Paused;

            // If all checks pass, we're ready for effects
            return GameState.Ready;
        }
        catch (Exception e)
        {
            CrowdControlMod.Instance.Logger.Error($"GameStateManager Error: {e}");
            return GameState.Error;
        }

        // Helper functions to keep things tidy

        // Check if the pause menu is open
        bool isPausedChecker()
        {
            PauseUi pauseUi = UiManager.Instance?.pause;
            return pauseUi != null && pauseUi.isActiveAndEnabled;
        }

        // Check if a chest UI is open
        bool chestOpen()
        {
            ChestWindowUi chestUi = UnityEngine.Object.FindObjectOfType<ChestWindowUi>();
            return chestUi != null
                   && chestUi.window != null
                   && chestUi.window.gameObject.activeInHierarchy;
        }

        // Check if the level up UI is open
        bool levelUpOpen()
        {
            LevelupScreen level = UnityEngine.Object.FindObjectOfType<LevelupScreen>();
            return (level != null && level.window != null && level.window.activeInHierarchy)
                   || LevelupScreen.isLevelingUp;
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