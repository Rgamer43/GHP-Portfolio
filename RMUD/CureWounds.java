import java.io.IOException;
import java.util.Random;

public class CureWounds extends Artifact {
    public CureWounds() {
        super("Cure Wounds", "Heals 5-15 HP. Costs 120 mana.", 300, "crwd", 120);
    }

    @Override
    public void use(Player p) {
        try {
            Server.getUserSession(p).oos.writeObject("You cast cure wounds.");
            p.heal(new Random().nextInt(10) + 5);
        }
        catch (IOException e) {

        }
    }
}
