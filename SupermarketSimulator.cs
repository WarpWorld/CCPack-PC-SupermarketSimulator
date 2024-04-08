using System;
using System.Collections.Generic;
using CrowdControl.Common;
using ConnectorType = CrowdControl.Common.ConnectorType;

namespace CrowdControl.Games.Packs.SupermarketSimulator
{

    public class SupermarketSimulator : SimpleTCPPack
    {
        public override string Host => "127.0.0.1";

        public override ushort Port => 51337;

        public override ISimpleTCPPack.MessageFormat MessageFormat => ISimpleTCPPack.MessageFormat.CrowdControlLegacy;

        public SupermarketSimulator(UserRecord player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }

        public override Game Game { get; } = new("Supermarket Simulator", "SupermarketSimulator", "PC", ConnectorType.SimpleTCPServerConnector);

        public override EffectList Effects => new List<Effect>
        {
                new Effect("Give $100", "money100") { Category = "Money"},
                new Effect("Give $1000", "money1000") { Category = "Money"},
                new Effect("Give $10000", "money10000") { Category = "Money"},
                new Effect("Take $100", "money-100") { Category = "Money"},
                new Effect("Take $1000", "money-1000") { Category = "Money"},
                new Effect("Take $10000", "money-10000") { Category = "Money"},

                new Effect("Game Speed: Ultra Slow", "ultraslow") { Duration = 15, Category = "Game Speed"},
                new Effect("Game Speed: Slow", "slow") { Duration = 30, Category = "Game Speed"},
                new Effect("Game Speed: Fast", "fast") { Duration = 30, Category = "Game Speed"},
                new Effect("Game Speed: Ultra Fast", "ultrafast") { Duration = 15, Category = "Game Speed"},

                new Effect("High FOV", "highfov") { Duration = 30, Category = "Game FOV"},
                new Effect("Low FOV", "lowfov") { Duration = 30, Category = "Game FOV"},

                new Effect("Set Language to English", "setlanguage_english") { Duration = 5, Category = "Game Language"},
                new Effect("Set Language to German", "setlanguage_german") { Duration = 5, Category = "Game Language"},
                new Effect("Set Language to French", "setlanguage_french") { Duration = 5, Category = "Game Language"},
                new Effect("Set Language to Italiano", "setlanguage_italiano") { Duration = 5, Category = "Game Language"},
                new Effect("Set Language to Espanol", "setlanguage_espanol") { Duration = 5, Category = "Game Language"},
                new Effect("Set Language to Portugal", "setlanguage_portugal") { Duration = 5, Category = "Game Language"},
                new Effect("Set Language to Brazil", "setlanguage_brazil") { Duration = 5, Category = "Game Language"},
                new Effect("Set Language to Nederlands", "setlanguage_nederlands") { Duration = 5, Category = "Game Language"},
                new Effect("Set Language to Turkce", "setlanguage_turkce") { Duration = 5, Category = "Game Language"},


                new Effect("Advance Time One Hour", "plushour") { Category = "Time"},
                new Effect("Rollback Time One Hour", "minushour") { Category = "Time"},

                new Effect("Open Store", "open") { Category = "Store"},
                new Effect("Close Store", "close") { Category = "Store"},
                new Effect("Turn Lights On", "lightson") { Category = "Store"},
                new Effect("Turn Lights Off", "lightsoff") { Category = "Store"},
                new Effect("Upgrade Store", "upgrade") { Category = "Store"},
                new Effect("Upgrade Storage", "upgradeb") { Category = "Store"},

                new Effect("Deliver Cereal", "box_cereal") { Category = "Boxes"},
                new Effect("Deliver Bread", "box_bread") { Category = "Boxes"},
                new Effect("Deliver Milk", "box_milk") { Category = "Boxes"},
                new Effect("Deliver Soda", "box_soda") { Category = "Boxes"},
                new Effect("Deliver Eggs", "box_eggs") { Category = "Boxes"},
                new Effect("Deliver Salmon", "box_salmon") { Category = "Boxes"},
                new Effect("Deliver Mayo", "box_mayo") { Category = "Boxes"},
                new Effect("Deliver Whiskey", "box_whiskey") { Category = "Boxes"},
                new Effect("Deliver Books", "box_book") { Category = "Boxes"},
                new Effect("Deliver Toilet Paper", "box_toilet") { Category = "Boxes"},
                new Effect("Deliver Cat Food", "box_cat") { Category = "Boxes"},
                new Effect("Deliver Lasagna", "box_lasag") { Category = "Boxes"},



                new Effect("Deliver Cereal to Player", "playerbox_cereal") { Category = "Player Boxes"},
                new Effect("Deliver Bread to Player", "playerbox_bread") { Category = "Player Boxes"},
                new Effect("Deliver Milk to Player", "playerbox_milk") { Category = "Player Boxes"},
                new Effect("Deliver Soda to Player", "playerbox_soda") { Category = "Player Boxes"},
                new Effect("Deliver Eggs to Player", "playerbox_eggs") { Category = "Player Boxes"},
                new Effect("Deliver Salmon to Player", "playerbox_salmon") { Category = "Player Boxes"},
                new Effect("Deliver Mayo to Player", "playerbox_mayo") { Category = "Player Boxes"},
                new Effect("Deliver Whiskey to Player", "playerbox_whiskey") { Category = "Player Boxes"},
                new Effect("Deliver Books to Player", "playerbox_book") { Category = "Player Boxes"},
                new Effect("Deliver Toilet Paper to Player", "playerbox_toilet") { Category = "Player Boxes"},
                new Effect("Deliver Cat Food to Player", "playerbox_cat") { Category = "Player Boxes"},
                new Effect("Deliver Lasagna to Player", "playerbox_lasag") { Category = "Player Boxes"},


                new Effect("Send Small Empty Box", "playeremptybox_eggs") { Category = "Empty Boxes"},
                new Effect("Send Medium Empty Box", "playeremptybox_cereal") { Category = "Empty Boxes"},
                new Effect("Send Large Empty Box", "playeremptybox_toilet") { Category = "Empty Boxes"},



                new Effect("Teleport Outside Store", "teleport_outsidestore") { Category = "Teleport"},
                new Effect("Teleport Across Street", "teleport_acrossstreet") { Category = "Teleport"},
                new Effect("Teleport Far Away", "teleport_faraway") { Category = "Teleport"},
                new Effect("Teleport to Computer", "teleport_computer") { Category = "Teleport"},

                new Effect("Force Cash Only", "forcepayment_cash") { Duration = 60, Category = "Payments"},
                new Effect("Force Card Only", "forcepayment_card") { Duration = 60, Category = "Payments"},

                new Effect("Force Math", "forcemath") { Description = "Forces the player to do the math for correct change!", Duration = 60, Category = "Math"},

                // new Effect("Close Checkout", "closecheckout") { Category = "Checkout"},
                // new Effect("Open Checkout", "opencheckout") { Category = "Checkout"},

                new Effect("Drop Held Item", "drop") { Category = "Held Items"},
                new Effect("Throw Held Item", "throw") { Category = "Held Items"},

                new Effect("Spawn Customer", "spawn") { Category = "Customers"},
                new Effect("Despawn Customer", "despawn") { Category = "Customers"},
                new Effect("Complain About Theft", "theft") { Category = "Customers"},
                new Effect("This is a robbery!", "robbery") { Category = "Customers"},
                new Effect("I'm at Soup!", "soup") { Category = "Customers"},
                new Effect("It's Breakfast Time!", "breakfast") { Category = "Customers"},
                new Effect("Boneless?", "boneless") { Category = "Customers"},

                new Effect("Hire Cashier", "hirecashier") { Category = "Employees"},
                new Effect("Fire Cashier", "firecashier") { Category = "Employees"},
                new Effect("Hire Restocker", "hirerestocker") { Category = "Employees"},
                new Effect("Fire Restocker", "firerestocker") { Category = "Employees"},

                new Effect("Raise All Prices", "pricesup") { Category = "Prices"},
                new Effect("Lower All Prices", "pricesdown") { Category = "Prices"},
                new Effect("Raise Random Price", "priceup") { Category = "Prices"},
                new Effect("Lower Random Price", "pricedown") { Category = "Prices"},

                new Effect("Remove Item From Shelf", "removeitem") { Category = "Shelves"},
                new Effect("Add Item To Shelf", "additem") { Category = "Shelves"},

        };
    }
}
