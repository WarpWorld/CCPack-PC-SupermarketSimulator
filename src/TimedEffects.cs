
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

        

        private Dictionary<string, object> customVariables = new Dictionary<string, object>();

        public T GetCustomVariable<T>(string key)
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

            }
        }

        public void removeEffect()
        {

            switch (type)
            {
                case TimedType.GAME_ULTRA_SLOW:
                case TimedType.GAME_SLOW:
                case TimedType.GAME_FAST:
                case TimedType.GAME_ULTRA_FAST:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            Time.timeScale = 1.0f;
                        });
                        break;
                    }
                case TimedType.LOW_FOV:
                case TimedType.HIGH_FOV:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            SaveManager saveManager = Singleton<SaveManager>.Instance;
                            CameraSettings camera = Singleton<CameraSettings>.Instance;
                            camera.SetFOV(saveManager.Settings.FOV);
                        });
                        break;
                    }
                case TimedType.SET_LANGUAGE:
                    {

                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            // Doesn't always seem to revert, not 100% sure. Might be frame/tick related?
                            SettingsMenuManager settingsMenuManager = Singleton<SettingsMenuManager>.Instance;
                            bool isChangingLanguage = (bool)CrowdDelegates.getProperty(settingsMenuManager, "m_ChangingLocale");
                            int oldLanguage = GetCustomVariable<int>("oldLanguage");
                            isChangingLanguage = false;
                            settingsMenuManager.SetLanguage(oldLanguage);
                        });
                        break;
                    }
                case TimedType.FORCE_CASH:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            TestMod.ForceUseCredit = false;
                            TestMod.ForceUseCash = false;
                        });
                        break;
                    }
                case TimedType.FORCE_CARD:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            TestMod.ForceUseCash = false;
                            TestMod.ForceUseCredit = false;
                        });
                        break;
                    }
                case TimedType.FORCE_MATH:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            TestMod.ForceMath = false;
                        });
                        break;
                    }
            }
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
                case TimedType.SET_LANGUAGE:
                    {
                        TestMod.ActionQueue.Enqueue(() =>
                        {
                            int newLanguage = GetCustomVariable<int>("newLanguage");
                            SettingsMenuManager settingsMenuManager = Singleton<SettingsMenuManager>.Instance;
                            bool isChangingLanguage = (bool)CrowdDelegates.getProperty(settingsMenuManager, "m_ChangingLocale");
                            isChangingLanguage = false;
                            settingsMenuManager.SetLanguage(newLanguage);
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

            try
            {
                int time = Volatile.Read(ref duration); ;
                while (time > 0)
                {
                    Interlocked.Add(ref duration, -time);
                    Thread.Sleep(time);

                    time = Volatile.Read(ref duration);
                }
                effect.removeEffect();
                lock (threads)
                {
                    threads.Remove(this);
                }
                new TimedResponse(id, 0, CrowdResponse.Status.STATUS_STOP).Send(ControlClient.Socket);
            }
            catch (Exception e)
            {
                TestMod.mls.LogInfo(e.ToString());
            }
        }
    }
}
