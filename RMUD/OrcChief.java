public class OrcChief extends Monster {
    public OrcChief(int l) {
        weapon = new Weapon("Orc Greatsword", "", 0, 4, new int[]{5,8},0,"ogs");
        atkSpeed = 5000;
        respawnSpeed = 65000;
        maxHp = 45;
        hp = maxHp;
        ac = 14;
        name = "Orc Chief";
        location = l;
        gold = 200;
        alias = "oc";
    }
}
