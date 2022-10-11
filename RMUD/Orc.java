public class Orc extends Monster {
    public Orc(int l) {
        weapon = new Weapon("Orc Scimitar", "", 0, 2, new int[]{4,6},0,"osc");
        atkSpeed = 5000;
        respawnSpeed = 45000;
        maxHp = 12;
        hp = maxHp;
        ac = 12;
        name = "Orc";
        location = l;
        gold = 30;
        alias = "o";
    }
}
