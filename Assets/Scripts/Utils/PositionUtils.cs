using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionUtils : MonoBehaviour
{
    public void PrintPosition() {
        Debug.Log(gameObject.name);
        Debug.Log("Global Position: " + gameObject.transform.position.x + "-" + gameObject.transform.position.y + "-" + gameObject.transform.position.z);
        Debug.Log("Local Position: " + gameObject.transform.localPosition.x + "-" + gameObject.transform.localPosition.y + "-" + gameObject.transform.localPosition.z);
    }
}
