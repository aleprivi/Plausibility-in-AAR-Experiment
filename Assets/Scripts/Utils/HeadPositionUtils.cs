using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadPositionUtils : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_EDITOR
            Debug.Log("Editor Mode");
            ARCamera = GameObject.Find("AR Camera");
        #endif
    }

    GameObject ARCamera;

    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR
            this.transform.position = ARCamera.transform.position;

            if (Input.GetKeyDown(KeyCode.X))
            {

                float x = Random.Range(0, 10);
                float y = Random.Range(0, 10);
                float z = Random.Range(0, 10);

                float r_x = Random.Range(0, 40);
                float r_y = Random.Range(0, 360);
                float r_z = Random.Range(0, 40);

                Vector3 tmp = new Vector3(x, y, z);
                Vector3 tmp_rot = new Vector3(r_x, r_y, r_z);

                ARCamera.transform.position = tmp;
                ARCamera.transform.Rotate(tmp_rot);

            }
        #endif
    }
}
