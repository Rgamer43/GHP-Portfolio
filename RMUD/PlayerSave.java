import java.io.Serializable;
import java.util.ArrayList;

public class PlayerSave implements Serializable {
    public int location;
    public String username;
    public int gold;
    public Weapon weapon;
    public Armor armor;
    public ArrayList<Item> inventory;
    public int maxHp;
    public int hp;
    public int ac;
    public int[] absc;
    public int pClass;
}
