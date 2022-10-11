import java.io.IOException;
import java.io.Serializable;
import java.util.ArrayList;
import java.util.Random;

public class Entity implements Serializable {

    public int location;

    public String name;

    public String alias;

    public int maxHp;
    public int hp;

    public AbilityScore[] absc = {
            new AbilityScore("STR", 0),
            new AbilityScore("DEX", 0),
            new AbilityScore("CON", 0),
            new AbilityScore("INT", 0),
            new AbilityScore("AGI", 0),
            new AbilityScore("DEF", 0)
    };

    public int ac;

    public Weapon weapon;
    public Armor armor;
    public int gold;
    public ArrayList<Item> inventory = new ArrayList<>();

    public Random random = new Random();

    public Item getItem(String name) {
        for (int x = 0; x < inventory.size(); x++) {
            if (inventory.get(x).name.toLowerCase().equals(name.toLowerCase()) ||
                    inventory.get(x).alias.toLowerCase().equals(name.toLowerCase())) return inventory.get(x);
        }

        if (armor.name.toLowerCase().equals(name.toLowerCase()) ||
                armor.alias.toLowerCase().equals(name.toLowerCase())) return armor;
        else if (weapon.name.toLowerCase().equals(name.toLowerCase()) ||
                weapon.alias.toLowerCase().equals(name.toLowerCase())) return weapon;

        return null;
    }

    public void calculateAC() {
        ac = 10 + armor.acb + absc[4].value;
    }

    public void takeDamage(Entity e, int d) throws IOException {
        double dd = d;
        dd *= 1 - (absc[5].value * 0.05);
        d = (int) dd;
        hp -= d;

        if(hp < 0) hp = 0;

        for (int x = 0; x < Server.locations[location].occupants.size(); x++) {
            Player p = Server.locations[location].occupants.get(x);
            Server.getUserSession(p).oos.writeObject(e.name + " hits " + name + " for " + d +
                    " damage using " + e.weapon.name + ". " + name +" is now at " + hp + "/" + maxHp + " HP");
        }

        if (this.getClass() == Player.class) Server.getUserSession((Player) this).oos.writeObject("setHPHUD " + hp + "/" + maxHp);

        if (hp == 0) onDeath(e);
    }

    public void attack(Entity target) throws IOException {
        int atk = random.nextInt(19) + 1;

        if (atk + absc[weapon.absc].value + weapon.atkBonus > target.ac || atk == 20) {
            int damage = random.nextInt(weapon.damage[1] + 1 - weapon.damage[0]) + weapon.damage[0];
            if (atk == 20) {
                damage *= 2;
                if(this.getClass() == Player.class) Server.getUserSession((Player) this).oos.writeObject("Critical Hit!");
            }
            target.takeDamage(this, damage);
        }
        else if(this.getClass() == Player.class) Server.getUserSession((Player) this).oos.writeObject("Attack missed");
    }

    public void onDeath(Entity killer) throws IOException {

    }

    public void heal(int amount) throws IOException {
        hp += amount;
        if (hp > maxHp) hp = maxHp;

        if (this.getClass() == Player.class)
            Server.getUserSession((Player) this).oos.writeObject("Healed " + amount + " HP");
        if (this.getClass() == Player.class) Server.getUserSession((Player) this).oos.writeObject("setHPHUD " + hp + "/" + maxHp);
    }

}
