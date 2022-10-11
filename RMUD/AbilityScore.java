import java.io.Serializable;

public class AbilityScore implements Serializable {
    public String name;
    public int value;

    public AbilityScore(String n, int v){
        name = n;
        value = v;
    }
}
