
using MyBox;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;

namespace BepinControl
{


    public enum TimedType
    {
        GAME_ULTRA_SLOW,
        GAME_SLOW,
        GAME_FAST,
        GAME_ULTRA_FAST,
        HIGH_FOV,
        LOW_FOV,
        SET_LANGUAGE,
        FORCE_CARD,
        FORCE_CASH,
        FORCE_MATH
    }


    public class Timed
    {
        public TimedType type;
        float old;

        

        private static Dictionary<string, object> customVariables = new Dictionary<string, object>();

        public static T GetCustomVariable<T>(string key)
        {
            if (customVariables.TryGetValue(key, out object value))
            {
                return (T)value;
            }

            throw new KeyNotFoundException($"Custom variable with key '{key}' not found.");
        }

        public void SetCustomVariables(Dictionary<string, object> variables)
        {
            customVariables = variables;
        }

        public Timed(TimedType t) { 
            type = t;
        }

        public void addEffect()
        {
            switch (type)
            {
                case TimedType.SET_LANGUAGE:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            int newLanguage = GetCustomVariable<int>("newLanguage");
                            SettingsMenuManager settingsMenuManager = Singleton<SettingsMenuManager>.Instance;
                            bool isChangingLanguage = (bool)CrowdDelegates.getProperty(settingsMenuManager, "m_ChangingLocale");
                            isChangingLanguage = false;
                            CrowdDelegates.setProperty(settingsMenuManager, "m_ChangingLocale", false);
                            settingsMenuManager.SetLanguage(newLanguage);
                        });
                        break;
                    }
                case TimedType.HIGH_FOV:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            CameraSettings camera = Singleton<CameraSettings>.Instance;
                            camera.SetFOV(140f);
                        });
                        break;
                    }
                case TimedType.LOW_FOV:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            CameraSettings camera = Singleton<CameraSettings>.Instance;
                            camera.SetFOV(30f);
                        });
                        break;
                    }
                case TimedType.FORCE_CASH:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            TestMod.ForceUseCredit = false;
                            TestMod.ForceUseCash = true;
                        });
                        break;
                    }
                case TimedType.FORCE_CARD:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            TestMod.ForceUseCash = false;
                            TestMod.ForceUseCredit = true;
                        });
                        break;
                    }
                case TimedType.FORCE_MATH:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            TestMod.ForceMath = true;
                        });
                        break;
                    }
            }
        }

    
        public static bool removeEffect(TimedType etype)
        {
            try
            {
                switch (etype)
                {
                    case TimedType.GAME_ULTRA_SLOW:
                    case TimedType.GAME_SLOW:
                    case TimedType.GAME_FAST:
                    case TimedType.GAME_ULTRA_FAST:
                        {
                            TestMod.ActionQueue.Enqueue(() =>
                            {
                                try
                                {
                                    Time.timeScale = 1.0f;
                                }
                                catch (Exception e)
                                {
                                    TestMod.mls.LogInfo(e.ToString());
                                    Timed.removeEffect(etype);
                                }
                            });
                            break;
                        }
                    case TimedType.LOW_FOV:
                    case TimedType.HIGH_FOV:
                        {
                            TestMod.ActionQueue.Enqueue(() =>
                            {
                                try
                                {
                                    SaveManager saveManager = Singleton<SaveManager>.Instance;
                                    CameraSettings camera = Singleton<CameraSettings>.Instance;
                                    camera.SetFOV(saveManager.Settings.FOV);
                                }
                                catch (Exception e)
                                {
                                    TestMod.mls.LogInfo(e.ToString());
                                    Timed.removeEffect(etype);
                                }
                            });
                            break;
                        }
                    case TimedType.SET_LANGUAGE:
                        {

                            TestMod.ActionQueue.Enqueue(() =>
                            {
                                try
                                {
                                    // Doesn't always seem to revert, not 100% sure. Might be frame/tick related?
                                    SettingsMenuManager settingsMenuManager = Singleton<SettingsMenuManager>.Instance;
                                    bool isChangingLanguage = (bool)CrowdDelegates.getProperty(settingsMenuManager, "m_ChangingLocale");
                                    int oldLanguage = Timed.GetCustomVariable<int>("oldLanguage");

                                    if (oldLanguage < 0) oldLanguage = 0;

                                    isChangingLanguage = false;
                                    CrowdDelegates.setProperty(settingsMenuManager, "m_ChangingLocale", false);
                                    settingsMenuManager.SetLanguage(oldLanguage);

                                }
                                catch (Exception e)
                                {
                                    TestMod.mls.LogInfo(e.ToString());
                                    Timed.removeEffect(etype);
                                }
                            });
                            break;
                        }
                    case TimedType.FORCE_CASH:
                        {
                            TestMod.ActionQueue.Enqueue(() =>
                            {
                                try
                                {
                                    TestMod.ForceUseCredit = false;
                                    TestMod.ForceUseCash = false;
                                }
                                catch (Exception e)
                                {
                                    TestMod.mls.LogInfo(e.ToString());
                                    Timed.removeEffect(etype);
                                }
                            });
                            break;
                        }
                    case TimedType.FORCE_CARD:
                        {
                            TestMod.ActionQueue.Enqueue(() =>
                            {
                                try { 
                                    TestMod.ForceUseCash = false;
                                    TestMod.ForceUseCredit = false;
                                }
                                catch (Exception e)
                                {
                                    TestMod.mls.LogInfo(e.ToString());
                                    Timed.removeEffect(etype);
                                }
                            });
                            break;
                        }
                    case TimedType.FORCE_MATH:
                        {
                            TestMod.ActionQueue.Enqueue(() =>
                            {
                                try
                                {
                                    TestMod.ForceMath = false;
                                }
                                catch (Exception e)
                                {
                                    TestMod.mls.LogInfo(e.ToString());
                                    Timed.removeEffect(etype);
                                }
                            });
                            break;
                        }
                }
            } catch(Exception e)
            {
                TestMod.mls.LogInfo(e.ToString());
                return false;
            }
            return true;
        }

        static int frames = 0;

        public void tick()
        {
            frames++;

            switch (type)
            {
                case TimedType.GAME_ULTRA_SLOW:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            Time.timeScale = 0.25f;
                        });
                        break;
                    }
                case TimedType.GAME_SLOW:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            Time.timeScale = 0.5f;
                        });
                        break;
                    }
                case TimedType.GAME_FAST:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            Time.timeScale = 2.0f;
                        });
                        break;
                    }
                case TimedType.GAME_ULTRA_FAST:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            Time.timeScale = 4.0f;
                        });
                        break;
                    }
            }
        }
    }
    public class TimedThread
    {
        public static List<TimedThread> threads = new List<TimedThread>();

        public readonly Timed effect;
        public int duration;
        public int remain;
        public int id;
        public bool paused;

        public static bool isRunning(TimedType t)
        {
            foreach (var thread in threads)
            {
                if (thread.effect.type == t) return true;
            }
            return false;
        }


        public static void tick()
        {
            foreach (var thread in threads)
            {
                if (!thread.paused)
                {
                   thread.effect.tick();
                }
            }
        }
        public static void addTime(int duration)
        {
            try
            {
                lock (threads)
                {
                    foreach (var thread in threads)
                    {
                        Interlocked.Add(ref thread.duration, duration+5);
                        if (!thread.paused)
                        {
                            int time = Volatile.Read(ref thread.remain);
                            new TimedResponse(thread.id, time, CrowdResponse.Status.STATUS_PAUSE).Send(ControlClient.Socket);
                            thread.paused = true;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                TestMod.mls.LogInfo(e.ToString());
            }
        }

        public static void tickTime(int duration)
        {
            try
            {
                lock (threads)
                {
                    foreach (var thread in threads)
                    {
                        int time = Volatile.Read(ref thread.remain);
                        time -= duration;
                        if (time < 0) time = 0;
                        Volatile.Write(ref thread.remain, time);
                    }
                }
            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo(e.ToString());
            }
        }

        public static void unPause()
        {
            try
            {
                lock (threads)
                {
                    foreach (var thread in threads)
                    {
                        if (thread.paused)
                        {
                            int time = Volatile.Read(ref thread.remain);
                            new TimedResponse(thread.id, time, CrowdResponse.Status.STATUS_RESUME).Send(ControlClient.Socket);
                            thread.paused = false;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                TestMod.mls.LogInfo(e.ToString());
            }
        }

        public TimedThread(int id, TimedType type, int duration, Dictionary<string, object> customVariables = null)
        {
            this.effect = new Timed(type);
            this.duration = duration;
            this.remain = duration;
            this.id = id;
            paused = false;

            if (customVariables == null)
            {
                customVariables = new Dictionary<string, object>();
            }

            this.effect.SetCustomVariables(customVariables);
            try
            {
                lock (threads)
                {
                    threads.Add(this);
                }
            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo(e.ToString());
            }
        }

        public void Run()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            effect.addEffect();
            bool error = false;
            try
            {
                do
                {
                    error = false;
                    int time = Volatile.Read(ref duration); ;
                    while (time > 0)
                    {
                        Interlocked.Add(ref duration, -time);
                        Thread.Sleep(time);

                        time = Volatile.Read(ref duration);
                    }
                    if (Timed.removeEffect(effect.type))
                    {
                        lock (threads)
                        {
                            threads.Remove(this);
                        }
                        new TimedResponse(id, 0, CrowdResponse.Status.STATUS_STOP).Send(ControlClient.Socket);
                    }
                    else error = true;
                } while (error);
            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo(e.ToString());
            }
        }
    }
}
