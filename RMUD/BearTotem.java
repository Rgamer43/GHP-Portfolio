import java.io.IOException;

public class BearTotem extends Artifact {
    public BearTotem() {
        super("Bear Totem", "A totem of a bear. Grants +10 STR for 5 seconds. Costs 90 mana",
                250, "brtm", 90);
    }

    @Override
    public void use(Player p) {
        p.activeArtifacts.add(this);
        Thread thread = new Thread(new Runnable() {
            @Override
            public void run() {
                try {
                    Server.getUserSession(p).oos.writeObject("The bear totem fills you with the strength of a bear.");
                    p.absc[0].value += 10;
                    Thread.sleep(5000);
                    p.absc[0].value -= 10;
                    Server.getUserSession(p).oos.writeObject("The bear-like strength leaves your body.");
                    p.activeArtifacts.remove(this);
                } catch (InterruptedException | IOException e) {
                    e.printStackTrace();
                }
            }
        });
        thread.start();
    }

    @Override
    public void onLogout(Player p) {
        p.absc[0].value -= 10;
    }
}
