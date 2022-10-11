import java.io.*;

public class PlayerSaveManager {

    static PlayerClass[] classes = new PlayerClass[]{
            new Fighter(),
            new Wizard()
    };

    public static Player load(PlayerSave save) throws EOFException {
        try {
            Player p = new Player();

            p.location = save.location;
            p.username = save.username;
            p.ac = save.ac;
            p.armor = save.armor;
            p.gold = save.gold;
            p.weapon = save.weapon;
            p.inventory = save.inventory;
            p.maxHp = save.maxHp;
            p.hp = save.hp;
            p.name = p.username;
            p.absc[0].value = save.absc[0];
            p.absc[1].value = save.absc[1];
            p.absc[2].value = save.absc[2];
            p.absc[3].value = save.absc[3];
            p.absc[4].value = save.absc[4];
            p.absc[5].value = save.absc[5];
            p.alias = p.name;
            p.pClass = classes[save.pClass];

            if(p.location >= Server.townSize) p.location = 2;

            return p;
        }
        catch (Exception e) {
            e.printStackTrace();
        }

        return null;
    }

    public static PlayerSave save(Player p){
        try {
            PlayerSave save = new PlayerSave();

            save.location = p.location;
            save.username = p.username;
            save.ac = p.ac;
            save.armor = p.armor;
            save.gold = p.gold;
            save.weapon = p.weapon;
            save.inventory = p.inventory;
            save.maxHp = p.maxHp;
            save.hp = p.hp;
            save.absc = new int[6];
            save.absc[0] = p.absc[0].value;
            save.absc[1] = p.absc[1].value;
            save.absc[2] = p.absc[2].value;
            save.absc[3] = p.absc[3].value;
            save.absc[4] = p.absc[4].value;
            save.absc[5] = p.absc[5].value;

            for (int x = 0; x < classes.length; x++)
                if (p.pClass.name.equals(classes[x].name))
                    save.pClass = x;

            if(save.location >= Server.townSize) save.location = 2;

            return save;
        }
        catch (Exception e) {
            e.printStackTrace();
        }

        return null;
    }

}
