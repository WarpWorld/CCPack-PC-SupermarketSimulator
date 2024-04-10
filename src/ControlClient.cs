/*
 * ControlValley
 * Stardew Valley Support for Twitch Crowd Control
 * Copyright (C) 2021 TerribleTable
 * LGPL v2.1
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;


namespace BepinControl
{
    public class ControlClient
    {
        public static readonly string CV_HOST = "127.0.0.1";
        public static readonly int CV_PORT = 51337;

        private Dictionary<string, CrowdDelegate> Delegate { get; set; }
        private IPEndPoint Endpoint { get; set; }
        private Queue<CrowdRequest> Requests { get; set; }
        private bool Running { get; set; }

        private bool paused = false;
        public static Socket Socket { get; set; }

        public bool inGame = true;

        public ControlClient()
        {
            Endpoint = new IPEndPoint(IPAddress.Parse(CV_HOST), CV_PORT);
            Requests = new Queue<CrowdRequest>();
            Running = true;
            Socket = null;

            Delegate = new Dictionary<string, CrowdDelegate>()
            {
                //when an effect comes in with the code it will call the paired function
                {"money100", CrowdDelegates.Money100},
                {"money1000", CrowdDelegates.Money1000},
                {"money10000", CrowdDelegates.Money10000},
                {"money-100", CrowdDelegates.MoneyN100},
                {"money-1000", CrowdDelegates.MoneyN1000},
                {"money-10000", CrowdDelegates.MoneyN10000},

                {"ultraslow", CrowdDelegates.SpeedUltraSlow},
                {"slow", CrowdDelegates.SpeedSlow},
                {"fast", CrowdDelegates.SpeedFast},
                {"ultrafast", CrowdDelegates.SpeedUltraFast},

                {"highfov", CrowdDelegates.HighFOV},
                {"lowfov", CrowdDelegates.LowFOV},

                {"setlanguage_english", CrowdDelegates.SetLanguage},
                {"setlanguage_german", CrowdDelegates.SetLanguage},
                {"setlanguage_french", CrowdDelegates.SetLanguage},
                {"setlanguage_italiano", CrowdDelegates.SetLanguage},
                {"setlanguage_espanol", CrowdDelegates.SetLanguage},
                {"setlanguage_portugal", CrowdDelegates.SetLanguage},
                {"setlanguage_brazil", CrowdDelegates.SetLanguage},
                {"setlanguage_nederlands", CrowdDelegates.SetLanguage},
                {"setlanguage_turkce", CrowdDelegates.SetLanguage},

                {"plushour", CrowdDelegates.PlusHour},
                {"minushour", CrowdDelegates.MinusHour},

                {"open", CrowdDelegates.OpenStore},
                {"close", CrowdDelegates.CloseStore},

                {"box_cereal", CrowdDelegates.SendBox},
                {"box_bread", CrowdDelegates.SendBox},
                {"box_milk", CrowdDelegates.SendBox},
                {"box_soda", CrowdDelegates.SendBox},
                {"box_eggs", CrowdDelegates.SendBox},
                {"box_salmon", CrowdDelegates.SendBox},
                {"box_mayo", CrowdDelegates.SendBox},
                {"box_whiskey", CrowdDelegates.SendBox},
                {"box_book", CrowdDelegates.SendBox},
                {"box_toilet", CrowdDelegates.SendBox},
                {"box_cat", CrowdDelegates.SendBox},
                {"box_lasag", CrowdDelegates.SendBox},


                {"playerbox_cereal", CrowdDelegates.PlayerSendBox},
                {"playerbox_bread", CrowdDelegates.PlayerSendBox},
                {"playerbox_milk", CrowdDelegates.PlayerSendBox},
                {"playerbox_soda", CrowdDelegates.PlayerSendBox},
                {"playerbox_eggs", CrowdDelegates.PlayerSendBox},
                {"playerbox_salmon", CrowdDelegates.PlayerSendBox},
                {"playerbox_mayo", CrowdDelegates.PlayerSendBox},
                {"playerbox_whiskey", CrowdDelegates.PlayerSendBox},
                {"playerbox_book", CrowdDelegates.PlayerSendBox},
                {"playerbox_toilet", CrowdDelegates.PlayerSendBox},
                {"playerbox_cat", CrowdDelegates.PlayerSendBox},
                {"playerbox_lasag", CrowdDelegates.PlayerSendBox},

                {"playeremptybox_eggs", CrowdDelegates.PlayerSendEmptyBox},
                {"playeremptybox_cereal", CrowdDelegates.PlayerSendEmptyBox},
                {"playeremptybox_toilet", CrowdDelegates.PlayerSendEmptyBox},

                {"invertx", CrowdDelegates.InvertX},
                {"inverty", CrowdDelegates.InvertY},

                {"forceexactchange", CrowdDelegates.ForceExactChange},
                {"forcerequirechange", CrowdDelegates.ForceRequireChange},
                {"allowmischarges", CrowdDelegates.AllowMischarges},
                {"forcelargebills", CrowdDelegates.ForceLargeBills},


                {"teleport_outsidestore", CrowdDelegates.TeleportPlayer},
                {"teleport_acrossstreet", CrowdDelegates.TeleportPlayer},
                {"teleport_faraway", CrowdDelegates.TeleportPlayer},
                {"teleport_computer", CrowdDelegates.TeleportPlayer},

                {"forcepayment_cash", CrowdDelegates.ForcePaymentType},
                {"forcepayment_card", CrowdDelegates.ForcePaymentType},

                {"forcemath", CrowdDelegates.ForceMath},

                // {"closecheckout", CrowdDelegates.CloseCheckout},
                // {"opencheckout", CrowdDelegates.OpenCheckout},

                {"throw", CrowdDelegates.ThrowItem},
                {"drop", CrowdDelegates.DropItem},

                {"spawn", CrowdDelegates.SpawnCustomer},
                {"despawn", CrowdDelegates.DespawnCustomer},
                {"theft", CrowdDelegates.Theft},
                {"robbery", CrowdDelegates.Robbery},
                {"soup", CrowdDelegates.Soup},
                {"boneless", CrowdDelegates.Boneless},
                {"breakfast", CrowdDelegates.Breakfast},

                {"lightson", CrowdDelegates.LightsOn},
                {"lightsoff", CrowdDelegates.LightsOff},

                {"upgrade", CrowdDelegates.UpgradeStore},
                {"upgradeb", CrowdDelegates.UpgradeStorage},

                {"hirecashier", CrowdDelegates.HireCashier},
                {"firecashier", CrowdDelegates.FireCashier},
                {"hirerestocker", CrowdDelegates.HireRestocker},
                {"firerestocker", CrowdDelegates.FireRestocker},

                {"pricesup", CrowdDelegates.RaisePrices},
                {"pricesdown", CrowdDelegates.LowerPrices},
                {"priceup", CrowdDelegates.RaisePrice},
                {"pricedown", CrowdDelegates.LowerPrice},

                {"removeitem", CrowdDelegates.RemoveItem},
                {"additem", CrowdDelegates.AddItem},
            };
        }

        public bool isReady()
        {
            try
            {
                //add check for whether the game is in a state it can accept effects
            }
            catch (Exception e)
            {
                TestMod.mls.LogError(e.ToString());
                return false;
            }

            return true;
        }

        public static void HideEffect(string code)
        {
            CrowdResponse res = new CrowdResponse(0, CrowdResponse.Status.STATUS_NOTVISIBLE);
            res.type = 1;
            res.code = code;
            res.Send(Socket);
        }

        public static void ShowEffect(string code)
        {
            CrowdResponse res = new CrowdResponse(0, CrowdResponse.Status.STATUS_VISIBLE);
            res.type = 1;
            res.code = code;
            res.Send(Socket);
        }

        public static void DisableEffect(string code)
        {
            CrowdResponse res = new CrowdResponse(0, CrowdResponse.Status.STATUS_NOTSELECTABLE);
            res.type = 1;
            res.code = code;
            res.Send(Socket);
        }

        public static void EnableEffect(string code)
        {
            CrowdResponse res = new CrowdResponse(0, CrowdResponse.Status.STATUS_SELECTABLE);
            res.type = 1;
            res.code = code;
            res.Send(Socket);
        }

        private void ClientLoop()
        {

            TestMod.mls.LogInfo("Connected to Crowd Control");

            var timer = new Timer(timeUpdate, null, 0, 200);

            try
            {
                while (Running)
                {
                    CrowdRequest req = CrowdRequest.Recieve(this, Socket);
                    if (req == null || req.IsKeepAlive()) continue;

                    lock (Requests)
                        Requests.Enqueue(req);
                }
            }
            catch (Exception)
            {
                TestMod.mls.LogInfo("Disconnected from Crowd Control");
                Socket.Close();
            }
        }

        public void timeUpdate(System.Object state)
        {
            inGame = true;

            if (!isReady()) inGame = false;

            if (!inGame)
            {
                TimedThread.addTime(200);
                paused = true;
            }
            else if (paused)
            {
                paused = false;
                TimedThread.unPause();
                TimedThread.tickTime(200);
            }
            else
            {
                TimedThread.tickTime(200);
            }
        }

        public bool IsRunning() => Running;

        public void NetworkLoop()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            while (Running)
            {

                TestMod.mls.LogInfo("Attempting to connect to Crowd Control");

                try
                {
                    Socket = new Socket(Endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                    if (Socket.BeginConnect(Endpoint, null, null).AsyncWaitHandle.WaitOne(10000, true) && Socket.Connected)
                        ClientLoop();
                    else
                        TestMod.mls.LogInfo("Failed to connect to Crowd Control");
                    Socket.Close();
                }
                catch (Exception e)
                {
                    TestMod.mls.LogInfo(e.GetType().Name);
                    TestMod.mls.LogInfo("Failed to connect to Crowd Control");
                }

                Thread.Sleep(10000);
            }
        }

        public void RequestLoop()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            while (Running)
            {
                try
                {

                    CrowdRequest req = null;
                    lock (Requests)
                    {
                        if (Requests.Count == 0)
                            continue;
                        req = Requests.Dequeue();
                    }

                    string code = req.GetReqCode();
                    try
                    {
                        CrowdResponse res;
                        if (!isReady())
                            res = new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY);
                        else
                            res = Delegate[code](this, req);
                        if (res == null)
                        {
                            new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, $"Request error for '{code}'").Send(Socket);
                        }

                        res.Send(Socket);
                    }
                    catch (KeyNotFoundException)
                    {
                        new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, $"Request error for '{code}'").Send(Socket);
                    }
                }
                catch (Exception)
                {
                    TestMod.mls.LogInfo("Disconnected from Crowd Control");
                    Socket.Close();
                }
            }
        }

        public void Stop()
        {
            Running = false;
        }

    }
}
