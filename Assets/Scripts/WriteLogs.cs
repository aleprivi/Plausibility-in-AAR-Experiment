using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//using System;
using System.Linq;

public class WriteLogs : MonoBehaviour
{
    public static string filename = "testfile";
    public static string userNum = "noUser";
    public static string CIPIC = "000";
    
    /*
    conditions:
    0 = random
    1 = NON intimate
    2 = intimate
    */
    public static int condition = 0;

    public static string GetLastCIPIC(){
        //get CIPIC user, if exist
        //if use persistentdatapath get newest directory inside and print it
        string[] dirs = Directory.GetDirectories(Application.persistentDataPath);
        string newestDir = dirs[0];
        foreach(string dir in dirs){
            if(Directory.GetCreationTime(dir) > Directory.GetCreationTime(newestDir)){
                newestDir = dir;
            }
        }
        
        //take the last folder name
        string folderdelimit = "/";
        #if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
            folderdelimit = "\\";
        #endif
        string last = newestDir.Split(folderdelimit).Last();
        //remove the last 4 char from the string and the first 7
        last = last.Substring(7, last.Length-11);
        CIPIC = last;
        return CIPIC;
    }
    public static string GetLastUser()
    {
        //Get Last User from file, if exists
        string path = Application.persistentDataPath + "/UserList.csv";
        if (File.Exists(path))
        {
            string[] availableUsers = File.ReadAllLines(path)[0].Split(',');
            Debug.Log("Last user: " + File.ReadAllLines(path)[0]);
            userNum = availableUsers[availableUsers.Length - 2];
            return availableUsers[availableUsers.Length - 2];
        }

        return null;
    }

    static string generateID(string prefix){
        string path = Application.persistentDataPath + "/UserList.csv";
        //Read the text from directly from the test.txt file
        string[] availableUsers = {};
        if (File.Exists(path))
        {
            availableUsers = File.ReadAllLines(path)[0].Split(',');
        }
        bool trovato = true;
        string user = "";
        while (trovato) {
            user = prefix + Random.Range(1000, 5000);
            trovato = false;
            foreach (string el in availableUsers) {
                if(el.Equals(user)) trovato = true;
            }
        }
        userNum = user;
        return userNum;
    }

    public static string GetNewUser()
    {
        return generateID("U");
    }

    public static void Init(string user) {
        userNum = user;
        Init();
    }

    public static void InitTestMode(){
        userNum = generateID("T");
        Init();
    }

    public static void Init() {
        string path = Application.persistentDataPath + "/UserList.csv";
        StreamWriter wr = new StreamWriter(path, true);
        wr.Write(userNum + ",");
        wr.Close();


        //Salvo i vari file necessari al log
        string Log_path = Application.persistentDataPath + "/" + filename + userNum + ".csv";

        string training = "/" + System.DateTime.Now.ToString("yyyy_MM_dd") + "_training.csv";
        string slater = "/" + System.DateTime.Now.ToString("yyyy_MM_dd") + "_slater.csv";
        string slaterTable = "/" + System.DateTime.Now.ToString("yyyy_MM_dd") + "_slaterTable.csv";
        string sdt = "/" + System.DateTime.Now.ToString("yyyy_MM_dd") + "_sdt.csv";

        Debug.Log("Training: " + training);

        path = Application.persistentDataPath + training;
        Debug.Log("Path: " + path);
        if (!File.Exists(path))
        {
            StreamWriter writer = new StreamWriter(path, true);
            string val = "User,Time,HeadX,HeadY,HeadZ,iPadX,iPadY,iPadZ,targetreached";
            writer.WriteLine(val);
            writer.Close();
            Debug.Log("File now exists. No problem... happy testing ;)");
        }

        path = Application.persistentDataPath + slater;
        if (!File.Exists(path))
        {
            StreamWriter writer = new StreamWriter(path, true);
            string val = "User,Step,Time,HeadX,HeadY,HeadZ,iPadX,iPadY,iPadZ,AgentX,AgentY, AgentZ";
            writer.WriteLine(val);
            writer.Close();
            Debug.Log("File now exists. No problem... happy testing ;)");
        }

        path = Application.persistentDataPath + slaterTable;
        if (!File.Exists(path))
        {
            StreamWriter writer = new StreamWriter(path, true);
            //La distanza non serve perchè la calcolo dopo
            string val = "User,Step,Time,SelectedAction,Reward,Condition,T1,T2,T3,T4,"+
            "T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T15,T16,T17,T18,T19,T20,T21,T22,T23,T24";
            writer.WriteLine(val);
            writer.Close();
            Debug.Log("File now exists. No problem... happy testing ;)");
        }

        path = Application.persistentDataPath + sdt;
        if (!File.Exists(path))
        {
            StreamWriter writer = new StreamWriter(path, true);
            string val = "Time,User,Condition1,Condition2,Selected";
            writer.WriteLine(val);
            writer.Close();
            Debug.Log("File now exists. No problem... happy testing ;)");
        }


        steps = 0;
    }





    public static void WriteFloatArray(float[] f, string filename){
        string path = Application.persistentDataPath + "/" + filename + ".csv";
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path);
        string val = "";
        foreach (float el in f) {
            val += el + ",";
        }
        //remove last ,
        val = val.Substring(0, val.Length - 1);
        writer.WriteLine(val);
        writer.Close();
    }
    public static void WriteFloatArray(float[][] f, string filename){
        string path = Application.persistentDataPath + "/" + filename + ".csv";
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path);
        string val = "";
        foreach (float[] el in f) {
            foreach (float el2 in el) {
                val += el2 + ";";
            }
            //remove last ;
            val = val.Substring(0, val.Length - 1);
            val += "\n";
        }
        writer.WriteLine(val);
        writer.Close();
    }




    public static void WriteTrainingLog(float currentTime, float HeadX, float HeadY, float HeadZ, float iPadX, float iPadY, float iPadZ, int targetreached) {
        string val = "User,Time,HeadX,HeadY,HeadZ,iPadX,iPadY,iPadZ,targetreached";
        string path = Application.persistentDataPath +  "/" + System.DateTime.Now.ToString("yyyy_MM_dd") + "_training.csv";
        StreamWriter writer = new StreamWriter(path, true);
        val = WriteLogs.userNum + "," + currentTime + "," + HeadX + "," + HeadY + "," + HeadZ + "," + iPadX + "," + iPadY + "," + iPadZ + "," + targetreached;
        writer.WriteLine(val);
        writer.Close();
    }

    static int steps = 0;
    public static void WriteSlaterLog(float currentTime, float HeadX, float HeadY, float HeadZ, float iPadX, float iPadY, float iPadZ, float AgentX, float AgentY, float AgentZ)
    {
        //string val = "User,Step,Time,HeadX,HeadY,HeadZ,iPadX,iPadY,iPadZ,AgentX,AgentY, AgentZ";
        string path = Application.persistentDataPath + "/" + System.DateTime.Now.ToString("yyyy_MM_dd") + "_slater.csv";
        StreamWriter writer = new StreamWriter(path, true);
        string val = WriteLogs.userNum + "," + steps + "," + currentTime + "," + HeadX + "," + HeadY + "," + HeadZ + "," + iPadX + "," + iPadY + "," + iPadZ + "," + AgentX + "," + AgentY + "," + AgentZ;
        steps++;
        writer.WriteLine(val);
        writer.Close();
    }



    public static void WriteQTable(float currentTime, int selectedAction, float reward, float[][] qtable)
    {
        //string val = "User,Step,Time,ActionNum,SelectedAction,Reward,Condition,T1,T2,T3,T4,"+
        //    "T5,T6,T7,T8,T9,T10,T11,T12,T13,T14,T16,T17,T18,T19,T20,T21,T22,T23,T24,T25,T26,T27,"+
        //    "T28,T29,T30,T31,T32,T33,T34,T35";
        string path = Application.persistentDataPath + "/" + System.DateTime.Now.ToString("yyyy_MM_dd") + "_slaterTable.csv";
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        string val = WriteLogs.userNum + "," + steps + "," + currentTime + "," + selectedAction + "," + reward + "," + condition + ",";

        foreach (float[] x in qtable)
        {
            foreach (float y in x)
            {
                val += y + ","; 
            }
        }
        writer.WriteLine(val);
        writer.Close();
    }
}
