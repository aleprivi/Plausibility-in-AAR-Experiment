using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProcedureFlowChart : MonoBehaviour
{
    public enum ProcedureStates {
        CalibrationReady,
        Calibration,
        ProcedureReady,
        Procedure,
        End
    }

    public string calibrationReadyMessage = "Punta al fondo della stanza...";
    public string calibrationMessage = "Calibrazione in corso";
    public string procedureReadyMessage = "Pronto per iniziare?";
    public string procedureMessage = "Avvicinati a...";
    public string endMessage = "Procedura terminata. Sei pronto per cominciare!";

    [HideInInspector] public ProcedureStates procedureState = ProcedureStates.CalibrationReady;

    Calibrator calibrator;
    ProcDefinition procDefinition;
    GUIManager guiManager;

    // Start is called before the first frame update
    void Start()
    {



        procedureState = ProcedureStates.CalibrationReady;
        calibrator = gameObject.GetComponent<Calibrator>();
        calibrator.procedureFlowChart = this;
        procDefinition = gameObject.GetComponent<ProcDefinition>();
        procDefinition.procedureFlowChart = this;
        guiManager = gameObject.GetComponent<GUIManager>();
        nextStep();
    }

    public void nextStep(){
        Debug.Log("Chiamato Next Step. Valore di procedureState: " + procedureState);
        //Debug.Log(procedureState);
        guiManager.showCoords(false);
        switch (procedureState)
        {
            case ProcedureStates.CalibrationReady:
                procedureState = ProcedureStates.Calibration;
                guiManager.showMessage(calibrationReadyMessage, -1);
                break;
            case ProcedureStates.Calibration:
                guiManager.showMessage(procedureReadyMessage, -2);
                procedureState = ProcedureStates.ProcedureReady;
                calibrator.startCalibration();
                break;
            case ProcedureStates.ProcedureReady:
            Debug.Log("Procedure is READY!");
                guiManager.showMessage(procedureReadyMessage, -1);
                procedureState = ProcedureStates.Procedure;
                break;
            case ProcedureStates.Procedure:
            guiManager.showMessage(procedureMessage, 2);
                procedureState = ProcedureStates.End;
                procDefinition.startProcedure();
                break;
            case ProcedureStates.End:
                guiManager.showMessage(endMessage, -2);
                break;
        }
    }
}
