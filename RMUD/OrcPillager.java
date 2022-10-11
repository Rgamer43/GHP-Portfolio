public class OrcPillager extends Monster {
    public OrcPillager(int l) {
        weapon = new Weapon("Orc Axe", "", 0, 5, new int[]{3,6},0,"oax");
        atkSpeed = 4000;
        respawnSpeed = 50000;
        maxHp = 16;
        hp = maxHp;
        ac = 13;
        name = "Orc Pillager";
        location = l;
        gold = 35;
        alias = "op";
    }
}
