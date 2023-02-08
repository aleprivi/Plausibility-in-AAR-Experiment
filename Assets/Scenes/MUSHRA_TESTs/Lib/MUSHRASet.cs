using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class MUSHRASet : MonoBehaviour
{

    //create a multidimension array of experimental conditions
    public ExperimentalCondition[,] conditions;
    public string conditionsFilename;
    public string instructionMessage;
    public TextMeshProUGUI instructionPanel;

    public MUSHRAConfig.SampleType sampleType;
    public MUSHRAConfig.SamplePosition samplePosition;
    public void Start(){
        createConditions();
        if(b_nextPage()){
//            Debug.Log("First Page Created");
//            Debug.Log("Conditions loaded: " + conditions.GetLength(0) + " pages, " + conditions.GetLength(1) + " conditions");
        }else{
            Debug.Log("Conditions seems to be empty!");
        }
        instructionPanel.text = instructionMessage;

        if(mushraType == MUSHRAConfig.MUSHRAType.SAQI){
            b_NextSAQI();
        }

        confirmationPanel.GetComponentInChildren<MUSHRAConfirmationPanel>().setSaveFileName(saveFileName);
    }

    public void createConditions(){
        //Load file from resources
        TextAsset file = Resources.Load<TextAsset>(conditionsFilename);
        string[] tmpconds = file.text.Split('\n');
        int pages = 0;
        int conditions = 0;

        for(int i = 1; i < tmpconds.Length; i++){
            string[] tmp = tmpconds[i].Split(',');
            int p = int.Parse(tmp[0]);
            int c = int.Parse(tmp[1]);
            if(p > pages) pages = p;
            if(c > conditions) conditions = c;
        }

        this.conditions = new ExperimentalCondition[pages, conditions];
        for(int i = 1; i < tmpconds.Length; i++){
            string[] tmp = tmpconds[i].Split(',');
            int p = int.Parse(tmp[0]);
            int c = int.Parse(tmp[1]);
//            Debug.Log(tmpconds[i]);
            this.conditions[p-1, c-1] = new ExperimentalCondition(tmp);
//            Debug.Log(this.conditions[p-1, c-1]);
        }

        //print all available conditions
        /*for(int i = 0; i < this.conditions.GetLength(0); i++){
            for(int j = 0; j < this.conditions.GetLength(1); j++){
                if(this.conditions[i,j] != null){
                    Debug.Log("Page " + i + " Condition " + j + ": " +this.conditions[i,j].ToString());
                }
            }
        }*/

    }


    private int currentPage = 0;
    public Transform oldButtonsPanel;
    public GameObject buttonPrefab;
    public GameObject confirmationPanel;
    public TextMeshProUGUI stepCounter;
    public string saveFileName;
    public bool b_nextPage(){ //Creo i bottoni con le condizioni corrette



        if(currentPage >= conditions.GetLength(0)){
            ShowConfirmationPanel(confirmationMessage, true);
            return false;
        }

        if(mushraType == MUSHRAConfig.MUSHRAType.DragAndDrop){
            //find a gridlayoutgroup component and check its name ends with "Modes"
            GridLayoutGroup[] grid = GameObject.FindObjectsOfType<GridLayoutGroup>();
            foreach(GridLayoutGroup g in grid){
                if(g.name.EndsWith("Modes")){
                    g.enabled = true;
                }
            }
        }

        //get all children of gameObject and destroy them
        while(gameObject.transform.childCount > 0){
            Transform child = gameObject.transform.GetChild(0);
            child.SetParent(oldButtonsPanel);
            if(mushraType != MUSHRAConfig.MUSHRAType.DragAndDrop){
                for(int i = 0; i < child.transform.childCount; i++){
                    Transform grandchild = child.transform.GetChild(i);
                    grandchild.gameObject.SetActive(false);
                }
                
            }
            
        }
                
        List<ExperimentalCondition> pageConditions = new List<ExperimentalCondition>();
        for(int i = 0; i < conditions.GetLength(1); i++){
//            Debug.Log(currentPage + "--" + i + "--"+ conditions[currentPage, i]);
            if(conditions[currentPage, i] != null){
                pageConditions.Add(conditions[currentPage, i]);
                foreach(string s in SAQIparams){
                    //Debug.Log(currentPage +  "--" + s);
                    conditions[currentPage, i].SAQI[s] = 0;
                }

                GameObject tmp = Instantiate(buttonPrefab, gameObject.transform);
                tmp.name = conditions[currentPage, i].name;
                switch(mushraType){
                    case MUSHRAConfig.MUSHRAType.Classic:
                        UIClassic tmpEL = tmp.AddComponent<UIClassic>();
                        tmpEL.page = currentPage;
                        tmpEL.item = i;     
                        break;
                    case MUSHRAConfig.MUSHRAType.DragAndDrop:
                        UIDragAndDrop tmpDD = tmp.AddComponent<UIDragAndDrop>();
                        tmpDD.page = currentPage;
                        tmpDD.item = i;
                        break;
                    case MUSHRAConfig.MUSHRAType.Elimination:
                        UIElimination tmpCL = tmp.AddComponent<UIElimination>();
                        tmpCL.page = currentPage;
                        tmpCL.item = i;                    
                        break;
                    case MUSHRAConfig.MUSHRAType.SAQI:
                        UISAQI tmpSAQI = tmp.AddComponent<UISAQI>();
                        tmpSAQI.page = currentPage;
                        tmpSAQI.item = i;
                        break;
                };
            }
        }
        currentPage++;
        stepCounter.text = "Step " + currentPage + "/" + conditions.GetLength(0);


        return true;
    }

    public void NextPage(){
        bool allConditionsRated = true;

        //check values in conditions
        for(int i = 0; i < conditions.GetLength(1) && allConditionsRated; i++){
            if(conditions[currentPage-1, i]!=null && conditions[currentPage-1, i].points == 0){
                allConditionsRated = false;
            }
        }

        if(!allConditionsRated){
            ShowConfirmationPanel(errorMessage, false);
        }else{
            if(b_nextPage()){
                Debug.Log("Page " + currentPage + " Created");
            }else{
                Debug.Log("Conditions seems to be empty!");
            }
        }





    }

    public TextMeshProUGUI SAQIinstruction;
    public string[] SAQIparams;
    public int currentSAQI = 0;


    private bool b_NextSAQI(){

        if(currentSAQI >= SAQIparams.Length){
            return false;
        }

        SAQIinstruction.text = SAQIparams[currentSAQI];
        //TO DO: reset sliders
        currentSAQI++;

        //Reset UISAQI
        for(int i = 0; i< gameObject.transform.childCount; i++){
            Transform child = gameObject.transform.GetChild(i);
            child.GetComponent<UISAQI>().Reset();
        }

        stepCounter.text = "Step " + currentSAQI + "/" + SAQIparams.Length;
        return true;

    }

    public void NextSAQI(){
        bool allConditionsRated = true;

        for(int i = 0; i < conditions.GetLength(1) && allConditionsRated; i++){
            //get all values of a dictionary
            //Debug.Log( SAQIparams[currentSAQI] + "--" + conditions[0, i].SAQI[SAQIparams[currentSAQI]]);  
            if(conditions[0, i]!=null){
                if(conditions[0, i].SAQI[SAQIparams[currentSAQI-1]] == 0){
                    allConditionsRated = false;
                }
            }
        }

        //get all children of UISAQI and destroy them
        /*for(int i = 0; i < gameObject.transform.childCount ; i++){
            Transform child = gameObject.transform.GetChild(i);
            child.GetComponent<UISAQI>().Reset();
        }*/


        if(!allConditionsRated){
            ShowConfirmationPanel(errorMessage, false);
        }else{
                if(!b_NextSAQI()){
                    ShowConfirmationPanel(confirmationMessage, true);
                }
        }

    }
    


    public void ChangeScore(int page, int item, float score){
        conditions[page, item].points = score;
    }

    public void HideConfirmationPanel(){
        confirmationPanel.SetActive(false);
    }


    public string confirmationMessage;
    public string errorMessage;

    private void ShowConfirmationPanel(string message, bool showConfirm){
        confirmationPanel.SetActive(true);        
        bool tmpSAQI = (mushraType == MUSHRAConfig.MUSHRAType.SAQI);
//        Debug.Log("SAQI: " + tmpSAQI);
        confirmationPanel.GetComponentInChildren<MUSHRAConfirmationPanel>().ShowConfirmationPanel(message, showConfirm,conditions, tmpSAQI, SAQIparams);
    }

    public MUSHRAConfig.MUSHRAType mushraType;
}


