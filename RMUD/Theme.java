public class Theme {
    public String[] roomAdjs;
    public Monster[] monsters;
    public Monster[] bossMonsters;
    public Item[] loot;

    public Theme(String[] roomAdjs, Monster[] monsters, Monster[] bossMonsters, Item[] loot) {
        this.roomAdjs = roomAdjs;
        this.monsters = monsters;
        this.bossMonsters = bossMonsters;
        this.loot = loot;
    }
}
