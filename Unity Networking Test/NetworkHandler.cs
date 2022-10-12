using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NetworkHandler : MonoBehaviour
{

    public static List<object[]> received = new List<object[]>();

    public GameObject local, network;

    // Start is called before the first frame update
    void Start()
    {
        Application.quitting += NetworkManager.Close;
    }

    // Update is called once per frame
    void Update()
    {
        if(NetworkManager.socket != null && NetworkManager.socket.Connected) NetworkManager.outgoing.Add(NetworkManager.Format(local.transform));
        if (received.Count > 0)
        {
            object[] rec = NetworkManager.Unformat(received[0]);
            network.transform.position = (Vector3)rec[0];
            network.transform.localScale = (Vector3)rec[1];
            network.transform.rotation = (Quaternion)rec[2];
            received.RemoveAt(0);
        }
    }

    public void Host()
    {
        Thread thread = new Thread(NetworkManager.Host);
        thread.Start();
    }

    public void Connect()
    {
        Thread thread = new Thread(NetworkManager.Connect);
        thread.Start();
    }
}
