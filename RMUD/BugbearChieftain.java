public class BugbearChieftain extends Monster {
    public BugbearChieftain(int l) {
        weapon = new Weapon("Greatclub", "Greatclub", 0, 4, new int[]{3,8},0,"gcb");
        atkSpeed = 5000;
        respawnSpeed = 50000;
        maxHp = 20;
        hp = maxHp;
        ac = 12;
        name = "Bugbear Chieftain";
        location = l;
        gold = 125;
        alias = "bcf";
    }
}
