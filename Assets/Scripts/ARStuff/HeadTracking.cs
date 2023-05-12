using System.Text;
using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using HearXR;

[RequireComponent(typeof(ARFaceManager))]

public class HeadTracking : MonoBehaviour
{
    [SerializeField]
    [Tooltip("An object whose rotation will be set according to the tracked face. It MUST contain AudioListener")]

/*
VARIABILI VARIE
*/
    //AR Stuff
    Transform DigitalTwinHead;  //La DigitalTwinHead
    ARFaceManager m_FaceManager;    //Il Quello che riconosce le facce, 
                                    //viene riconosciuto automaticamente e 
                                    //si trova solitamente dentro la camera
    public ARCameraManager m_CameraManager; //La camera
    public iPadOrientationControl ipoc; //L'oggetto che oscura lo schermo
    public GUIManager guiManager;   //La gestione della GUI

    //Rotazioni Testa
    public enum EarTrackingType{Airpods, iPadHead, iPad, None};
    //public bool isAirPodsAvailable = false;
    public EarTrackingType earTrackingType = EarTrackingType.Airpods;
    private Quaternion _calibratedOffset = Quaternion.identity, _lastRotation = Quaternion.identity, _fixedTrackingRot = Quaternion.identity;

    //Posizione Testa
    public int headHeight;
    public enum HeadTrackingType{HeadAR, iPadAR, iPadAndHeightAR, None};
    public HeadTrackingType headTrackingType = HeadTrackingType.HeadAR;

    
    //Avvio
    void Start(){
        HeadphoneMotion.Init();

        if (HeadphoneMotion.IsHeadphoneMotionAvailable()) 
        {
            HeadphoneMotion.OnHeadRotationQuaternion += HandleHeadRotationQuaternion;
            HeadphoneMotion.StartTracking();
            Debug.Log("Airpods correctly configured");
        }else{
            Debug.Log("Error! AirPods not available");
        }
    }

    void Awake()
    {
            m_FaceManager = GetComponent<ARFaceManager>();
    }

//SEMPLIFICARE QUESTO METODO
    void OnEnable()
        {
            Application.onBeforeRender += OnBeforeRender;
        }

    void OnDisable()
        {
            Application.onBeforeRender -= OnBeforeRender;
        }


/*
LOGICHE
*/

    //Handle rotazione Airpods
    private async void HandleHeadRotationQuaternion(Quaternion rotation){

        //Tracking Orientation
        if (earTrackingType == EarTrackingType.Airpods){
            if (_calibratedOffset == Quaternion.identity){
                DigitalTwinHead.rotation = rotation;
            }else{
                DigitalTwinHead.rotation = rotation * Quaternion.Inverse(_calibratedOffset);
            }
            //Debug.Log("using Airpods");
        }else if(earTrackingType == EarTrackingType.iPadHead){
            //Segnaposto, viene fatto dentro OnBeforeRender --> Necessito della testa
        //Tracking rotazione iPad
        }else if(earTrackingType == EarTrackingType.iPad){
            DigitalTwinHead.transform.rotation = m_CameraManager.transform.rotation;
        }
        //No Tracking, Se non ho disabilitato la rotazione salvo sempre l'ultima posizione
        //altrimenti la applico
        if(earTrackingType != EarTrackingType.None){
            _fixedTrackingRot = transform.rotation;
        }else{
            DigitalTwinHead.rotation = _fixedTrackingRot;
        }

        

        /*
        Ultima parte earTracking
        */
        //Head Orientation
        if(earTrackingType == EarTrackingType.iPadHead){
            //Nothing to be done
        //Tracking solo iPad
        }

        _lastRotation = rotation;  //...se voglio calibrare
    }


    void OnBeforeRender()
        {

        //Se non c'è la testa Visualizza un errore
        if (DigitalTwinHead == null)
        {
            Debug.Log("NO TWIN HEAD!!");
            return;
        }

        //HEAD TRACKING NULL
        if (headTrackingType == HeadTrackingType.None) return;


        if(headTrackingType == HeadTrackingType.iPadAR){
            //DigitalTwinHead.transform.position = m_CameraManager.transform.position;
        }

        //HEAD TRACKING HeadAR posiziona la sola testa
        if (m_FaceManager.trackables.count == 0) {
            if (ipoc != null) ipoc.check_BeHeaded(true);
            return;
        }


        foreach (ARFace face in m_FaceManager.trackables){

            if (face.trackingState == TrackingState.Tracking){
                if (ipoc != null) ipoc.check_BeHeaded(false);
                if (guiManager != null) guiManager.showCoords(false);

                /*
                * QUESTO COMANDO POSIZIONA CORRETTAMENTE L'OSSERVATORE
                */
                if(headTrackingType == HeadTrackingType.HeadAR){
                    DigitalTwinHead.transform.position = face.transform.position;
                }
                
                //RotazioneTestiPad
                if(earTrackingType == EarTrackingType.iPadHead){
                    DigitalTwinHead.transform.rotation = face.transform.rotation;
                }

                errorPlayed = false;
                isHeadAvailable = true;

            }else {
                if (ipoc != null) ipoc.check_BeHeaded(true);
                if (guiManager != null) guiManager.showCoords(true);
                playAudioErrorOnce();
                isHeadAvailable = false;
            }
        }

        //HEAD TRACKING iPadAR posiziona la testa e l'iPad
        if(headTrackingType == HeadTrackingType.iPadAR){
            DigitalTwinHead.transform.position = m_CameraManager.transform.position;
        }

        //HeadTrackingType.iPadAndHeightAR posiziona la testa come iPad e aggiunge l'altezza
        if(headTrackingType == HeadTrackingType.iPadAndHeightAR){
            DigitalTwinHead.transform.position = m_CameraManager.transform.position;
            DigitalTwinHead.transform.position += new Vector3(0, (0.0f+headHeight)/100.0f, 0);
        }

    }

    void Update(){}



    /*
    * Utilities
    */
    public AudioSource audioError;  //Emette un bip se non trova la testa
    bool errorPlayed = false;
    void playAudioErrorOnce() {
        if (audioError != null && !errorPlayed) audioError.Play();
        errorPlayed = true;
    }

    bool isHeadAvailable;
    public bool getHeadState() {
        return isHeadAvailable;
    }

    public void CalibrateAirpods(){
        _calibratedOffset = _lastRotation;
    }
}
