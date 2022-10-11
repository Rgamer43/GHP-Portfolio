import java.io.IOException;

public class Wizard extends PlayerClass{
    public Wizard(){
        name = "Wizard";
        desc = "A powerful spellcaster";
        abilityDesc = "Arcane Regeneration: Costs mana equal to your max mana. Heals HP equal to 3 x INT.";
        absc = new int[]{-1, 1, 0, 2, 1, 0};
        startingGear.add(new MagicMissile());
        startingGear.add(new HealingPotion());
        startingGear.add(new HealingPotion());
    }

    @Override
    public void ability(Player p) {
        if (p.mana >= p.maxMana) {
            try {
                Server.getUserSession(p).oos.writeObject("You use Arcane Regeneration!");
                p.heal(3 * p.absc[3].value);
                p.mana -= p.maxMana;
            }
            catch (IOException e) {

            }
        }
    }
}
