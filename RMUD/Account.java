import java.io.Serializable;

public class Account implements Serializable {
    public String username;
    public String password;
    public boolean hasPlayedBefore;

    public Account(String u, String p){
        username = u;
        password = p;
        hasPlayedBefore = false;
    }
}
