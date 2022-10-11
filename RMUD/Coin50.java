import java.io.IOException;

public class Coin50 extends Item {
    public Coin50() {
        super("Gold Coin (worth 50 Gold)", "A gold coin, worth 50 gold. Not lost on death. \n" +
                "Use to convert to gold. Consumable", 50, "coin");
        //consumable = true;
    }

    @Override
    public void onActivate(Player p) throws IOException {
        p.gold += 50;
        Server.getUserSession(p).oos.writeObject("Coin converted!");

        p.inventory.remove(this);
    }
}
