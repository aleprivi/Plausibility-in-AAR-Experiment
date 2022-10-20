using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoudnessSceneSelector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadCorr()
    {
        SceneManager.LoadScene("Loudness CorrCorr");
    }
    public void LoadReal()
    {
        SceneManager.LoadScene("Loudness RealCorr");
    }

    public void LoadRRT60()
    {
        SceneManager.LoadScene("Loudness RealRT60");
    }


}
