using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class InitConfig : MonoBehaviour
{
    public TMP_InputField Usernum_input;

    // Start is called before the first frame update
    void Start()
    {
        Usernum_input.text = WriteLogs.GetNewUser();
    }

    public void StartExperiment() {
        WriteLogs.Init();
        SceneManager.LoadScene("CHITraining");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
