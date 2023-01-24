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
    //bool prec = true;
    //public float LerpFactor = 0.3f;
    public bool isEnabled = true;
    public void setHeadphoneRotation(bool value){
        isEnabled = value;
    }
    private async void HandleHeadRotationQuaternion(Quaternion rotation)
    {
        if (isEnabled)
        {
            //Lerp quaternion
            //Quaternion tmp = Quaternion.Lerp(_lastRotation, rotation, LerpFactor);

            if (_calibratedOffset == Quaternion.identity)
            {
                transform.rotation = rotation;
                //transform.rotation = tmp;
            }
            else
            {
                transform.rotation = rotation * Quaternion.Inverse(_calibratedOffset);
                //transform.rotation = tmp * Quaternion.Inverse(_calibratedOffset);
            }
        }
        //prec = !prec;

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
