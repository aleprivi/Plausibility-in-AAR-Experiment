using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

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

    //public HeadTracking headTrack;

    public string saveFileName;
    public int page;
    //public int condition;

    public float x;
    public float y;

    public ExperimentalCondition(string[] parameters, string saveFileName, int page, int condition){
        //elimino sempre i primi due valori che sono pagina e condition (già gestiti)
        this.saveFileName = saveFileName;
        this.page = page;
        //this.condition = condition;
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

            //if(testSource != null) Debug.Log("TestSource found!");
        }
    }

    public void setCondition(){
//int EarTrack
//int HeadTrack,

        //4.EarTrack -- ear, ipadhead, ipad
        //5.HeadTrack -- head, head+height, ipad

//        Debug.Log("HRTF= " + parameters[0]);
        if(testSource == null) return;

        //0.HRTF -- personal, kemar, none
        int HRTF = 0;
        if(parameters[0] == "personal"){
            HRTF =1;
        } else if(parameters[0] == "kemar"){
            HRTF = 2;
        } else if(parameters[0] == "none"){
            HRTF = 0;
        }

        //1.Reverb -- calibrated, large, none
        int reverbType = 0;
        if(parameters[1] == "calibrated"){
            reverbType = 1;
        } else if(parameters[1] == "large"){
            reverbType = 2;
        } else if(parameters[1] == "none"){
            reverbType = 0;
        }

        //3.HeadPhoneEQ - equalized, none
        bool calibVolume = true;
        if(parameters[2] == "+5"){
            calibVolume = false;
        }else if(parameters[2] == "calibrated"){
            calibVolume = true;
        }


        //8.RealFake -- real, fake
        bool isVirtual = false;
        if(parameters[8] == "real"){
            isVirtual = false;
        }else{
            isVirtual = true;
        }

        //7.SampleType -- voice, noise, ecological
        //Sample Type
        Debug.Log("SampleType= " + parameters[7]);
        SourceTestProc.SourceType srcTp= SourceTestProc.SourceType.Ecological;
        if(parameters[7] == "voice"){
            srcTp = SourceTestProc.SourceType.Voice;
        }else if(parameters[7] == "noise"){
            srcTp = SourceTestProc.SourceType.Noise;
        }else if(parameters[7] == "ecological"){
            srcTp = SourceTestProc.SourceType.Ecological;
        }else if(parameters[7] == "casa"){
            srcTp = SourceTestProc.SourceType.Casa;
        }else if(parameters[7] == "cosa"){
            srcTp = SourceTestProc.SourceType.Cosa;
        }else if(parameters[7] == "corda"){
            srcTp = SourceTestProc.SourceType.Corda;
        }else if(parameters[7] == "legname"){
            srcTp = SourceTestProc.SourceType.Legname;
        }

        //6.SamplePos -- 0, -45
        SourceTestProc.SourceHeight srcPos = SourceTestProc.SourceHeight.Feet;
        if(parameters[6] == "0"){
            srcPos = SourceTestProc.SourceHeight.Sight;
        } else if(parameters[6] == "-45"){
            srcPos = SourceTestProc.SourceHeight.Feet;
        }

        //3.HeadPhoneEQ - equalized, none
        bool headPhEQ = false;
        if(parameters[3] == "equalized"){
            headPhEQ = true;
        } else if(parameters[3] == "none"){
            headPhEQ = false;
        }

        //4.EarTrack -- ear, ipadhead, ipad
        HeadTracking.EarTrackingType earTrk = HeadTracking.EarTrackingType.Airpods;
        if(parameters[4] == "ear"){
            earTrk = HeadTracking.EarTrackingType.Airpods;
        } else if(parameters[4] == "ipadhead"){
            earTrk = HeadTracking.EarTrackingType.iPadHead;
        } else if(parameters[4] == "ipad"){
            earTrk = HeadTracking.EarTrackingType.iPad;
        }

        //5.HeadTrack -- head, head+height, ipad
        HeadTracking.HeadTrackingType headTrk = HeadTracking.HeadTrackingType.HeadAR;
        if(parameters[5] == "head"){
            headTrk = HeadTracking.HeadTrackingType.HeadAR;
        } else if(parameters[5] == "head+height"){
            headTrk = HeadTracking.HeadTrackingType.iPadAndHeightAR;
        } else if(parameters[5] == "ipad"){
            headTrk = HeadTracking.HeadTrackingType.iPadAR;
        }

        testProc.setCondition(HRTF,reverbType,headPhEQ,calibVolume,earTrk,headTrk,srcPos,srcTp,isVirtual);
            
    }

    public void saveLog(float x, float y, string SAQIcond){
        string sq = (SAQIcond == "")? "" : SAQI[SAQIcond].ToString();
        string path = Application.persistentDataPath + "/" + saveFileName + "_" + SceneManager.GetActiveScene().name + ".csv";
        string text = page + "," + 0 + "," + name + "," + x.ToString() + "," + 
        y.ToString() + "," + points.ToString() + "," + SAQIcond + "," + sq + "\n";
        System.IO.File.AppendAllText(path, text);
    }







}


