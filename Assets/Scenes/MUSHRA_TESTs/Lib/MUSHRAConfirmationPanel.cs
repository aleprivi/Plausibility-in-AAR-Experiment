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

    public void ConfirmAndPrint(){

        //Save to file
        string  tmp_saveFileName = saveFileName + "_" + System.DateTime.Now.ToString("yy_MM_dd__HH_mm_ss");
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
        SceneManager.LoadScene("EndScene");

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

    public void setSaveFileName(string saveFileName){
        this.saveFileName = saveFileName;
        //get all files names
        /*DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/");
        FileInfo[] info = dir.GetFiles(saveFileName + "*.csv");
        Debug.Log("Found " + info.Length + " old files of the same user: using Data");
        //get newest file in FileInfo array
        
        FileInfo f = dir.GetFiles(saveFileName + "*.csv").OrderByDescending(p => p.LastWriteTime).First();
        Debug.Log("Using file " + f.Name);
        
        string fileN = Application.persistentDataPath + "/" + f.Name;

        string[] lines = System.IO.File.ReadAllLines(fileN);

        MUSHRASet tmpSet = GameObject.FindObjectOfType<MUSHRASet>();
        Debug.Log(tmpSet);
        for(int i = 1; i < lines.Length; i++){
            string[] items = lines[i].Split(',');
            for(int x = 0; x < tmpSet.conditions.GetLength(0); x++){
                for(int y = 0; y < tmpSet.conditions.GetLength(1); y++){
                    if(tmpSet.conditions[x,y].name == items[0]){
                        tmpSet.conditions[x,y].points = float.Parse(items[1]);
                        break;
                    }
                }
            }
            Debug.Log("Found " + items[0] + " with " + items[1] + " points");
        }*/
        

        /*string path = Application.persistentDataPath + "/" + saveFileName + ".csv";
        if(System.IO.File.Exists(path)){
            Debug.Log("File already exists!");
        }*/
    }

    public MUSHRAConfig.MUSHRAType mushraType;
}


