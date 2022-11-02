using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoudnessCalSource : MonoBehaviour
{
    SDN currentSDN;
    AudioSource audioSource;
    public InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        currentSDN = gameObject.GetComponent<SDN>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    bool alreadymoved = false;
    bool ok = false;
    private void Update()
    {
        if (!alreadymoved && Time.time > 0.5) {
            alreadymoved = true;
            gameObject.transform.position = new Vector3(transform.position.x, transform.position.y+1f, transform.position.z);
        }
        if (!ok && Time.time > 1) {
            ok = true;
            gameObject.transform.position = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        }
    }

    public void playSource(bool loop) {
        currentSDN.volumeGain = float.Parse(inputField.text);
        audioSource.loop = loop;
        audioSource.Play();
    }


}
