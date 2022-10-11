public class Armor extends Equipment{

    public int acb; //AC bonus

    public Armor(String n, String d, int p, int a, String al) {
        super(n, d, p, EquipmentSlot.armor, al);
        acb = a;
    }
}
