public class Equipment extends Item{

    public boolean isEquipped = false;

    public EquipmentSlot slot;

    public Equipment(String n, String d, int p, EquipmentSlot s, String a) {
        super(n, d, p, a);
        slot = s;
    }
}
