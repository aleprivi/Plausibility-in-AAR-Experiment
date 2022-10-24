using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProcDefinition : MonoBehaviour
{
    public abstract void startProcedure();
    public abstract void endProcedure();
    public string name;
}
