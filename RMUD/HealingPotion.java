import java.io.IOException;
import java.util.Random;

public class HealingPotion extends Item{
    public HealingPotion() {
        super("Healing Potion", "A potion of healing. Heals 5-10 HP. Consumable", 20, "hp");
        consumable = true;
    }

    @Override
    public void onActivate(Player p) throws IOException {
        p.heal(new Random().nextInt(6) + 5);
    }
}
