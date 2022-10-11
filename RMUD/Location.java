import java.io.DataOutputStream;
import java.io.IOException;
import java.io.ObjectOutputStream;
import java.io.Serializable;
import java.net.SocketException;
import java.util.ArrayList;

public class Location implements Serializable {
    public String name;
    public String entryMsg;
    public ArrayList<Integer> exits = new ArrayList<>();

    public String alias;

    public int id;

    public ArrayList<Player> occupants = new ArrayList<>();
    public ArrayList<Monster> inhabitants = new ArrayList<>();

    public void enter(Player p, boolean displayLeave) throws IOException {
         if (displayLeave) for (int x = 0; x < Server.locations[p.location].occupants.size(); x++)
            try { Server.getUserSession(Server.locations[p.location].occupants.get(x)).oos.writeObject(
                    p.name + " leaves the location, moving into " + name); }
            catch (SocketException e) {

            }
        Server.locations[p.location].occupants.remove(p);
        occupants.add(p);
        for (int x = 0; x < occupants.size(); x++)
            Server.getUserSession(occupants.get(x)).oos.writeObject(
                    p.name + " enters the location");

        ObjectOutputStream pDos = Server.getUserSession(p).oos;

        String s = "setRoomDesc ";

        pDos.writeObject("---");
        pDos.writeObject(name);
        s += name + "\n";
        pDos.writeObject(entryMsg);
        s += entryMsg + "\n";
        pDos.writeObject("There are the following monsters and players in this location:");
        s += "There are the following monsters and players in this location:\n";
        for (int x = 0; x < occupants.size(); x++) {
            pDos.writeObject("    " + occupants.get(x).name);
            s += "    " + occupants.get(x).name + "\n";
        }
        for (int x = 0; x < inhabitants.size(); x++) {
            pDos.writeObject("    " + inhabitants.get(x).name + " (" + inhabitants.get(x).alias + ")");
            s += "    " + inhabitants.get(x).name + " (" + inhabitants.get(x).alias + ")\n";
        }
        pDos.writeObject("This location has the following exits:");
        s += "This location has the following exits:\n";
        for (int x = 0; x < exits.size(); x++) {
            pDos.writeObject("    " + Server.locations[exits.get(x)].name + " (" + Server.locations[exits.get(x)].alias + ")");
            s += "    " + Server.locations[exits.get(x)].name + " (" + Server.locations[exits.get(x)].alias + ")\n";
        }

        //System.out.println("p.location (prev): " + Server.getUserSession(p).player.location);
        Server.getUserSession(p).player.location = id;
        //System.out.println("p.location (new): " + Server.getUserSession(p).player.location);

        for (int x = 0; x < occupants.size(); x++)
            Server.getUserSession(occupants.get(x)).oos.writeObject(s);
    }

    public boolean resolveInput(Player player, String[] input) throws IOException {
        return false;
    }

    public void chat(Player p, String msg) throws IOException {
        System.out.println(occupants.size());
        for (int x = 0; x < occupants.size(); x++) {
            Server.getUserSession(occupants.get(x)).oos.writeObject(p.username + " says: " + msg);
            System.out.println(occupants.get(x).username);
        }

        System.out.println("Chat in " + name + ": " + p.username + " says: " + msg);
    }

    public Entity getEntity(String name) {
        for (int x = 0; x < occupants.size(); x++) {
            if (occupants.get(x).name.toLowerCase().equals(name) ||
                    occupants.get(x).alias.toLowerCase().equals(name))
                return occupants.get(x);
        }

        for (int x = 0; x < inhabitants.size(); x++) {
            if (inhabitants.get(x).name.toLowerCase().equals(name) ||
                    inhabitants.get(x).alias.toLowerCase().equals(name))
                return inhabitants.get(x);
        }

        return null;
    }

    public String getRoomDesc() {
        String s = "setRoomDesc ";

        s += name + "\n";
        s += entryMsg + "\n";
        s += "There are the following monsters and players in this location:\n";
        for (int x = 0; x < occupants.size(); x++) {
            s += "    " + occupants.get(x).name + "\n";
        }
        for (int x = 0; x < inhabitants.size(); x++) {
            s += "    " + inhabitants.get(x).name + " (" + inhabitants.get(x).alias + ")\n";
        }
        s += "This location has the following exits:\n";
        for (int x = 0; x < exits.size(); x++) {
            s += "    " + Server.locations[exits.get(x)].name + " (" + Server.locations[exits.get(x)].alias + ")\n";
        }

        return s;
    }

    public void AddExit(int i) {
        if(!exits.contains(i)) exits.add(i);
    }
}
