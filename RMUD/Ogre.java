public class Ogre extends Monster {
    public Ogre(int l) {
        weapon = weapon = new Weapon("Ogre Club", "Ogre Club", 0, 2, new int[]{2,12},0,"ogcb");
        atkSpeed = 5500;
        respawnSpeed = 60000;
        maxHp = 35;
        hp = maxHp;
        ac = 10;
        name = "Ogre";
        location = l;
        gold = 150;
        alias = "or";
    }
}