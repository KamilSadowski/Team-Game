using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Hud : MonoBehaviour
{
    CanvasGroup canvasGroup;
    TextMesh[] textMeshes;
    TextMeshPro[] textMeshesPro;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        textMeshes = GetComponentsInChildren<TextMesh>();
        textMeshesPro = GetComponentsInChildren<TextMeshPro>();
    }

    public void HideHud()
    {
        if (!canvasGroup)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        if (textMeshes == null)
        {
            textMeshes = GetComponentsInChildren<TextMesh>();
        }
        if (textMeshesPro == null)
        {
            textMeshesPro = GetComponentsInChildren<TextMeshPro>();
        }

        canvasGroup.alpha = 0.0f;
        foreach (TextMesh textMesh in textMeshes)
        {
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 0.0f);
        }
        foreach (TextMeshPro textMesh in textMeshesPro)
        {
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 0.0f);
        }

    }

    public void ShowHud()
    {
        if (!canvasGroup)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        if (textMeshes.Length == 0)
        {
            textMeshes = GetComponentsInChildren<TextMesh>();
        }
        if (textMeshesPro.Length == 0)
        {
            textMeshesPro = GetComponentsInChildren<TextMeshPro>();
        }

        canvasGroup.alpha = 1.0f;
        foreach (TextMesh textMesh in textMeshes)
        {
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 1.0f);
        }
        foreach (TextMeshPro textMesh in textMeshesPro)
        {
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 1.0f);
        }

    }
}
