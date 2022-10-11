import java.io.IOException;

public class Blacksmith extends Location{

    public Shop shop;

    public Blacksmith() {
        name = "Blacksmith";
        id = 3;
        entryMsg = "You come into the blacksmith's shop. A lantern lights up the cold, stone building." +
                "'Hello.' the \nblacksmith says through his thick beard. Use [talk] to talk to the blacksmith";
        exits.add(0);
        shop = new Shop();
        shop.items.add(new Longsword());
        shop.items.add(new Quarterstaff());
        shop.items.add(new Greatsword());
        shop.items.add(new Chainmail());
        shop.items.add(new MagicMissile());
        shop.items.add(new Fireball());
        shop.items.add(new PlateArmor());
        shop.items.add(new MithralLongsword());
        shop.items.add(new NecroticTouch());

        alias = "3";
    }

    @Override
    public boolean resolveInput(Player player, String[] input) throws IOException {
        if (input[0].equals("talk")) {
            Server.getUserSession(player).oos.writeObject("'Here's what I have for sale,' the blacksmith grunts." +
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
