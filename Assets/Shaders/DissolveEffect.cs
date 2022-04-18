using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    // Start is called before the first frame update

    private Material material;
    
    [SerializeField] private float  DissolveSpeed = 2;
    [SerializeField] private bool   Reversed;

    private float time;
    private static readonly int MyTime = Shader.PropertyToID("_MyTime");


    void Start()
    {
        var shader = Shader.Find("Dissolve");

        if(!shader) shader = Shader.Find("GeneratedFromGraph-Dissolve");
        if(!shader) shader = Shader.Find("DissolveGenerated");
        if(!shader) { Debug.Log("Shader not found", this); return; }

        material                                = new Material(shader);
        GetComponent<SpriteRenderer>().material = material;

        time = Reversed ? 2f : 0f;
    }

    private void OnDestroy()
    {
        if(material) Destroy(material);
    }

    // Update is called once per frame
    void Update()
    {
        time += (Reversed ? -1f : 1f) * DissolveSpeed * Time.deltaTime;

        if (material) material.SetFloat(MyTime, time);
        else material = GetComponent<SpriteRenderer>().material;

        if (Mathf.Abs(time) > 2f)
        {
            time = Reversed ? 2f : 0f;
        }
    }
}
