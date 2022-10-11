import java.io.IOException;
import java.util.Random;

public class HPPot extends Item{
    public HPPot() {
        super("HPPot", "A potion of healing. Heals 5-10 HP. Consumable", 35,"hpp");
        consumable = true;
    }

    @Override
    public void onActivate(Player p) throws IOException {
        p.heal(new Random().nextInt(6) + 5);
    }
}
