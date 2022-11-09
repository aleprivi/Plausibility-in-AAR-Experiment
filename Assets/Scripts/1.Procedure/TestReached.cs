using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestReached : MonoBehaviour
{
    public TestProc testProc;
    public GameObject testObject;

    void Update()
    {
        if (Vector3.Distance(this.transform.position, testObject.transform.position) < 0.3)
        {
            testProc.SetTarget();
        }
    }
}
