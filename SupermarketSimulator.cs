using System.Diagnostics.CodeAnalysis;
using ConnectorLib.SimpleTCP;
using CrowdControl.Common;
using ConnectorType = CrowdControl.Common.ConnectorType;

namespace CrowdControl.Games.Packs.SupermarketSimulator;

public class SupermarketSimulator : SimpleTCPPack<SimpleTCPServerConnector>
{
    public override string Host => "127.0.0.1";
    public override ushort Port => 51337;

    public SupermarketSimulator(UserRecord player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }

    public override Game Game { get; } = new("Supermarket Simulator", "SupermarketSimulator", "PC", ConnectorType.SimpleTCPServerConnector);

    public override EffectList Effects { get; } = new List<Effect>
    {
        new("Give $100", "money100") { Description = "Give the player $100!", Category = "Money", Alignment = (Alignment)Morality.SlightlyHelpful + Orderliness.Controlled },
        new("Give $1000", "money1000") { Description = "Give the player $1,000!", Category = "Money", Alignment = (Alignment)Morality.Helpful + Orderliness.Controlled },
        new("Give $10000", "money10000") { Description = "Give the player $10,000!", Category = "Money", Alignment = (Alignment)Morality.VeryHelpful + Orderliness.Controlled },
        new("Take $100", "money-100") { Description = "Take $100 from the player. They must have $100 or more for this to trigger.", Category = "Money", Alignment = (Alignment)Morality.SlightlyHarmful + Orderliness.Controlled },
        new("Take $1000", "money-1000") { Description = "Take $1,000 from the player. They must have $1,000 or more for this to trigger.", Category = "Money", Alignment = (Alignment)Morality.Harmful + Orderliness.Controlled },
        new("Take $10000", "money-10000") { Description = "Take $10,000 from the player. They must have $10,000 or more for this to trigger.", Category = "Money", Alignment = (Alignment)Morality.VeryHarmful + Orderliness.Controlled },

        new("Game Speed: Ultra Slow", "ultraslow") { Description = "Make everything in the game run very slow!", Duration = 15, Category = "Game Speed", Alignment = (Alignment)Morality.VeryHarmful + Orderliness.Controlled },
        new("Game Speed: Slow", "slow") { Description = "Make everything in the game run slow!", Duration = 30, Category = "Game Speed", Alignment = (Alignment)Morality.Harmful + Orderliness.Controlled },
        new("Game Speed: Fast", "fast") { Description = "Make everything in the game run fast!", Duration = 30, Category = "Game Speed", Alignment = (Alignment)Morality.SlightlyHarmful + Orderliness.Controlled },
        new("Game Speed: Ultra Fast", "ultrafast") { Description = "Make everything in the game run very fast!", Duration = 15, Category = "Game Speed", Alignment = (Alignment)Morality.Harmful + Orderliness.Chaotic },

        new("High FOV", "highfov") { Description = "Set the FOV for the player higher than normal!", Duration = 30, Category = "Game FOV", Alignment = (Alignment)Morality.SlightlyHelpful + Orderliness.Controlled },
        new("Low FOV", "lowfov") { Description = "Set the FOV for the player lower, giving them tunnel vision!", Duration = 30, Category = "Game FOV", Alignment = (Alignment)Morality.Harmful + Orderliness.Controlled },

        new("Set Language to English", "setlanguage_english") { Description = "Set Language to English.", Duration = 60, Category = "Game Language", Alignment = (Alignment)Morality.Neutral + Orderliness.Chaotic },
        new("Set Language to French", "setlanguage_french") { Description = "Set Language to French.", Duration = 60, Category = "Game Language", Alignment = (Alignment)Morality.Neutral + Orderliness.Chaotic },
        new("Set Language to German", "setlanguage_german") { Description = "Set Language to German.", Duration = 60, Category = "Game Language", Alignment = (Alignment)Morality.Neutral + Orderliness.Chaotic },
        new("Set Language to Italian", "setlanguage_italiano") { Description = "Set Language to Italian.", Duration = 60, Category = "Game Language", Alignment = (Alignment)Morality.Neutral + Orderliness.Chaotic },
        new("Set Language to Spanish", "setlanguage_espanol") { Description = "Set Language to Spanish.", Duration = 60, Category = "Game Language", Alignment = (Alignment)Morality.Neutral + Orderliness.Chaotic },
        new("Set Language to Portuguese (Portugal)", "setlanguage_portugal") { Description = "Set Language to Portuguese (Portugal).", Duration = 60, Category = "Game Language", Alignment = (Alignment)Morality.Neutral + Orderliness.Chaotic },
        new("Set Language to Portuguese (Brazil)", "setlanguage_brazil") { Description = "Set Language to Portuguese (Brazil).", Duration = 60, Category = "Game Language", Alignment = (Alignment)Morality.Neutral + Orderliness.Chaotic },
        new("Set Language to Dutch", "setlanguage_nederlands") { Description = "Set Language to Dutch.", Duration = 60, Category = "Game Language", Alignment = (Alignment)Morality.Neutral + Orderliness.Chaotic },
        new("Set Language to Turkey", "setlanguage_turkce") { Description = "Set Language to Turkey.", Duration = 60, Category = "Game Language", Alignment = (Alignment)Morality.Neutral + Orderliness.Chaotic },


        new("Advance Time One Hour", "plushour") { Description = "Advance time One Hour", Category = "Time", Alignment = (Alignment)Morality.Neutral + Orderliness.Controlled /* TODO: Impact may depend on current schedule */ },
        new("Rollback Time One Hour", "minushour") { Description = "Rollback time One Hour", Category = "Time", Alignment = (Alignment)Morality.Neutral + Orderliness.Controlled /* TODO: Impact may depend on current schedule */ },

        new("Open Store", "open") { Description = "Open the store!", Category = "Store", Alignment = (Alignment)Morality.Helpful + Orderliness.Controlled },
        new("Close Store", "close") { Description = "Close the store!", Category = "Store", Alignment = (Alignment)Morality.Harmful + Orderliness.Controlled },
        new("Turn Lights On", "lightson") { Description = "Turn the lights on!", Category = "Store", Alignment = (Alignment)Morality.SlightlyHelpful + Orderliness.Controlled },
        new("Turn Lights Off", "lightsoff") { Description = "Turn the lights off!", Category = "Store", Alignment = (Alignment)Morality.SlightlyHarmful + Orderliness.Controlled },
        new("Upgrade Store", "upgrade") { Description = "Upgrade the store.", Category = "Store", Alignment = (Alignment)Morality.VeryHelpful + Orderliness.Controlled },
        new("Upgrade Storage", "upgradeb") { Description = "Upgrade the storage.", Category = "Store", Alignment = (Alignment)Morality.Helpful + Orderliness.Controlled },
        new("Spawn Garbage", "spawngarbage") { Description = "Spawn random garbage in the store.", Category = "Store", Alignment = (Alignment)Morality.SlightlyHarmful + Orderliness.Chaotic },

        new("Deliver Cereal", "box_cereal") { Category = "Boxes", Alignment = (Alignment)Morality.SlightlyHelpful + Orderliness.Controlled },
        new("Deliver Bread", "box_bread") { Category = "Boxes", Alignment = (Alignment)Morality.SlightlyHelpful + Orderliness.Controlled },
        new("Deliver Milk", "box_milk") { Category = "Boxes", Alignment = (Alignment)Morality.SlightlyHelpful + Orderliness.Controlled },
        new("Deliver Soda", "box_soda") { Category = "Boxes", Alignment = (Alignment)Morality.SlightlyHelpful + Orderliness.Controlled },
        new("Deliver Eggs", "box_eggs") { Category = "Boxes", Alignment = (Alignment)Morality.SlightlyHelpful + Orderliness.Controlled },
        new("Deliver Salmon", "box_salmon") { Category = "Boxes", Alignment = (Alignment)Morality.Helpful + Orderliness.Controlled },
        new("Deliver Mayo", "box_mayo") { Category = "Boxes", Alignment = (Alignment)Morality.SlightlyHelpful + Orderliness.Controlled },
        new("Deliver Whiskey", "box_whiskey") { Category = "Boxes", Alignment = (Alignment)Morality.Helpful + Orderliness.Controlled },
        new("Deliver Books", "box_book") { Category = "Boxes", Alignment = (Alignment)Morality.SlightlyHelpful + Orderliness.Controlled },
        new("Deliver Toilet Paper", "box_toilet") { Category = "Boxes", Alignment = (Alignment)Morality.SlightlyHelpful + Orderliness.Controlled },
        new("Deliver Cat Food", "box_cat") { Category = "Boxes", Alignment = (Alignment)Morality.SlightlyHelpful + Orderliness.Controlled },
        new("Deliver Lasagna", "box_lasag") { Category = "Boxes", Alignment = (Alignment)Morality.Helpful + Orderliness.Controlled },

        new("Clear Unused Boxes", "clearunusedboxes") { Description = "This will clear any boxes not previously touched by the store owner. Does not include furniture boxes.", Category = "Boxes", Alignment = (Alignment)Morality.SlightlyHelpful + Orderliness.Controlled },


        new("Send a Small Empty Box", "playeremptybox_eggs") { Description = "Send a small empty box to the player.", Category = "Player Boxes", Alignment = (Alignment)Morality.Neutral + Orderliness.SlightlyChaotic },
        new("Send a Medium Empty Box", "playeremptybox_cereal") { Description = "Send a small empty box to the player.", Category = "Player Boxes", Alignment = (Alignment)Morality.Neutral + Orderliness.SlightlyChaotic },
        new("Send a Large Empty Box", "playeremptybox_toilet") { Description = "Send a small empty box to the player.", Category = "Player Boxes", Alignment = (Alignment)Morality.Neutral + Orderliness.SlightlyChaotic },

        new("Send Cereal to Player", "playerbox_cereal") { Description = "Send some cereal to the player.", Category = "Player Boxes", Alignment = (Alignment)Morality.Neutral + Orderliness.SlightlyChaotic },
        new("Send Bread to Player", "playerbox_bread") { Description = "Send some bread to the player.", Category = "Player Boxes", Alignment = (Alignment)Morality.Neutral + Orderliness.SlightlyChaotic },
        new("Send Milk to Player", "playerbox_milk") { Description = "Send some milk to the player.", Category = "Player Boxes", Alignment = (Alignment)Morality.Neutral + Orderliness.SlightlyChaotic },
        new("Send Soda to Player", "playerbox_soda") { Description = "Send some soda to the player.", Category = "Player Boxes", Alignment = (Alignment)Morality.Neutral + Orderliness.SlightlyChaotic },
        new("Send Eggs to Player", "playerbox_eggs") { Description = "Send some eggs to the player.", Category = "Player Boxes", Alignment = (Alignment)Morality.Neutral + Orderliness.SlightlyChaotic },
        new("Send Salmon to Player", "playerbox_salmon") { Description = "Send some salmon to the player.", Category = "Player Boxes", Alignment = (Alignment)Morality.Neutral + Orderliness.SlightlyChaotic },
        new("Send Mayo to Player", "playerbox_mayo") { Description = "Send some mayo to the player.", Category = "Player Boxes", Alignment = (Alignment)Morality.Neutral + Orderliness.SlightlyChaotic },
        new("Send Whiskey to Player", "playerbox_whiskey") { Description = "Send some whiskey to the player.", Category = "Player Boxes", Alignment = (Alignment)Morality.Neutral + Orderliness.SlightlyChaotic },
        new("Send Books to Player", "playerbox_book") { Description = "Send some books to the player.", Category = "Player Boxes", Alignment = (Alignment)Morality.Neutral + Orderliness.SlightlyChaotic },
        new("Send Toilet Paper to Player", "playerbox_toilet") { Description = "Send some toilet paper to the player.", Category = "Player Boxes", Alignment = (Alignment)Morality.Neutral + Orderliness.SlightlyChaotic },
        new("Send Cat Food to Player", "playerbox_cat") { Description = "Send some cat food to the player.", Category = "Player Boxes", Alignment = (Alignment)Morality.Neutral + Orderliness.SlightlyChaotic },
        new("Send Lasagna to Player", "playerbox_lasag") { Description = "Send some lasagna to the player.", Category = "Player Boxes", Alignment = (Alignment)Morality.Neutral + Orderliness.SlightlyChaotic },

        new("Teleport Outside Store", "teleport_outsidestore") { Description = "Teleport the player to the delivery spot outside.", Category = "Teleport", Alignment = (Alignment)Morality.Neutral + Orderliness.Controlled },
        new("Teleport Across Street", "teleport_acrossstreet") { Description = "Teleport the player across the street from the store.",Category = "Teleport", Alignment = (Alignment)Morality.Neutral + Orderliness.Controlled },
        new("Teleport Far Away", "teleport_faraway") { Description = "Teleport the player somewhere far away around town!.",Category = "Teleport", Alignment = (Alignment)Morality.Harmful + Orderliness.Chaotic },
        new("Teleport to Computer", "teleport_computer") { Description = "Teleport the player to their computer.",Category = "Teleport", Alignment = (Alignment)Morality.Helpful + Orderliness.Controlled },

        new("Force Cash Only", "forcepayment_cash") { Description = "Force all customers to pay with cash only.", Duration = 60, Category = "Payments", Alignment = (Alignment)Morality.SlightlyHarmful + Orderliness.Controlled },
        new("Force Card Only", "forcepayment_card") { Description = "Force all customers to pay with card only.", Duration = 60, Category = "Payments", Alignment = (Alignment)Morality.Neutral + Orderliness.Controlled /* TODO: Could be SlightlyHelpful if speeds checkout */ },
        new("Force Exact Change", "forceexactchange") { Description = "Force all customers to pay with exact change when paying in cash.", Duration = 60, Category = "Payments", Alignment = (Alignment)Morality.SlightlyHarmful + Orderliness.Controlled },
        new("Customers Overpay", "forcerequirechange") { Description = "Force all customers to not pay with exact change when paying in cash.", Duration = 60, Category = "Payments", Alignment = (Alignment)Morality.Helpful + Orderliness.Controlled /* Assumes store profits from overpay */ },
        new("Force Large Bills", "forcelargebills") { Description = "Force customers that are paying with cash to use \"Large Bills\".", Duration = 60, Category = "Payments", Alignment = (Alignment)Morality.SlightlyHarmful + Orderliness.Controlled },

        new("Allow Credit Mischarges", "allowmischarges") { Description = "Allow the caisher to mischarge the customer by a certain amount.", Duration = 60, Category = "Payments", Alignment = (Alignment)Morality.SlightlyHelpful + Orderliness.Controlled },
        new("Force Math", "forcemath") { Description = "Forces the player to do the math for correct change!", Duration = 60, Category = "Payments", Alignment = (Alignment)Morality.SlightlyHarmful + Orderliness.Controlled },


        new("Invert X-Axis", "invertx") { Description = "Invert the X-Axis of the players controls!", Duration = 30, Category = "Axis Control", Alignment = (Alignment)Morality.SlightlyHarmful + Orderliness.Chaotic },
        new("Invert Y-Axis", "inverty") { Description = "Invert the Y-Axis of the players controls!", Duration = 30, Category = "Axis Control", Alignment = (Alignment)Morality.SlightlyHarmful + Orderliness.Chaotic },

        // new("Close Checkout", "closecheckout") { Category = "Checkout"},
        // new("Open Checkout", "opencheckout") { Category = "Checkout"},

        new("Drop Held Item", "drop") { Category = "Held Items", Alignment = (Alignment)Morality.SlightlyHarmful + Orderliness.Chaotic },
        new("Throw Held Item", "throw") { Category = "Held Items", Alignment = (Alignment)Morality.SlightlyHarmful + Orderliness.Chaotic },

        new("Spawn Customer", "spawn") { Category = "Customers", Alignment = (Alignment)Morality.SlightlyHelpful + Orderliness.Controlled },
        new("Despawn Customer", "despawn") { Category = "Customers", Alignment = (Alignment)Morality.SlightlyHarmful + Orderliness.Controlled },
        new("Complain About Theft", "theft") { Category = "Customers", Alignment = (Alignment)Morality.Neutral + Orderliness.Neutral },
        new("This is a robbery!", "robbery") { Category = "Customers", Alignment = (Alignment)Morality.Neutral + Orderliness.Neutral },
        new("I'm at Soup!", "soup") { Category = "Customers", Alignment = (Alignment)Morality.Neutral + Orderliness.Neutral },
        new("It's Breakfast Time!", "breakfast") { Category = "Customers", Alignment = (Alignment)Morality.Neutral + Orderliness.Neutral },
        new("Boneless?", "boneless") { Category = "Customers", Alignment = (Alignment)Morality.Neutral + Orderliness.Neutral },

        new("Hire Cashier", "hirecashier") { Category = "Employees", Alignment = (Alignment)Morality.Helpful + Orderliness.Controlled },
        new("Fire Cashier", "firecashier") { Category = "Employees", Alignment = (Alignment)Morality.Harmful + Orderliness.Controlled },
        new("Hire Restocker", "hirerestocker") { Category = "Employees", Alignment = (Alignment)Morality.Helpful + Orderliness.Controlled },
        new("Fire Restocker", "firerestocker") { Category = "Employees", Alignment = (Alignment)Morality.Harmful + Orderliness.Controlled },

        new("Hire Customer Helper", "hirecustomerhelper") { Category = "Employees", Alignment = (Alignment)Morality.SlightlyHelpful + Orderliness.Controlled },
        new("Fire Customer Helper", "firecustomerhelper") { Category = "Employees", Alignment = (Alignment)Morality.SlightlyHarmful + Orderliness.Controlled },

        new("Raise All Prices", "pricesup") { Category = "Prices", Alignment = (Alignment)Morality.Neutral + Orderliness.Controlled },
        new("Lower All Prices", "pricesdown") { Category = "Prices", Alignment = (Alignment)Morality.Neutral + Orderliness.Controlled },
        new("Raise Random Price", "priceup") { Category = "Prices", Alignment = (Alignment)Morality.Neutral + Orderliness.Chaotic },
        new("Lower Random Price", "pricedown") { Category = "Prices", Alignment = (Alignment)Morality.Neutral + Orderliness.Chaotic },

        new("Remove Item From Shelf", "removeitem") { Category = "Shelves", Alignment = (Alignment)Morality.SlightlyHarmful + Orderliness.Controlled },
        new("Add Item To Shelf", "additem") { Category = "Shelves", Alignment = (Alignment)Morality.SlightlyHelpful + Orderliness.Controlled },

        new("Hype Train", "event-hype-train") { Alignment = (Alignment)Morality.Neutral + Orderliness.Neutral }
    };
}