using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{ // Start is called before the first frame update

    private Material material;

    [SerializeField] private float DissolveSpeed = .25f;
    [SerializeField] private bool  Reversed;
    [SerializeField] private Texture2D DissolveMap;
    [SerializeField] private Texture2D DissolveNormalMap;


    private float time;
    private static readonly int   MyTime = Shader.PropertyToID("_MyTime");
    private static readonly int   Enabled = Shader.PropertyToID("_Enabled");
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private static readonly int DissolveWidth = Shader.PropertyToID("_DissolveWidth");
    private static readonly int DissolveColor = Shader.PropertyToID("_DissolveColor");
    private static readonly int PrimaryColor = Shader.PropertyToID("_PrimaryColor");
    private static readonly int SecondaryColor = Shader.PropertyToID("_SecondaryColor");
    private static readonly int TertiaryColor = Shader.PropertyToID("_TertiaryColor");

    public Color[] colour = new Color[3];
    private static readonly int DissolveMap1 = Shader.PropertyToID("_DissolveMap");
    private static readonly int NormalMap = Shader.PropertyToID("_NormalMap");

    void Start()
    {
        var shader = Shader.Find("Custom/Dissolve");
        
        if (!shader) { Debug.Log("Shader not found", this); return; }

        var renderRef = GetComponentInParent<SpriteRenderer>();
        if (!renderRef) renderRef = transform.parent.GetComponentInChildren<SpriteRenderer>();

        material = new Material(shader);
        renderRef.material = material;
        

        time = Reversed ? DissolveSpeed : 0f;


        if (material)
        {
            material.SetFloat(Enabled, 1);
            material.SetColor(DissolveColor, Random.ColorHSV());
            material.SetFloat(DissolveWidth, 0.1f);
            material.SetColor(PrimaryColor, colour[0]);
            material.SetColor(SecondaryColor, colour[1]);
            material.SetColor(TertiaryColor, colour[2]);
            material.SetTexture(DissolveMap1,DissolveMap);
            material.SetTexture(NormalMap,DissolveNormalMap);
        }
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
