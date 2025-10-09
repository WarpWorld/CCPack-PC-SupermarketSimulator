﻿using System;
using System.Collections;
using System.Linq;
using System.Threading;
using ConnectorLib.JSON;

namespace CrowdControl.Delegates.Effects;

/// <summary>Manages the execution of a timed effect.</summary>
public class TimedEffectState
{
    public readonly EffectRequest Request;
    public readonly SITimeSpan Duration;
    public readonly Effect Effect;
    public readonly NetworkClient Client;
    
    public SITimeSpan TimeRemaining;
    
    public enum EffectState
    {
        NotStarted,
        Running,
        Paused,
        Finished,
        Errored
    }

    public EffectState State { get; private set; } = EffectState.NotStarted;
    
    private int m_stateLock;
    
    private bool TryGetLock() => Interlocked.CompareExchange(ref m_stateLock, 1, 0) == 0;
    private void ReleaseLock() => m_stateLock = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimedEffectState"/> class.
    /// </summary>
    /// <param name="effect">The effect handler.</param>
    /// <param name="request">The effect request.</param>
    /// <param name="duration">The duration of the timed effect.</param>
    public TimedEffectState(Effect effect, EffectRequest request, SITimeSpan duration)
    {
        Effect = effect;
        Client = effect.Client;
        Request = request;
        Duration = duration;
        TimeRemaining = duration;
    }

    /// <summary>Runs the timed thread and starts the execution of the effect.</summary>
    public IEnumerator Start()
    {
        EffectResponse response = null;
        bool locked = false;
        try
        {
            // ReSharper disable once AssignmentInConditionalExpression
            while (!(locked = TryGetLock())) yield return null;
            if (State != EffectState.NotStarted) yield break;

            try
            {
                response = Effect.Start(Request);
                TimeRemaining = Duration;
                State = EffectState.Running;
            }
            catch (Exception e)
            {
                response = EffectResponse.Failure(Request.id, StandardErrors.ExceptionThrown);
                CrowdControlMod.Instance.Logger.Error(e.Message);
                State = EffectState.Errored;
            }
        }
        finally
        {
            if (locked)
            {
                ReleaseLock();
                Client.Send(response);
            }
        }
    }

    /// <summary>Pauses the current timed effect.</summary>
    public IEnumerator Pause()
    {
        EffectResponse response = null;
        bool locked = false;
        try
        {
            // ReSharper disable once AssignmentInConditionalExpression
            while (!(locked = TryGetLock())) yield return null;
            if (State != EffectState.Running) yield break;

            try
            {
                response = Effect.Pause(Request);
                State = EffectState.Paused;
            }
            catch (Exception e)
            {
                response = EffectResponse.Failure(Request.id, StandardErrors.ExceptionThrown);
                CrowdControlMod.Instance.Logger.Error(e.Message);
                State = EffectState.Errored;
            }
        }
        finally
        {
            if (locked)
            {
                ReleaseLock();
                Client.Send(response);
            }
        }
    }

    /// <summary>Unpauses the current timed effect.</summary>
    public IEnumerator Resume()
    {
        EffectResponse response = null;
        bool locked = false;
        try
        {
            // ReSharper disable once AssignmentInConditionalExpression
            while (!(locked = TryGetLock())) yield return null;
            if (State != EffectState.Paused) yield break;

            try
            {
                response = Effect.Resume(Request);
                State = EffectState.Running;
            }
            catch (Exception e)
            {
                response = EffectResponse.Failure(Request.id, StandardErrors.ExceptionThrown);
                CrowdControlMod.Instance.Logger.Error(e.Message);
                State = EffectState.Errored;
            }
        }
        finally
        {
            if (locked)
            {
                ReleaseLock();
                Client.Send(response);
            }
        }
    }

    /// <summary>Stops the current timed effect early.</summary>
    /// <remarks>This should not be called unless the effect terminates prematurely.</remarks>
    public IEnumerator Stop()
    {
        EffectResponse response = null;
        bool locked = false;
        try
        {
            // ReSharper disable once AssignmentInConditionalExpression
            while (!(locked = TryGetLock())) yield return null;
            if (State == EffectState.Finished) yield break;

            try
            {
                response = Effect.Stop(Request) ?? EffectResponse.Finished(Request.id);
                State = EffectState.Finished;
            }
            catch (Exception e)
            {
                response = EffectResponse.Failure(Request.id, StandardErrors.ExceptionThrown);
                CrowdControlMod.Instance.Logger.Error(e.Message);
                State = EffectState.Errored;
            }
        }
        finally
        {
            if (locked)
            {
                ReleaseLock();
                Client.Send(response);
            }
        }
    }

    private static readonly IEnumerator EMPTY_ENUMERATOR = Enumerable.Empty<object>().GetEnumerator();
    
    /// <summary>Advances the time of the current timed effect and executes the effect.</summary>
    public IEnumerator Tick()
    {
        EffectResponse response = null;
        bool locked = false;
        try
        {
            // ReSharper disable once AssignmentInConditionalExpression
            // ReSharper disable once NotDisposedResourceIsReturned - it's the empty singleton enumerator
            while (!(locked = TryGetLock())) return EMPTY_ENUMERATOR;

            switch (State)
            {
                case EffectState.Running when !CrowdControlMod.Instance.GameStateManager.IsReady(Request.code):
                    return Pause();
                case EffectState.Paused when CrowdControlMod.Instance.GameStateManager.IsReady(Request.code):
                    return Resume();
                case EffectState.Running:
                {
                    try
                    {
                        if (TimeRemaining > 0)
                        {
                            Effect.Tick(Request);
                            TimeRemaining -= CrowdControlMod.DeltaTime;
                        }
                        else
                        {
                            response = Effect.Stop(Request) ?? EffectResponse.Finished(Request.id);
                            State = EffectState.Finished;
                            TimeRemaining = SITimeSpan.Zero;
                        }
                    }
                    catch (Exception e)
                    {
                        response = EffectResponse.Failure(Request.id, StandardErrors.ExceptionThrown);
                        CrowdControlMod.Instance.Logger.Error(e.Message);
                        State = EffectState.Errored;
                    }
                    break;
                }
            }
            return EMPTY_ENUMERATOR;
        }
        finally
        {
            if (locked)
            {
                ReleaseLock();
                if(response!=null)Client.Send(response);
            }
        }
    }
}
