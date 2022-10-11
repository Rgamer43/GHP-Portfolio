public class Hobgoblin extends Monster {
    public Hobgoblin(int l) {
        weapon = new Weapon("Mace", "Mace", 0, 1, new int[]{3,6},0,"mc");
        atkSpeed = 4000;
        respawnSpeed = 45000;
        maxHp = 8;
        hp = maxHp;
        ac = 13;
        name = "Hobgoblin";
        location = l;
        gold = 35;
        alias = "hgb";
    }
}
