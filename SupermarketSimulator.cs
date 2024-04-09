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
                new Effect("Give $100", "money100") { Description = "Give the player $100!", Category = "Money"},
                new Effect("Give $1000", "money1000") { Description = "Give the player $1,000!", Category = "Money"},
                new Effect("Give $10000", "money10000") { Description = "Give the player $10,000!", Category = "Money"},
                new Effect("Take $100", "money-100") { Description = "Take $100 from the player. They must have $100 or more for this to trigger.", Category = "Money"},
                new Effect("Take $1000", "money-1000") { Description = "Take $1,000 from the player. They must have $1,000 or more for this to trigger.", Category = "Money"},
                new Effect("Take $10000", "money-10000") { Description = "Take $10,000 from the player. They must have $10,000 or more for this to trigger.", Category = "Money"},

                new Effect("Game Speed: Ultra Slow", "ultraslow") { Description = "Make everything in the game run very slow!", Duration = 15, Category = "Game Speed"},
                new Effect("Game Speed: Slow", "slow") { Description = "Make everything in the game run slow!", Duration = 30, Category = "Game Speed"},
                new Effect("Game Speed: Fast", "fast") { Description = "Make everything in the game run fast!", Duration = 30, Category = "Game Speed"},
                new Effect("Game Speed: Ultra Fast", "ultrafast") { Description = "Make everything in the game run very fast!", Duration = 15, Category = "Game Speed"},

                new Effect("High FOV", "highfov") { Description = "Set the FOV for the player higher than normal!", Duration = 30, Category = "Game FOV"},
                new Effect("Low FOV", "lowfov") { Description = "Set the FOV for the player lower, giving them tunnel vision!", Duration = 30, Category = "Game FOV"},

                new Effect("Set Language to English", "setlanguage_english") { Description = "Set Language to English.", Duration = 60, Category = "Game Language"},
                new Effect("Set Language to French", "setlanguage_french") { Description = "Set Language to French.", Duration = 60, Category = "Game Language"},
                new Effect("Set Language to German", "setlanguage_german") { Description = "Set Language to German.", Duration = 60, Category = "Game Language"},
                new Effect("Set Language to Italian", "setlanguage_italiano") { Description = "Set Language to Italian.", Duration = 60, Category = "Game Language"},
                new Effect("Set Language to Spanish", "setlanguage_espanol") { Description = "Set Language to Spanish.", Duration = 60, Category = "Game Language"},
                new Effect("Set Language to Portuguese (Portugal)", "setlanguage_portugal") { Description = "Set Language to Portuguese (Portugal).", Duration = 60, Category = "Game Language"},
                new Effect("Set Language to Portuguese (Brazil)", "setlanguage_brazil") { Description = "Set Language to Portuguese (Brazil).", Duration = 60, Category = "Game Language"},
                new Effect("Set Language to Dutch", "setlanguage_nederlands") { Description = "Set Language to Dutch.", Duration = 60, Category = "Game Language"},
                new Effect("Set Language to Turkey", "setlanguage_turkce") { Description = "Set Language to Turkey.", Duration = 60, Category = "Game Language"},


                new Effect("Advance Time One Hour", "plushour") { Description = "Advance time One Hour", Category = "Time"},
                new Effect("Rollback Time One Hour", "minushour") { Description = "Rollback time One Hour", Category = "Time"},

                new Effect("Open Store", "open") { Description = "Open the store!", Category = "Store"},
                new Effect("Close Store", "close") { Description = "Close the store!", Category = "Store"},
                new Effect("Turn Lights On", "lightson") { Description = "Turn the lights on!", Category = "Store"},
                new Effect("Turn Lights Off", "lightsoff") { Description = "Turn the lights off!", Category = "Store"},
                new Effect("Upgrade Store", "upgrade") { Description = "Upgrade the store.", Category = "Store"},
                new Effect("Upgrade Storage", "upgradeb") { Description = "Upgrade the storage.", Category = "Store"},

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

                new Effect("Send a Small Empty Box", "playeremptybox_eggs") { Description = "Send a small empty box to the player.", Category = "Player Boxes"},
                new Effect("Send a Medium Empty Box", "playeremptybox_cereal") { Description = "Send a small empty box to the player.", Category = "Player Boxes"},
                new Effect("Send a Large Empty Box", "playeremptybox_toilet") { Description = "Send a small empty box to the player.", Category = "Player Boxes"},

                new Effect("Send Cereal to Player", "playerbox_cereal") { Description = "Send some cereal to the player.", Category = "Player Boxes"},
                new Effect("Send Bread to Player", "playerbox_bread") { Description = "Send some bread to the player.", Category = "Player Boxes"},
                new Effect("Send Milk to Player", "playerbox_milk") { Description = "Send some milk to the player.", Category = "Player Boxes"},
                new Effect("Send Soda to Player", "playerbox_soda") { Description = "Send some soda to the player.", Category = "Player Boxes"},
                new Effect("Send Eggs to Player", "playerbox_eggs") { Description = "Send some eggs to the player.", Category = "Player Boxes"},
                new Effect("Send Salmon to Player", "playerbox_salmon") { Description = "Send some salmon to the player.", Category = "Player Boxes"},
                new Effect("Send Mayo to Player", "playerbox_mayo") { Description = "Send some mayo to the player.", Category = "Player Boxes"},
                new Effect("Send Whiskey to Player", "playerbox_whiskey") { Description = "Send some whiskey to the player.", Category = "Player Boxes"},
                new Effect("Send Books to Player", "playerbox_book") { Description = "Send some books to the player.", Category = "Player Boxes"},
                new Effect("Send Toilet Paper to Player", "playerbox_toilet") { Description = "Send some toilet paper to the player.", Category = "Player Boxes"},
                new Effect("Send Cat Food to Player", "playerbox_cat") { Description = "Send some cat food to the player.", Category = "Player Boxes"},
                new Effect("Send Lasagna to Player", "playerbox_lasag") { Description = "Send some lasagna to the player.", Category = "Player Boxes"},


                new Effect("Teleport Outside Store", "teleport_outsidestore") { Description = "Teleport the player to the delivery spot outside.", Category = "Teleport"},
                new Effect("Teleport Across Street", "teleport_acrossstreet") { Description = "Teleport the player across the street from the store.",Category = "Teleport"},
                new Effect("Teleport Far Away", "teleport_faraway") { Description = "Teleport the player somewhere far away around town!.",Category = "Teleport"},
                new Effect("Teleport to Computer", "teleport_computer") { Description = "Teleport the player to their computer.",Category = "Teleport"},

                new Effect("Force Cash Only", "forcepayment_cash") { Description = "Force all customers to pay with cash only.", Duration = 60, Category = "Payments"},
                new Effect("Force Card Only", "forcepayment_card") { Description = "Force all customers to pay with card only.", Duration = 60, Category = "Payments"},
                new Effect("Force Exact Change", "forceexactchange") { Description = "Force all customers to pay with exact change when paying in cash.", Duration = 60, Category = "Payments"},
                new Effect("Allow Credit Mischarges", "allowmischarges") { Description = "Allow the caisher to mischarge the customer by a certain amount.", Duration = 60, Category = "Payments"},
                new Effect("Force Math", "forcemath") { Description = "Forces the player to do the math for correct change!", Duration = 60, Category = "Payments"},


                new Effect("Invert X-Axis", "invertx") { Description = "Invert the X-Axis of the players controls!", Duration = 30, Category = "Axis Control"},
                new Effect("Invert Y-Axis", "inverty") { Description = "Invert the Y-Axis of the players controls!", Duration = 30, Category = "Axis Control"},

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
