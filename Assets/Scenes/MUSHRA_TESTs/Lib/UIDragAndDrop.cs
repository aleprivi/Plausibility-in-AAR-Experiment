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
    public void Start(){
        //canvas = GetComponentInParent<Canvas>();
        val = GetComponentInChildren<TextMeshProUGUI>();
        val.text = gameObject.name;
        mushraSet = GetComponentInParent<MUSHRASet>();
        gameObject.GetComponent<Button>().onClick.AddListener(PlayCondition);
    }
    // Start is called before the first frame update
    //private Canvas canvas;
    public Vector2 currentPosition;
    public void OnDrag(PointerEventData data)
    {
        if(isReference) return;
        transform.position = Input.mousePosition;
        float x = Input.mousePosition.x;
        float points = (x-50)*100/(Screen.width-100);
        val.text = points.ToString();
        mushraSet.conditions[page, item].points = points;
        currentPosition = Input.mousePosition;
    }

    public void PlayCondition(){
        Debug.Log("Play " + gameObject.name);
    }

    /*public void Refresh(){
        Debug.Log("Refresh " + gameObject.name);
        transform.position = currentPosition;
    }*/
    
    public void PrintScore(){
        Debug.Log(val.text);
    }
}
