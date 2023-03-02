using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class UIClassic : MonoBehaviour
{
    private TextMeshProUGUI val;
    public int page = 0;
    public int item = 0;
    public MUSHRASet mushraSet;

    public bool isReference = false;
    private ExperimentalCondition refCondition;
    public void Start(){
          if(mushraSet == null) mushraSet = GetComponentInParent<MUSHRASet>();

        string samT = "";
        switch(mushraSet.sampleType){
            case SourceTestProc.SourceType.Noise:
                samT = "noise";
                break;
            case SourceTestProc.SourceType.Ecological:
                samT = "ecologic";
                break;
            case SourceTestProc.SourceType.Voice:
                samT = "voice";
                break;
        }

        string samP = "";
        switch(mushraSet.samplePosition){
            case MUSHRAConfig.SamplePosition.head:
                samP = "0";
                break;
            case MUSHRAConfig.SamplePosition.feet:
                samP = "-45";
                break;
        }

        if(isReference){
            string[] parameters = {"-1","-1","Ref","none","none","none","none","none","none",samP,samT,"real"};
            refCondition = new ExperimentalCondition(parameters, mushraSet.saveFileName, -1, -1);
        }
        
        for(int i = 0; i < gameObject.transform.childCount; i++){
            Transform child = gameObject.transform.GetChild(i);
            if(child.name == "Play"){
                child.GetComponent<Button>().onClick.AddListener(PlayCondition);
                //child.GetComponentInChildren<TextMeshProUGUI>().text = gameObject.name;
            }
            if(child.name == "Value"){
                child.GetComponent<Slider>().onValueChanged.AddListener(RemoveCondition);
            }
        }
    }


    public void PlayCondition(){
        if(isReference){
            refCondition.setCondition();
            return;
        }
        Debug.Log("Play " + gameObject.name);
        mushraSet.conditions[page, item].setCondition();
    }

    public void RemoveCondition(float value){
        //Debug.Log("Changed value " + gameObject.name);
        mushraSet.conditions[page, item].points = value;
/*        int max = 0;
        for(int i = 0; i < mushraSet.conditions.GetLength(1); i++){
            if(mushraSet.conditions[page, i]!=null && mushraSet.conditions[page, i].points > max){
                max = (int)mushraSet.conditions[page, i].points;
            }
        }
        Debug.Log("Max: " + max);
        mushraSet.conditions[page, item].points = max + 1;
        //grey out every button in children
        for(int i = 0; i < gameObject.transform.childCount; i++){
            Transform child = gameObject.transform.GetChild(i);
            child.GetComponent<Button>().interactable = false;
        }*/
    }
    
    public void PrintScore(){
        Debug.Log(val.text);
    }
}
