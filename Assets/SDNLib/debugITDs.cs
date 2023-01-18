using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Linq;

public class debugITDs : MonoBehaviour{
    public SDNEnvConfig envConfig;
    public HRTFmanager hrtfManager;
    public void Update(){
        //unity keydown event
        
        /*if(Input.GetKeyDown(KeyCode.Q)){
            float[] azEl_direct = hrtfManager.getAzElInteraural(this.gameObject.transform.position);
            Debug.Log("azEl_direct: " + azEl_direct[0] + ", " + azEl_direct[1]);
            int[] ind = envConfig.getIndices(azEl_direct[0], azEl_direct[1]);
            Debug.Log("ind: " + ind[0] + ", " + ind[1]);
            int[] itds = envConfig.getInterpolated_ITDs(azEl_direct);
            Debug.Log("itds -- " + itds[0] + ", " + itds[1]);
            //
                //get the current ITD
            //    envConfig.debugITDs();
        }*/
    }
}