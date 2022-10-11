package rgamer43.github.io.rutils;

public class Role {
    public String name;
    public int cost;
    public boolean limitedTime = false;

    public Role(String n, int c) {
        name = n;
        cost = c;
    }

    public Role(String n, int c, boolean l) {
        name = n;
        cost = c;
        limitedTime = l;
    }
}
