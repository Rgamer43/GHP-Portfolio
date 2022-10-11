import java.io.IOException;

public class Bank extends Location {

    public Shop shop;

    public Bank() {
        name = "Bank";
        id = 5;
        entryMsg = "You come into the bank. Behind the counter stands the \n" +
                "banker. 'Hello!' he says. Use [talk] to talk to the banker";
        exits.add(0);
        shop = new Shop();
        shop.items.add(new Coin50());

        alias = "5";
    }

    @Override
    public boolean resolveInput(Player player, String[] input) throws IOException {
        if (input[0].equals("talk")) {
            Server.getUserSession(player).oos.writeObject("'What can I do for you this fine day?' the banker asks cheerfully." +
                    " Use [buy <item>] \nto purchase an item");
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
