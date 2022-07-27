//QUESTO DOVREBBE ESSERE ALGORITMO Q-LEARNING

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LinearAgent : Agent {
    public float[][] q_table;   // The matrix containing the values estimates.
    float learning_rate = 0.2f; // Sostituisce il valore precedente con un rate di 0.2
    int action = -1;
    float gamma = 0.15f; // quanto del "future state" vado a considerare? se basso, agente è "myopic"
    float e = 0.7f; // Se p< epsilon fai azione random
    //float eMin = 0.1f; // Epsilon non diminuisce nel tempo
    //int annealingSteps = 100; // Number of steps to lower e to eMin.
    public int lastState;


    //!!!!!!!!!Sostituire con una matrice con dei valori vuoti?
    //CREA LO SPAZIO PARAMETRI (in base al numerod di stati [3 o 4] e azioni) E LO INZIALIZZA A 0
    public override void SendParameters (EnvironmentParameters env)
	{
        q_table = new float[env.state_size][];
		action = 0;
		for (int i = 0; i < env.state_size; i++) {
			q_table [i] = new float[env.action_size];
			for (int j = 0; j < env.action_size; j++) {
				q_table [i] [j] = 0.0f;
			}
		}
	}

    /// Decide l'azione da fare, basandosi sulla tabella precedente
	public override float[] GetAction() {
        printQTable();
//        Debug.Log("Step di Q-Learning: ");

        //Prendo Index dell'azione con ricompensa massima
        action = q_table[lastState].ToList().IndexOf(q_table[lastState].Max());
        if (Random.Range(0f, 1f) < e) {
            //In base al numero di azioni ne prende una casuale
            action = Random.Range(0, q_table[lastState].Length - 1);
        }
        //if (e > eMin) { e = e - ((1f - eMin) / (float)annealingSteps); } //Questo non serve in quanto non previsto da Slater
        //GameObject.Find("ETxt").GetComponent<Text>().text = "Epsilon: " + e.ToString("F2");
        float currentQ = q_table[lastState][action];
        GameObject.Find("QTxt").GetComponent<Text>().text = "Current Q-value: " + currentQ.ToString("F2");
		return new float[1] {action};
	}

    void printQTable() {
        WriteLogs.WriteQTable("Qtable", q_table);


    }

    ///??????????
    ///??????????
    ///??????????
    /// <summary>
    /// Gets the values stored within the Q table.
    /// </summary>
    /// <returns>The average Q-values per state.</returns>
	public override float[] GetValue() {
        float[] value_table = new float[q_table.Length];
        for (int i = 0; i < q_table.Length; i++)
        {
            value_table[i] = q_table[i].Average();
        }
		return value_table;
	}

    /// <summary>
    /// Updates the value estimate matrix given a new experience (state, action, reward).
    /// </summary>
    /// <param name="state">The environment state the experience happened in.</param>
    /// <param name="reward">The reward recieved by the agent from the environment for it's action.</param>
    /// <param name="done">Whether the episode has ended</param>
    public override void SendState(int state, float reward, bool done)
    {
        //Debug.Log(state.ToArray().ToString() + "-" + reward + "-" + done);

        int nextState = Mathf.FloorToInt(state);
        if (action != -1) {
		    if (done == true)
		    {
		        q_table[lastState][action] += learning_rate * (reward - q_table[lastState][action]);
		    } 
		    else
		    {
		        q_table[lastState][action] += learning_rate * (reward + gamma * q_table[nextState].Max() - q_table[lastState][action]);
		    }
        }
        lastState = nextState;
	}
}
