import java.io.DataOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.util.ArrayList;

public class Shop {
    public ArrayList<Item> items = new ArrayList<>();

    public void displayItems(Player p) throws IOException {
        ObjectOutputStream dos = Server.getUserSession(p).oos;

        for (int x = 0; x < items.size(); x++) {
            String text = "    " + items.get(x).name;
            text += " (" + items.get(x).alias + ")";
            text += " (" + items.get(x).price + " gold)";
            text += ": " + items.get(x).desc;

            if (Weapon.class.isAssignableFrom(items.get(x).getClass())) {
                Weapon w = (Weapon) items.get(x);
                text += " - Uses " + p.absc[w.absc].name + ", +" + w.atkBonus + " to attack, " +
                        w.damage[0] + "-" + w.damage[1] + " damage";
            }

            else if(Armor.class.isAssignableFrom(items.get(x).getClass())) {
                Armor a = (Armor) items.get(x);
                text += " - +" + a.acb + " AC";
            }

            dos.writeObject(text);
        }
    }

    public void buy(Player p, String name) throws IOException {
        ObjectOutputStream dos = Server.getUserSession(p).oos;
        for (int x = 0; x < items.size(); x++) {
            if (items.get(x).name.toLowerCase().equals(name) || items.get(x).alias.toLowerCase().equals(name)) {
                if (p.gold < items.get(x).price) dos.writeObject(items.get(x).name + " costs " +
                        items.get(x).price + " gold. You need " + (items.get(x).price - p.gold) + " more gold.");
                else {
                    p.gold -= items.get(x).price;
                    p.inventory.add(items.get(x));
                    dos.writeObject("Purchased " + items.get(x).name + " for " + items.get(x).price + " gold");
                    PlayerSaveManager.save(p);
                }

                break;
            }
            else if (x == items.size()-1) dos.writeObject("Item not found");
        }
    }
}
