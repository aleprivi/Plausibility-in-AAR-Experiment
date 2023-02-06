using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HRTFToggle : MonoBehaviour
{
    public SourceTestProc stp;
    public int HRTFVal = 1;

    //on valuechanged event
    public void OnValueChanged(bool isOn)
    {
//        Debug.Log("Toggle is " + (isOn ? "On" : "Off"));
        if(isOn)
        {
            stp.switchHRTF(HRTFVal);
        }
    }
}
