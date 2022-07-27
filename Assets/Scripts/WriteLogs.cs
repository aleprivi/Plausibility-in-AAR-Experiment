using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WriteLogs : MonoBehaviour
{
    public static string filename = "testfile";
    public static string userNum = "P000";

    public static string GetNewUser()
    {
        string path = Application.persistentDataPath + "/UserList.csv";
        //Read the text from directly from the test.txt file

        string[] availableUsers = { };

        if (File.Exists(path))
        {
            StreamReader reader = new StreamReader(path);
            availableUsers = reader.ReadLine().Split(",");
            reader.Close();
        }

        bool trovato = true;
        string user = "";
        while (trovato) {
            user = "U" + Random.Range(1000, 5000);
            trovato = false;
            foreach (string el in availableUsers) {
                if(el.Equals(user)) trovato = true;
            }
        }
        userNum = user;
        return userNum;
    }

    public static void Init(string user) {
        userNum = user;
        Init();
    }

    public static void Init() {
        string path = Application.persistentDataPath + "/UserList.csv";
        StreamWriter wr = new StreamWriter(path, true);
        wr.Write(userNum + ",");
        wr.Close();

        string Log_path = Application.persistentDataPath + "/" + filename + userNum + ".csv";
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(Log_path, true);
        string val = "Time,Action,STEPS,z_POSAGENT,x_USER,y_USER,z_USER,REWARD,DISTANCE";
        writer.WriteLine(val);
        writer.Close();
    }



    private void Start()
    {
    }

    public static void WriteExperimentLog(string val)
    {
        string path = Application.persistentDataPath + "/ExperimentLog_" + userNum + ".csv";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(Time.time +  "," + val);
        writer.Close();
    }

    public static void WriteTrainingLog(string val) {
        string path = Application.persistentDataPath + "/Training_" + userNum + ".csv";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(Time.time + "," + val);
        writer.Close();
    }

    public static void WriteQTable(string filename, float[][] val)
    {
        string path = Application.persistentDataPath + "/QTable_" + filename + userNum + ".csv";
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.Write(Time.time + ",");
        foreach (float[] x in val)
        {
            foreach (float y in x)
            {
                writer.Write(y+",");
            }
        }
        writer.WriteLine("");
        writer.Close();
    }
}
