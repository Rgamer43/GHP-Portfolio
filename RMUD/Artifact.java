import java.io.IOException;

public class Artifact extends Item {
    public int manaCost;

    public Artifact(String n, String d, int p, String a, int m) {
        super(n, d, p, a);
        manaCost = m;
    }

    @Override
    public void onActivate(Player p) throws IOException {
        if (p.mana >= manaCost) {
            p.mana -= manaCost;
            Server.getUserSession(p).oos.writeObject("setManaHUD " + p.mana + "/" + p.maxMana);
            use(p);
        }
        else Server.getUserSession(p).oos.writeObject("Not enough mana. Item costs " + manaCost + "mana.");
    }

    public void use(Player p) {}
    public void onLogout(Player p) {}
}
