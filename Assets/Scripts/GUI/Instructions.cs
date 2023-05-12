using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Instructions : MonoBehaviour
{
    public TMP_Text text;
    public string[] InstructionSet;
    int currentInstruction = 0;

    public Texture2D texture;
    public GameObject points;
    // Start is called before the first frame update
    GameObject[] go;

    public string nextScene;
    
    void Start()
    {   
        //Write WriteLogs here
        Debug.Log("CIPIC= " + WriteLogs.CIPIC);
        text.text = InstructionSet[currentInstruction];
        //create a rawimage
        go = new GameObject[InstructionSet.Length];
        for(int i = 0; i < InstructionSet.Length; i++){
            go[i] = new GameObject("RawImage");
            go[i].AddComponent<RawImage>();
            go[i].GetComponent<RawImage>().texture = texture;
            go[i].transform.SetParent(points.transform);
            go[i].GetComponent<RawImage>().color = new Color(1, 1, 1, 0.1f);
        }

        go[0].GetComponent<RawImage>().color = new Color(1, 1, 1, 1f);
    }

    public void NextInstruction()
    {
        currentInstruction++;
        if (currentInstruction >= InstructionSet.Length)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
        }else{
            text.text = InstructionSet[currentInstruction];
            lightPoints();
        }
    }

    public void PreviousInstruction()
    {
        currentInstruction--;
        if (currentInstruction < 0)
        {
            currentInstruction = 0;
        }
        text.text = InstructionSet[currentInstruction];
        lightPoints();
    }

    void lightPoints(){
        for(int i = 0; i < InstructionSet.Length; i++){
            go[i].GetComponent<RawImage>().color = new Color(1, 1, 1, 0.1f);
        }
        go[currentInstruction].GetComponent<RawImage>().color = new Color(1, 1, 1, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
