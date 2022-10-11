import java.io.IOException;

public class TemplateRoom extends Location {
    public LootChest chest;

    @Override
    public boolean resolveInput(Player player, String[] input) throws IOException {
        if (input[0].equals("open") && chest != null) {
            if (!chest.isLooted) {
                player.gold += chest.gold;
                Server.getUserSession(player).oos.writeObject("You found " + chest.gold + " gold in the chest");

                for (int x = 0; x < chest.items.size(); x++) {
                    player.inventory.add(chest.items.get(x));
                    Server.getUserSession(player).oos.writeObject("You found " + chest.items.get(x).name + " in the chest");
                }

                chest.isLooted = true;
            }
            else Server.getUserSession(player).oos.writeObject("The chest is empty");

            return true;
        }

        return false;
    }
}
