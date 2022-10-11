public class Skeleton extends Monster {
    public Skeleton(int l) {
        weapon = new Longsword();
        atkSpeed = 5000;
        respawnSpeed = 40000;
        maxHp = 4;
        hp = maxHp;
        ac = 10;
        name = "Skeleton";
        location = l;
        gold = 20;
        alias = "sk";
    }
}
