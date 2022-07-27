using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using System.Text;
using System;

public class Calibration : MonoBehaviour
{
    public GameObject ExperimentLogic;
    public GameObject Agente;
    public GameObject Goal;
    public GameObject ARCamera;
    public GameObject Room;
    public Text etx;
    public LinearEnvironment l_env;
    public TrainingEnvironment tr_env;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StringBuilder m_Info = new StringBuilder();
        m_Info.Clear();
        m_Info.Append("Rotation: ");
        m_Info.Append(Math.Round(ARCamera.transform.rotation.eulerAngles.x, 2));
        m_Info.Append("-");
        m_Info.Append(Math.Round(ARCamera.transform.rotation.eulerAngles.y, 2));
        m_Info.Append("-");
        m_Info.Append(Math.Round(ARCamera.transform.rotation.eulerAngles.z, 2));
        m_Info.AppendLine();
        m_Info.Append("Position: ");
        m_Info.Append(Math.Round(ARCamera.transform.position.x, 2));
        m_Info.Append("-");
        m_Info.Append(Math.Round(ARCamera.transform.position.y, 2));
        m_Info.Append("-");
        m_Info.Append(Math.Round(ARCamera.transform.position.z, 2));
        etx.text = m_Info.ToString();
    }


    int step = 0;

    public void Calibrate() {

        step++;

        Debug.Log("Reposition!");
        Vector3 calibratedPosition = new Vector3(ARCamera.transform.position.x, ARCamera.transform.position.y, ARCamera.transform.position.z);
        Vector3 eulerAngle = ARCamera.transform.rotation.eulerAngles;


        //ExperimentLogic.transform.position= calibratedPosition;
        //Tolgo alla Room l'altezza dello USER
        Room.transform.position = new Vector3(calibratedPosition.x, ARCamera.transform.position.y - 1.85f,calibratedPosition.z);
        Room.transform.rotation = Quaternion.Euler(0, eulerAngle.y + 180, 0);
        


        ExperimentLogic.transform.position = calibratedPosition;
        ExperimentLogic.transform.rotation = Quaternion.Euler(0,eulerAngle.y+180,0);

        GameObject.FindObjectOfType<rotateHead>().CalibrateHeadphones();

        //Goal.transform.localPosition = new Vector3(0, 0, 5);



        //Room.transform.rotation = Quaternion.Euler(0, eulerAngle.y + 180, 0);

        if(step == 1)
            tr_env.StartTraining();
        else {
            l_env.Restart();
        }
    }
}
