using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class earTrackingTypeToggle : MonoBehaviour
{
    public HeadTracking.EarTrackingType type = HeadTracking.EarTrackingType.Airpods;
    public HeadTracking headTracking;

    //on valuechanged event
    public void OnValueChanged(bool isOn)
    {
        if(isOn)
        {
            headTracking.earTrackingType = type;
//            Debug.Log("Setting Tracking Type: " + type);
        }
    }
}
