import java.io.IOException;

public class Alchemist extends Location{

    public Shop shop;

    public Alchemist() {
        name = "Alchemist";
        id = 4;
        entryMsg = "You come into the alchemist's shop. Racks of potions line the walls. \n" +
                "Across the room is a counter, behind which stands an old woman. Use [talk] to talk to the alchemist";
        exits.add(0);
        shop = new Shop();
        shop.items.add(new HealingPotion());
        shop.items.add(new STRCrystal());
        shop.items.add(new DEXCrystal());
        shop.items.add(new CONCrystal());
        shop.items.add(new INTCrystal());
        shop.items.add(new AGICrystal());
        shop.items.add(new DEFCrystal());
        shop.items.add(new GreaterHealingPotion());
        shop.items.add(new TeleportScroll0());

        alias = "4";
    }

    @Override
    public boolean resolveInput(Player player, String[] input) throws IOException {
        if (input[0].equals("talk")) {
            Server.getUserSession(player).oos.writeObject("'Here's what I have for sale,' you hear in your mind." +
                    " Use [buy <item>] to purchase an item");
            shop.displayItems(player);

            return true;
        }

        else if(input[0].equals("buy")) {
            if (input.length == 1) Server.getUserSession(player).oos.writeObject("When using buy, please format it as [buy <item>]");
            else {
                String msg = "";
                for (int x = 1; x < input.length; x++) msg += input[x] + " ";
                String msg1 = msg.substring(0, msg.length()-1);
                msg = msg1;

                shop.buy(player, msg);
            }

            return true;
        }

        return false;
    }
}
