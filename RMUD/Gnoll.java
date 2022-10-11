public class Gnoll extends Monster {
    public Gnoll(int l) {
        weapon = new Weapon("Gnoll Claws", "Gnoll Claws", 0, 2, new int[]{0,2},0,"gcl");
        atkSpeed = 3500;
        respawnSpeed = 35000;
        maxHp = 4;
        hp = maxHp;
        ac = 10;
        name = "Gnoll";
        location = l;
        gold = 15;
        alias = "gn";
    }
}
