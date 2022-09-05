using System.Text;
using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using HearXR;

    //[RequireComponent(typeof(ARSession))]
    [RequireComponent(typeof(ARFaceManager))]

public class HeadDistance : MonoBehaviour
{
    public GameObject FaceInfoText;

    public bool UseAirBudsPro = true;

    [SerializeField]
    [Tooltip("An object whose rotation will be set according to the tracked face. It MUST contain AudioListener")]
    Transform DigitalTwinHead;

    ARSession m_Session;

    ARFaceManager m_FaceManager;

    public ARCameraManager m_CameraManager;

    StringBuilder m_Info = new StringBuilder();

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
        m_Info.Clear();
        if (m_FaceManager.trackables.count == 0) {
            if (ipoc != null) ipoc.check_BeHeaded(true);
            m_Info.Append("Head LOST!");
        }

        foreach (ARFace face in m_FaceManager.trackables)
            {

            if (face.trackingState == TrackingState.Tracking)
            {
                if (ipoc != null) ipoc.check_BeHeaded(false);

                //COMPONGO STRINGA
                var camera = m_CameraManager.GetComponent<Camera>();

                //m_Info.Append("Camera: ");
                //m_Info.Append(Math.Round(camera.transform.position.x, 2));
                //m_Info.Append(" ");
                //m_Info.Append(Math.Round(camera.transform.position.y, 2));
                //m_Info.Append(" ");
                //m_Info.Append(Math.Round(camera.transform.position.z, 2));
                //m_Info.AppendLine();
                //m_Info.Append("Head: ");
                //m_Info.Append(Math.Round(face.transform.position.x, 2));
                //m_Info.Append(" ");
                //m_Info.Append(Math.Round(face.transform.position.y, 2));
                //m_Info.Append(" ");
                //m_Info.Append(Math.Round(face.transform.position.z, 2));
                //m_Info.AppendLine();
                m_Info.Append("Dist: ");
                m_Info.Append(Math.Round(Vector3.Distance(face.transform.position, camera.transform.position), 2) * 100 + "cm");
                m_Info.Append(" Height: ");
                m_Info.Append(Math.Round(face.transform.position.y, 2));

                /*
                 * QUESTO COMANDO POSIZIONA CORRETTAMENTE L'OSSERVATORE
                 */
                DigitalTwinHead.transform.position = face.transform.position;



            }
            else {
                if (ipoc != null) ipoc.check_BeHeaded(true);
                m_Info.Append("Head LOST! " + face.trackingState);
            }
        }

        if (FaceInfoText) FaceInfoText.GetComponent<Text>().text = m_Info.ToString();
//        Debug.Log(m_Info);

    }

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
