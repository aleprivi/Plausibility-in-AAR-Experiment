using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

public class MUSHRAConfirmationPanel : MonoBehaviour
{

    //create a multidimension array of experimental conditions
    public ExperimentalCondition[,] conditions;
    private string saveFileName;
    public bool saveSAQI = false;

    public string[] SAQIconditions;

    public string nextSceneName;
    public void ConfirmAndPrint(){

        //Save to file
        string  tmp_saveFileName = "Results_" + saveFileName + "_" + System.DateTime.Now.ToString("yy_MM_dd__HH_mm_ss");
        //string  tmp_saveFileName = saveFileName;


        string path = Application.persistentDataPath + "/" + tmp_saveFileName + ".csv";
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
        WriteLogs.WriteStage();
        SceneManager.LoadScene(nextSceneName);

    }

    public void HideConfirmationPanel(){
        gameObject.SetActive(false);
    }

    public void ShowConfirmationPanel(string message, bool showConfirm, ExperimentalCondition[,] conditions, bool saveSAQI, string[] SAQIconditions){
        this.conditions = conditions;
        this.saveSAQI = saveSAQI;
        this.SAQIconditions = SAQIconditions;
        gameObject.SetActive(true);
        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = message;
        gameObject.transform.Find("Confirm").gameObject.SetActive(showConfirm);
    }

    public void setSaveFileName(string saveFileName, MUSHRASet mushraSet){
        this.saveFileName = saveFileName;

        string sceneName = SceneManager.GetActiveScene().name;

        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/");
        FileInfo[] info = dir.GetFiles(saveFileName + "_" + sceneName + "*.csv");        

        if(info.Length <= 0){
            return;
            Debug.Log("Trovati " + info.Length + " file di questo utente");
        }

        string fileN = Application.persistentDataPath + "/" + saveFileName + "_" + sceneName + ".csv";
        string[] lines = System.IO.File.ReadAllLines(fileN);

        foreach(string line in lines){
            string[] values = line.Split(',');
            int page = int.Parse(values[0]);
            //int condition = int.Parse(values[1]);
            for(int i = 0; i < mushraSet.conditions.GetLength(1); i++){
                if(mushraSet.conditions[page-1, i].name == values[2]){
                    float points = float.Parse(values[5]);
                    mushraSet.conditions[page-1, i].points = points;
                    mushraSet.conditions[page-1, i].page = page;
                    //mushraSet.conditions[page-1, i].condition = condition;
                    //mushraSet.conditions[page-1, condition-1].name = values[2];
                    //POSIZIONA X e Y
                    mushraSet.conditions[page-1, i].x = float.Parse(values[3]);
                    mushraSet.conditions[page-1, i].y = float.Parse(values[4]);
                    if(mushraSet.mushraType == MUSHRAConfig.MUSHRAType.SAQI){
                        mushraSet.conditions[page-1, i].SAQI[values[6]] = float.Parse(values[7]);
                    }
                }
            }
        }
    }

    public MUSHRAConfig.MUSHRAType mushraType;
}


