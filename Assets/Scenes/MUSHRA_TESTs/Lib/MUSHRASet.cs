using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;

public class MUSHRASet : MonoBehaviour
{

    //create a multidimension array of experimental conditions
    public ExperimentalCondition[,] conditions;
    public string conditionsFilename;
    public string instructionMessage;
    public TextMeshProUGUI instructionPanel;

    public SourceTestProc.SourceType sampleType;
    public MUSHRAConfig.SamplePosition samplePosition;
    public void Start(){
        //disable confirmationpanel
        GameObject.Find("ConfirmPanel").SetActive(false);

        

        createConditions();
        /*if(b_nextPage(true)){
//            Debug.Log("First Page Created");
//            Debug.Log("Conditions loaded: " + conditions.GetLength(0) + " pages, " + conditions.GetLength(1) + " conditions");
        }else{
            Debug.Log("Conditions seems to be empty!");
        }*/

        bool allConditionsRated = true;
        while(allConditionsRated && currentPage < conditions.GetLength(0)){
            b_nextPage(true);
            
            for(int i = 0; i < conditions.GetLength(1) && allConditionsRated; i++){
                if(conditions[currentPage-1, i]!=null && conditions[currentPage-1, i].points == 0){
                    allConditionsRated = false;
                }
            }
//            Debug.Log("Page Created");
        }
        instructionPanel.text = instructionMessage;




        if(mushraType != MUSHRAConfig.MUSHRAType.SAQI){ return;}        
        allConditionsRated = true;


        while(allConditionsRated  && currentSAQI < SAQIparams.Length){

            for(int i = 0; i < conditions.GetLength(1) && allConditionsRated; i++){
                b_NextSAQI();
                
                if(conditions[0, i]!=null){
                    if(conditions[0, i].SAQI[SAQIparams[currentSAQI-1]] == 0){
                        allConditionsRated = false;
                    }
                }
            }
        }

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
            this.conditions[p-1, c-1] = new ExperimentalCondition(tmp,saveFileName, p, c);
//            Debug.Log(this.conditions[p-1, c-1]);
            foreach(string s in SAQIparams){
                //Debug.Log(currentPage +  "--" + s);
                //check if a key exist in dictionary
                if(!this.conditions[p-1, c-1].SAQI.ContainsKey(s)){
                    this.conditions[p-1, c-1].SAQI[s] = 0;
                }
            }
        }

        //get a random number between 0 and 9
        //shuffle the conditions
        for(int i = 0; i < this.conditions.GetLength(0); i++){
            for(int j = 0; j < this.conditions.GetLength(1); j++){
                if(this.conditions[i,j] != null){
                    int random = Random.Range(0, this.conditions.GetLength(1));
                    ExperimentalCondition tmp = this.conditions[i, j];
                    this.conditions[i, j] = this.conditions[i, random];
                    this.conditions[i, random] = tmp;
                }
            }
        }


        
        


        confirmationPanel.GetComponentInChildren<MUSHRAConfirmationPanel>().setSaveFileName(saveFileName, this);

        //print all available conditions
        /*for(int i = 0; i < this.conditions.GetLength(0); i++){
            for(int j = 0; j < this.conditions.GetLength(1); j++){
                if(this.conditions[i,j] != null){
                    Debug.Log("Page " + i + " Condition " + j + ": " +this.conditions[i,j].ToString());
                }
            }
        }*/
    }


    public int currentPage = 0;
    public Transform oldButtonsPanel;
    public GameObject buttonPrefab;
    public GameObject confirmationPanel;
    public TextMeshProUGUI stepCounter;
    public string saveFileName;
    public bool b_nextPage(bool preload){ //Creo i bottoni con le condizioni corrette



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
                

                GameObject tmp;

                //Già nel log lo sposto
                if(preload && mushraType == MUSHRAConfig.MUSHRAType.DragAndDrop && (conditions[currentPage, i].x != 0 || conditions[currentPage, i].y != 0)){
                    tmp = Instantiate(buttonPrefab, oldButtonsPanel);
                    tmp.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                    tmp.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                    tmp.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 60);
                    tmp.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 60);
                    tmp.transform.position = new Vector3(conditions[currentPage, i].x, conditions[currentPage, i].y+1, 0);
//                    Debug.Log("Repositioning");
                }else{
                    tmp = Instantiate(buttonPrefab, gameObject.transform);
                }

                tmp.name = conditions[currentPage, i].name;
                switch(mushraType){
                    case MUSHRAConfig.MUSHRAType.Classic:
                        UIClassic tmpEL = tmp.AddComponent<UIClassic>();
                        tmpEL.page = currentPage;
                        tmpEL.item = i;     
                        break;
                    case MUSHRAConfig.MUSHRAType.DragAndDrop:
                        UIDragAndDrop tmpDD = tmp.AddComponent<UIDragAndDrop>();
                        tmpDD.mushraSet = this;
                        tmpDD.page = currentPage;
                        tmpDD.item = i;
                        break;
                    case MUSHRAConfig.MUSHRAType.Elimination:
                        UIElimination tmpCL = tmp.AddComponent<UIElimination>();
                        tmpCL.mushraSet = this;
                        tmpCL.page = currentPage;
                        tmpCL.item = i;
                        if(conditions[currentPage, i].points != 0){
                            tmpCL.RemoveCondition();
                        }   
                        break;
                    case MUSHRAConfig.MUSHRAType.SAQI:
                        UISAQI tmpSAQI = tmp.AddComponent<UISAQI>();
                        tmpSAQI.mushraSet = this;
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
            if(b_nextPage(false)){
                Debug.Log("Page " + currentPage + " Created");
            }else{
                Debug.Log("Conditions seems to be empty!");
            }
        }





    }

    public TextMeshProUGUI SAQIinstruction;
    public TextMeshProUGUI SAQImin;
    public TextMeshProUGUI SAQImax;
    public string[] SAQIparams;
    public string[] SAQIDefMin;
    public string[] SAQIDefMax;
    public int currentSAQI = 0;


    private bool b_NextSAQI(){

        if(currentSAQI >= SAQIparams.Length){
            return false;
        }

        SAQIinstruction.text = SAQIparams[currentSAQI];
        SAQImin.text = SAQIDefMin[currentSAQI];
        SAQImax.text = SAQIDefMax[currentSAQI];
        //TO DO: reset sliders
        currentSAQI++;

        //Reset UISAQI
        for(int i = 0; i< gameObject.transform.childCount; i++){
            Transform child = gameObject.transform.GetChild(i);
            if(conditions[currentPage-1, i].SAQI[SAQIparams[currentSAQI-1]] != 0){
                child.GetComponent<UISAQI>().Set(conditions[currentPage-1, i].SAQI[SAQIparams[currentSAQI-1]]);
//                Debug.Log("Trovato!");
            }else{
                child.GetComponent<UISAQI>().Reset();
                //Debug.Log("Resettato!");
            }
        }

        //create an array containing number from 0 to 9 and shuffle it
//        Debug.Log("Number of objects " + gameObject.transform.childCount);
        /*int[] shuffled = Enumerable.Range(0,  gameObject.transform.childCount).ToArray();
        //shuffle the array with for cycle
        for(int i = 0; i < shuffled.Length; i++){
            int tmp = shuffled[i];
            int r = Random.Range(0, shuffled.Length);
//            Debug.Log("Random " + r);
            shuffled[i] = shuffled[r];
            shuffled[r] = tmp;
        }
        if(shuffled.Length == 2){
            int r = Random.Range(0, 2);
            Debug.Log("Random " + r);
            if(r == 1){
                shuffled[0] = 5;
                shuffled[1] = 4;
            }else{
                shuffled[0] = 4;
                shuffled[1] = 5;
            }
        }


        //print the array inline
        //Debug.Log("Shuffled array: " + string.Join(", ", shuffled.Select(x => x.ToString()).ToArray()));
        Debug.Log(shuffled[0] + "-" + shuffled[1]);
        for(int i = 0; i < shuffled.Length; i++){
            //Debug.Log("Setting " + i + " to " + shuffled[i]);
            gameObject.transform.GetChild(i).SetSiblingIndex(shuffled[i]);
        }
        for(int i = 0; i < shuffled.Length; i++){
            //Debug.Log("Setting " + i + " to " + shuffled[i]);
            gameObject.transform.GetChild(i).SetSiblingIndex(shuffled[i]-2);
        }*/

        GridLayoutGroup grid = gameObject.GetComponent<GridLayoutGroup>();

        int r = Random.Range(0, 2);
        if(Random.Range(0, 2) == 1){
            grid.startCorner = GridLayoutGroup.Corner.LowerLeft;
        }else{
            grid.startCorner = GridLayoutGroup.Corner.UpperRight;
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


