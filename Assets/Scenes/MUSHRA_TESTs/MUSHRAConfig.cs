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

    public string[] SAQIParams;

    public void Awake(){
        //Set the correct environment
        MUSHRASet mushraSet = null;
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

        if(mushraSet != null){
            mushraSet.conditionsFilename = conditionFilename;
            mushraSet.saveFileName = saveFileName;
            mushraSet.confirmationMessage = confirmationMessage;
            mushraSet.errorMessage = errorMessage;
            mushraSet.instructionMessage = instructionMessage;
            mushraSet.mushraType = mushraType;
            mushraSet.SAQIparams = SAQIParams;
        }
    }
}
