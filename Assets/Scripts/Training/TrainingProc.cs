public class TrainingProg: ProcDefinition{

    public ProcedureFlowChart procedureFlowChart;
    public override void startProcedure(){
        Debug.Log("Training started");
        //StartCoroutine(Procedure());
        Procedure();
    }
    public override void endProcedure(){
        procedureFlowChart.nextStep();
        Debug.Log("End Training");
    }
}