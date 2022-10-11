public class DungeonEntrance extends Location{
    public DungeonEntrance() {
        name = "Dungeon Entrance";
        entryMsg = "You come to in a section between three hills, covered with dark green grass. The rain \n" +
                "forms puddles on the ground. Behind you is the town square, some 150 yards away. Ahead \n" +
                "of you is an open stone doorway, leading into the dungeon.";
        id = 2;
        exits.add(0);
        exits.add(Server.townSize);

        alias = "2";
    }
}
