public class OrcWarlord extends Monster {
    public OrcWarlord(int l) {
        weapon = new Weapon("Orc Greataxe", "", 0, 1, new int[]{14,18},0,"oga");
        atkSpeed = 5000;
        respawnSpeed = 65000;
        maxHp = 60;
        hp = maxHp;
        ac = 14;
        name = "Orc Warlord";
        location = l;
        gold = 300;
        alias = "owl";
    }
}
