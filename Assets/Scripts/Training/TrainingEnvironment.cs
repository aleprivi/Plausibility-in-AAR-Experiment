using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

using UnityEngine.SceneManagement;

public class TrainingEnvironment : MonoBehaviour
{
    public GameObject front;
    public GameObject back;
    int first = 0;
    public Text instructions;

    public Button startButton;

    public GameObject user;

    GameObject activeObject;
    // Start is called before the first frame update
    void Start()
    {
        first = Random.Range(1, 3);
        Debug.Log(first);
        activeObject = (first == 1) ? front : back;
        instructions.text = "Posiziona al centro del bollino, guarda dritto di fronte a te premi START";
        front.SetActive(false);
        back.SetActive(false);
    }

    bool firstTargetReached = false;

    public void StartTraining() {
        Vector3 tmp_pos = new Vector3(0,0, Random.Range(2f, 4f));
        front.transform.localPosition = tmp_pos;
        tmp_pos = new Vector3(0, 0, Random.Range(-2f, -4f));
        back.transform.localPosition = tmp_pos;
        activeObject.SetActive(true);
        activeObject.GetComponent<AudioSource>().Play();
        instructions.text = "Vai in direzione della voce...";
        StartCoroutine(clearText());
        startButton.gameObject.SetActive(false);
    }

    public void SetTarget(){
        
        front.SetActive(false);
        back.SetActive(false);
        if (!firstTargetReached)
        {
            activeObject = (first == 1) ? back : front;
            activeObject.SetActive(true);
            firstTargetReached = true;
            activeObject.GetComponent<AudioSource>().Play();
            instructions.text = "BRAVO! Prova di nuovo!";
            StartCoroutine(clearText());
        }
        else
        {
            instructions.text = "Bravo, hai completato il training! Ora partiamo con l'esperimento...";
            startButton.gameObject.SetActive(true);
        }
    }

    public float TextFadeTime = 3f;
    IEnumerator clearText() {
        yield return new WaitForSeconds(TextFadeTime);
        //if (instructions.text.Equals("Bravo, hai completato il training! Ora partiamo con l'esperimento..."))
        //{
        //    instructions.text = "Dovrai sentirti a tuo agio all'interno della stanza...";
        //}
        //else { instructions.text = ""; }
        instructions.text = "";
    }

}
