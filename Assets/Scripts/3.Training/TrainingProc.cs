using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrainingProc: ProcDefinition{

    GUIManager guiManager;
    public override void startProcedure(){
        Debug.Log("Training started");
        guiManager = procedureFlowChart.gameObject.GetComponent<GUIManager>();
        //Vector3 tmp_pos = new Vector3(0, 0, Random.Range(0f, 1.5f));
        //front.transform.localPosition = tmp_pos;
        //tmp_pos = new Vector3(0, 0, Random.Range(0f, -1.5f));
        //back.transform.localPosition = tmp_pos;
        activeObject.SetActive(true);
        activeObject.GetComponent<AudioSource>().Play();
    }
    GameObject activeObject;
    public override void endProcedure()
    {
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
}