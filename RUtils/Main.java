package rgamer43.github.io.rutils;

import ch.qos.logback.classic.Level;
import ch.qos.logback.classic.Logger;
import ch.qos.logback.classic.LoggerContext;
import com.mongodb.ConnectionString;
import com.mongodb.client.*;
import com.mongodb.client.MongoClient;
import com.mongodb.client.model.Updates;
import net.dv8tion.jda.api.JDA;
import net.dv8tion.jda.api.JDABuilder;
import net.dv8tion.jda.api.entities.Activity;
import net.dv8tion.jda.api.entities.Guild;
import net.dv8tion.jda.api.entities.Member;
import net.dv8tion.jda.api.entities.emoji.Emoji;
import net.dv8tion.jda.api.events.Event;
import net.dv8tion.jda.api.events.interaction.command.SlashCommandInteractionEvent;
import net.dv8tion.jda.api.events.message.MessageReceivedEvent;
import net.dv8tion.jda.api.hooks.ListenerAdapter;
import net.dv8tion.jda.api.interactions.commands.OptionType;
import net.dv8tion.jda.api.interactions.commands.build.Commands;
import net.dv8tion.jda.api.interactions.commands.build.OptionData;
import net.dv8tion.jda.api.requests.GatewayIntent;
import org.bson.Document;
import org.slf4j.LoggerFactory;

import java.text.DecimalFormat;
import java.time.OffsetDateTime;
import java.time.temporal.ChronoUnit;
import java.time.temporal.TemporalField;
import java.util.*;

import static com.mongodb.client.model.Filters.eq;
import static com.mongodb.client.model.Sorts.descending;

public class Main extends ListenerAdapter {

    public static final int BASE_XP_TO_NEXT_LEVEL = 20, MONEY_PER_LEVEL = 100, LUCKY_MSG_PAYOUT_PER_LEVEL = 10;
    public static final double XP_REQ_SCALER = 1.0815, MONEY_SCALER = 1.1;
    public static final long MSG_COOLDOWN = 5 * 1000;
    public static final String XP_NAME = ":identification_card: Social Credit Score", XP_ABREV = "SCR", MONEY_NAME = "Organs",
        MONEY_SYMBOL = ":anatomical_heart:";
    public static final String[] PING_RESPONSES = {
            "GO AWAY!",
            "NO!",
            "Objectively false!",
            "Objectively true!",
            "Lol no",
            "Hiring hitman...\n\n\nHitman hired!",
            "Ha ha ha ha ha ha",
            "ooh",
            "Go away!",
            "Shut up!",
            "???",
            "Be quiet!",
            "I'm busy!",
            "No u",
            "...",
            MONEY_SYMBOL,
            MONEY_NAME + "!",
            XP_NAME + "!",
            "Greg Abbott is a little piss baby",
            "Greg Abbott is a little piss baby",
            "Greg Abbott is a little piss baby"
    };

    public static  final Role[] roles = {
            new Role("Organ Dealer", 2500),
            new Role("Organ Trader", 10000),
            new Role("Organ Collector", 20000),
            new Role("Organ Boss", 50000),
            new Role("Organ Lord", 100000),
            new Role("new role", 3000),
            new Role("Hollow Knight Cult", 6500),
            new Role("Arsonist", 15000),
            new Role("Wizard", 21000),
            new Role("Bot", 75000),
            new Role("Public Police", 9110),
            new Role("Archmage", 50000),
            new Role("Lich", 100000),
            new Role("Infernal", 666),
            new Role("Abyssal", 666),
            new Role("Hyped for Tome", 1000, true),
            new Role("Supreme Pyromancer", 75000),
            new Role("Millionaire", 1000000),
            new Role("Owner", 35000000)
    };

    static MongoCollection<Document> users;

    static JDABuilder jda;
    static JDA built;

    static boolean sentLastMsg = false;

    public static void main(String[] args) throws InterruptedException {
        System.out.println("Starting...");

        String token = "Redacted";

        System.out.println("Loading Mongo...");
        ConnectionString cs = new ConnectionString("Redacted");
        MongoClient mongo = MongoClients.create(cs);
        MongoDatabase db = mongo.getDatabase("database");
        users = db.getCollection("users");
        System.out.println("Mongo loaded");
        System.out.println("Mongo Cluster Version: " + db.runCommand(new Document("buildInfo", 1)).get("version"));

        LoggerContext loggerContext = (LoggerContext) LoggerFactory.getILoggerFactory();
        Logger rootLogger = loggerContext.getLogger("org.mongodb.driver");
        rootLogger.setLevel(Level.ERROR);

        Thread thread = new Thread(new Runnable() {
            @Override
            public void run() {
                while(true) {
                    try {
                        System.out.println("Restarting bot...");
                        jda = JDABuilder.createDefault(token);
                        jda.enableIntents(GatewayIntent.GUILD_MESSAGES, GatewayIntent.MESSAGE_CONTENT);
                        jda.addEventListeners(new Main());

                        //Build after event listeners are added
                        built = jda.build();
                        built.updateCommands().addCommands(
                                Commands.slash("score", "Check a user's social credit score")
                                        .addOption(OptionType.USER, "user", "Who's score to check"),
                                Commands.slash("rankings", "Get a leaderboard of " + XP_NAME),
                                Commands.slash("wealth", "Get a leaderboard of " + MONEY_NAME),
                                Commands.slash("shop", "View the shop"),
                                Commands.slash("buyrole", "Buy a role with " + MONEY_NAME)
                                        .addOption(OptionType.INTEGER, "roleid", "Which role to buy"),
                                Commands.slash("pay", "Give your " + MONEY_NAME + " to another user")
                                        .addOption(OptionType.USER, "user", "Who to give the " + MONEY_NAME + " to")
                                        .addOption(OptionType.INTEGER, "amount", "How many " + MONEY_NAME + " to give"),
                                Commands.slash("ping", "Check the bot's ping time"),
                                Commands.slash("gamble", "Risk your " + MONEY_NAME + ", in exchange for profit!")
                                    .addOption(OptionType.INTEGER, "amount", "How many " + MONEY_NAME + " to gamble")
                        ).queue();

                        built.getPresence().setActivity(Activity.watching("you"));

                        System.out.println("Bot restarted");
                        try {
                            Thread.sleep(1000 * 60 * 60);

//                            Runtime rt = Runtime.getRuntime();
//                            rt.exec("java -jar RUtils.jar");

                            System.exit(0);
                        } catch (InterruptedException e) {
                            e.printStackTrace();
                        }
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                }
            }
        });

        thread.start();
    }

    public static boolean checkIfUserInDB(String id) {
        FindIterable<Document> result = users.find(eq("discID", id));
        if(result.cursor().hasNext()) {
            System.out.println("User already in DB");
            return true;
        } else {
            System.out.println("User not in DB");

            Document doc = new Document();
            doc.put("discID", id);
            doc.put("xp", 0);
            doc.put("level", 0);
            doc.put("money", 0);
            doc.put("lastMsg", (long)0);
            users.insertOne(doc);

            return false;
        }
    }

    public static boolean checkIfUserInDB(String id, String name) {
        boolean r = checkIfUserInDB(id);
        Document current = users.find(eq("discID", id)).first();
        users.updateOne(current, Updates.set("name", name));
        return r;
    }

    @Override
    public void onMessageReceived(MessageReceivedEvent event) {
        try {
            if (true) {
                System.out.println("Received message event! Msg: " + event.getMessage().getContentRaw());

                if(!event.getAuthor().getId().equals("1023646646162178128"))
                    sentLastMsg = false;
                else sentLastMsg = true;
                if ((event.getMessage().getContentRaw().contains("<@1023646646162178128>") || new Random().nextInt(200) <= 1) && !sentLastMsg) {
                    System.out.println("Adding response to msg...");
                    sentLastMsg = true;

                    Document b = users.find(eq("discID", "1023646646162178128")).first();
                    if(b.get("lastMsg") != null && new Date().getTime() - (long) b.get("lastMsg") > MSG_COOLDOWN) {
                        if (hasRole(event.getGuild(), event.getMember(), "Comrade") && new Random().nextInt(3) >= 2)
                            event.getMessage().reply("Hello comrade!").queue();
                        else
                            event.getMessage().reply(PING_RESPONSES[new Random().nextInt(PING_RESPONSES.length)]).queue();
                    }
                }

                String id = event.getAuthor().getId();
                checkIfUserInDB(id, event.getAuthor().getName());

                Document current = users.find(eq("discID", id)).first();

                if (current.get("lastMsg") != null && new Date().getTime() - (long) current.get("lastMsg") > MSG_COOLDOWN) {
                    System.out.println("User: " + event.getAuthor().getName());
                    System.out.println("Prev XP: " + current.get("xp"));
                    users.updateOne(current, Updates.combine(
                            Updates.set("xp", (int) current.get("xp") + 5), Updates.set("lastMsg", new Date().getTime()))
                    );

                    if (new Random().nextInt(100) + 1 <= (int) current.get("level") + 5) {
                        current = users.find(eq("discID", id)).first();
                        int payout = (int) current.get("level") * LUCKY_MSG_PAYOUT_PER_LEVEL;
                        int money = (int) current.get("money");
                        money += payout;

                        System.out.println("Prev Money: " + current.get("money"));
                        users.updateOne(current, Updates.set("money", money));
//                    event.getMessage().reply("You got lucky! Payout: " + payout).queue();
                        event.getMessage().addReaction(Emoji.fromUnicode("U+1FAC0")).queue();
                        System.out.println(event.getAuthor().getName() + " got a lucky message! Payout: " + payout);

                        current = users.find(eq("discID", id)).first();
                        System.out.println("New Money: " + current.get("money"));
                    }
                    current = users.find(eq("discID", id)).first();
                    System.out.println("New XP: " + current.get("xp"));

                    int level = (int) current.get("level"), xp = (int) current.get("xp"),
                            next = getXPToNextLevel(level);

                    while (xp >= next) {
                        current = users.find(eq("discID", id)).first();
                        level = (int) current.get("level");
                        xp = (int) current.get("xp");
                        next = getXPToNextLevel(level);

                        level++;
                        next = getXPToNextLevel(level);

                        int payment = (int) (Math.pow(level, MONEY_SCALER) * MONEY_PER_LEVEL);
                        if (level == 5) payment += 150;
                        else if (level == 10) payment += 325;
                        else if (level == 20) payment += 650;
                        else if (level == 50) payment *= 2;
                        else if (level == 1000) payment *= 10;

                        String msg = "<@" + id + "> has leveled up! They are now level " + level + "!\n" + XP_NAME + ": " + prettify(xp) + "/" + prettify(next) +
                                ".\n<@" + id + "> has also received " + prettify(payment) + " " + MONEY_SYMBOL + ", for a total of " +
                                prettify((int) current.get("money") + payment) + " " + MONEY_SYMBOL + ".";

                        if (!event.getAuthor().getName().equals("Rgamer43")) {
                            if (level == 5) {
                                event.getGuild().addRoleToMember(
                                        event.getMember(),
                                        event.getGuild().getRolesByName("Citizen", true).get(0)).queue();
                                msg += "\n<@" + id + "> is now a citizen.";
                            } else if (level == 10) {
                                event.getGuild().addRoleToMember(event.getMember(),
                                        event.getGuild().getRolesByName("Trusted Citizen", true).get(0)).queue();
                                msg += "\n<@" + id + "> is now a trusted citizen.";
                            } else if (level == 20) {
                                event.getGuild().addRoleToMember(event.getMember(),
                                        event.getGuild().getRolesByName("Comrade", true).get(0)).queue();
                                msg += "\n<@" + id + "> is now a comrade.";
                            } else if (level == 50) {
                                event.getGuild().addRoleToMember(event.getMember(),
                                        event.getGuild().getRolesByName("Oligarch", true).get(0)).queue();
                                msg += "\n<@" + id + "> is now an oligarch. Better not mess with them!";
                            }
                        } else if (level == 100) {
                            event.getGuild().addRoleToMember(event.getMember(),
                                    event.getGuild().getRolesByName("Minister", true).get(0)).queue();
                            msg += "\n<@" + id + "> is now a minister. Their ascension is complete.";
                        }

                        event.getChannel().sendMessage(msg).queue();
                        users.updateOne(current, Updates.combine(
                                Updates.set("level", level),
                                Updates.set("money", (int) current.get("money") + payment)));

                        try {
                            Thread.sleep(25);
                        } catch (InterruptedException e) {
                            e.printStackTrace();
                        }
                    }
                } else System.out.println("Message was sent too soon!");
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public void onSlashCommandInteraction(SlashCommandInteractionEvent event) {
        try {
            if (event.getName().equals("score")) {
                String user, name;
                if (event.getOption("user") != null) {
                    user = event.getOption("user").getAsUser().getId();
                    name = event.getOption("user").getAsUser().getName();
                } else {
                    user = event.getUser().getId();
                    name = event.getUser().getName();
                }

                Document current = users.find(eq("discID", user)).first();
                event.reply("Citizen Report: " + name +
                        "\n" + "Level: " + current.get("level").toString() +
                        "\n" + XP_NAME + ": " + prettify((Integer) current.get("xp")) + "/" + prettify(getXPToNextLevel((Integer) current.get("level"))) + " " + XP_ABREV +
                        "\n" + MONEY_NAME + ": " + prettify((Integer) current.get("money")) + MONEY_SYMBOL)
                        .queue();
            } else if (event.getName().equals("rankings")) {
                FindIterable rankings = users.find().limit(25).sort(descending("xp"));
                String msg = XP_NAME + " Leaderboard\n";
                List<Document> r = new ArrayList<>();
                rankings.into(r);

                for (int i = 0; i < r.size(); i++) {
                    msg += (i + 1) + ". " + r.get(i).get("name") + " - " + prettify((Integer) r.get(i).get("xp")).toString() + " " +
                            XP_ABREV + "/Level " + r.get(i).get("level").toString() + "\n";
                }

                event.reply(msg).queue();
            } else if (event.getName().equals("wealth")) {
                FindIterable rankings = users.find().limit(25).sort(descending("money"));
                String msg = MONEY_NAME + " Leaderboard\n";
                List<Document> r = new ArrayList<>();
                rankings.into(r);

                for (int i = 0; i < r.size(); i++) {
                    msg += (i + 1) + ". " + r.get(i).get("name") + " - " + prettify((Integer) r.get(i).get("money")) + " " + MONEY_SYMBOL + "\n";
                }

                event.reply(msg).queue();
            } else if (event.getName().equals("shop")) {
                String msg = "**Roles** (use ``/buyrole <id>`` to buy):```";

                for (int i = 0; i < roles.length; i++) {
                    msg += "\n" + i + ". " + roles[i].name + " - " + prettify(roles[i].cost) + " " + MONEY_NAME;
                    if(roles[i].limitedTime) msg += " - -LIMITED TIME! Get it while you can!-";
                }
                msg += "```";

                event.reply(msg).queue();
            } else if (event.getName().equals("buyrole")) {
                if (event.getOption("roleid") == null) event.reply("You must enter a Role ID").queue();
                else {
                    int id = event.getOption("roleid").getAsInt();
                    if (id < 0 || id >= roles.length)
                        event.reply("Role ID ust be a number from 0 to " + (roles.length - 1)).queue();
                    else if (hasRole(event.getGuild(), event.getMember(), roles[id].name))
                        event.reply("You already have that role.").queue();
                    else {
                        Document current = users.find(eq("discID", event.getUser().getId())).first();
                        int money = (int) current.get("money");

                        if (money < roles[id].cost) event.reply("You do not have enough money. You need " +
                                prettify(roles[id].cost - money) + " more " + MONEY_SYMBOL).queue();
                        else {
                            money -= roles[id].cost;
                            users.updateOne(current, Updates.set("money", money));

                            event.getGuild().addRoleToMember(event.getMember(),
                                    event.getGuild().getRolesByName(roles[id].name, true).get(0)).queue();

                            event.reply("<@" + event.getUser().getId() + ">, you are now a " + roles[id].name +
                                    ". You have " + prettify(money) + " " + MONEY_SYMBOL + " left.").queue();
                        }
                    }
                }
            } else if (event.getName().equals("pay")) {
                if (event.getOption("amount") == null || event.getOption("user") == null)
                    event.reply("You must enter a user and an amount").queue();
                else {
                    Document current = users.find(eq("discID", event.getUser().getId())).first();
                    int money = (int) current.get("money");
                    int amount = event.getOption("amount").getAsInt();

                    if (money < amount) event.reply("You do not have enough money. You need " +
                            prettify(amount - money) + " more " + MONEY_NAME.toLowerCase(Locale.ROOT) + ".").queue();
                    else if(amount < 1) event.reply("You must pay at least 1 " + MONEY_SYMBOL).queue();
                    else {
                        money -= amount;
                        users.updateOne(current, Updates.set("money", money));

                        Document target = users.find(eq("discID", event.getOption("user").getAsUser().getId())).first();
                        users.updateOne(target, Updates.set("money", (int) target.get("money") + amount));

                        current = users.find(eq("discID", event.getUser().getId())).first();
                        target = users.find(eq("discID", event.getOption("user").getAsUser().getId())).first();
                        event.reply("<@" + event.getUser().getId() + "> paid " + prettify(amount) + " " + MONEY_SYMBOL + " to <@" +
                                event.getOption("user").getAsUser().getId() + ">.\n<@" + event.getUser().getId() + "> has " +
                                prettify((Integer) current.get("money")) + " " + MONEY_SYMBOL + " left. <@" + event.getOption("user").getAsUser().getId() +
                                "> now has " + prettify((Integer) target.get("money")) + " " + MONEY_SYMBOL + ".")
                                .queue();
                    }
                }

            } else if (event.getName().equals("ping")) {
                long ping = -1 * OffsetDateTime.now().until(event.getTimeCreated(), ChronoUnit.MILLIS);
                event.reply("Ping: " + ping + "ms").queue();
            } else if (event.getName().equals("gamble")) {
                if (event.getOption("amount") == null)
                    event.reply("You must enter an amount to gamble.").queue();
                else {
                    Document current = users.find(eq("discID", event.getUser().getId())).first();
                    int money = (int) current.get("money");
                    int amount = event.getOption("amount").getAsInt();

                    if (money < amount) event.reply("You do not have enough money. You need " +
                            prettify(amount - money) + " more " + MONEY_NAME + ".").queue();
                    else if(amount < money/10) event.reply("You must gamble at least 1/10 of your " + MONEY_NAME + " (" +
                            prettify(money/10) + " " + MONEY_SYMBOL + ").").queue();
                    else if(amount > (int)current.get("level") * 1000) event.reply("You can only gamble up to " +
                            prettify((int)current.get("level") * 1000) + " " + MONEY_SYMBOL +  ".").queue();
                    else {
                        if (new Random().nextInt(99) + 1 < Math.min(45 + ((int) current.get("level") / 15), 65)) {
                            int orig = amount;

                            double levelBonus = (((int) current.get("level")) / 400.0);

                            double sizeBonus = Math.sqrt(amount)/650;
                            final int MIN_SIZE_FOR_BONUS = 7500;
                            if(amount < MIN_SIZE_FOR_BONUS) sizeBonus = 0;
                            else if(amount < MIN_SIZE_FOR_BONUS * 2) sizeBonus /= 1.5;

                            double allOrNothingBonus = 0;
                            if(amount > ((int)current.get("money"))/2) allOrNothingBonus = 1 + levelBonus + sizeBonus;

                            double bonus = levelBonus + sizeBonus + allOrNothingBonus;
                            amount *= (1.0 + bonus);
                            users.updateOne(current, Updates.set("money", (int) current.get("money") + amount));

                            current = users.find(eq("discID", event.getUser().getId())).first();
                            event.reply("You won! You gained " + prettify(amount) + " " + MONEY_SYMBOL + " (+" +
                                    Math.round(bonus*100) + "%, bet " + prettify(orig) + " " + MONEY_NAME + "), for a total of " +
                                    prettify((int)current.get("money")) + " " + MONEY_SYMBOL + ". Your odds were " +
                                    Math.min(45 + ((int) current.get("level") / 15), 65) + "% (+1% for every 15 levels).\nBonuses:\n```Total: " +
                                    Math.round(bonus*100) + "%\n-Level: " + Math.round(levelBonus*100) +
                                    "% (+1% for every 5 levels)\n-Bet Size: " +
                                    Math.round(sizeBonus*100) + "% (Must be at least " + prettify(MIN_SIZE_FOR_BONUS) + " " +
                                    MONEY_NAME + " to apply, reduced until " +
                                    prettify(MIN_SIZE_FOR_BONUS*2) + " " + MONEY_NAME +")\n-All or Nothing: " + Math.round(allOrNothingBonus*100)
                                    + "% (+100% " + MONEY_NAME + " and doubles other bonuses when gambling at least 1/2 of your total " + MONEY_NAME + ")```").queue();
                        } else {
                            users.updateOne(current, Updates.set("money", (int) current.get("money") - amount));
                            current = users.find(eq("discID", event.getUser().getId())).first();
                            event.reply("You lost! You lost " + prettify(amount) + " " + MONEY_SYMBOL + ", and have " +
                                    prettify((int)current.get("money")) + " " + MONEY_SYMBOL + " remaining. Your odds were " +
                                    Math.min(45 + ((int) current.get("level") / 15), 65) + "% (+1% for every 15 levels).").queue();

                        }
                    }
                }
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public int getXPToNextLevel(int level) {
        double x = 0;
        for(int i = 0; i <= level; i++) x += BASE_XP_TO_NEXT_LEVEL * Math.pow(XP_REQ_SCALER, i);
        return (int) x;
    }

    public String prettify(int num) {
        return new DecimalFormat().format(num);
    }

    public boolean hasRole(Guild guild, Member member, String role) {
        try {
            net.dv8tion.jda.api.entities.Role r = guild.getRolesByName(role, true).get(0);
            return member.getRoles().contains(r);
        } catch(Exception e) {
            return false;
        }
    }
}
