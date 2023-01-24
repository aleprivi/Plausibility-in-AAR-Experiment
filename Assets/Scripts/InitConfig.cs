using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitConfig : MonoBehaviour
{
    public TMP_InputField Usernum_input;
    public TMP_InputField CIPIC_input;
    public TextMeshProUGUI Usernum_text;
    public TextMeshProUGUI CIPIC_text;

    void Start()
    {
        //Debug.Log(Application.persistentDataPath);
        string s;
        if(WriteLogs.userNum != "noUser") {
            s = WriteLogs.userNum;
        }else{
            Debug.Log("No user found");
            s = WriteLogs.GetLastUser();
                if(s == null){
                    s = WriteLogs.GetNewUser();
                }
        }
        string c = WriteLogs.GetLastCIPIC();

        if(Usernum_input != null) Usernum_input.text = s;
        if(Usernum_text != null) Usernum_text.text = s;
        if(CIPIC_input != null) CIPIC_input.text = c;
        if(CIPIC_text != null) CIPIC_text.text = c;
    }

    public void StartExperiment() {
        WriteLogs.Init(Usernum_input.text);
        SceneManager.LoadScene("0a1.ExperimentMenu");
    }

    public void StartTest(){
        WriteLogs.InitTestMode();
        SceneManager.LoadScene("CHITraining");
    }

    public void NewUser(){
        Usernum_input.text = WriteLogs.GetNewUser();
    }

    public void OnClicked(Button button)
    {
        SceneManager.LoadScene(button.name);
    }

    public void StartSlaterExp(int condition)
    {
        WriteLogs.condition = condition;
        SceneManager.LoadScene("2.ISMAR Exp_SlaterExperiment");
    }
}
