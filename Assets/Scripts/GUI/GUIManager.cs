using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class GUIManager : MonoBehaviour
{
    public bool debugMode = false;

    public GameObject instructionPanel;
    public Text instructionText;
    Button instructionButton;

    TextMeshProUGUI headDist, headHeight, headRot, headLost;
    Text coordsText;
    public void showMessage(string message, float duration)
    {
        if(!debugMode){return;}
        instructionPanel.SetActive(true);
        instructionText.text = message;

// -1 = hide button
// -2 = hide button and don't use timer

        if(duration == -1){
            instructionButton.gameObject.SetActive(true);
        }else if(duration == -2){
            instructionButton.gameObject.SetActive(false);
        } else {
            instructionButton.gameObject.SetActive(false);
            StartCoroutine(clearText(duration));
        }
    }

    IEnumerator clearText(float textFadeTime) {
        Debug.Log("Ciao");
        yield return new WaitForSeconds(textFadeTime);
        instructionText.text = "";
        instructionPanel.SetActive(false);
    }
    

    public void showCoords(bool isHeadLost){
        if(debugMode && !isHeadLost){
            float headDistance = Vector3.Distance(head.transform.position, iPad.transform.position);
            headDist.text = "Distance: " + Math.Round(headDistance*100, 2) +"cm";
            headHeight.text = "Height: " + Math.Round((head.transform.position.y-iPad.transform.position.y)*100, 2)+"cm";
            headRot.text = "Rotation: " + Math.Round(head.transform.rotation.eulerAngles.x, 2)+"°";
            headLost.text = "";
        }
        if(isHeadLost){
            headDist.text = "";
            headHeight.text = "";
            headRot.text = "";
            headLost.text = "HEAD LOST!";
        }
    }

    public void showAlgoStats(float distance, float lastReward, float state) {
        Debug.Log("Distance: " + distance + " Last Reward: " + lastReward + " State: " + state);
    }


    //HideElementsLogic
    bool visible = false;
    public GameObject[] meshesToHide;
    public GameObject[] GUIElementsToHide;
    public Button toggleButton;

    public void Awake(){
        if(debugMode){
            toggleButton.gameObject.SetActive(true);
            instructionText = instructionPanel.GetComponentInChildren<Text>();
            instructionButton = instructionPanel.GetComponentInChildren<Button>();
            headDist = GameObject.Find("headDist").GetComponent<TextMeshProUGUI>();
            headHeight = GameObject.Find("headHeight").GetComponent<TextMeshProUGUI>();
            headRot = GameObject.Find("headRot").GetComponent<TextMeshProUGUI>();
            headLost = GameObject.Find("headLost").GetComponent<TextMeshProUGUI>();
        } else {
            toggleButton.gameObject.SetActive(false);
        }
        visible = true;
        toggleMeshesandGUI();
    }

    public void toggleMeshesandGUI(){
        visible = !visible;

        foreach (GameObject el in meshesToHide) {
            el.GetComponent<MeshRenderer>().enabled = visible;
        }

        foreach (GameObject el in GUIElementsToHide) {
            el.SetActive(visible);
        }

    }



    public GameObject head;
    public GameObject iPad;
        // Update is called once per frame
}
