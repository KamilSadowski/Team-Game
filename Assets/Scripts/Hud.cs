using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hud : MonoBehaviour
{
    CanvasGroup canvasGroup;
    TextMesh[] textMeshes;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        textMeshes = GetComponentsInChildren<TextMesh>();
    }

    public void HideHud()
    {
        canvasGroup.alpha = 0.0f;
        foreach (TextMesh textMesh in textMeshes)
        {
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 0.0f);
        }

    }

    public void ShowHud()
    {
        canvasGroup.alpha = 1.0f;
        foreach (TextMesh textMesh in textMeshes)
        {
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 1.0f);
        }

    }
}
