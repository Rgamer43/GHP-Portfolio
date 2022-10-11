public class Help {
    public static String generic = "The following commands will give more specific help: \n" +
                                   "    [help cmds] - lists all general commands. These can be used anywhere. \n" +
                                   "    [help stats] - displays what each stat does. \n" +
                                   "    [help aliases] - displays help about aliases. \n" +
                                   "    [help ability] - displays info about your class ability";

    public static String cmds = " The following commands can be used anywhere: \n" +
                                "   [help <section>] - displays help message for specified section. Leave out section \n" +
                                "       for general help \n" +
                                "   [inventory] or [inv] - displays your inventory, armor, weapon, and gold \n" +
                                "   [inspect <item>] or [ins <item>] - displays item details of an item that is either \n" +
                                "       equipped or in your inventory \n" +
                                "   [equip <item>] - equips an item in its relevant slot \n" +
                                "   [chat <message>] - sends a message to all other players in your location \n" +
                                "   [attack <target>] - attack an entity in you location \n" +
                                "   [hp] - displays your hp and max hp \n" +
                                "   [ac] - displays your Armor Class (AC) \n" +
                                "   [stats] - displays your ability scores" +
                                "   [use <item>] - activates specified item \n" +
                                "   [enter <location>] - enter a location that has an exit to your current location \n" +
                                "   [logout] - logs out of RMUD and quits the application. PLEASE use this instead of just quitting \n" +
                                "       the application. \n" +
                                "   [ability] - activates your class ability \n";

    public static String stats = "There are 6 stats in the game, each with a different function: \n" +
                                "   STR - Affects attack bonuses and damage on some weapons \n" +
                                "   DEX - Affects attack bonuses and damage on some weapons \n" +
                                "   CON - Increases your max HP by 5 \n" +
                                "   INT - Affects attack bonuses and damage on some weapons \n" +
                                "   AGI - Increases your AC by 1 \n" +
                                "   DEF - reduces damage taken by 5%. Capped at 19";

    public static String aliases = "Alaises are shorter versions of names. they are the thing in parantheses after a \n" +
                                   "name. Use them in place of something's name";

    public static String intro = "How To Play: Look through the other help menus (run help to see them). Enter the \n" +
            "dungeon  and slay monsters in order to get gold. Purchase ability crystals from the alchemist to increase your stats.";
}
