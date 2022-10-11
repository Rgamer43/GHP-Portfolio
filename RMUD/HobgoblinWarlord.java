public class HobgoblinWarlord extends Monster {
    public HobgoblinWarlord(int l) {
        weapon = new Weapon("Flail", "Flail", 0, 2, new int[]{5,12},0,"fll");
        atkSpeed = 4500;
        respawnSpeed = 55000;
        maxHp = 24;
        hp = maxHp;
        ac = 13;
        name = "Hobgoblin Warlord";
        location = l;
        gold = 135;
        alias = "hgw";
    }
}
