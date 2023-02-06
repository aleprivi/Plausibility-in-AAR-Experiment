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
    public void Start(){
        mushraSet = GetComponentInParent<MUSHRASet>();
        
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
        Debug.Log("Play " + gameObject.name);
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
