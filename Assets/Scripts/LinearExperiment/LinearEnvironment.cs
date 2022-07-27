using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EnvironmentParameters
{
    public int state_size { get; set; }
    public int action_size { get; set; }
    public List<string> action_descriptions { get; set; }
    public string action_space_type { get; set; }
    public string state_space_type { get; set; }
    public int num_agents { get; set; }
}

public class LinearEnvironment : MonoBehaviour
{
    public float reward;
    public bool done;
    public int maxSteps;
    public int currentStep;
    public bool begun; //Iniziata la simulazione?
    public bool acceptingSteps;

    public Agent agent;
    public float[] actions;

    public EnvironmentParameters envParameters;

    public float startingTime = 0;

    public Button startButton;


    public bool intimate_Condition = false;
    public AudioClip sampleAudio;
    public AudioClip silence;
    public AudioSource audioSteps;

    float episodeReward;

    public GameObject CHIagent;
    public float agentDistanceFromUser = 3.7f;
    public GameObject CHIuser;
    public GameObject CHIgoal;
    public int goalDistanceFromUser = 5;

    float maxDistance = 1.2f;

    public float get2DDistance(GameObject a, GameObject b) {
        Vector3 a_y = new(a.transform.position.x, 0f, a.transform.position.z);
        Vector3 b_y = new(b.transform.position.x, 0f, b.transform.position.z);
        float dist = Vector3.Distance(a_y, b_y);
        return dist;
    }

    //Visibile dall'esterno, permette il REACHGOAL
    public Text reachedLabel;
    public void setReachedGoal() {
        acceptingSteps = false;
        done = true;
        reachedLabel.text = "Sei arrivato nella posizione corretta! L'esperimento è terminato, grazie per aver partecipato!";
    }


//1.RIAVVIO
    public void Restart() {

        reachedLabel.text = "";

        if (intimate_Condition) maxDistance = 0.38f;
        maxSteps = 30;
        
        acceptingSteps = true;

        envParameters = new EnvironmentParameters()
        {
            state_size = intimate_Condition ? 4 : 3, //Se intimate condition selezionata stati=3 altrimenti 4
            action_descriptions = new List<string>() { "Forward1", "Forward2", "BackWard1", "Backward2", "Stay", "ComeHere" },
            action_size = 6,
            action_space_type = "discrete",
            state_space_type = "discrete",
            num_agents = 1
        };
        acceptingSteps = true;
        startingTime = Time.time;

        agent = new LinearAgent();
        agent.SendParameters(envParameters);

        reward = 0;
        currentStep = 0;
        done = false;


        //DA SISTEMARE
        CHIagent.transform.localPosition = new Vector3(0.0f, 0.0f, agentDistanceFromUser);
        CHIgoal.transform.localPosition = new Vector3(0, 0.0f, -goalDistanceFromUser);

        episodeReward = 0;

        preDistance = agentDistanceFromUser;

        Step();
        acceptingSteps = true;
        begun = true;

        startButton.gameObject.SetActive(false);
        SendToPrivi.resetInit = true;
    }

    //2.STEP
    float preDistance;

    float agentStepTime = 1f;
    float agentWaitTime = 0f;


    public void Step()
    {
        acceptingSteps = false;
        currentStep += 1;

        reward = 0;
        actions = agent.GetAction();

        int action = Mathf.FloorToInt(actions[0]);



        //MIDDLE STEP
        reward = -0.05f;

        //0-Forward1, 1-Forward2, 2-BackWard1, 3-Backward2, 4-Stay, 5-Comehere

        agentWaitTime = 2.4f + Random.Range(0f, 0.4f);
        playedSound = false;

        AudioSource audiotmp = CHIagent.GetComponent<AudioSource>();

        int metres = 0;

        switch (action)
        {
            case 0:
                Debug.Log("Step " + currentStep + " - AZIONE F1");
                metres = 1;
                agentStepTime = 0.8f;
                audiotmp.clip = sampleAudio;
                if (currentStep != 1)
                {
                    audioSteps.Play();
                }
                break;

            case 1:
                Debug.Log("Step " + currentStep + " - AZIONE F2");
                metres = 2;
                agentStepTime = 1.5f;
                audiotmp.clip = sampleAudio;
                if (currentStep != 1)
                {
                    audioSteps.Play();
                }
                break;

            case 2:
                Debug.Log("Step " + currentStep + " - AZIONE B1");
                metres = -1;
                agentStepTime = 1.8f;
                audiotmp.clip = sampleAudio;
                if (currentStep != 1)
                {
                    audioSteps.Play();
                }
                break;

            case 3:
                Debug.Log("Step " + currentStep + " - AZIONE B2");
                metres = -2;
                agentStepTime = 3.2f;
                audiotmp.clip = sampleAudio;
                if (currentStep != 1)
                {
                    audioSteps.Play();
                }
                break;

            case 4:
                Debug.Log("Step " + currentStep + " - AZIONE STAY");
                metres = 0;
                agentStepTime = 0f;
                agentWaitTime = 4.2f;
                audiotmp.clip = silence;
                break;

            case 5:
                Debug.Log("Step " + currentStep + " - AZIONE ComeHere");
                metres = 0;
                agentWaitTime = 3.7f;
                agentStepTime = 0f;
                audiotmp.clip = sampleAudio;
                break;

        }

        actualTime = agentStepTime;
        //resetto il contatore, in maniera che riesca ad avere i metri al secondo
        if (agentStepTime > 0)
            agentStepTime = metres / agentStepTime;
        //else
        //    Debug.Log(Time.time);


    }




//3. VIENE ESEGUITO UPDATE FINO A QUANDO NON ARRIVO ALLA FINE DELL'AZIONE E POI ENDSTEP
    float actualTime = 0;
    bool playedSound = true;
    // Update is called once per frame
    void Update()
    {
        if (!begun || done)
        {
            return;
        }

        float UserGoalDist = get2DDistance(CHIuser, CHIgoal);
        float AgentGoalDist = get2DDistance(CHIagent, CHIgoal);

        if (AgentGoalDist-UserGoalDist < maxDistance)
        {
            Debug.Log("NON VA BENE!!! entrato nella Intimate!");
            //DA TENERE?
            CHIagent.transform.localPosition = new Vector3(CHIagent.transform.localPosition.x, 0, CHIagent.transform.localPosition.z + maxDistance);
            actualTime = -1;
        }


        //Controllo se ho raggiunto il tempo scaduto (v. Paper)
        if (Time.time - startingTime > 420)
        {
            GameObject.Find("ETxt").GetComponent<Text>().text = "Tempo Scaduto!";
            Debug.Log("Tempo Scaduto!");
            done = true;
            return;
        }


        //SE NON DONE....

        if (actualTime > 0)
        {
            CHIagent.transform.localPosition = Vector3.MoveTowards(CHIagent.transform.localPosition, CHIgoal.transform.localPosition, agentStepTime * Time.deltaTime);
            actualTime -= Time.deltaTime;
        }
        else
        {
            if (!playedSound)
            {
                Debug.Log("Time to play the sound ;) ");
                playedSound = true;
                AudioSource audiotmp = CHIagent.GetComponent<AudioSource>();
                audioSteps.Stop();
                audiotmp.Play();
            }
            agentWaitTime -= Time.deltaTime;
            if (agentWaitTime <= 0)
            {
                Debug.Log("Step " + currentStep + " - Tempo di sistema: " + Time.time);
                EndStep();
            }
        }


        //MOVIMENTO CON FRECCE DELLA TASTIERA
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 tmp = Vector3.MoveTowards(CHIuser.transform.position, CHIgoal.transform.position, 1 * Time.deltaTime);
            CHIuser.transform.position = tmp;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 tmp = Vector3.MoveTowards(CHIuser.transform.position, CHIgoal.transform.position, -1 * Time.deltaTime);
            CHIuser.transform.position = tmp;
        }

        //CHECK FINALE SE i due sono vicini o lontani
        //float agent_userdist = Vector3.Distance(CHIagent.transform.position, CHIuser.transform.position);


        
    }


//4.FINE DELLO STEP e poi riparto
//CALCOLO LA REWARD
    public void EndStep()
    {

        //DA QUI GESTISCO LA REWARD

        //Transform actor = CHIuser.transform;
        //Transform goal = CHIgoal.transform;


        float agent_userdist = get2DDistance(CHIagent, CHIuser);


        float dist = get2DDistance(CHIuser, CHIgoal);

        if (preDistance > dist)
        {
            //V. Paper!
            reward = 1 + (preDistance - dist);
        }
        else
        {
            reward = -1;
        }

        Debug.Log("Ricompensa: " + reward);

        preDistance = dist;

        episodeReward += reward;
        //GameObject.Find("RTxt").GetComponent<Text>().text = "Episode Reward: " + episodeReward.ToString("F2");


        //STAMPO I LOGs
        //STEPS, z_POSAGENT; x_USER; y_USER; z_USER; REWARD; DISTANCE
        string ss = actions[0] + "," + currentStep + ",";
        ss += CHIagent.transform.position.z + ",";
        ss += CHIuser.transform.position.x + "," + CHIuser.transform.position.y + "," + CHIuser.transform.position.z + ",";
        ss += reward + ",";
        ss += dist;

        WriteLogs.WriteExperimentLog(ss);

        Debug.Log("Last State" + ((LinearAgent)agent).lastState + " - Current State " + collectState());
        agent.SendState(collectState(), reward, done);
        acceptingSteps = true;

        if (acceptingSteps == true)
        {
            if (done == false)
            {
                Step();
            }
        }

    }




    /// TORNA LO STATO ATTUALE
    public int collectState()
    {
        float d = Vector3.Distance(CHIagent.transform.position, CHIuser.transform.position);

        if (d > 7.6f)
        {
            return 0; //not Engaged
        }
        else if (d <= 7.6f && d > 3.7f)
        {
            return 1; //public
        }
        else{
            if (intimate_Condition && d <= 1.2f)
            {
                return 3; //Intimate, se prevista
            }
            else {
                return 2; //Social
            }
        }
    }
}
