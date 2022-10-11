using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class Log
{
    private static List<string> log = new List<string>();
    private static bool initted = false;
    private static Thread thread;
    private static string persistentDataPath = Application.persistentDataPath;

    public static StreamWriter streamWriter;
    public static bool shouldClose = false;

    public static void LogMsg(string m)
    {
        Debug.Log(m);
        log.Add(m);
        if (!initted)
        {
            thread = new Thread(new ThreadStart(LogLoop));
            thread.Start();
        }
    }

    public static void LogWarning(string m)
    {
        Debug.LogWarning(m);
        log.Add("WARNING: " + m);
        if (!initted)
        {
            thread = new Thread(new ThreadStart(LogLoop));
            thread.Start();
        }
    }

    public static void LogError(string m)
    {
        Debug.LogError(m);
        log.Add("ERROR: " + m);
        if (!initted)
        {
            thread = new Thread(new ThreadStart(LogLoop));
            thread.Start();
        }
    }

    public static void LogException(System.Exception m)
    {
        Debug.LogException(m);
        log.Add("EXCEPTION: " + m.ToString() + "\n" + m.StackTrace);
        if (!initted)
        {
            thread = new Thread(new ThreadStart(LogLoop));
            thread.Start();
        }
    }

    public static void LogFatal (string m)
    {
        Debug.LogWarning(m);
        log.Add("FATAL ERROR: " + m);
        if (!initted)
        {
            thread = new Thread(new ThreadStart(LogLoop));
            thread.Start();
        }
    }

    public static void LogSevere(string m)
    {
        Debug.LogWarning(m);
        log.Add("SEVERE ERROR: " + m);
        if (!initted)
        {
            thread = new Thread(new ThreadStart(LogLoop));
            thread.Start();
        }
    }

    public static void LogImportant(string m)
    {
        Debug.Log(m);
        log.Add("IMPORTANT: " + m);
        if (!initted)
        {
            thread = new Thread(new ThreadStart(LogLoop));
            thread.Start();
        }
    }

    public static void LogLoop()
    {
        Debug.Log("Started log thread");
        FileStream logFile = File.Create(persistentDataPath + "/log.txt");
        logFile.Dispose();

        StreamWriter writer = new StreamWriter(persistentDataPath + "/log.txt", true);
        streamWriter = writer;

        initted = true;

        while (true)
        {
            while (log.Count > 0)
            {
                writer.WriteLine("[" + System.DateTime.Now.ToLongTimeString() + "] " + log[0]);
                log.RemoveAt(0);
            }

            if(shouldClose)
            {
                streamWriter.Close();
                break;
            }
        }

        writer.Close();
    }
}
