public class Bugbear extends Monster {
    public Bugbear(int l) {
        weapon = new Weapon("Spiked Club", "Spiked Club", 0, 3, new int[]{2,5},0,"spcl");
        atkSpeed = 3500;
        respawnSpeed = 35000;
        maxHp = 12;
        hp = maxHp;
        ac = 12;
        name = "Goblin";
        location = l;
        gold = 25;
        alias = "gb";
    }
}
