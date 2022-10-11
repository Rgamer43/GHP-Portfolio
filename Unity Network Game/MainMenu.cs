using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Text nameInput, ip, joinPort, hostPort;
    public NetworkManager network;
    public UnityTransport transport;
    public GameObject gameManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Join()
    {
        if (ip.text == "") ip.text = "127.0.0.1";
        if (joinPort.text == "") joinPort.text = "7777";

        if (ip.text != "" && joinPort.text != "")
        {
            transport.ConnectionData.Address = ip.text;
            transport.ConnectionData.Port = (ushort)int.Parse(joinPort.text);
            network.StartClient();
            Destroy(gameObject);
        } else
        {
            if (ip.text == "") transport.ConnectionData.Address = "127.0.0.1";
            if(joinPort.text == "") transport.ConnectionData.Port = (ushort)1001;
            network.StartClient();
            Destroy(gameObject);
        }

        gameManager.SetActive(true);
    }

    public void Host()
    {
        if (hostPort.text == "") hostPort.text = "7777";

        if (hostPort.text != "")
        {
            transport.ConnectionData.Port = (ushort)int.Parse(hostPort.text);
            network.StartHost();
            Destroy(gameObject);
        } else
        {
            transport.ConnectionData.Port = (ushort)1001;
            network.StartHost();
            Destroy(gameObject);
        }

        gameManager = Instantiate(gameManager);
        gameManager.GetComponent<NetworkObject>().Spawn();
        gameManager.SetActive(true);
    }
}
