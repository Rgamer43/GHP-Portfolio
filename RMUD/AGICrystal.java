import java.io.IOException;

public class AGICrystal extends AbilityCrystal {
    public AGICrystal() {
        super("Agility Crystal", "A crystal that can be bonded with, increasing your AGI by 1", "agicrys");
        absc = 4;
    }

    @Override
    public void onActivate(Player p) throws IOException {
        super.onActivate(p);
        p.calculateAC();
        PlayerSaveManager.save(p);
    }
}
