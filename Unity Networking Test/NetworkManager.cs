using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;

public class NetworkManager
{

    public static IPHostEntry host;
    public static IPAddress ipAddress;
    public static IPEndPoint endPoint;
    public static int port = 5000;
    public static int waitListSize = 1;

    public static Socket socket;
    public static BinaryFormatter bf = new BinaryFormatter();

    public static byte[] bytes = new byte[1024];

    [SerializeField]
    public static List<System.Object> outgoing = new List<System.Object>();

    public static void Host()
    {
        host = Dns.GetHostEntry("localhost");
        ipAddress = host.AddressList[0];
        endPoint = new IPEndPoint(ipAddress, port);

        socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(endPoint);
        socket.Listen(waitListSize);

        Debug.Log("Waiting for connection...");
        socket = socket.Accept();

        Debug.Log("Connected: " + socket.Connected);

        while(true)
        {
            if (outgoing.Count > 0)
            {
                Debug.Log("Outgoing Count: " + outgoing.Count);
                Debug.Log("Current Outgoing: " + outgoing[0]);
                Write(outgoing[0]);
                outgoing.RemoveAt(0);
            }
            Thread.Sleep(10);
        }
    }

    public static void Connect()
    {
        host = Dns.GetHostEntry("localhost");
        ipAddress = host.AddressList[0];
        endPoint = new IPEndPoint(ipAddress, port);

        socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        socket.Connect(endPoint);

        while(true)
        {
            object[] o = (object[])Read();
            NetworkHandler.received.Add(Unformat(o));
            Thread.Sleep(10);
        }
    }

    public static void Write(System.Object o)
    {
        Debug.Log(o);
        MemoryStream ms = new MemoryStream();
        Debug.Log(ms);
        bf.Serialize(ms, o);
        socket.Send(ms.ToArray());
    }

    public static System.Object Read()
    {
        int b = socket.Receive(bytes);
        MemoryStream ms = new MemoryStream();

        ms.Write(bytes, 0, b);
        ms.Seek(0, SeekOrigin.Begin);

        return bf.Deserialize(ms);
    }

    public static void Close()
    {
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }

    public static System.Object[] Format(object o)
    {
        if(o is Transform)
        {
            Transform t = (Transform)o;
            System.Object[] f = new System.Object[4];
            f[0] = typeof(Transform);

            f[1] = new float[] { t.position.x, t.position.y, t.position.z };
            f[2] = new float[] { t.localScale.x, t.localScale.y, t.localScale.z };
            f[3] = new float[] { t.rotation.x, t.rotation.y, t.rotation.z, t.rotation.w };

            return f;
        }

        return null;
    }

    public static System.Object[] Unformat(object o)
    {
        if(o is object[])
        {
            object[] obj = (object[])o;

            if(obj[0].GetType() == typeof(Transform))
            {

                float[] p = (float[])obj[1];
                Vector3 pos = new Vector3(p[0], p[1], p[2]);

                float[] s = (float[])obj[2];
                Vector3 scl = new Vector3(s[0], s[1], s[2]);

                float[] r = (float[])obj[3];
                Quaternion rot = new Quaternion(r[0], r[1], r[2], r[3]);

                return new object[] { pos, scl, rot };
            }
        }

        return null;
    }

}
