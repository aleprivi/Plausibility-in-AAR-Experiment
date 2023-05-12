using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SourceTestProc : ProcDefinition {

    public void stopMusic() {
        sourceAudio.Stop();
    }



    GUIManager guiManager;
    public override void startProcedure() {
        Debug.Log("Test started");
        guiManager = procedureFlowChart.gameObject.GetComponent<GUIManager>();
        sourceObject.SetActive(true);
        //sourceAudio.Play();
    }
    public GameObject sourceObject;
    AudioSource sourceAudio;
    SDN sourceSDN;
    public override void endProcedure()
    {
        procedureFlowChart.nextStep();
        Debug.Log("Test ended");
    }

    void Start()
    {
        sourceObject.SetActive(false);
        sourceSDN = sourceObject.GetComponent<SDN>();
        sourceAudio = sourceObject.GetComponent<AudioSource>();
    
        //load data from volumes.csv from resources
        TextAsset csv = Resources.Load("volumes") as TextAsset;
        string[] lines = csv.text.Split('\n');
        string[] voice = lines[0].Split(',');
        voiceVolumes = new float[voice.Length-1];
        string[] voiceup = lines[1].Split(',');
        voiceVolumesUp = new float[voiceup.Length-1];
        string[] noise = lines[4].Split(',');
        noiseVolumes = new float[noise.Length-1];
        string[] noiseup = lines[5].Split(',');
        noiseVolumesUp = new float[noiseup.Length-1];
        string[] feet = lines[6].Split(',');
        feetVolumes = new float[feet.Length-1];
        string[] feetup = lines[6].Split(',');
        feetVolumesUp = new float[feetup.Length-1];
        for(int i=1; i < 13; i++){
            voiceVolumes[i-1] = float.Parse(voice[i])*10;
            voiceVolumesUp[i-1] = float.Parse(voiceup[i])*10;
            noiseVolumes[i-1] = float.Parse(noise[i])*10;
            noiseVolumesUp[i-1] = float.Parse(noiseup[i])*10;
            feetVolumes[i-1] = float.Parse(feet[i])*10;
            feetVolumesUp[i-1] = float.Parse(feetup[i])*10;
        }
    }

    public AudioClip voice, voiceAirpods, noise,
        noiseAirpods, ecological, ecologicalAirpods;


    public float[] voiceVolumes;
    public float[] noiseVolumes;
    public float[] feetVolumes;
    public float[] voiceVolumesUp;
    public float[] noiseVolumesUp;
    public float[] feetVolumesUp;


    public enum SourceType { Voice = 1, Noise = 2, Ecological = 3, Casa=10, Cosa=11, Corda=12, Legname=13 };
    SourceType sourceType = SourceType.Voice;
    public void playType(int type) {
        sourceType = (SourceType)type;
        switch (sourceType)
        {
            case SourceType.Voice:
                sourceAudio.clip = (headphoneCalibration) ? voiceAirpods : voice;
                Debug.Log("Voice");
                break;
            case SourceType.Noise:
                sourceAudio.clip = (headphoneCalibration) ? noiseAirpods: noise;
                break;
            case SourceType.Ecological:
                sourceAudio.clip = (headphoneCalibration) ? ecologicalAirpods : ecological;
                break;
        }
        setVolume();
        if(isVirtual){
//            Debug.Log("entrato");
            sourceObject.GetComponent<AudioSource>().Play();
        }
        //sourceObject.GetComponent<AudioSource>().Play();
    }

    bool realvolume = true;
    public void switchVolume() {
        realvolume = !realvolume;
        setVolume();
    }

    public void switchVolume(bool real) {
        realvolume = real;
        setVolume();
    }

    void setVolume() {

            /*if(parameters[1] == "calibrated"){
                testProc.switchRoom(true);
                testProc.switchReverb(true);
            } else if(parameters[1] == "large"){
                testProc.switchRoom(false);
                testProc.switchReverb(true);
            } else if(parameters[1] == "none"){
                testProc.switchReverb(false);
            }*/
        

        float vol = 1;
//        Debug.Log("Headphone calibration: " + headphoneCalibration);
        int index = (headphoneCalibration) ? 0 : 6;

        if(useReflections == false){
            index += 4;
        }else{
            index += (corridor) ? 0 : 2;
        }

        index += (useHRTF) ? 0 : 1;
        
        //Debug.Log("Index selezionato: " + index);
        float v = 0.0f;
        switch (sourceType)
        {
            case SourceType.Voice:
                if(realvolume){
                    v = voiceVolumes[index];
                }else{
                    v = voiceVolumesUp[index];
                }
                if(!realvolume){
                    v*=2;
                }
                sourceSDN.volumeGain = v;
                Debug.Log("Volume: " + index +  "-" + voiceVolumes[index] + realvolume);
                break;
            case SourceType.Noise:
                if(realvolume){
                    v = noiseVolumes[index];
                }else{
                    v = noiseVolumesUp[index];
                }
                if(!realvolume){
                    v*=2;
                }
                sourceSDN.volumeGain = v;
                Debug.Log("Volume: " + index +  "-" + noiseVolumes[index] + realvolume);
                break;
            case SourceType.Ecological:
                if(realvolume){
                    v = feetVolumes[index];
                }else{
                    v = feetVolumesUp[index];
                }
                if(!realvolume){
                    v*=2;
                }
                sourceSDN.volumeGain = v;
                Debug.Log("Volume: " + index +  "-" + feetVolumes[index] + realvolume);
                break;
        }
        
    }

    bool headphoneCalibration = true;
    public void calibrateHeadphone()
    {
        headphoneCalibration = !headphoneCalibration;
        playType((int)sourceType);
        setVolume();
    }

    public void calibrateHeadphone(bool value)
    {
        headphoneCalibration = value;
        playType((int)sourceType);
        setVolume();
    }


    public enum SourcePosition { Real = 0, Fake = 1};
    SourcePosition sourcePosition = SourcePosition.Real;

    bool isVirtual = true;
    public void playReality(){
        isVirtual = !isVirtual;
        if(isVirtual){
            sourceAudio.Play();
        }else{
            sourceAudio.Stop();
        }
        setVolume();
    }

    public void playReality(bool value){
        isVirtual = value;
        if(isVirtual){
            sourceAudio.Play();
        }else{
            sourceAudio.Stop();
        }
        setVolume();
    }


    public enum SourceHeight { Sight = 0, Feet = 1};
    SourceHeight sourceHeight = SourceHeight.Sight;
    public void playHeight(int height){
        float y = 0;
        sourceHeight = (SourceHeight)height;
        switch (sourceHeight)
        {
            case SourceHeight.Feet:
                y = 0.3f;
                break;
            case SourceHeight.Sight:
                y = 1.6f;
                break;
        }
        sourceObject.transform.position = new Vector3(sourceObject.transform.position.x, y, sourceObject.transform.position.z);
    
    }

    bool corridor = true;
    public GameObject corridorRoom;
    public GameObject randoRoom;
    public void switchRoom() {
        corridor = !corridor;
        corridorRoom.SetActive(corridor);
        randoRoom.SetActive(!corridor);
        setVolume();
    }

    public void switchRoom(bool value) { //Corridor True, Random False
        corridor = value;
        corridorRoom.SetActive(corridor);
        randoRoom.SetActive(!corridor);
        setVolume();
    }

    public SDNEnvConfig envConfig;
    bool useHRTF = true;
    public void switchHRTF(int type) {
        //Debug.Log("HRTF: " + type);
        switch(type){
            case 0:
                envConfig.SwitchHRTF(envConfig.CIPIC);
                sourceSDN.applyHrtfToReflections = true;
                sourceSDN.applyHrtfToDirectSound = true;
                useHRTF = true;
                break;
            case 1:
                envConfig.SwitchHRTF("165");
                sourceSDN.applyHrtfToReflections = true;
                sourceSDN.applyHrtfToDirectSound = true;
                useHRTF = true;
                break;
            case 2:
                sourceSDN.applyHrtfToReflections = false;
                sourceSDN.applyHrtfToDirectSound = false;
                useHRTF = false;
                break;
        }
        setVolume();
    }

    bool useReflections = true;
    public void switchReverb()
    {
        useReflections = !useReflections;
        sourceSDN.doLateReflections = useReflections;
        setVolume();
    }

    public void switchReverb(bool value)
    {
        useReflections = value;
        sourceSDN.doLateReflections = useReflections;
        setVolume();
    }

    public HeadTracking headTracker;
    //public HeadTracking headDistanceScript;
    //public HeadPositionUtils headPositionUtilsScript;

    //public HeadTracking headRotator;
    //bool useHead = true;
    /*public void switchHead() {
        useHead = !useHead;
        //headDistanceScript.enabled = useHead;
        headPositionUtilsScript.enabled = !useHead;
        //headTracker.setHeadphoneRotation(useHead);
        setVolume();
    }*/

    public void setCondition(int HRTF, int reverbType, bool headphoneEQ, bool calibVolume, HeadTracking.EarTrackingType earTrack,HeadTracking.HeadTrackingType headTrack, SourceHeight SamplePos,SourceType sourceType,bool isVirtual){
/*
        string s_log = "";
        switch(HRTF){
            case 0:
            s_log = "none,";
            break;
            case 1:
            s_log = "personal,";
            break;
            case 2:
            s_log = "kemar,";
            break;
        }
        switch(reverbType){
            case 0:
            s_log += "Rev-none,";
            break;
            case 1:
            s_log += "Rev-calibrated,";
            break;
            case 2:
            s_log += "Rev-large,";
            break;
        }
        // EarTrack	 HeadTrack	 SamplePos	SourceType	 Real
        s_log += (calibVolume)? "Vol-calibrated,": "5db,";
        s_log += (headphoneEQ) ?"HeadPh-equalized,": "none,";
*/
        string s_log = "";
        s_log += "HRTF:";
        s_log += (HRTF==0)? "No - ": (HRTF+" - ");
        s_log += "reverb:" + reverbType + " - ";
        s_log += (headphoneEQ) ?" HeadPh EQ - ": "NO HeadPh EQ - ";
        s_log += (calibVolume) ?" Normal Volume - ": "+5 Volume - ";
        s_log += "Track:";
        switch(earTrack){
            case HeadTracking.EarTrackingType.Airpods:
                s_log += "airpods+";
                break;
            case HeadTracking.EarTrackingType.iPadHead:
                s_log += "ipadHead+";
                break;
            case HeadTracking.EarTrackingType.iPad:
                s_log += "ipad+";
                break;
        }
        switch(headTrack){
            case HeadTracking.HeadTrackingType.HeadAR:
                s_log += "Head";
                break;
            case HeadTracking.HeadTrackingType.iPadAndHeightAR:
                s_log += "ipadHeight";
                break;
            case HeadTracking.HeadTrackingType.iPadAR:
                s_log += "iPad";
                break;
        }
//SourceHeight SamplePos,SourceType sourceType,bool isVirtua
        s_log += " - SamplePos:";
        switch(SamplePos){
            case SourceHeight.Feet:
                s_log += "-45";
                break;
            case SourceHeight.Sight:
                s_log += "0";
                break;
        }
        s_log += " - SourceType:";
        switch(sourceType){
            case SourceType.Voice:
                s_log += "Voice";
                break;
            case SourceType.Noise:
                s_log += "Noise";
                break;
            case SourceType.Ecological:
                s_log += "Ecological";
                break;
        }
        Debug.Log(s_log);
        //s_log += " - isVirtual:" + isVirtual;

        //SWITCH HRTF
        switch(HRTF){
            case 0:
                sourceSDN.applyHrtfToReflections = false;
                sourceSDN.applyHrtfToDirectSound = false;
                break;
            case 1:
                envConfig.SwitchHRTF(envConfig.CIPIC);
                sourceSDN.applyHrtfToReflections = true;
                sourceSDN.applyHrtfToDirectSound = true;
                break;
            case 2:
                envConfig.SwitchHRTF("165");
                sourceSDN.applyHrtfToReflections = true;
                sourceSDN.applyHrtfToDirectSound = true;
                break;
        }

        //SWITCH REVERB Type
        switch(reverbType){
            case 0:
                sourceSDN.doLateReflections = false;
                break;
            case 1:
                sourceSDN.doLateReflections = true;
                corridorRoom.SetActive(true);
                randoRoom.SetActive(false);
                break;
            case 2:
                sourceSDN.doLateReflections = true;
                corridorRoom.SetActive(false);
                randoRoom.SetActive(true);
                break;
        }


        //Type + Headphone Calibration
        switch (sourceType)
        {
            case SourceType.Voice:
                sourceAudio.clip = (headphoneEQ) ? voiceAirpods : voice;
                Debug.Log("Voice");
                break;
            case SourceType.Noise:
                sourceAudio.clip = (headphoneEQ) ? noiseAirpods: noise;
                break;
            case SourceType.Ecological:
                sourceAudio.clip = (headphoneEQ) ? ecologicalAirpods : ecological;
                break;
        }
        if(isVirtual){
            sourceAudio.Play();
        }else{
            sourceAudio.Stop();
        }

        //Volume
        int index = (headphoneEQ) ? 0 : 6;
        switch(reverbType){
            case 0:
                index += 4;
            break;
            case 1:
                index += 0;
            break;
            case 2:
                index += 2;
            break;
        }
        index += (HRTF > 0) ? 0 : 1;
        Debug.Log("Volume= " + index);
        //Debug.Log("Index selezionato: " + index);
        float v = 0.0f;
        switch (sourceType)
        {
            case SourceType.Voice:
                if(calibVolume){
                    v = voiceVolumes[index];
                }else{
                    v = voiceVolumesUp[index];
                }
                sourceSDN.volumeGain = v;
                s_log += " - Index " + index + " - Volume: " + v;
//              Debug.Log("Volume: " + index +  "-" + voiceVolumes[index] + realvolume);
                break;
            case SourceType.Noise:
                if(calibVolume){
                    v = noiseVolumes[index];
                }else{
                    v = noiseVolumesUp[index];
                }
                sourceSDN.volumeGain = v;
                s_log += " - Index " + index + " - Volume: " + v;
//              Debug.Log("Volume: " + index +  "-" + noiseVolumes[index] + realvolume);
                break;
            case SourceType.Ecological:
                if(calibVolume){
                    v = feetVolumes[index];
                }else{
                    v = feetVolumesUp[index];
                }
                sourceSDN.volumeGain = v;
                s_log += " - Index " + index + " - Volume: " + v;
//              Debug.Log("Volume: " + index +  "-" + feetVolumes[index] + realvolume);
                break;
        }

        if(isVirtual){
            sourceAudio.Play();
        }else{
            sourceAudio.Stop();
        }

//        Debug.Log(s_log);
        //setVolume();




        //Get startReal
        StartReal startReal = GameObject.FindObjectOfType<StartReal>();

        //8.RealFake -- real, fake
        //7.SampleType -- voice, noise, ecological
        //bool isVirtual = false;
        if(isVirtual){
                sourceAudio.Play();
                startReal.SendParam(0);
        }else{
            sourceAudio.Stop();
//            Debug.Log("Source Type: " + (int)sourceType);
            startReal.SendParam((int)sourceType);
            /*if(sourceType == SourceType.Voice){
                startReal.SendParam(1);
            }else if(sourceType == SourceType.Noise){
                startReal.SendParam(2);
            }else if(sourceType == SourceType.Ecological){
                startReal.SendParam(3);
            }*/
            return;
        }
        
        float y = 0;
        switch (SamplePos)
        {
            case SourceHeight.Feet:
                y = 0.3f;
                break;
            case SourceHeight.Sight:
                y = 1.7f;
                break;
        }
        sourceObject.transform.position = new Vector3(sourceObject.transform.position.x, y, sourceObject.transform.position.z);

        HeadTracking headTracker = GameObject.FindObjectOfType<HeadTracking>();

        //4.EarTrack -- ear, ipadhead, ipad
        headTracker.earTrackingType = earTrack;
        //5.HeadTrack -- head, head+height, ipad
        headTracker.headTrackingType = headTrack;
    }
}