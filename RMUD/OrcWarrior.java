public class OrcWarrior extends Monster {
    public OrcWarrior(int l) {
        weapon = new Weapon("Orc Battleaxe", "", 0, 2, new int[]{5,7},0,"ob");
        atkSpeed = 4500;
        respawnSpeed = 45000;
        maxHp = 22;
        hp = maxHp;
        ac = 13;
        name = "Orc Warrior";
        location = l;
        gold = 40;
        alias = "ow";
    }
}
