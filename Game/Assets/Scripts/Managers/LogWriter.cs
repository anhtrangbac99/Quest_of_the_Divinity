using System;
using System.IO;
using UnityEngine;

public class LogWriter : MonoBehaviour
{
    public static LogWriter Instance
    {
        get { return instance; }
        private set { instance = value; }
    }

    private static LogWriter instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    public string LogFileName
    {
        get { return log_file_name; }
        private set { log_file_name = value; }
    }

    private string log_file_name;

    public void Write(string data)
    {
        try
        {
            if (!File.Exists(LogFileName))
                File.WriteAllText(LogFileName, data + "\n");
            else
                File.AppendAllText(LogFileName, data + "\n");
        }
        catch (Exception e)
        {
            Debug.Log($"Error on writing to file: {LogFileName}: {e}");
        }
    }

    public void SetLogName()
    {
        string file_name = $"Gamelog_{DateTime.Now.ToString("dd_MM_H_mm_ss")}.json";
        string file_path = Application.persistentDataPath + "\\" + file_name;

        Debug.Log($"The log file is stored at: {Application.persistentDataPath}, named: {file_name}");
        LogFileName = file_path;
    }
}

[Serializable]
public class LogData
{
    public string environment;

    public string action;

    public LogData(string environment_state)
    {
        environment = environment_state;
        action = null;
    }

    public void SetTakenAction(string taken_action)
    {
        action = taken_action;
    }
}