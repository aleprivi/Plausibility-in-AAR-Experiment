using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Reached : MonoBehaviour
{
    public TrainingProc trainingProc;
    public GameObject user;

    void Start(){
        #if UNITY_EDITOR
            user = GameObject.Find("AR Camera");
        #endif
    }
    void Update()
    {
        if (Vector3.Distance(this.transform.position, user.transform.position) < 0.5)
        {
            trainingProc.SetTarget();
        }
    }
}
