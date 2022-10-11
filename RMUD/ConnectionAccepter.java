import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.EOFException;
import java.io.IOException;
import java.net.*;

public class ConnectionAccepter extends Thread{
    public void run() {
        try {
            ServerSocket ss = new ServerSocket(7778);

            while (true) {
                try {
                    Socket socket = null;
                    socket = ss.accept();
                    System.out.println("Accepted connection from " + socket);

                    Server.sessions.add(new UserSession(socket));

                    Server.sessions.get(Server.sessions.size() - 1).start();

                    //socket.close();
                }
                catch (Exception e) {
                    e.printStackTrace();
                }
            }
        }
        catch (Exception e){
            e.printStackTrace();
        }
    }
}
