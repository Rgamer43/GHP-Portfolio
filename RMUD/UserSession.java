import java.net.Socket;
import java.io.*;
import java.net.SocketException;
import java.util.ArrayList;

public class UserSession extends Thread implements Serializable {

    public Socket socket;

    public ObjectOutputStream oos;
    public ObjectInputStream ois;

    public Player player;

    public boolean loggingOut = false;

    public UserSession(Socket s) throws IOException {
        socket = s;
        oos = new ObjectOutputStream(s.getOutputStream());
        ois = new ObjectInputStream(s.getInputStream());
    }

    public void run() {
        System.out.println("Started thread " + currentThread().getId());
        try {
            oos.writeObject("Connected to server!");
            oos.writeObject("Welcome");
            oos.writeObject("to");
            oos.writeObject("Rgamer43's Multi User Dungeon");
            oos.writeObject("-----");
            System.out.println("Sent intro msg");
            //System.out.println(ois.readObject());
            String i = (String) ois.readObject();
            System.out.println(i);

            if (i.equals("createPFile")) {
                oos.writeObject("By what name should you be called?");
                System.out.println("Setting up player");
                playerSetUp((String) ois.readObject());
                onJoin();
            }
            else if (i.equals("loadPFile")) {
                player = PlayerSaveManager.load((PlayerSave) ois.readObject());
                onJoin();
            }

        } catch (IOException | ClassNotFoundException e) {
            e.printStackTrace();
        }
    }

    public void playerSetUp(String username) throws IOException, ClassNotFoundException {
        Player p = new Player();
        p.username = username;
        oos.writeObject("---");
        oos.writeObject("Character Creation");
        oos.writeObject("Choose your class: \n" +
                "[Fighter] or [Wizard]");

        Fighter f = new Fighter();
        oos.writeObject(f.name + ": " + f.desc);
        oos.writeObject("Special Ability: " + f.abilityDesc);
        oos.writeObject("Starts with:");
        oos.writeObject("  STR: " + f.absc[0]);
        oos.writeObject("  DEX: " + f.absc[1]);
        oos.writeObject("  CON: " + f.absc[2]);
        oos.writeObject("  INT: " + f.absc[3]);
        oos.writeObject("  AGI: " + f.absc[4]);
        oos.writeObject("  DEF: " + f.absc[5]);

        Wizard w = new Wizard();
        oos.writeObject(w.name + ": " + w.desc);
        oos.writeObject("Special Ability: " + w.abilityDesc);
        oos.writeObject("Starts with:");
        oos.writeObject("  STR: " + w.absc[0]);
        oos.writeObject("  DEX: " + w.absc[1]);
        oos.writeObject("  CON: " + w.absc[2]);
        oos.writeObject("  INT: " + w.absc[3]);
        oos.writeObject("  AGI: " + w.absc[4]);
        oos.writeObject("  DEF: " + w.absc[5]);

        oos.writeObject("Enter your choice");
        String choice = (String) ois.readObject();

        while (!choice.toLowerCase().equals("fighter") && !choice.toLowerCase().equals("wizard")){
            oos.writeObject("Please enter either 'fighter' or 'wizard'");
            choice = (String) ois.readObject();
        }

        oos.writeObject("You have selected: " + choice);
        oos.writeObject("Confirm? [y/n]");
        String c = (String) ois.readObject();

        while (!c.toLowerCase().equals("y") && !c.toLowerCase().equals("n")){
            System.out.println("Please enter either 'y' or 'n'");
            c = (String) ois.readObject();
        }

        while (c.equals("n")) {
            oos.writeObject("Enter your choice");
            choice = (String) ois.readObject();

            while (!choice.toLowerCase().equals("fighter") && !choice.toLowerCase().equals("wizard")){
                System.out.println("Please enter either 'fighter' or 'wizard'");
                choice = (String) ois.readObject();
            }

            oos.writeObject("You have selected: " + choice);
            oos.writeObject("Confirm? [y/n]");
            c = (String) ois.readObject();

            while (!c.toLowerCase().equals("y") && !c.toLowerCase().equals("n")){
                System.out.println("Please enter either 'y' or 'n'");
                c = (String) ois.readObject();
            }
        }

        if (choice.toLowerCase().equals("fighter")) p.pClass = new Fighter();
        else if (choice.toLowerCase().equals("wizard")) p.pClass = new Wizard();

        p.gold = 50;

        p.armor = new NoArmor();
        p.weapon = new NoWeapon();

        p.inventory = p.pClass.startingGear;

        p.location = 0;

        p.name = p.username;

        for (int x = 0; x < p.pClass.absc.length; x++)
            p.absc[x].value = p.pClass.absc[x];

        p.maxHp = 10 + (p.absc[2].value * 5);
        p.hp = p.maxHp;

        p.calculateAC();

        p.alias = p.name;

        oos.writeObject("Character Creation finished");

        player = p;

        oos.writeObject("psIncoming");

        oos.writeObject(PlayerSaveManager.save(player));

        oos.writeObject("TIP: Use [help] to get help");

        //onJoin();
    }

    public void onJoin() throws IOException, ClassNotFoundException {
//        AccountManager.getAccount(player.username).hasPlayedBefore = true;
//        AccountManager.save();

        Server.locations[player.location].enter(player, true);

        oos.writeObject("setHPHUD " + player.hp + "/" + player.maxHp);

        player.maxMana = 100 + (10 * player.absc[3].value);
        player.mana = player.maxMana;
        oos.writeObject("setManaHUD " + player.mana + "/" + player.maxMana);

        Thread manaRegen = new Thread(new Runnable() {
            @Override
            public void run() {
                while(socket.isConnected()) {
                    try {
                        if (player.mana < player.maxMana && !loggingOut) {
                            Thread.sleep(1000);
                            oos.writeObject("setManaHUD " + player.mana + "/" + player.maxMana);
                            player.mana++;
                        }
                        player.maxMana = 100 + (10 * player.absc[3].value);
                    } catch (InterruptedException | IOException e) {
                        e.printStackTrace();
                    }
                }
            }
        });
        manaRegen.start();

        while (true) {
            update();
        }
    }

    public void update() throws IOException, ClassNotFoundException, SocketException {
        if (!loggingOut) resolveInput((String) ois.readObject());
    }

    public void resolveInput(String i) throws IOException {
        String[] input = i.toLowerCase().split(" ");

        if (input.length > 0) {

            if (i.equalsIgnoreCase("help ability")) oos.writeObject(player.pClass.abilityDesc);

            else if (input[0].equals("help")) {
                if(input.length > 1) {
                    if (input[1].equals("cmds")) oos.writeObject(Help.cmds);
                    else if (input[1].equals("stats")) oos.writeObject(Help.stats);
                    else if(input[1].equals("aliases")) oos.writeObject(Help.aliases);
                    else if(input[1].equals("intro")) oos.writeObject(Help.intro);
                    else {
                        oos.writeObject("Didn't recognize help command. Printing generic help.");
                        oos.writeObject(Help.generic);
                    }
                }
                else oos.writeObject(Help.generic);
            }

            else if(input[0].equals("inventory") || input[0].equals("inv")) {
                oos.writeObject("---");
                oos.writeObject("Armor: " + player.armor.name + " (" + player.armor.alias + ")");
                oos.writeObject("Weapon: " + player.weapon.name + " (" + player.weapon.alias + ")");
                oos.writeObject("Gold: " + player.gold);
                oos.writeObject("Inventory:");
                for (int x = 0; x < player.inventory.size(); x++) {
                    Item it = player.inventory.get(x);
                    oos.writeObject("  " + it.name + " (" + it.alias + ")");
                }
                oos.writeObject("Use [inspect <item>] to see item details");
            }

            else if(input[0].equals("inspect") || input[0].equals("ins")) {
                if (input.length == 1) oos.writeObject("When using inspect, please format it as [inspect <item>]");
                else {
                    String msg = "";
                    for (int x = 1; x < input.length; x++) msg += input[x] + " ";
                    String msg1 = msg.substring(0, msg.length() - 1);
                    msg = msg1;
                    if (player.getItem(msg) != null) {
                        oos.writeObject("---");
                        Item item = player.getItem(msg);
                        oos.writeObject(item.name + " (" + item.alias + ")");
                        oos.writeObject(item.price + " gold");
                        oos.writeObject(item.desc);

                        if (Weapon.class.isAssignableFrom(item.getClass())) {
                            Weapon weapon = (Weapon) item;
                            oos.writeObject(weapon.atkBonus + " + " + player.absc[weapon.absc].name + " to attack");
                            oos.writeObject(weapon.damage[0] + "-" + weapon.damage[1] + " damage");
                        } else if (Armor.class.isAssignableFrom(item.getClass())) {
                            Armor armor = (Armor) item;
                            oos.writeObject("+" + armor.acb + " defense");
                        }

                        if (Equipment.class.isAssignableFrom(item.getClass())) {
                            oos.writeObject("Use [equip <item>] to equip an item");
                        }
                    } else oos.writeObject("Item not found");
                }
            }

            else if(input[0].equals("equip")) {
                if (input.length == 1) oos.writeObject("When using equip, please format it as [equip <item>]");
                else {
                    String msg = "";
                    for (int x = 1; x < input.length; x++) msg += input[x] + " ";
                    String msg1 = msg.substring(0, msg.length()-1);
                    msg = msg1;
                    if(player.getItem(msg) != null) {
                        if (Equipment.class.isAssignableFrom(player.getItem(msg).getClass())) {
                            if(Weapon.class.isAssignableFrom(player.getItem(msg).getClass())) {
                                Weapon weapon = player.weapon;
                                msg = "";
                                for (int x = 1; x < input.length; x++) msg += input[x] + " ";
                                msg1 = msg.substring(0, msg.length()-1);
                                msg = msg1;
                                Item item = player.getItem(msg);
                                if(player.weapon.getClass() != NoWeapon.class)
                                        player.inventory.add(weapon);
                                player.weapon = new NoWeapon();
                                player.weapon = (Weapon) player.getItem(msg);
                                player.inventory.remove(item);
                                oos.writeObject("Equipped " + msg);
                                PlayerSaveManager.save(player);
                            }
                            else if(Armor.class.isAssignableFrom(player.getItem(msg).getClass())) {
                                Armor armor = player.armor;
                                msg = "";
                                for (int x = 1; x < input.length; x++) msg += input[x] + " ";
                                msg1 = msg.substring(0, msg.length()-1);
                                msg = msg1;
                                Item item = player.getItem(msg);
                                if(player.armor.getClass() != NoArmor.class) player.inventory.add(armor);
                                player.armor = new NoArmor();
                                player.armor = (Armor) player.getItem(msg);
                                player.inventory.remove(item);
                                oos.writeObject("Equipped " + msg);
                                player.calculateAC();
                                PlayerSaveManager.save(player);
                            }
                        } else oos.writeObject("Item is not equippable");
                    }
                    else oos.writeObject("Item not found");
                }
            }

            else if(input[0].equals("chat")) {
                if (input.length == 1) oos.writeObject("When using chat, please format it as [chat <message>]");
                else {
                    String msg = "";
                    for (int x = 1; x < input.length; x++) msg += input[x] + " ";
                    Server.locations[player.location].chat(player, msg);
                }
            }

            else if(input[0].equals("attack")) {
                if (input.length == 1) oos.writeObject("When using attack, please format it as [attack <target>]");
                else {
                    String msg = "";
                    for (int x = 1; x < input.length; x++) msg += input[x] + " ";
                    String msg1 = msg.substring(0, msg.length()-1);
                    msg = msg1;
                    Entity e = Server.locations[player.location].getEntity(msg);
                    if (e == null) oos.writeObject("Entity not found");
                    else {
                        player.attack(e);
                        if (e.getClass() == Player.class) PlayerSaveManager.save((Player) e);
                    }
                }
            }

            else if(input[0].equals("hp")) oos.writeObject("HP: " + player.hp + "/" + player.maxHp);

            else if(input[0].equals("ac")) oos.writeObject("AC: " + player.ac);

            else if(input[0].equals("stats")) {
                oos.writeObject("Your ability scores are:");
                for (int x = 0; x < player.absc.length; x++)
                    oos.writeObject("    " + player.absc[x].name + ": " + player.absc[x].value);
            }

            else if(input[0].equals("use")) {
                if (input.length == 1) oos.writeObject("When using use, please format it as [use <item>]");
                else {
                    String msg = "";
                    for (int x = 1; x < input.length; x++) msg += input[x] + " ";
                    String msg1 = msg.substring(0, msg.length()-1);
                    msg = msg1;
                    if (player.getItem(msg) != null) player.getItem(msg).activate(player);
                    PlayerSaveManager.save(player);
                }
            }

            else if(input[0].equals("enter")) {
                if (input.length == 1) oos.writeObject("When using enter, please format it as [enter <location>]");
                else {
                    ArrayList<Integer> ex = new ArrayList<>();
                    String msg = "";
                    for (int x = 1; x < input.length; x++) msg += input[x] + " ";
                    String msg1 = msg.substring(0, msg.length()-1);
                    msg = msg1;
                    for (int x = 0; x < Server.locations[player.location].exits.size(); x++) {
                        if (Server.locations[Server.locations[player.location].exits.get(x)].name.toLowerCase().equals(msg) ||
                                Server.locations[Server.locations[player.location].exits.get(x)].alias.toLowerCase().equals(msg)) {
                            Server.locations[Server.locations[player.location].exits.get(x)].enter(player, true);
                            PlayerSaveManager.save(player);
                            break;
                        }

                        if (x == Server.locations[player.location].exits.size()-1) oos.writeObject("Location not found");
                    }
                }
            }

            else if (input[0].equals("logout")) {
                logOut();
            }

            else if (input[0].equals("ability")) player.pClass.ability(player);

            else if(!Server.locations[player.location].resolveInput(player, input)) {
                oos.writeObject("Didn't recognize command");
            }
        }
    }

    public void logOut() throws IOException {
        loggingOut = true;

        for (int x = 0; x < player.activeArtifacts.size(); x++)
            player.activeArtifacts.get(x).onLogout(player);

        if (player.abilityActive) player.pClass.onLogout(player);

        oos.writeObject("psIncoming");
        oos.writeObject(PlayerSaveManager.save(player));
        oos.writeObject("logOut");

        PlayerSaveManager.save(player);

        Location l = Server.locations[player.location];
        for (int x = 0; x < l.occupants.size(); x++)
            Server.getUserSession(l.occupants.get(x)).oos.writeObject(
                    player.name + " has left the game");

        Server.locations[player.location].occupants.remove(player);
        socket.close();

        System.out.println(player.name + " has logged out");

        Server.sessions.remove(this);
    }
}
