using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideMeshes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool visible = true;
    public GameObject[] elements;


    public void ToggleMeshes(){
        visible = !visible;

        foreach (GameObject el in elements) {
            el.GetComponent<MeshRenderer>().enabled = visible;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
