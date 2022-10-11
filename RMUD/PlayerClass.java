import java.io.Serializable;
import java.util.ArrayList;

public class PlayerClass implements Serializable {
    public String name;
    public String desc;
    public String abilityDesc;

    public int[] absc = new int[6];

    public ArrayList<Item> startingGear = new ArrayList<>();

    public PlayerClass(){

    }

    public void ability(Player p) {

    }

    public void onLogout(Player p) {

    }
}
