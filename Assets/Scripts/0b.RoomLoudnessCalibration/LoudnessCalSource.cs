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

    bool useHRTF = true;
    public void switchHRTF() {
        useHRTF = !useHRTF;
        currentSDN.applyHrtfToReflections = useHRTF;
        currentSDN.applyHrtfToDirectSound = useHRTF;
    }

    bool useReverb = true;
    public void switchReverb() {
        useReverb = !useReverb;
        currentSDN.doLateReflections = useReverb;
    }

    public AudioClip[] audios;
    public float[] xs;
    public float[] ys;

    public void loadSource(int num) {
        audioSource.Stop();
        audioSource.transform.position = new Vector3(0, ys[num], xs[num]);
        currentSDN.volumeGain = float.Parse(inputField.text);
        audioSource.clip = audios[num];
        audioSource.Play();
    }


    bool corridor = true;
    public GameObject corridorRoom;
    public GameObject randoRoom;
    public void switchRoom() {
        corridor = !corridor;
        corridorRoom.SetActive(corridor);
        randoRoom.SetActive(!corridor);
    }
}
