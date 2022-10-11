import java.io.IOException;

public class TeleportScroll0 extends Item{
    public TeleportScroll0() {
        super("Scroll of Teleportation (Town Square)", "Teleports user to the town square. Consumable",
                65,"tp0");
        consumable = true;
    }

    @Override
    public void onActivate(Player p) throws IOException {
        Server.getUserSession(p).oos.writeObject("You read the scroll and begin to teleport...");
        Server.locations[0].enter(p, true);
    }
}
