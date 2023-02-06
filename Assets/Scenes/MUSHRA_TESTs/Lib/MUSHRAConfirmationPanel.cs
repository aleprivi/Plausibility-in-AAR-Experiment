using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class MUSHRAConfirmationPanel : MonoBehaviour
{

    //create a multidimension array of experimental conditions
    public ExperimentalCondition[,] conditions;
    public string saveFileName;
    public bool saveSAQI = false;

    public string[] SAQIconditions;

    public void ConfirmAndPrint(){

        //Save to file
        string path = Application.persistentDataPath + "/" + saveFileName + ".csv";
        string text = "Condition Name,";
        if(!saveSAQI){ //SAVE MUSHRA
            text += "Points\n";
            for(int i = 0; i < conditions.GetLength(0); i++){
                for(int j = 0; j < conditions.GetLength(1); j++){
                    if(conditions[i, j] != null){
                        text += conditions[i, j].name + "," + conditions[i, j].points +"\n";
                    }
                }
            }
        }else{ //SAVE SAQUI

            foreach(string s in SAQIconditions){
                text += s + ",";
            }
            text += "\n";
            for(int j = 0; j < conditions.GetLength(1); j++){
                if(conditions[0, j] != null){
                    text += conditions[0, j].name + ",";
                    foreach(string s in SAQIconditions){
                        text += conditions[0, j].SAQI[s] + ",";
                    }
                    /*foreach(KeyValuePair<string, float> entry in conditions[i, j].SAQI){
                        text += entry.Key + ":" + entry.Value + ",";
                    }*/
                    text += "\n";
                }
            }
        }
        
        System.IO.File.WriteAllText(path, text);
        Debug.Log("File correctly saved to " + path);

    }

    public void HideConfirmationPanel(){
        gameObject.SetActive(false);
    }

    public void ShowConfirmationPanel(string message, bool showConfirm, ExperimentalCondition[,] conditions, string saveFileName, bool saveSAQI, string[] SAQIconditions){
        this.conditions = conditions;
        this.saveFileName = saveFileName;
        this.saveSAQI = saveSAQI;
        this.SAQIconditions = SAQIconditions;
        gameObject.SetActive(true);
        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = message;
        gameObject.transform.Find("Confirm").gameObject.SetActive(showConfirm);
    }

    public MUSHRAConfig.MUSHRAType mushraType;
}


