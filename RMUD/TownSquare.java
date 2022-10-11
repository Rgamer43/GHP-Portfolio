public class TownSquare extends Location{
    public TownSquare(){
        name = "Town Square";
        entryMsg = "You come out into an open plaza. There are several stone buildings around it. In the \n" +
                "center lies a polished, stone statue of a humanoid figure covered in a draping robe.";
        exits.add(1);
        exits.add(2);
        exits.add(3);
        exits.add(4);
        exits.add(5);
        id = 0;

        alias = "0";
    }
}
