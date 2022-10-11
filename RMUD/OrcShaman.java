public class OrcShaman extends Monster {
    public OrcShaman(int l) {
        weapon = new Weapon("Orc Staff", "", 0, -2, new int[]{10,14},3,"ost");
        atkSpeed = 6000;
        respawnSpeed = 50000;
        maxHp = 10;
        hp = maxHp;
        ac = 11;
        name = "Orc Shaman";
        location = l;
        gold = 40;
        alias = "os";
    }
}
