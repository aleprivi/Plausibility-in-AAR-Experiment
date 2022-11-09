using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class FakeCalibrator : Calibrator
{
    public override void startCalibration(){
    
        Debug.Log("Calibration started");
        Debug.Log("Calibration ended");
        GameObject.FindObjectOfType<ProcedureFlowChart>().nextStep();
        
    }

}
