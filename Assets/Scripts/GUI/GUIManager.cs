using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class GUIManager : MonoBehaviour
{
    public bool debugMode = false;
    public bool RLDebugMode = false;
    public GameObject instructionPanel;
    Text instructionText;
    Button instructionButton;

    //Pannello UserInfo
    TextMeshProUGUI headDist, headHeight, headRot, headLost;

    //Pannello IA Info
    TextMeshProUGUI userTarget, userAgent, lastReward, lastState, lastAction;

    //Tabella
    TextMeshProUGUI[,] scoreTable;
    Text coordsText;

    public void showMessage(string message, float duration) {
        //if(!debugMode){return;}
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
        yield return new WaitForSeconds(textFadeTime);
        instructionText.text = "";
        instructionPanel.SetActive(false);
    }
    

    public void showCoords(bool isHeadLost){
        if(debugMode && !isHeadLost){
            float headDistance = Vector3.Distance(head.transform.position, iPad.transform.position);
            headDist.text = "Distance: " + Math.Round(headDistance*100, 1) +"cm";
            headHeight.text = "Height: " + Math.Round((head.transform.position.y-iPad.transform.position.y)*100, 1)+"cm";
            headRot.text = "Rotation: " + Math.Round(head.transform.rotation.eulerAngles.y, 0)+"°";
            headLost.text = "";
        }
        if(isHeadLost){
            headDist.text = "";
            headHeight.text = "";
            headRot.text = "";
            headLost.text = "HEAD LOST!";
        }
    }

    public void showAlgoStats(float lastRew, int state, int lastAct) {
        if(debugMode){
            lastReward.text = "Reward: " + Math.Round(lastRew, 4);
            switch(state){
                case 0:
                    lastState.text = "State: Not Engaged";
                    break;
                case 1:
                    lastState.text = "State: Public";
                    break;
                case 2:
                    lastState.text = "State: Social";
                    break;
                case 3:
                    lastState.text = "State: Intimate";
                    break;
            }
            switch(lastAct){
                case 0:
                    lastAction.text = "Action: " + "FORW1";
                    break;
                case 1:
                    lastAction.text = "Action: " + "FORW2";
                    break;
                case 2:
                    lastAction.text = "Action: " + "BACK1";
                    break;
                case 3:
                    lastAction.text = "Action: " + "BACK2";
                    break;
                case 4:
                    lastAction.text = "Action: " + "STAY";
                    break;
                case 5:
                    lastAction.text = "Action: " + "COMEH";
                    break;
                default:
                    userTarget.text = "Target: " + "None";
                    break;
            }
        }
    }

    public void showScoreTable(float[][] table){
        if(debugMode){
            for(int i = 0; i < table.Length; i++){
                for(int j = 0; j < table[i].Length; j++){
                    if(table[i][j] != 0){
                        scoreTable[i,j].text = Math.Round(table[i][j], 3).ToString();
                    }else{
                        scoreTable[i,j].text = "-";
                    }
                }
            }
        }
    }

    public void showElementPositions(float UserTarget, float UserAgent) {
        if(debugMode){
            userTarget.text = "User Target: " + Math.Round(UserTarget, 2) + "m";
            userAgent.text = "User Agent: " + Math.Round(UserAgent, 2) +"m";
        }
    }

    //HideElementsLogic
    bool visible = false;
    public GameObject[] meshesToHide;
    public GameObject[] GUIElementsToHide;
    public Button toggleButton;
    public Button debugButton;

    public void Awake(){
        instructionText = instructionPanel.GetComponentInChildren<Text>();
        instructionButton = instructionPanel.GetComponentInChildren<Button>();
        if(debugMode){
            toggleButton.gameObject.SetActive(true);
            headDist = GameObject.Find("headDist").GetComponent<TextMeshProUGUI>();
            headHeight = GameObject.Find("headHeight").GetComponent<TextMeshProUGUI>();
            headRot = GameObject.Find("headRot").GetComponent<TextMeshProUGUI>();
            headLost = GameObject.Find("headLost").GetComponent<TextMeshProUGUI>();

            userTarget = GameObject.Find("usrTargetDst").GetComponent<TextMeshProUGUI>();
            userAgent = GameObject.Find("usrAgentDst").GetComponent<TextMeshProUGUI>();
            lastReward = GameObject.Find("lastReward").GetComponent<TextMeshProUGUI>();
            lastState = GameObject.Find("lastState").GetComponent<TextMeshProUGUI>();
            lastAction = GameObject.Find("lastAction").GetComponent<TextMeshProUGUI>();

            scoreTable = new TextMeshProUGUI[4,6];
            for(int i = 0; i < 4; i++){
                for(int j = 0; j < 6; j++){
                    string name = "C" + i + "" + j;
                    scoreTable[i,j] = GameObject.Find(name).GetComponent<TextMeshProUGUI>();
                }
            }

        } else {
            toggleButton.gameObject.SetActive(false);
        }

        debugButton.gameObject.SetActive(false);
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
