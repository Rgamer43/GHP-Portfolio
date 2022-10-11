import java.io.IOException;

public class CONCrystal extends AbilityCrystal {
    public CONCrystal() {
        super("Constitution Crystal", "A crystal that can be bonded with, increasing your CON by 1", "concrys");
        absc = 2;
        price = 75;
    }

    @Override
    public void onActivate(Player p) throws IOException {
        super.onActivate(p);
        p.calculateMaxHP();
        PlayerSaveManager.save(p);
    }
}
