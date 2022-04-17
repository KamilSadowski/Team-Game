using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionCamera : MonoBehaviour
{
    Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        camera.aspect = (float)Screen.width / (float)Screen.height;
    }
}
