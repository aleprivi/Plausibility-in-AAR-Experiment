using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainExperiment : ProcDefinition
{
    public override void startProcedure()
    {
        Debug.Log("Procedure started");
    }

    public override void endProcedure()
    {
        Debug.Log("Procedure ended");
    }

        // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        
    }
}

