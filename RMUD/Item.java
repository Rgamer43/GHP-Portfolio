import java.io.IOException;
import java.io.Serializable;

public class Item implements Serializable {
    public String name;
    public String desc;
    public String alias;

    public int price;

    public boolean consumable = false;

    public Item(String n, String d, int p, String a) {
        name = n;
        desc = d;
        price = p;
        alias = a;
    }

    public void activate(Player p) throws IOException {
        onActivate(p);

        if (consumable) p.inventory.remove(this);
    }

    public void onActivate(Player p) throws IOException {
        Server.getUserSession(p).oos.writeObject(name + " cannot be activated");
    }
}
