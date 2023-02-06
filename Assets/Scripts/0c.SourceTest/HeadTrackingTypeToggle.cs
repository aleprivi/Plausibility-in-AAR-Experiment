using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadTrackingTypeToggle : MonoBehaviour
{
    public HeadTracking.HeadTrackingType type = HeadTracking.HeadTrackingType.HeadAR;
    public HeadTracking headTracking;

    //on valuechanged event
    public void OnValueChanged(bool isOn)
    {
        if(isOn)
        {
            headTracking.headTrackingType = type;
//            Debug.Log("Setting Tracking Type: " + type);
        }
    }
}
