using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class UIDragAndDrop : MonoBehaviour, IDragHandler
{
    private TextMeshProUGUI val;
    public int page = 0;
    public int item = 0;
    public MUSHRASet mushraSet;

    public bool isReference = false;
    private ExperimentalCondition refCondition;
    public void Start(){
        if(mushraSet == null) mushraSet = GetComponentInParent<MUSHRASet>();
//        Debug.Log(gameObject.name);
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

        //canvas = GetComponentInParent<Canvas>();
        val = GetComponentInChildren<TextMeshProUGUI>();
        val.text = gameObject.name;
        gameObject.GetComponent<Button>().onClick.AddListener(PlayCondition);
    }
    // Start is called before the first frame update
    //private Canvas canvas;
    public Vector2 currentPosition;
    public void OnDrag(PointerEventData data)
    {
        GridLayoutGroup[] grid = GameObject.FindObjectsOfType<GridLayoutGroup>();
        //Debug.Log("----------------");
        grid = GameObject.FindObjectsOfType<GridLayoutGroup>();
        foreach(GridLayoutGroup g in grid){
            if(g.name.EndsWith("Modes") && g.enabled){
                g.enabled = false;
            }
        }

        if(isReference) return;
        transform.position = Input.mousePosition;
        float x = Input.mousePosition.x;
        float points = (x-50)*100/(Screen.width-100);
        //val.text = points.ToString();
        mushraSet.conditions[page, item].points = points;
        currentPosition = Input.mousePosition;
    }

    public void PlayCondition(){
        if(isReference){
            refCondition.setCondition();
            return;
        }
//        Debug.Log("Play " + gameObject.name);
        mushraSet.conditions[page, item].setCondition();
    }

    /*public void Refresh(){
        Debug.Log("Refresh " + gameObject.name);
        transform.position = currentPosition;
    }*/
    
    public void PrintScore(){
        Debug.Log(val.text);
    }
}
