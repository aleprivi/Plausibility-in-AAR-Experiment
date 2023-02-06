using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleTypeToggle : MonoBehaviour
{
    public StartReal sr;
    public int type = 1;

    //on valuechanged event
    public void OnValueChanged(bool isOn)
    {
//        Debug.Log("Toggle is " + (isOn ? "On" : "Off"));
        if(isOn)
        {
            sr.setType(type);
        }
    }
}
