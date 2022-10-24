using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibrator : MonoBehaviour
{
//Modificare la calibration in base all'altezza del soggetto. Oppure settare la stanza
//tenendo il leggio a una misura fissa dal pavimento

    //public GameObject ExperimentLogic;
    public GameObject ARCamera;
    public GameObject Room;
    public GameObject[] experimentElements;
    public ProcedureFlowChart procedureFlowChart;

    public void Start(){
        name = "Calibration";
    }

    public void startCalibration(){
        Debug.Log("Calibration started");

        Debug.Log("Reposition!");
        Vector3 calibratedPosition = new Vector3(ARCamera.transform.position.x, ARCamera.transform.position.y, ARCamera.transform.position.z);
        Vector3 eulerAngle = ARCamera.transform.rotation.eulerAngles;

        Room.transform.position = new Vector3(calibratedPosition.x, ARCamera.transform.position.y - 1.85f,calibratedPosition.z);
        Room.transform.rotation = Quaternion.Euler(0, eulerAngle.y, 0);
        
        foreach(GameObject gameObject in experimentElements){
            gameObject.transform.position = new Vector3(calibratedPosition.x + gameObject.transform.position.x, 
                                        calibratedPosition.y, 
                                        calibratedPosition.z + gameObject.transform.position.z);
            gameObject.transform.RotateAround(calibratedPosition, Vector3.up, eulerAngle.y);
        }

//        ExperimentLogic.transform.position = calibratedPosition;
//        ExperimentLogic.transform.rotation = Quaternion.Euler(0,eulerAngle.y+180,0);

        GameObject.FindObjectOfType<rotateHead>().CalibrateHeadphones();


        Debug.Log("Calibration ended");
        //Riassegno il controllo al main
        procedureFlowChart.nextStep();
    }

}
