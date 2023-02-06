using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SourceTestProc : ProcDefinition {

    GUIManager guiManager;
    public override void startProcedure() {
        Debug.Log("Test started");
        guiManager = procedureFlowChart.gameObject.GetComponent<GUIManager>();
        sourceObject.SetActive(true);
        sourceAudio.Play();
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
    }

    public AudioClip voice, voiceAirpods, noise,
        noiseAirpods, ecological, ecologicalAirpods;


    public float[] voiceVolumes;
    public float[] noiseVolumes;
    public float[] feetVolumes;


    public enum SourceType { Voice = 0, Noise = 1, Ecological = 2 };
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
            Debug.Log("entrato");
            sourceObject.GetComponent<AudioSource>().Play();
        }
        //sourceObject.GetComponent<AudioSource>().Play();
    }

    bool realvolume = true;
    public void switchVolume() {
        realvolume = !realvolume;
        setVolume();
    }

    void setVolume() {
        float vol = 1;
        int index = (headphoneCalibration) ? 0 : 8;
        index += (corridor) ? 0 : 4;
        index += (useHRTF) ? 0 : 2;
        index += (useReflections) ? 0: 1;
        
        //Debug.Log("Index selezionato: " + index);

        switch (sourceType)
        {
            case SourceType.Voice:
                sourceSDN.volumeGain = voiceVolumes[index];
                //Debug.Log("Volume: " + voiceVolumes[index]);
                break;
            case SourceType.Noise:
                sourceSDN.volumeGain = noiseVolumes[index];
                //Debug.Log("Volume: " + noiseVolumes[index]);
                break;
            case SourceType.Ecological:
                sourceSDN.volumeGain = feetVolumes[index];
                //Debug.Log("Volume: " + feetVolumes[index]);
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


    public enum SourceHeight { Sight = 0, Feet = 1};
    SourceHeight sourceHeight = SourceHeight.Sight;
    public void playHeight(int height){
        float y = 0;
        sourceHeight = (SourceHeight)height;
        switch (sourceHeight)
        {
            case SourceHeight.Feet:
                y = 0.1f;
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

    public SDNEnvConfig envConfig;
    bool useHRTF = true;
    public void switchHRTF(int type) {
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

    public HeadTracking headTracker;
    //public HeadTracking headDistanceScript;
    public HeadPositionUtils headPositionUtilsScript;

    //public HeadTracking headRotator;
    //bool useHead = true;
    /*public void switchHead() {
        useHead = !useHead;
        //headDistanceScript.enabled = useHead;
        headPositionUtilsScript.enabled = !useHead;
        //headTracker.setHeadphoneRotation(useHead);
        setVolume();
    }*/
}