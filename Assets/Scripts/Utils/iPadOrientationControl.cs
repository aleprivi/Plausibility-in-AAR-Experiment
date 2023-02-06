using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine;
using UnityEngine.UI;

public class iPadOrientationControl : MonoBehaviour
{


    bool hide_blackscreen = false;

    public Camera ARCamera;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isEditor)
        {
            GetComponent<Image>().enabled = false;
        }
        this.GetComponent<Image>().CrossFadeAlpha(0, 0.01f, false);
        currentTime = waitingTime;
    }

    bool toMuchRotation = false;
    bool isBeheaded = false;

    bool isFaded = false;

    public float waitingTime = 3f;


    float currentTime = 0;
    public void fade() {
        if (!isFaded && (toMuchRotation || isBeheaded)) {
            currentTime -= Time.deltaTime;
        }
        if (!isFaded && currentTime <= 0) {
            isFaded = true;
            this.GetComponent<Image>().CrossFadeAlpha(1, 1f, false);
        }

        if (isFaded && !toMuchRotation && !isBeheaded) {
            currentTime = waitingTime;
            isFaded = false;
            this.GetComponent<Image>().CrossFadeAlpha(0, 1f, false);
        }
    }

    //public bool hasHead = true;
    
    void Update()
    {

        //se troppo in alto o troppo in basso fade
        float x_rot = ARCamera.transform.eulerAngles.x;

        x_rot = x_rot > 180 ? x_rot - 360 : x_rot;
        x_rot = Mathf.Abs(x_rot);

        x_rot -= 45;

        toMuchRotation = x_rot > 0;

        fade();
    }

    public void check_BeHeaded(bool headTracked){

        isBeheaded = headTracked;

    }
}
