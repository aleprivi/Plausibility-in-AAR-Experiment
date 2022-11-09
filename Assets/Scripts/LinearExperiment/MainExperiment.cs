using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentParameters
{
    public int state_size { get; set; }
    public int action_size { get; set; }
    public List<string> action_descriptions { get; set; }
}

public class MainExperiment : ProcDefinition
{
    //IA Params
    public GameObject AudioAgent;
    public GameObject GoalObject;
    public float[] actions;
    public bool UseIntimateCondition;
    GameObject digitalHead;

    //Sounds to be played
    public AudioClip sampleAudio;
    public AudioClip BreatheSound;
    public AudioSource audioSteps; //Step Sound (deve essere messa dentro l'agent ma a livello pavimento)
    public AudioClip DoorOpensSound;
    public AudioClip SilenceSound;

    public GUIManager guiManager; //GUI Manager

    public int maxSteps = 30;

    //Internal Vars
    EnvironmentParameters envParameters;
    float MinUserAgentDistance = 1.2f;
    float startingTime;
    LinearAgent agent;
    float reward = 0;
    int currentStep = 0;
    float totalReward = 0;

    //util function per calcolare la Distanza 2D
    public float get2DDistance(GameObject a, GameObject b) {
        Vector3 a_y = new(a.transform.position.x, 0f, a.transform.position.z);
        Vector3 b_y = new(b.transform.position.x, 0f, b.transform.position.z);
        float dist = Vector3.Distance(a_y, b_y);
        return dist;
    }

    public override void endProcedure()
    {
        Debug.Log("Procedure ended");
        isExperimentRunning = false;
        procedureFlowChart.nextStep();
    }


    public bool isExperimentRunning; //L'esperimento is running?
    float actualTime; //Il tempo Attuale che manca alla fine
    bool playedSound; //Il suono è già partito??
    void Update()
    {
        //Se l'esperimento non è avviato non faccio nulla
        if(!isExperimentRunning) return;

        float UserGoalDist = get2DDistance(digitalHead, GoalObject);
        float AgentGoalDist = get2DDistance(AudioAgent, GoalObject);

        guiManager.showElementPositions(AgentGoalDist-UserGoalDist, UserGoalDist);

        //Se mi avvicino troppo faccio in modo che si allontani in maniera da essere alla distanza minima
        if (AgentGoalDist-UserGoalDist < MinUserAgentDistance)
        {
            //?? Modificare fermando
            //Debug.Log("NON VA BENE!!! entrato nella Intimate!");
            float distanceToStepOut = MinUserAgentDistance - (AgentGoalDist-UserGoalDist); //La distanza che deve fare l'agente per uscire dalla zona intima
            AudioAgent.transform.localPosition = Vector3.MoveTowards(AudioAgent.transform.localPosition, GoalObject.transform.localPosition, -distanceToStepOut); //Muovo l'agente lontano dallo user            
            actualTime = -1;
        }


        //Controllo se ho raggiunto il tempo scaduto (v. Paper)
        if (Time.time - startingTime > 420)
        {
            //??
            //GameObject.Find("ETxt").GetComponent<Text>().text = "Tempo Scaduto!";
            endProcedure();
            Debug.Log("Tempo Scaduto!");
            return;
        }


        //Se first step
        if(firstStep){
            if(preStepTime > 0){
                preStepTime -= Time.deltaTime;
            }else{
                AudioSource audiotmp = AudioAgent.GetComponent<AudioSource>();
                audiotmp.clip = SilenceSound;
                firstStep = false;
                audioSteps.Play();
            }
            return;
        }


        //SE NON DONE Procedo....
        if (actualTime > 0)
        {
            AudioAgent.transform.localPosition = Vector3.MoveTowards(AudioAgent.transform.localPosition, GoalObject.transform.localPosition, agentStepTime * Time.deltaTime);
            actualTime -= Time.deltaTime;
        }
        else
        {
            if (!playedSound)
            {
                //Debug.Log("Time to play the sound ;) ");
                playedSound = true;
                AudioSource audiotmp = AudioAgent.GetComponent<AudioSource>();
                audioSteps.Stop();
                audiotmp.Play();
            }
            agentWaitTime -= Time.deltaTime;
            if (agentWaitTime <= 0)
            {
                if(guiManager.debugMode && guiManager.RLDebugMode) {
                    guiManager.debugButton.gameObject.SetActive(true);
                }else{
//                    Debug.Log("Step " + currentStep + " - Tempo di sistema: " + Time.time);
                    EndStep();
                }
            }
        }

        //MOVIMENTO CON FRECCE DELLA TASTIERA
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 tmp = Vector3.MoveTowards(digitalHead.transform.position, GoalObject.transform.position, 1 * Time.deltaTime);
            digitalHead.transform.position = tmp;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 tmp = Vector3.MoveTowards(digitalHead.transform.position, GoalObject.transform.position, -1 * Time.deltaTime);
            digitalHead.transform.position = tmp;
        }

    }

    public void debugNextStep(){
        guiManager.debugButton.gameObject.SetActive(false);
        EndStep();
    }

    //Quando avvio preparo l'ambiente
    public void Start(){
        //AudioAgent.SetActive(false);
        isExperimentRunning = false;
        digitalHead = GameObject.FindGameObjectWithTag("DigitalHead");
        //Che condizione?
        if (UseIntimateCondition) MinUserAgentDistance = 0.38f;

        #if UNITY_EDITOR
            digitalHead = GameObject.Find("AR Camera");
        #endif
        
        //acceptingSteps = true;

        envParameters = new EnvironmentParameters()
        {
            state_size = UseIntimateCondition ? 4 : 3, //Se intimate condition selezionata stati=3 altrimenti 4
            action_descriptions = new List<string>() { "Forward1", "Forward2", "BackWard1", "Backward2", "Stay", "ComeHere" },
            action_size = 6
        };

        agent = new LinearAgent();
        agent.SendParameters(envParameters);

        

        //reward = 0;
        //currentStep = 0;
        
        
        //SERVE??
        //done = false;

        //Posiziono l'audio AGENT, ma è da sistemare con le rotazioni corrette
        //posizione iniziale??
        //AudioAgent.transform.localPosition = new Vector3(0.0f, 0.0f, agentDistanceFromUser);
        //GoalObject.transform.localPosition = new Vector3(0, 0.0f, -goalDistanceFromUser);

        totalReward = 0;

        preStepDistance = get2DDistance(AudioAgent, digitalHead);


        //acceptingSteps = true;
        //begun = true;

        //E QUESTO?
        //startButton.gameObject.SetActive(false);
        SendToPrivi.resetInit = true;
    }


    public override void startProcedure()
    {
        Debug.Log("Procedure started");

        AudioAgent.SetActive(true);
        GoalObject.SetActive(true);

    //CHE ROBA È?
        //acceptingSteps = true;
        startingTime = Time.time;

        //Avvio con l'azione 6 (Starting Action)
        Step(6);
        isExperimentRunning = true;
    }


    float preStepDistance;  //Distanza prima della partenza dello Step
    float preStepTime = 2f; //Il reward totale
    bool firstStep = true; //Primo step?
    float agentStepTime = 1f;
    float agentWaitTime = 0f;

    public void Step(int action)
    {
        Debug.Log("Step Done");
        //SEMAFORO?
        //acceptingSteps = false;

        currentStep += 1;

        reward = 0;



        //?? MIDDLE STEP
        reward = -0.05f;

        //0-Forward1, 1-Forward2, 2-BackWard1, 3-Backward2, 4-Stay, 5-Comehere

        //Definizione del paper: 2.4 secondi più un tempo random
        agentWaitTime = 2.4f;
        playedSound = false;

        AudioSource audiotmp = AudioAgent.GetComponent<AudioSource>();
        audiotmp.Stop();

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
                //Perchè??
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
                //Perchè??
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
                audiotmp.clip = BreatheSound;
                break;

            case 5:
                Debug.Log("Step " + currentStep + " - AZIONE ComeHere");
                metres = 0;
                agentWaitTime = 3.7f;
                agentStepTime = 0f;
                audiotmp.clip = sampleAudio;
                break;
            case 6: //STEP INIZIALE
                Debug.Log("Step " + currentStep + " - Entering The Room");
                metres = 2;
                agentStepTime = 1.5f;
                audiotmp.clip = DoorOpensSound;
                audiotmp.Play();
                break;

        }

        actualTime = agentStepTime;
        //resetto il contatore, in maniera che riesca ad avere i metri al secondo
        if (agentStepTime > 0)
            agentStepTime = metres / agentStepTime;
        //else
        //    Debug.Log(Time.time);

    }


    public void EndStep()
    {

        //DA QUI GESTISCO LA REWARD
        float UserGoalDist = get2DDistance(digitalHead, GoalObject);

        if (preStepDistance > UserGoalDist)
        {
            //V. Paper!
            reward = 1 + (preStepDistance - UserGoalDist);
        }
        else
        {
            reward = -1;
        }

        //Debug.Log("Ricompensa: " + reward);

        preStepDistance = UserGoalDist;

        totalReward += reward;
        
        //STAMPO I LOGs??
        //STEPS, z_POSAGENT; x_USER; y_USER; z_USER; REWARD; DISTANCE
        /*string ss = actions[0] + "," + currentStep + ",";
        ss += CHIagent.transform.position.z + ",";
        ss += CHIuser.transform.position.x + "," + CHIuser.transform.position.y + "," + CHIuser.transform.position.z + ",";
        ss += reward + ",";
        ss += dist;*/

        //??
        //WriteLogs.WriteExperimentLog(ss);



        //Debug.Log("Last State" + ((LinearAgent)agent).lastState + " - Current State " + collectState());
        agent.SendState(collectState(), reward, false);


            //Chiedo l'azione corretta all'agente
        actions = agent.GetAction();

        //Recupero la prima, perchè da definizione potrebbe calcolarne più di una
        int action = Mathf.FloorToInt(actions[0]);

        guiManager.showAlgoStats(reward, collectState(), action);
        guiManager.showScoreTable(agent.q_table);

        Step(action);

    }

        public int collectState()
    {
        float d = Vector3.Distance(AudioAgent.transform.position, digitalHead.transform.position);

        if (d > 7.6f)
        {
            return 0; //not Engaged
        }
        else if (d <= 7.6f && d > 3.7f)
        {
            return 1; //public
        }
        else{
            if (UseIntimateCondition && d <= 1.2f)
            {
                return 3; //Intimate, se prevista
            }
            else {
                return 2; //Social
            }
        }
    }
}

