public class Weapon extends Equipment{

    public int atkBonus;
    public int[] damage = new int[2];
    public int absc;

    public Weapon(String n, String d, int p,int atk, int[] da, int absc, String a) {
        super(n, d, p, EquipmentSlot.weapon, a);
        atkBonus = atk;
        damage[0] = da[0];
        damage[1] = da[1];
        this.absc = absc;
    }
}
