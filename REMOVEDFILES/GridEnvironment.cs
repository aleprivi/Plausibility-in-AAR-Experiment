using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GridEnvironment : Environment
{

    public bool intimate_Condition = false;
    public AudioClip comehere;
    public AudioClip forward;
    public AudioClip backward;
    public AudioClip silence;
    public string[] players;
    public GameObject visualAgent;
    int numObstacles;
    int numGoals;
    int gridSizeW;
    int gridSizeH;
    int[] objectPositions;
    float episodeReward;

    public GameObject CHIagent;
    public int agentPosition = 37;
    public GameObject CHIuserListener;
    public GameObject CHIuser;
    public int userPosition = 27;
    public GameObject CHIgoal;
    public int goalPosition = 0;
    public GameObject breathe;

    float maxDistance = 3.7f;

    void Start()
    {
        if (intimate_Condition) maxDistance = 1.2f;
        maxSteps = 30;
        waitTime = 0.001f;
        BeginNewGame();
    }

    /// <summary>
    /// Restarts the learning process with a new Grid.
    /// </summary>
    public void BeginNewGame()
    {
        int gridSizeSetW = 40;
        int gridSizeSetH = 1;

        numGoals = 1;
        numObstacles = 1;
        gridSizeW = gridSizeSetW;
        gridSizeH = gridSizeSetH;

        SetUp();
        agent = new InternalAgent();
        agent.SendParameters(envParameters);
        Reset();

    }

    /// <summary>
    /// Established the Grid.
    /// </summary>
    public override void SetUp()
    {
        envParameters = new EnvironmentParameters()
        {
            observation_size = 0,
            state_size = gridSizeW * gridSizeH,
            /*QUI ANDRA MODIFICATO IN BASE ALLE DIMENSIONI*/
            action_descriptions = new List<string>() { "Forward1", "Forward2", "BackWard1", "Backward2", "Stay", "ComeHere" },
            action_size = 6,
            env_name = "GridWorld",
            action_space_type = "discrete",
            state_space_type = "discrete",
            num_agents = 1
        };

        SetEnvironment();
    }

    float actualTime = 0;
    bool playedSound = false;
    // Update is called once per frame
    void Update()
    {
        CHIuserListener.transform.position = CHIuser.transform.position;

        if (actualTime > 0)
        {
            visualAgent.transform.position = Vector3.MoveTowards(visualAgent.transform.position, new Vector3(0, 0, 0), agentStepTime * Time.deltaTime);
            actualTime -= Time.deltaTime;

            if (Vector3.Distance(visualAgent.transform.position, CHIagent.transform.position) < maxDistance) {
                actualTime = -1;
            }

        }
        else {
            if (!playedSound) {
                playedSound = true;
                AudioSource audiotmp = visualAgent.GetComponent<AudioSource>();
                audiotmp.Play();
            }
            agentWaitTime -= Time.deltaTime;
            if (agentWaitTime <= 0)
            {
                Debug.Log(Time.time);
                RunMdp();
            }
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            Transform tmp = CHIuser.transform;
            tmp.position = new Vector3(tmp.transform.position.x, 0, tmp.transform.position.z+1*Time.deltaTime);
            CHIuserListener.transform.position = tmp.position;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Transform tmp = CHIuser.transform;
            tmp.position = new Vector3(tmp.transform.position.x, 0, tmp.transform.position.z - 1 * Time.deltaTime);
            CHIuserListener.transform.position = tmp.position;
        }
    }





    /// <summary>
    /// Gets the agent's current position and transforms it into a discrete integer state.
    /// </summary>
    /// <returns>The state.</returns>
    public override List<float> collectState()
    {
        List<float> state = new List<float>();
        //foreach (GameObject actor in actorObjs)
        //{
        //    if (actor.tag == "agent")
        //    {
        //        float point = (gridSizeW * actor.transform.position.x) + actor.transform.position.z;
        //        state.Add(point);
        //    }
        //}

        float point = (gridSizeW * CHIagent.transform.position.x) + CHIagent.transform.position.z;
        state.Add(point);
        return state;
    }

    /// <summary>
    /// Resizes the grid to the specified size.
    /// </summary>
    public void SetEnvironment()
    {
        GameObject.Find("Plane").transform.localScale = new Vector3(gridSizeH / 10.0f, 1f, gridSizeW / 10.0f);
        GameObject.Find("Plane").transform.position = new Vector3((gridSizeH - 1) / 2f, -0.5f, (gridSizeW - 1) / 2f);
        GameObject.Find("sN").transform.localScale = new Vector3(1, 1, gridSizeH + 2);
        GameObject.Find("sS").transform.localScale = new Vector3(1, 1, gridSizeH);
        GameObject.Find("sN").transform.position = new Vector3((gridSizeH - 1) / 2f, 0.0f, gridSizeW);
        GameObject.Find("sS").transform.position = new Vector3((gridSizeH - 1) / 2f, 0.0f, -1);
        GameObject.Find("sE").transform.localScale = new Vector3(1, 1, gridSizeW + 2);
        GameObject.Find("sW").transform.localScale = new Vector3(1, 1, gridSizeW + 2);
        GameObject.Find("sE").transform.position = new Vector3(gridSizeH, 0.0f, (gridSizeW - 1) / 2f);
        GameObject.Find("sW").transform.position = new Vector3(-1, 0.0f, (gridSizeW - 1) / 2f);

        HashSet<int> numbers = new HashSet<int>();

        objectPositions = new int[] {27, 37, 0};

    }

    /// <summary>
    /// Draws the value estimation spheres on the grid.
    /// </summary>

    /// <summary>
    /// Resets the episode by placing the objects in their original positions.
    /// </summary>
    public override void Reset()
    {
        base.Reset();

        CHIagent.transform.position = new Vector3(0.0f, 0.0f, agentPosition);
        CHIuserListener.transform.position = CHIuser.transform.position;
        CHIgoal.transform.position = new Vector3(0, 0.0f, goalPosition);
        visualAgent = CHIagent;


        episodeReward = 0;
        EndReset();
    }

    /// <summary>
    /// Allows the agent to take actions, and set rewards accordingly.
    /// </summary>
    /// <param name="action">Action.</param>
    ///
    float preDistance = 0f;


    Vector3 agentTargetVector = new Vector3(0, 0, 0);
    float agentStepTime = 1f;
    float agentWaitTime = 0f;

    public override void MiddleStep(int action)
    {
        reward = -0.05f;

        /*
        // 0 - Forward, 1 - Backward, 2 - Left, 3 - Right
        if (action == 3)
        {
            Collider[] blockTest = Physics.OverlapBox(new Vector3(visualAgent.transform.position.x + 1, 0, visualAgent.transform.position.z), new Vector3(0.3f, 0.3f, 0.3f));
            if (blockTest.Where(col => col.gameObject.tag == "wall").ToArray().Length == 0)
            {
                visualAgent.transform.position = new Vector3(visualAgent.transform.position.x + 1, 0, visualAgent.transform.position.z);
            }
        }

        if (action == 2)
        {
            Collider[] blockTest = Physics.OverlapBox(new Vector3(visualAgent.transform.position.x - 1, 0, visualAgent.transform.position.z), new Vector3(0.3f, 0.3f, 0.3f));
            if (blockTest.Where(col => col.gameObject.tag == "wall").ToArray().Length == 0)
            {
                visualAgent.transform.position = new Vector3(visualAgent.transform.position.x - 1, 0, visualAgent.transform.position.z);
            }
        }

        if (action == 0)
        {
            Collider[] blockTest = Physics.OverlapBox(new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z + 1), new Vector3(0.3f, 0.3f, 0.3f));
            if (blockTest.Where(col => col.gameObject.tag == "wall").ToArray().Length == 0)
            {
                visualAgent.transform.position = new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z + 1);
            }
        }

        if (action == 1)
        {
            Collider[] blockTest = Physics.OverlapBox(new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z - 1), new Vector3(0.3f, 0.3f, 0.3f));
            if (blockTest.Where(col => col.gameObject.tag == "wall").ToArray().Length == 0)
            {
                visualAgent.transform.position = new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z - 1);
            }
        }

        Collider[] hitObjects = Physics.OverlapBox(visualAgent.transform.position, new Vector3(0.3f, 0.3f, 0.3f));
        if (hitObjects.Where(col => col.gameObject.tag == "goal").ToArray().Length == 1)
        {
            reward = 1;
            done = true;
        }
        if (hitObjects.Where(col => col.gameObject.tag == "pit").ToArray().Length == 1)
        {
            reward = -1;
            //done = true;
        }

        */


        //0-Forward1, 1-Forward2, 2-BackWard1, 3-Backward2, 4-Stay, 5-Comehere

        agentWaitTime = 2.4f + Random.Range(0f, 0.4f) ;
        playedSound = false;

        AudioSource audiotmp = visualAgent.GetComponent<AudioSource>();

        int metres = 0;

        if (action == 0)
        {
            Debug.Log("AZIONE F1");
            //Collider[] blockTest = Physics.OverlapBox(new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z + 1), new Vector3(0.3f, 0.3f, 0.3f));
            //if (blockTest.Where(col => col.gameObject.tag == "wall").ToArray().Length == 0)
            //{
            //    visualAgent.transform.position = new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z - 1);
            //}
            metres = 1;
            agentTargetVector = new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z + 1);
            agentStepTime = 0.8f;
            audiotmp.clip = forward;
        }

        if (action == 1)
        {
            Debug.Log("AZIONE F2");
            //Collider[] blockTest = Physics.OverlapBox(new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z + 2), new Vector3(0.3f, 0.3f, 0.3f));
            //if (blockTest.Where(col => col.gameObject.tag == "wall").ToArray().Length == 0)
            //{
            //    visualAgent.transform.position = new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z - 2);
            //}
            metres = 2;
            agentTargetVector = new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z + 2);
            agentStepTime = 1.5f;
            audiotmp.clip = forward;
        }

        if (action == 2)
        {
            Debug.Log("AZIONE B1");
            //Collider[] blockTest = Physics.OverlapBox(new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z - 1), new Vector3(0.3f, 0.3f, 0.3f));
            //if (blockTest.Where(col => col.gameObject.tag == "wall").ToArray().Length == 0)
            //{
            //    visualAgent.transform.position = new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z + 1);
            //}
            metres = -1;
            agentTargetVector = new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z - 1);
            agentStepTime = 1.8f;
            audiotmp.clip = backward;
        }

        if (action == 3)
        {
            Debug.Log("AZIONE B2");
            //Collider[] blockTest = Physics.OverlapBox(new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z - 2), new Vector3(0.3f, 0.3f, 0.3f));
            //if (blockTest.Where(col => col.gameObject.tag == "wall").ToArray().Length == 0)
            //{
            //    visualAgent.transform.position = new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z + 2);
            //}
            metres = -2;
            agentTargetVector = new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z - 2);

            agentStepTime = 3.2f;
            audiotmp.clip = backward;
        }

        if (action == 4)
        {
            agentTargetVector = new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z);
            Debug.Log("AZIONE STAY");
            metres = 2;
            agentStepTime = 0f;
            agentWaitTime = 4.2f;
            audiotmp.clip = silence;
        }

        if (action == 5)
        {
            agentTargetVector = new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z);
            Debug.Log("AZIONE ComeHere");
            metres = 0;
            agentWaitTime = 3.7f;
            agentStepTime = 0f;
            audiotmp.clip = comehere;
        }
        actualTime = agentStepTime;
        //resetto il contatore, in maniera che riesca ad avere i metri al secondo
        if (agentStepTime > 0)
            agentStepTime = metres/ agentStepTime;
        else

            //audiotmp.Play();
            Debug.Log(Time.time);

        //DA QUI GESTISCO LA REWARD

        Transform actor = CHIuser.transform;
        Transform goal = CHIgoal.transform;


        float agent_userdist = Vector3.Distance(visualAgent.transform.position, actor.position);

        if (intimate_Condition)
        {
            if (agent_userdist < 1.2f) {
                Debug.Log("NON VA BENE!!! entrato nella Intimate!");
                visualAgent.transform.position = new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z + (1.3f - agent_userdist));
            }
        }
        else {
            //Sono nella Social Condition
            if (agent_userdist < 3.7f)
            {
                Debug.Log("NON VA BENE!!! entrato nella Intimate!");
                visualAgent.transform.position = new Vector3(visualAgent.transform.position.x, 0, visualAgent.transform.position.z + (3.8f - agent_userdist));
            }
        }


        float dist = Vector3.Distance(actor.position, goal.position);

        if (preDistance > dist)
        {

            //V. Paper!
            reward = 1 + (preDistance - dist);
            Debug.Log("Ricompensa: " + (1 + (preDistance - dist)));

        }
        else {
            reward = -1;
            Debug.Log("Ricompensa: -1");

        }

        preDistance = dist;

        episodeReward += reward;
        GameObject.Find("RTxt").GetComponent<Text>().text = "Episode Reward: " + episodeReward.ToString("F2");

    }
}
