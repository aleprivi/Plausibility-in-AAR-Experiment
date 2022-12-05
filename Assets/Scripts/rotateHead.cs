using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HearXR;

public class rotateHead : MonoBehaviour
{

    public bool smoothMovement = false;

    void Start()
    {
        HeadphoneMotion.Init();

        if (HeadphoneMotion.IsHeadphoneMotionAvailable())
        {
            HeadphoneMotion.OnHeadRotationQuaternion += HandleHeadRotationQuaternion;
            HeadphoneMotion.StartTracking();
        }
    }

    private Quaternion _calibratedOffset = Quaternion.identity;
    private Quaternion _lastRotation = Quaternion.identity;
    bool prec = true;
    private void HandleHeadRotationQuaternion(Quaternion rotation)
    {
        if (prec)
        {
            if (_calibratedOffset == Quaternion.identity)
            {
                transform.rotation = rotation;
            }
            else
            {
                transform.rotation = rotation * Quaternion.Inverse(_calibratedOffset);
            }
        }
        prec = !prec;

        //transform.rotation = rotation;
        _lastRotation = rotation;
    }

    public void CalibrateHeadphones()
    {
        _calibratedOffset = _lastRotation;
    }

    void Update()
    {
        
    }
}
