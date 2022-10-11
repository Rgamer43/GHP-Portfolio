import java.io.IOException;
import java.util.ArrayList;
import java.util.Random;

public class Monster extends Entity implements Runnable {
    public int atkSpeed;
    public int respawnSpeed;

    @Override
    public void run() {
        while (true) {
            if(hp > 0) {
                try {
                    Thread.sleep(atkSpeed);
                    ArrayList<Player> occupants = Server.locations[location].occupants;
                    if (occupants.size() > 0 && hp > 0) attack(occupants.get(random.nextInt(occupants.size())));
                } catch (InterruptedException | IOException e) {
                    e.printStackTrace();
                }
            }
            else {
                try {
                    String n = name;
                    name = n + " Corpse";
                    String a = alias;
                    alias = a + "c";

                    for (int x = 0; x < Server.locations[location].occupants.size(); x++) {
                        Player p = Server.locations[location].occupants.get(x);
                        Server.getUserSession(p).oos.writeObject(n + " has died. It is now " + name);
                        Server.getUserSession(p).oos.writeObject("setRoomDesc " + Server.locations[location].getRoomDesc());
                    }

                    Thread.sleep(respawnSpeed);
                    hp = maxHp;
                    System.out.println("Respawned " + name + " in " + Server.locations[location].name);
                    name = n;
                    alias = a;

                    for (int x = 0; x < Server.locations[location].occupants.size(); x++) {
                        Player p = Server.locations[location].occupants.get(x);
                        Server.getUserSession(p).oos.writeObject(name + " has respawned. It is now " + name);
                        Server.getUserSession(p).oos.writeObject("setRoomDesc " + Server.locations[location].getRoomDesc());
                    }
                } catch (InterruptedException | IOException e) {
                    e.printStackTrace();
                }
            }
        }
    }

    @Override
    public void onDeath(Entity killer) throws IOException {
        if (killer.getClass() == Player.class) {
            Player p = (Player) killer;
            Random random = new Random();
            if(!name.contains("Corpse")) {
                Server.getUserSession((Player) killer).oos.writeObject("You slayed the " + name);
                Double g = (double) gold * random.nextInt(5) / 3.0;
                p.gold += g;
                Server.getUserSession(p).oos.writeObject("You gained " + gold + " gold from the " + name);
            }
            PlayerSaveManager.save(p);
        }
    }
}
