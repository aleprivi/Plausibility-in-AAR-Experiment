using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TestProc: ProcDefinition{

    public override void startProcedure(){
        Debug.Log("Test Started");
        testObject.SetActive(true);
        testObject.GetComponent<AudioSource>().Play();
        isTestRunning = true;
    }
    public override void endProcedure()
    {
        procedureFlowChart.nextStep();
        Debug.Log("Training ended");
    }

    public GameObject testObject;
    //public GameObject testObject2;
    public GameObject pointA, pointB;

    public AudioClip clipA, clipB;
    void Start()
    {
        testObject.SetActive(false);
        isTestRunning = false;
    }

    bool isTestRunning;
    bool firstTarget = true;

    void Update(){
        if(!isTestRunning) return;
        //testObject2.transform.position = Vector3.MoveTowards(testObject2.transform.position, firstTarget ? pointB.transform.position : pointA.transform.position, 2f*Time.deltaTime);
        testObject.transform.position = Vector3.MoveTowards(testObject.transform.position, pointA.transform.position, 2f*Time.deltaTime);
    }

    public void SetTarget(){
        firstTarget = !firstTarget;
        testObject.GetComponent<AudioSource>().clip = firstTarget ? clipA : clipB;
        testObject.GetComponent<AudioSource>().Play();
        testObject.transform.position = pointB.transform.position;
    }
}