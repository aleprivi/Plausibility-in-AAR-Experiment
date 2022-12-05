using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrainingProc: ProcDefinition{

    GUIManager guiManager;

    private IEnumerator coroutine;
    public override void startProcedure(){
        Debug.Log("Training started");
        guiManager = procedureFlowChart.gameObject.GetComponent<GUIManager>();
        //Vector3 tmp_pos = new Vector3(0, 0, Random.Range(0f, 1.5f));
        //front.transform.localPosition = tmp_pos;
        //tmp_pos = new Vector3(0, 0, Random.Range(0f, -1.5f));
        //back.transform.localPosition = tmp_pos;
        activeObject.SetActive(true);
        activeObject.GetComponent<AudioSource>().Play();
        coroutine = SaveData();
        StartCoroutine(coroutine);
    }
    GameObject activeObject;
    public override void endProcedure()
    {
        StopCoroutine(coroutine);
        procedureFlowChart.nextStep();
        Debug.Log("Training ended");
    }

    public GameObject front, back;
    int first = 0;
    void Start()
    {
        first = Random.Range(1, 3);
        activeObject = (first == 1) ? front : back;
        front.SetActive(false);
        back.SetActive(false);
    }

    bool firstTargetReached = false;

    public void SetTarget(){
        front.SetActive(false);
        back.SetActive(false);
        if (!firstTargetReached)
        {
            activeObject = (first == 1) ? back : front;
            activeObject.SetActive(true);
            firstTargetReached = true;
            activeObject.GetComponent<AudioSource>().Play();
            guiManager.showMessage("BRAVO! Prova di nuovo!", 4);
        }
        else
        {
            endProcedure();
        }
    }


    public GameObject head;
    public GameObject iPad;
    public float LogWritePerSecond = 10;
    //string val = "User,Step,Time,HeadX,HeadY,HeadZ,iPadX,iPadY,iPadZ,targetreached";
    private IEnumerator SaveData()
    {
        while (true)
        {
            float wt = 1.0f/LogWritePerSecond;
            yield return new WaitForSeconds(wt);
            Vector3 h = head.transform.position;
            Vector3 i = iPad.transform.position;
            WriteLogs.WriteTrainingLog(Time.time,h.x,h.y,h.z,i.x,i.y,i.z,firstTargetReached?1:0);
            //print("WaitAndPrint " + Time.time);
        }
    }
}