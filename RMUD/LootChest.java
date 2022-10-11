import java.util.ArrayList;

public class LootChest implements Runnable {

    public int gold;
    public ArrayList<Item> items = new ArrayList<>();

    public int resetTime;

    public boolean isLooted = false;

    @Override
    public void run() {
        while (true) {
            try {
                Thread.sleep(resetTime);
                isLooted = false;
            } catch (InterruptedException e) {
                e.printStackTrace();
            }

        }
    }
}
