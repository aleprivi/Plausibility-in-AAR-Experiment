using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class UIElimination : MonoBehaviour{
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
            case SourceTestProc.SourceType.Casa:
                samT = "casa";
                break;
            case SourceTestProc.SourceType.Cosa:
                samT = "cosa";
                break;
            case SourceTestProc.SourceType.Legname:
                samT = "legname";
                break;
            case SourceTestProc.SourceType.Corda:
                samT = "corda";
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
            if(child.name == "Remover"){
                child.GetComponent<Slider>().onValueChanged.AddListener(Remover);
            }
        }


    }


    public void PlayCondition(){
        if(isReference){
            refCondition.setCondition();
            return;
        }
//        Debug.Log("Play " + gameObject.name);
        mushraSet.conditions[page, item].setCondition();
    }

    public void Remover(float value){
        if(value >= 100){
            RemoveAndWriteCondition();
        }
    }

    public void ResetSlider(){
        for(int i = 0; i < gameObject.transform.childCount; i++){
            Transform child = gameObject.transform.GetChild(i);
            if(child.name == "Remover"){
                child.GetComponent<Slider>().value=0;
            }
        }
    }

    public void RemoveCondition(){

        //grey out every button in children
        for(int i = 0; i < gameObject.transform.childCount; i++){
            Transform child = gameObject.transform.GetChild(i);
            if(child.name == "Remover"){
                child.GetComponent<Slider>().interactable = false;
            }
            if(child.name == "Play"){
                child.GetComponent<Button>().interactable = false;
            }
        }

    }

    public void RemoveAndWriteCondition(){
    
//        Debug.Log("Remove " + gameObject.name);
        int max = 0;
        for(int i = 0; i < mushraSet.conditions.GetLength(1); i++){
            if(mushraSet.conditions[page, i]!=null && mushraSet.conditions[page, i].points > max){
                max = (int)mushraSet.conditions[page, i].points;
            }
        }
//        Debug.Log("Max: " + max);
        mushraSet.conditions[page, item].points = max + 1;
        
        RemoveCondition();
        mushraSet.conditions[page, item].saveLog(0,0,"");
        
        bool allRated = true;
        for(int i = 0; i < mushraSet.conditions.GetLength(0); i++){
            for(int j = 0; j < mushraSet.conditions.GetLength(1); j++){
                if(mushraSet.conditions[i, j] != null && mushraSet.conditions[i, j].points == 0){
                    allRated = false;
                }
            }
        }


        if(allRated && mushraSet.currentPage >= mushraSet.conditions.GetLength(0)){
                
            string saveFileName = mushraSet.saveFileName;
            string  tmp_saveFileName = saveFileName + "_" + System.DateTime.Now.ToString("yy_MM_dd__HH_mm_ss");
            //string  tmp_saveFileName = saveFileName;


            string path = Application.persistentDataPath + "/" + tmp_saveFileName + ".csv";
            string text = "Condition Name,";
            text += "Points\n";
            for(int i = 0; i < mushraSet.conditions.GetLength(0); i++){
                for(int j = 0; j < mushraSet.conditions.GetLength(1); j++){
                    if(mushraSet.conditions[i, j] != null){
                        text += mushraSet.conditions[i, j].name + "," + mushraSet.conditions[i, j].points +"\n";
                    }
                }
            }

            System.IO.File.WriteAllText(path, text);
            
            //get object of type MUSHRAConfig
            MUSHRAConfig mushraConfig = GameObject.FindObjectOfType<MUSHRAConfig>();
            WriteLogs.WriteStage();
            SceneManager.LoadScene(mushraConfig.nextSceneName);
        }
    }

    public void PrintScore(){
        Debug.Log(val.text);
    }
}
