using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{ // Start is called before the first frame update

    private Material material;

    [SerializeField] private float DissolveSpeed = 2;
    [SerializeField] private bool  Reversed;

    private                 float time;
    private static readonly int   MyTime = Shader.PropertyToID("_MyTime");
    private static readonly int   Enabled = Shader.PropertyToID("_Enabled");


    void Start()
    {
        var shader = Shader.Find("Custom/Dissolve");
        
        if (!shader) { Debug.Log("Shader not found", this); return; }

        material                                = new Material(shader);
        GetComponent<SpriteRenderer>().material = material;

        time = Reversed ? 2f : 0f;


        if (material) material.SetFloat(Enabled, 1);

    }

    private void OnDestroy()
    {
        if (material) Destroy(material);
    }

    // Update is called once per frame
    void Update()
    {
        time += (Reversed ? -1f : 1f) * DissolveSpeed * Time.deltaTime;

        if (material) material.SetFloat(MyTime, time);
        else material = GetComponent<SpriteRenderer>().material;

        if (time > 2.0f || time < 0.0f)
        {
            time = Reversed ? 2f : 0f;
            // TODO: enable when finished testing if (material) material.SetFloat(Enabled, 0);
        }
    }
}
