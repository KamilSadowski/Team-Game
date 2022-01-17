using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ControlledDestroyOnLoad : MonoBehaviour
{
    [SerializeField] string[] accessibleScenes;
    Scene currentScene;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneChange();
    }

    // Update is called once per frame
    private void SceneChange()
    {   
        currentScene = SceneManager.GetActiveScene();
        
        
        bool isAccessible = false;
        foreach (string scene in accessibleScenes)
        {
            if (currentScene.name == scene)
            {
                isAccessible = true;
            }
        }
        
        if (!isAccessible) Destroy(this.gameObject);
       
    }

    private void FixedUpdate()
    {
     if (currentScene != SceneManager.GetActiveScene())
        {
            SceneChange();
        }
    }
}
