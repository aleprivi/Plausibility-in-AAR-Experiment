using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Reached : MonoBehaviour
{
    public TrainingEnvironment tr_env;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //string xxx = Math.Round(this.transform.position.x, 2) + "::";
        //xxx += Math.Round(this.transform.position.y, 2) + "::";
        //xxx += Math.Round(this.transform.position.z, 2);
        //Debug.Log(xxx);

        if (Vector3.Distance(this.transform.position, tr_env.user.transform.position) < 0.5)
        {
            tr_env.SetTarget();
        }
    }
}
