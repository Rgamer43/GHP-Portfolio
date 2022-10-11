import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.util.ArrayList;

public class AccountManager {

    public static ArrayList<Account> accounts = new ArrayList<>();

    public static void load(){
        try {
            FileInputStream file = new FileInputStream(".accounts.save");
            ObjectInputStream in = new ObjectInputStream(file);

            accounts = (ArrayList<Account>) in.readObject();

            in.close();
            file.close();

            System.out.println("Loaded accounts");
        }
        catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static void save(){
        try {
            FileOutputStream file = new FileOutputStream(".accounts.save");
            ObjectOutputStream out = new ObjectOutputStream(file);

            out.writeObject(accounts);

            out.close();
            file.close();

            System.out.println("Saved accounts");
        }
        catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static boolean attemptLogIn(String u, String p){
        for (int x = 0; x < accounts.size(); x++){
            if (accounts.get(x).username.equals(u) && accounts.get(x).password.equals(p))
                return true;
        }

        return false;
    }

    public static boolean isUsernameTaken(String u){
        for (int x = 0; x < accounts.size(); x++){
            if(accounts.get(x).username.equals(u)) return true;
        }

        return false;
    }

    public static Account getAccount(String u) {
        for (int x = 0; x < accounts.size(); x++){
            if(accounts.get(x).username.equals(u)) return accounts.get(x);
        }

        return null;
    }
}
