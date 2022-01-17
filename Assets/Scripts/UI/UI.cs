using UnityEngine;
using TMPro;
using System.Collections;

public class UI : MonoBehaviour
{
    [SerializeField] private CanvasGroup menu;
    private bool isViewing = false;
    private static readonly float animationTime = 0.5f;


    public bool ToggleMenu()
    {
        if (!isViewing)
        {
            StartCoroutine(ShowCanvas(menu, 1.0f, true));
            return true;
        }
        else
            StartCoroutine(ShowCanvas(menu, 0.0f, false));
        return false;

        //Can assume it'll never be higher than 1.
    
    }


    private void Awake()
    {
        if (menu != null)
        {
            menu.alpha = 0.0f;
            menu.interactable = false;
            menu.blocksRaycasts = false;
        }
    }
    private IEnumerator ShowCanvas(CanvasGroup group, float target, bool isBlockRaycast)
    {
        if (group != null)
        {
            float startAlpha = group.alpha;
            float t = 0.0f;

            group.interactable = target >= 1.0f;
            group.blocksRaycasts = isBlockRaycast;

            //Alpha hasn't changed and it should only ever be 1 or 0 
            isViewing = (target > .5f);

            while (t < animationTime)
            {
                t = Mathf.Clamp(t + Time.deltaTime, 0.0f, animationTime);
                group.alpha = Mathf.SmoothStep(startAlpha, target, t / animationTime);
                yield return null;
            }

        }
    }
}
