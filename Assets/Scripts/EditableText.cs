using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EditableText : MonoBehaviour
{
    TextMesh text;
    bool isVisible = true;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMesh>();
    }

    public void TextVisible(bool visible)
    {
        if (!text)
        {
            text = GetComponent<TextMesh>();
        }
        if (visible == isVisible) return;
        Color col = text.color;
        col.a = visible ? 1.0f : 0.0f;
        text.color = col;
        isVisible = visible;
    }
    public void SetText(string newText)
    {
        if (!text)
        {
            text = GetComponent<TextMesh>();
        }
        text.text = newText;
    }
}
