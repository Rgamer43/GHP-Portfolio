public class Goblin extends Monster {
    public Goblin(int l) {
        weapon = new Weapon("Scimitar", "Scimitar", 0, 1, new int[]{2,6},0,"sci");
        atkSpeed = 4500;
        respawnSpeed = 45000;
        maxHp = 6;
        hp = maxHp;
        ac = 11;
        name = "Goblin";
        location = l;
        gold = 15;
        alias = "gb";
    }
}
