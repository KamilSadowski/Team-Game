using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{ // Start is called before the first frame update

    private Material material;

    [SerializeField] private float DissolveSpeed = .25f;
    [SerializeField] private bool  Reversed;

    private                 float time;
    private static readonly int   MyTime = Shader.PropertyToID("_MyTime");
    private static readonly int   Enabled = Shader.PropertyToID("_Enabled");
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private static readonly int DissolveWidth = Shader.PropertyToID("_DissolveWidth");
    private static readonly int DissolveColor = Shader.PropertyToID("_DissolveColor");
    private static readonly int PrimaryColor = Shader.PropertyToID("_PrimaryColor");
    private static readonly int SecondaryColor = Shader.PropertyToID("_SecondaryColor");
    private static readonly int TertiaryColor = Shader.PropertyToID("_TertiaryColor");

    public Color[] colour = new Color[3];

    void Start()
    {
        var shader = Shader.Find("Custom/Dissolve");
        
        if (!shader) { Debug.Log("Shader not found", this); return; }

        var renderRef = GetComponent<SpriteRenderer>();
        if (!renderRef) renderRef = GetComponentInChildren<SpriteRenderer>();

        material                                = new Material(shader);
        renderRef.material = material;
        

        time = Reversed ? DissolveSpeed : 0f;


        if (material) material.SetFloat(Enabled, 1);
            material.SetColor(DissolveColor, Random.ColorHSV());

    }

    private void OnDestroy()
    {
        if (material) Destroy(material);
    }

    // Update is called once per frame
    void Update()
    {
        time += (Reversed ? -1f : 1f) * DissolveSpeed * Time.deltaTime;

        if (material)
        {
            material.SetFloat(MyTime, time);
            material.SetFloat(DissolveAmount, time );
            material.SetFloat(DissolveWidth, 0.1f);
            material.SetColor(PrimaryColor, colour[0]);
            material.SetColor(SecondaryColor, colour[1]);
            material.SetColor(TertiaryColor, colour[2]);
        }
        else
        {
            material = GetComponent<SpriteRenderer>().material;
        }

        if (time > DissolveSpeed * 2 || time < 0.0f)
        {
            time = Reversed ? DissolveSpeed : 0f;
            // TODO: enable when finished testing if (material) material.SetFloat(Enabled, 0);
        }
    }
}
