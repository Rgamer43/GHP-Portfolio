import java.io.IOException;
import java.util.ArrayList;
import java.util.Random;

public class Server {

    public static ArrayList<UserSession> sessions = new ArrayList<>();
    public static ConnectionAccepter ca = new ConnectionAccepter();

    public static int townSize = 6;
    public static int dungeonSize = 50;
    public static int floors;

    public static Location[] locations = new Location[]{
            new TownSquare(), //0
            new ResurrectionShrine(), //1
            new DungeonEntrance(), //2
            new Blacksmith(), //3
            new Alchemist(), //4
            new Bank(), //5
    };


    public static void main(String[] args) throws IOException, InterruptedException {
        System.out.println("Started server");
        //AccountManager.load();

        generateDungeon();

        for (int x = 0; x < locations.length; x++) {
            for (int y = 0; y < locations[x].inhabitants.size(); y++) {
                Runnable runnable = locations[x].inhabitants.get(y);
                Thread thread = new Thread(runnable);
                thread.start();
            }
        }

        ca.start();
    }

    public static int getUserSessionID(Player p) {
        for (int x = 0; x < sessions.size(); x++) {
            if(sessions.get(x).player == p) return x;
        }

        return -1;
    }

    public static UserSession getUserSession(Player p) {
        return sessions.get(getUserSessionID(p));
    }

    public static void generateDungeon() throws InterruptedException {
        System.out.println("Generating Dungeon");

        Random random = new Random();

        Location[] loc1 = locations;

        Theme[] themes = new Theme[]{
                new Theme(
                        new String[]{"grimy","smelly","rotten","dark","putrid","chilly","bloody"},
                        new Monster[]{new Zombie(-1), new Skeleton(-1), new Ghost(-1), new Gnoll(-1)},
                        new Monster[]{new Ogre(-1)},
                        new Item[]{new Longsword(), new HealingPotion(), new HealingPotion(), new Coin50()}
                ),
                new Theme(
                        new String[]{"grimy","smelly","rotten","dark","bloody","chaotic","messy"},
                        new Monster[]{new Goblin(-1), new Bugbear(-1), new Hobgoblin(-1)},
                        new Monster[]{new BugbearChieftain(-1), new HobgoblinWarlord(-1)},
                        new Item[]{new HealingPotion(), new HealingPotion(), new GreaterHealingPotion(), new TeleportScroll0(),
                        new Weapon("Mace", "Mace", 0, 1, new int[]{5,10},0,"mc"),
                        new Weapon("Spiked Club", "Spiked Club", 0, 3, new int[]{4,8},0,"spcl"),
                        new Weapon("Scimitar", "Scimitar", 0, 1, new int[]{2,6},0,"sci"),
                        new Coin50(), new Coin50(), new BearTotem()}
                ),
                new Theme(
                        new String[]{"grimy","smelly","rotten","dark","putrid","bloody","wartorn","chaotic","messy"},
                        new Monster[]{new Orc(-1), new OrcWarrior(-1), new OrcShaman(-1), new OrcPillager(-1)},
                        new Monster[]{new OrcChief(-1), new OrcWarlord(-1)},
                        new Item[]{new Greatsword(), new Weapon("Orc Greatsword", "", 0, 4, new int[]{5,8},0,"ogs"),
                                new Weapon("Orc Scimitar", "", 0, 2, new int[]{4,6},0,"osc"), new HealingPotion(),
                        new Coin50(), new CureWounds()}
                )
        };

        floors = themes.length;

        locations = new Location[townSize + (dungeonSize * floors)];

        for (int x = 0; x < townSize; x++) {
            locations[x] = loc1[x];
        }

        for (int floor = 0; floor < floors; floor++) {
            System.out.println("Generating floor " + floor);

            Theme theme = themes[floor];
            int prevLocs = townSize + (dungeonSize * floor);
//            System.out.println("prevLocs = " + prevLocs);
//            System.out.println("prevLocs + (dungeonSize * floor) = " + ((dungeonSize * floor) + prevLocs));
//            System.out.println("prevLocs + dungeonSize = " + (prevLocs + dungeonSize));

            Thread.sleep(1500);

            for (int x = prevLocs; x < prevLocs + dungeonSize; x++) {
//                System.out.println("x = " + x);
//                System.out.println("Generating room " + x);
                TemplateRoom r = new TemplateRoom();
                String[] roomTypes = new String[]{"Room", "Chamber", "Hallway", "Bedroom", "Tunnel", "Guard Room", "Cave", "Cavern",
                        "Kitchen", "Dining Room", "Dormitory", "Bathroom", "Living Room", "Throne Room", "Crevice", "Meeting Room", "Office"};

                String roomType = roomTypes[random.nextInt(roomTypes.length)];
                if (x == prevLocs + dungeonSize - 1) roomType = "Stairs";
                r.name = roomType + " " + x;
                r.id = x;
                r.entryMsg = "You come into a " + theme.roomAdjs[random.nextInt(theme.roomAdjs.length)] + " " + roomType.toLowerCase();

                r.inhabitants = new ArrayList<>();

                if (random.nextInt(99) + 1 > 40) {
                    if (random.nextInt(99) < 80) {
                        r.inhabitants.add(cloneMonster(theme.monsters[random.nextInt(theme.monsters.length)]));
                        r.inhabitants.get(0).location = x;
                    } else {
                        r.inhabitants.add(cloneMonster(theme.monsters[random.nextInt(theme.monsters.length)]));
                        r.inhabitants.get(0).location = x;

                        r.inhabitants.add(cloneMonster(theme.monsters[random.nextInt(theme.monsters.length)]));
                        r.inhabitants.get(1).location = x;
                    }
                } else if (random.nextInt(99) + 1 > 85) {
                    r.inhabitants.add(cloneMonster(theme.bossMonsters[random.nextInt(theme.bossMonsters.length)]));
                    r.inhabitants.get(0).location = x;
                }

                if (random.nextInt(99) + 1 > 75) {
                    //System.out.println("Generating chest");
                    r.chest = new LootChest();
                    r.chest.gold = (random.nextInt(9) + random.nextInt(9) + 2) * 10;

                    int lootNum = random.nextInt(4);
                    for (int y = 0; y < lootNum; y++)
                        r.chest.items.add(theme.loot[random.nextInt(theme.loot.length)]);

                    r.entryMsg += "\nThere is also a chest in this room. Use [open] to \n open it";
                }

                r.alias = new String().valueOf(r.id);

                if (x == prevLocs + dungeonSize-1 && floor != floors-1)
                    r.AddExit(x+1);

                locations[x] = r;

                if (x == prevLocs && floors > 0)
                    if(r.id != 6) r.AddExit(x-1);
                    else r.AddExit(2);

//                System.out.println("Finished generating room " + x);
            }

            for (int x = prevLocs; x < prevLocs + dungeonSize; x++) {
                int e = random.nextInt(3) + 2;
                if (x == prevLocs || x == prevLocs + dungeonSize - 1) e = random.nextInt(3) + 3;
                for (int y = 0; y < e; y++) {
                    if (x == townSize && y == 0) locations[x].exits.add(2);
                    else {
                        int z = random.nextInt(dungeonSize) + prevLocs;
                        locations[x].AddExit(z);
                        locations[locations[x].exits.get(locations[x].exits.size() - 1)].exits.add(x);
                    }
                }
            }

            System.out.println("Finished generating floor " + floor);
        }

        locations[townSize].exits.remove(1);
//        System.out.println(locations[townSize].exits.toString());

        System.out.println("Finished generating dungeon");
    }

    public static Monster cloneMonster(Monster m) {
        Monster c = new Monster();
        c.name = m.name;
        c.weapon = m.weapon;
        c.location = m.location;
        c.atkSpeed = m.atkSpeed;
        c.respawnSpeed = m.respawnSpeed;
        c.gold = m.gold;
        c.maxHp = m.maxHp;
        c.hp = m.hp;
        c.ac = m.ac;
        c.absc = m.absc;
        c.inventory = m.inventory;
        c.alias = m.alias;

        return c;
    }
}
