import java.io.IOException;
import java.util.Random;

public class GreaterHealingPotion extends Item{
    public GreaterHealingPotion() {
        super("Greater Healing Potion", "A powerful potion of healing. Heals 10-25 HP. Consumable", 50,"ghp");
        consumable = true;
    }

    @Override
    public void onActivate(Player p) throws IOException {
        p.heal(new Random().nextInt(16) + 10);
    }
}
