using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleHeightToggle : MonoBehaviour
{
    public SourceTestProc stp;
    public int heighttype = 1;

    //on valuechanged event
    public void OnValueChanged(bool isOn)
    {
//        Debug.Log("Toggle is " + (isOn ? "On" : "Off"));
        if(isOn)
        {
            stp.playHeight(heighttype);
        }
    }
}
