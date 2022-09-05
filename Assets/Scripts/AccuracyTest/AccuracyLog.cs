using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;
using System.IO;

public class AccuracyLog : MonoBehaviour
{
    public Text testo;
    public Text testo_timer;
    Camera ars;
    ARSessionOrigin arso;
    public GameObject head;

    Vector3 startingPosition;
    Quaternion startingRotation;


    // Start is called before the first frame update
    void Start()
    {
        
        ars = GameObject.FindObjectOfType<Camera>();
        arso = GameObject.FindObjectOfType<ARSessionOrigin>();


        string path = Application.persistentDataPath + "/accuracyTest.csv";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        string val = "Time,SeqNum," +
            "Cam_X_Normalized,Cam_Y_Normalized,Cam_Z_Normalized," +
            "Cam_X_Raw,Cam_Y_Raw,Cam_Z_Or," +
            "Y_ROT," +
            "Head_X_Normalized,Head_Y_Normalized,Head_Z_Normalized," +
            "Head_X_Raw,Head_Y_Raw,Head_Z_Raw";

        writer.WriteLine(val);
        writer.Close();
    }

    public void printThings() {

        StringBuilder m_Info = new StringBuilder();
        m_Info.Clear();
        m_Info.Append("ARSession: ");
        m_Info.Append(Math.Round(ars.transform.position.x, 2));
        m_Info.Append("-");
        m_Info.Append(Math.Round(ars.transform.position.y, 2));
        m_Info.Append("-");
        m_Info.Append(Math.Round(ars.transform.position.z, 2));
        m_Info.AppendLine();
        m_Info.Append("ARSessionOrigin: ");
        m_Info.Append(Math.Round(arso.transform.position.x, 2));
        m_Info.Append("-");
        m_Info.Append(Math.Round(arso.transform.position.y, 2));
        m_Info.Append("-");
        m_Info.Append(Math.Round(arso.transform.position.z, 2));
        m_Info.AppendLine();
        m_Info.Append("Oggetto: ");
        m_Info.Append(Math.Round(head.transform.position.x, 2));
        m_Info.Append("-");
        m_Info.Append(Math.Round(head.transform.position.y, 2));
        m_Info.Append("-");
        m_Info.Append(Math.Round(head.transform.position.z, 2));
        m_Info.AppendLine();
        testo.text = m_Info.ToString();
    }

    public void setStartingPosition() {
        StartCoroutine(TrackPos(Time.time, "center"));
    }

    // Update is called once per frame
    void Update()
    {
    }

    bool first = true;

    public int recording_seconds = 20; 

    IEnumerator TrackPos(float startingTime,string type)
    {
        if (first) {
            startingPosition = ars.transform.position;
            startingRotation = ars.transform.rotation;
            first = false;
        }

        testo.text = "Inizio Tracking. Attendi...";

        string path = Application.persistentDataPath + "/accuracyTest.csv";
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        int cont = 0;
        while (Time.time - startingTime < recording_seconds)
        {
            testo_timer.text = Math.Round(recording_seconds - (Time.time-startingTime),1).ToString();

            StringBuilder m_Info = new StringBuilder();
            m_Info.Append(Time.time + ",");
            //m_Info.Append(type + ",");

            cont++;
            m_Info.Append(cont + ",");


            float x_or = ars.transform.position.x - startingPosition.x;
            float z_or = ars.transform.position.z - startingPosition.z;

            float x = x_or * Mathf.Cos(startingRotation.eulerAngles.y * Mathf.Deg2Rad) - z_or * Mathf.Sin(startingRotation.eulerAngles.y * Mathf.Deg2Rad);
            float z = x_or * Mathf.Sin(startingRotation.eulerAngles.y * Mathf.Deg2Rad) + z_or * Mathf.Cos(startingRotation.eulerAngles.y * Mathf.Deg2Rad);

            //XYZ Ruotati e Messi a Origine.... Y non è routato in quanto rimane sullo stesso piano
            m_Info.Append(x.ToString() + ",");
            m_Info.Append(ars.transform.position.y - startingPosition.y + ",");
            m_Info.Append(z.ToString() + ",");

            //XYZ RAW
            m_Info.Append(ars.transform.position.x + ",");
            m_Info.Append(ars.transform.position.y + ",");
            m_Info.Append(ars.transform.position.z + ",");


            //YRotation
            m_Info.Append(ars.transform.rotation.eulerAngles.y + ",");

            //XYZHead Ruotati.... Y non è routato in quanto rimane sullo stesso piano

            float xhead_or = head.transform.position.x - startingPosition.x;
            float zhead_or = head.transform.position.z - startingPosition.z;

            float xhead = xhead_or * Mathf.Cos(startingRotation.eulerAngles.y * Mathf.Deg2Rad) - zhead_or * Mathf.Sin(startingRotation.eulerAngles.y * Mathf.Deg2Rad);
            float zhead = xhead_or * Mathf.Sin(startingRotation.eulerAngles.y * Mathf.Deg2Rad) + zhead_or * Mathf.Cos(startingRotation.eulerAngles.y * Mathf.Deg2Rad);

            m_Info.Append(xhead.ToString() + ",");
            m_Info.Append(ars.transform.position.y - startingPosition.y +",");
            m_Info.Append(zhead.ToString() + ",");


            //XYZHEAD RAW
            m_Info.Append(head.transform.position.x + ",");
            m_Info.Append(head.transform.position.y + ",");
            m_Info.Append(head.transform.position.z);
            
            writer.WriteLine(m_Info);
            yield return null;
        }
        writer.Close();

        testo.text = "Fine, muoviti!";


    }

}
