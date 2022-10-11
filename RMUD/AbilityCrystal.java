import java.io.IOException;

public class AbilityCrystal extends Item {

    public int absc;

    public AbilityCrystal(String n, String d, String a) {
        super(n, d, 50, a);
        consumable = true;
    }

    @Override
    public void onActivate(Player p) throws IOException {
        p.absc[absc].value += 1;
        Server.getUserSession(p).oos.writeObject("As you bond with the crystal, you feel your " + p.absc[absc].name +
                " increase");
        PlayerSaveManager.save(p);
        Server.getUserSession(p).oos.writeObject("setHPHUD " + p.hp + "/" + p.maxHp);
    }
}
