public class Ghost extends Monster{
    public Ghost(int l) {
        weapon = new Weapon("Spectral Punch", "Spectral Punch", 0, 4, new int[]{0,2},3,"sppnc");
        atkSpeed = 4500;
        respawnSpeed = 45000;
        maxHp = 7;
        hp = maxHp;
        ac = 10;
        name = "Ghost";
        location = l;
        gold = 20;
        alias = "gh";
    }
}
