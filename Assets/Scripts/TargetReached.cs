using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetReached : MonoBehaviour
{
    MainExperiment slaterExperiment;
    GameObject digitalHead;
    
    bool targetReached = false;
    void Start()
    {
        slaterExperiment = GameObject.FindObjectOfType<MainExperiment>();
        digitalHead = GameObject.FindGameObjectWithTag("DigitalHead");
        
    }

    void Update()
    {
        if (slaterExperiment.get2DDistance(digitalHead, this.gameObject) < 0.5 && !targetReached) {
            Debug.Log("Goal Object Identifies that the Target has been Reached!");
            slaterExperiment.endProcedure();
            targetReached = true;
        }
    }
}
