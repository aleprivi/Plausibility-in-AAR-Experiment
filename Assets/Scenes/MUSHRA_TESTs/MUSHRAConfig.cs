using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MUSHRAConfig: MonoBehaviour
{
    public string conditionFilename;
    public string saveFileName;
    public string confirmationMessage;
    public string errorMessage;
    public string instructionMessage;

    public enum MUSHRAType{Classic, DragAndDrop, Elimination, SAQI};
    public MUSHRAType mushraType;

    //public enum SampleType{noise, ecologic, voice, casa, cosa, corda, legname};
    public enum SamplePosition{head, feet};

    public SourceTestProc.SourceType sampleType;
    public SamplePosition samplePosition;
    public string[] SAQIParams;
    public string[] SAQIDefMin;
    public string[] SAQIDefMax;

    public string nextSceneName;

    public void Awake(){
        //Set the correct environment
        MUSHRASet mushraSet = null;
        //Debug.Log("===========cia0===========");
        for(int i = 0; i < gameObject.transform.childCount; i++){
            Transform child = gameObject.transform.GetChild(i);
            bool destroyElement = false;
            //Check if a string starts with a specific word
            if(child.name.StartsWith("DD") && mushraType != MUSHRAType.DragAndDrop){destroyElement = true;}
            if(child.name.StartsWith("EL") && mushraType != MUSHRAType.Elimination){destroyElement = true;}
            if(child.name.StartsWith("CL") && mushraType != MUSHRAType.Classic){destroyElement = true;}
            if(child.name.StartsWith("SAQI") && mushraType != MUSHRAType.SAQI){destroyElement = true;}

            switch(mushraType){
                case MUSHRAType.Classic:
                    if(child.name == "CLModes") mushraSet = child.GetComponent<MUSHRASet>();
                    break;
                case MUSHRAType.DragAndDrop:
                    if(child.name == "DDModes") mushraSet = child.GetComponent<MUSHRASet>();
                    break;
                case MUSHRAType.Elimination:
                    if(child.name == "ELModes") mushraSet = child.GetComponent<MUSHRASet>();
                    break;
                case MUSHRAType.SAQI:
                    if(child.name == "SAQIModes") mushraSet = child.GetComponent<MUSHRASet>();
                    break;
            };
            if(destroyElement) Destroy(child.gameObject);
        }

        //Leggo il file name dal WriteLog
        saveFileName = WriteLogs.userNum;

        TextAsset saqi_info = Resources.Load("SAQIdef") as TextAsset;
        string[] saqi_lines = saqi_info.text.Split('\n');
        SAQIParams = new string[saqi_lines.Length];
        SAQIDefMin = new string[saqi_lines.Length];
        SAQIDefMax = new string[saqi_lines.Length];

        for(int i=0; i < saqi_lines.Length; i++){
            string[] saqi_line = saqi_lines[i].Split(',');
            for(int j=0; j < saqi_line.Length; j++){
                SAQIParams[i] = saqi_line[0];
                SAQIDefMin[i] = saqi_line[1];
                SAQIDefMax[i] = saqi_line[2];
            }
        }

        Debug.Log("Sample Type: " + sampleType); 

        if(mushraSet != null){
            mushraSet.conditionsFilename = conditionFilename;
            mushraSet.saveFileName = saveFileName;
            mushraSet.confirmationMessage = confirmationMessage;
            mushraSet.errorMessage = errorMessage;
            mushraSet.instructionMessage = instructionMessage;
            mushraSet.mushraType = mushraType;
            mushraSet.SAQIparams = SAQIParams;
            mushraSet.sampleType = sampleType;
            mushraSet.samplePosition = samplePosition;
            mushraSet.SAQIDefMin = SAQIDefMin;
            mushraSet.SAQIDefMax = SAQIDefMax;
        }

        //get confirmation pane
        GameObject confirmationPane = GameObject.Find("ConfirmPanel");
        confirmationPane.GetComponent<MUSHRAConfirmationPanel>().nextSceneName = nextSceneName;
    }
}
