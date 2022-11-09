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

public class HeadDistance : MonoBehaviour
{
    public bool UseAirBudsPro = true;

    [SerializeField]
    [Tooltip("An object whose rotation will be set according to the tracked face. It MUST contain AudioListener")]
    Transform DigitalTwinHead;
    ARSession m_Session;
    ARFaceManager m_FaceManager;
    public ARCameraManager m_CameraManager;
    bool m_FaceTrackingSupported;
    bool m_FaceTrackingWithWorldCameraSupported;

    void Awake()
    {
            m_FaceManager = GetComponent<ARFaceManager>();
            m_Session = GetComponent<ARSession>();
        
    }

    void OnEnable()
        {
            Application.onBeforeRender += OnBeforeRender;

            // Detect face tracking with world-facing camera support
            var subsystem = m_Session ? m_Session.subsystem : null;
            if (subsystem != null)
            {
                var configs = subsystem.GetConfigurationDescriptors(Allocator.Temp);
                if (configs.IsCreated)
                {
                    using (configs)
                    {
                        foreach (var config in configs)
                        {
                            if (config.capabilities.All(Feature.FaceTracking))
                            {
                                m_FaceTrackingSupported = true;
                            }

                            if (config.capabilities.All(Feature.WorldFacingCamera | Feature.FaceTracking))
                            {
                                m_FaceTrackingWithWorldCameraSupported = true;
                            }
                        }
                    }
                }
            }
        }

    void OnDisable()
        {
            Application.onBeforeRender -= OnBeforeRender;
        }

    public iPadOrientationControl ipoc;

    void OnBeforeRender()
        {
        if (DigitalTwinHead == null)
        {
            Debug.Log("NO TWIN HEAD!!");
            return;
        }
        //        Debug.Log("N° of heads found: " + m_FaceManager.trackables.count);
        if (m_FaceManager.trackables.count == 0) {
            if (ipoc != null) ipoc.check_BeHeaded(true);
        }

        foreach (ARFace face in m_FaceManager.trackables)
            {

            if (face.trackingState == TrackingState.Tracking)
            {
                if (ipoc != null) ipoc.check_BeHeaded(false);
                if (guiManager != null) guiManager.showCoords(false);

                //COMPONGO STRINGA
                var camera = m_CameraManager.GetComponent<Camera>();

                /*
                 * QUESTO COMANDO POSIZIONA CORRETTAMENTE L'OSSERVATORE
                 */
                DigitalTwinHead.transform.position = face.transform.position;

                errorPlayed = false;
                isHeadAvailable = true;

            }
            else {
                if (ipoc != null) ipoc.check_BeHeaded(true);
                if (guiManager != null) guiManager.showCoords(true);
                playErrorOnce();
                isHeadAvailable = false;
            }
        }

    }

    bool errorPlayed = false;
    void playErrorOnce() {
        if (audioError != null && !errorPlayed) audioError.Play();
        errorPlayed = true;
    }


    bool isHeadAvailable;
    public bool getHeadState() {
        return isHeadAvailable;
    }
    public AudioSource audioError;


    public GUIManager guiManager;
    void Update()
    {
        
        /*if (m_CameraManager.requestedFacingDirection == CameraFacingDirection.World && !m_FaceTrackingWithWorldCameraSupported)
        {
            m_Info.Append("Face tracking in world facing camera mode is not supported.\n");
        }

        if (DigitalTwinHead)
        {
            //DigitalTwinHead.gameObject.SetActive(m_CameraManager.currentFacingDirection == CameraFacingDirection.World);
        }*/

        
    }
}
