using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class UISAQI : MonoBehaviour
{
    private TextMeshProUGUI val;
    public int page = 0;
    public int item = 0;
    public MUSHRASet mushraSet;

    public bool isReference = false;
    private ExperimentalCondition refCondition;
    public void Start(){
        if(mushraSet == null) mushraSet = GetComponentInParent<MUSHRASet>();
        //mushraSet = GetComponentInParent<MUSHRASet>();

        string samT = "";
        switch(mushraSet.sampleType){
            case MUSHRAConfig.SampleType.noise:
                samT = "noise";
                break;
            case MUSHRAConfig.SampleType.ecologic:
                samT = "ecologic";
                break;
            case MUSHRAConfig.SampleType.voice:
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
            refCondition = new ExperimentalCondition(parameters);
        }
        
        for(int i = 0; i < gameObject.transform.childCount; i++){
            Transform child = gameObject.transform.GetChild(i);
            if(child.name == "Play"){
                child.GetComponent<Button>().onClick.AddListener(PlayCondition);
                child.GetComponentInChildren<TextMeshProUGUI>().text = gameObject.name;
            }
            if(child.name == "Value"){
                child.GetComponent<Slider>().onValueChanged.AddListener(UpdateCondition);
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

    public void UpdateCondition(float value){
        //Debug.Log(gameObject.name + " " + value);
        //Debug.Log("currentPage " + page);
        //Debug.Log("Current SAQI param " + (mushraSet.currentSAQI-1));
        //Debug.Log("Current Parameter " + mushraSet.SAQIparams[mushraSet.currentSAQI]);
        mushraSet.conditions[page, item].SAQI[mushraSet.SAQIparams[mushraSet.currentSAQI-1]] = value;
    }

    public void Reset(){
        //get slider in children
        for(int i = 0; i < gameObject.transform.childCount; i++){
            Transform child = gameObject.transform.GetChild(i);
            if(child.name == "Value"){
                child.GetComponent<Slider>().value = 0;
                return;
            }
        }
    }
    
    public void PrintScore(){
        Debug.Log(val.text);
    }
}
