using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentalCondition
{
    public string name;
    //public string value;
    public float points;

    public string[] parameters;

    //A dictionary of parameters
    public Dictionary<string, float> SAQI = new Dictionary<string, float>();

    public SourceTestProc testProc;
    public GameObject testSource;

    public HeadTracking headTrack;

    public ExperimentalCondition(string[] parameters){
        //elimino sempre i primi due valori che sono pagina e condition (già gestiti)
        this.name = parameters[2];

        //save a subset of parameters
        this.parameters = new string[parameters.Length - 3];
        for(int i = 3; i < parameters.Length; i++){
            this.parameters[i-3] = parameters[i];
        }
        this.points = 0;

        //find gameobject containing a specific component
        
        if(GameObject.Find("PROCEDURE") != null){
            testProc = GameObject.Find("PROCEDURE").GetComponent<SourceTestProc>();
            testSource = testProc.GetComponent<SourceTestProc>().sourceObject;
            //save object of type HeadTracking in headTrack
            headTrack = GameObject.FindObjectOfType<HeadTracking>();
            //if(testSource != null) Debug.Log("TestSource found!");
        }
    }

    public void setCondition(){
//        Debug.Log("HRTF= " + parameters[0]);
        if(testSource != null){
            Debug.Log("Setting condition " + name);
            //0.HRTF -- personal, kemar, none
            if(parameters[0] == "kemar"){
                testProc.switchHRTF(0);
            } else if(parameters[0] == "personal"){
                testProc.switchHRTF(1);
            } else if(parameters[0] == "none"){
                testProc.switchHRTF(2);
            }

//            Debug.Log("Rev= " + parameters[1]);
            //1.Reverb -- calibrated, large, none
            if(parameters[1] == "calibrated"){
                testProc.switchRoom(true);
                testProc.switchReverb(true);
            } else if(parameters[1] == "large"){
                testProc.switchRoom(false);
                testProc.switchReverb(true);
            } else if(parameters[1] == "none"){
                testProc.switchReverb(false);
            }

            //2.SampleVolume -- calibrated, +5
            if(parameters[2] == "calibrated"){
                testProc.switchVolume(false);
            } else if(parameters[2] == "+5"){
                testProc.switchVolume(true);
            }

            //3.HeadPhoneEQ - equalized, none
            if(parameters[3] == "equalized"){
                testProc.switchVolume(true);
            } else if(parameters[3] == "none"){
                testProc.switchVolume(false);
            }

            //4.EarTrack -- ear, ipadhead, ipad
            if(parameters[4] == "ear"){
                headTrack.earTrackingType = HeadTracking.EarTrackingType.Airpods;
            } else if(parameters[4] == "ipadhead"){
                headTrack.earTrackingType = HeadTracking.EarTrackingType.iPadHead;
            } else if(parameters[4] == "ipad"){
                headTrack.earTrackingType = HeadTracking.EarTrackingType.iPad;
            }

            //5.HeadTrack -- head, head+height, ipad
            if(parameters[5] == "head"){
                headTrack.headTrackingType = HeadTracking.HeadTrackingType.HeadAR;
            } else if(parameters[5] == "head+height"){
                headTrack.headTrackingType = HeadTracking.HeadTrackingType.iPadAndHeightAR;
            } else if(parameters[5] == "ipad"){
                headTrack.headTrackingType = HeadTracking.HeadTrackingType.iPadAR;
            }

            //6.SamplePos -- 0, -45
            if(parameters[6] == "-45"){
                testProc.playHeight(0);
            } else if(parameters[6] == "0"){
                testProc.playHeight(1);
            }
        }
    }

}


