using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAnimation : MonoBehaviour
{

    [SerializeField] float AnimationTime;

    private CanvasGroup group;

    void Start()
    {
        group = GetComponent<CanvasGroup>();
        StartCoroutine(ShowCanvas(1f));
    }

    IEnumerator ShowCanvas(float target)
    {
        if (group)
        {
            float startAlpha = group.alpha;
            float t = 0.0f;

            group.interactable = target >= 1.0f;
            group.blocksRaycasts = target >= 1.0f;

            while (t < AnimationTime)
            {
                t = Mathf.Clamp(t + Time.deltaTime, 0.0f, AnimationTime);
                group.alpha = Mathf.SmoothStep(startAlpha, target, t / AnimationTime);
                yield return null;
            }
        }
    }
}
