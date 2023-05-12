using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CameraCoords : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI X;
    public TextMeshProUGUI Y;
    public TextMeshProUGUI Z;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        X.text = "X: " + transform.position.x.ToString();
        Y.text = "Y: " + transform.position.y.ToString();
        Z.text = "Z: " + transform.position.z.ToString();   
    }
}
