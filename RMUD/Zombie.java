public class Zombie extends Monster{
    public Zombie(int l) {
        weapon = new ZombieFists();
        atkSpeed = 6000;
        respawnSpeed = 45000;
        maxHp = 5;
        hp = maxHp;
        ac = 8;
        name = "Zombie";
        location = l;
        gold = 15;
        alias = "z";
    }
}
