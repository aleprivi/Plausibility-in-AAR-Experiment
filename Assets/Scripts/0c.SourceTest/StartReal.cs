using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class StartReal : MonoBehaviour
{
    // Start is called before the first frame update

    public void Start()
    {
        Debug.Log(Application.persistentDataPath);
    }

    public TextMeshProUGUI respMsg;

    bool isVirtual = true;
    public void OnClick()
    {
        isVirtual = !isVirtual;
        foreach(GameObject go in guiElements)
        {
            go.SetActive(isVirtual);
        }
        StartCoroutine(GetRequest());
    }
        IEnumerator GetRequest()
    {
        int tosend = (!isVirtual)? type:0;
        Debug.Log("Sending: " + tosend);

        string uri = "https://www.alessandroprivitera.it/CHITEST/StartMusic.php?type=" + tosend;
        Debug.Log(uri);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();
            respMsg.text = webRequest.downloadHandler.text;
        }
                
    }

    public GameObject[] guiElements;


    public int type = 1;
    /*
    0-> stop
    1->voice
    2->noise
    3->step
    */
    public void setType(int t){
        type = t;
        if(!isVirtual){
            StartCoroutine(GetRequest());
        }
    }
}
