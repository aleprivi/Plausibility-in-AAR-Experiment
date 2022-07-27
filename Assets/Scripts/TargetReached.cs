using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TargetReached : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    


    // Update is called once per frame
    void Update()
    {

        LinearEnvironment linEnvironment = GameObject.FindObjectOfType<LinearEnvironment>();

        

        GameObject xx = GameObject.FindGameObjectWithTag("DigitalHead");
        //Debug.Log("Dist dal goal " + linEnvironment.get2DDistance(xx, this.gameObject));
        if (linEnvironment.get2DDistance(xx, this.gameObject) < 0.5) {
            linEnvironment.setReachedGoal();
            //SceneManager.LoadScene("CHITraining");
        }
    }
}
