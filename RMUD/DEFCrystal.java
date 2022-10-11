import java.io.IOException;

public class DEFCrystal extends AbilityCrystal {
    public DEFCrystal() {
        super("Defense Crystal", "A crystal that can be bonded with, increasing your DEF by 1 (capped at 19 DEF)", "defcrys");
        absc = 5;
    }

    @Override
    public void onActivate(Player p) throws IOException {
        super.onActivate(p);

        if (p.absc[5].value > 19) p.absc[5].value = 19;

        PlayerSaveManager.save(p);
    }
}
