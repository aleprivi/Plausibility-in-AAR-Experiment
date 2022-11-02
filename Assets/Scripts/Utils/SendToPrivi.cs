using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SendToPrivi : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       StartCoroutine(GetRequest());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject[] objs1;
    public GameObject[] objs2;

    public bool sendToServer = true;

    int state = 0;
    public static bool resetInit = true;

        IEnumerator GetRequest()
    {
        while (true & sendToServer)
        {
                if (resetInit) {
                    string uri = "https://www.alessandroprivitera.it/CHITEST/clear.php";
                    using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
                    {
                        yield return webRequest.SendWebRequest();
                    }
                    resetInit = false;
                    state++;
                }

                if (state == 2)
                {
                    foreach (GameObject go in objs2)
                    {
                        string uri = "https://www.alessandroprivitera.it/CHITEST/send.php?nome=" + go.name + "&x=" +
                            go.transform.position.x + "&y=" + go.transform.position.y + "&z=" + go.transform.position.z;
                        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
                        {
                            yield return webRequest.SendWebRequest();
                        }
                    }
                }
                else {
                    foreach (GameObject go in objs1)
                    {
                        string uri = "https://www.alessandroprivitera.it/CHITEST/send.php?nome=" + go.name + "&x=" +
                            go.transform.position.x + "&y=" + go.transform.position.y + "&z=" + go.transform.position.z;
                        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
                        {
                            yield return webRequest.SendWebRequest();
                            string[] pages = uri.Split('/');
                            int page = pages.Length - 1;
                            switch (webRequest.result)
                            {
                                case UnityWebRequest.Result.ConnectionError:
                                case UnityWebRequest.Result.DataProcessingError:
                                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                                    break;
                                case UnityWebRequest.Result.ProtocolError:
                                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                                    break;
                                case UnityWebRequest.Result.Success:
    //                                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                                    break;
                            }
                        }
                    }
                }
                yield return new WaitForSeconds(0.5f);

        }
    }
}
