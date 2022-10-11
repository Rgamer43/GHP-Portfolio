import java.io.IOException;

public class Fighter extends PlayerClass {
    public Fighter(){
        name = "Fighter";
        desc = "A tanky warrior";
        abilityDesc = "Martial Prowess: Increases STR by 5 for 10 seconds. Costs mana equal to your max mana.";
        absc = new int[]{2, 0, 1, -1, 0, 1};
        startingGear.add(new Longsword());
        startingGear.add(new HealingPotion());
        startingGear.add(new HealingPotion());
    }

    @Override
    public void ability(Player p) {
        if (p.mana >= p.maxMana) {
            try {
                Server.getUserSession(p).oos.writeObject("You use Martial Prowess!");
                p.absc[0].value += 5;
                p.mana -= p.maxMana;
                p.abilityActive = true;

                Thread thread = new Thread(new Runnable() {
                    @Override
                    public void run() {
                        try {
                            Thread.sleep(10000);
                            p.absc[0].value -= 5;
                            p.abilityActive = false;
                            Server.getUserSession(p).oos.writeObject("Martial Prowess has expired.");
                        } catch (InterruptedException | IOException e) {
                            e.printStackTrace();
                        }
                    }
                });
                thread.start();
            }
            catch (IOException e) {

            }
        }
    }

    @Override
    public void onLogout(Player p) {
        if (p.abilityActive) p.absc[0].value -= 5;
    }
}
