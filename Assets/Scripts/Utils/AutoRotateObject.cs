using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotateObject : MonoBehaviour
{
    // Start is called before the first frame update
public bool activateRotation = false;
public float rotationSpeed = 3f;
public float rotationAngle = 0f;
public bool rotateX = true;
public bool avanti = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotationAngle += rotationSpeed * Time.deltaTime;
        if(activateRotation){
            if(rotateX){
                transform.Rotate(0, 10*rotationSpeed * Time.deltaTime, 0, Space.Self);
            }else{
                transform.Rotate(10*rotationSpeed * Time.deltaTime, 0, 0, Space.Self);
            }
            if(rotationAngle > 360f){
                rotationAngle = 0f;
                rotateX = !rotateX;
            }
            

        }
        if(avanti){
                transform.Translate(0, 0, rotationSpeed*Time.deltaTime);
            }
    }
}
