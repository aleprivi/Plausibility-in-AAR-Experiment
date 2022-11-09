using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProcDefinition : MonoBehaviour
{
    [HideInInspector] public ProcedureFlowChart procedureFlowChart;
    public abstract void startProcedure();
    public abstract void endProcedure();
}
