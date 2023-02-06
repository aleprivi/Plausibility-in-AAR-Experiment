using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentalCondition
{
    public string name;
    //public string value;
    public float points;

    public string[] parameters;

    //A dictionary of parameters
    public Dictionary<string, float> SAQI = new Dictionary<string, float>();


    public ExperimentalCondition(string[] parameters){
        //elimino sempre i primi due valori che sono pagina e condition (già gestiti)
        this.name = parameters[2];

        //save a subset of parameters
        this.parameters = new string[parameters.Length - 3];
        for(int i = 4; i < parameters.Length; i++){
            this.parameters[i-3] = parameters[i];
        }
        this.points = 0;
    }
    /*public ExperimentalCondition(string name, string value)
    {
        this.name = name;
        this.value = value;
        this.points = 0;
    }*/

}


