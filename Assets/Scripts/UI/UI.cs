using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour
{
    [SerializeField] protected CanvasGroup menu;
    private static readonly float kAnimationTime = 0.5f;


    public bool ToggleMenu()
    {
        ToggleMenu(menu);
        return menu.interactable;
    }

    public void ToggleMenu(CanvasGroup c)
    {
        var target = Mathf.Abs(c.alpha - 1f);
        StartCoroutine(ShowCanvas(c, target, kAnimationTime));
    }

    public void ToggleMenuInstant(CanvasGroup c)
    {
        StopAllCoroutines();
        c.alpha = Mathf.Abs(c.alpha - 1f);
        c.blocksRaycasts = !c.blocksRaycasts;
        c.interactable = !c.interactable;
    }

    void LateUpdate()
    {
       
    }

    void Start()
    {
        if (menu != null)
        {
            menu.alpha = 0.0f;
            menu.interactable = false;
            menu.blocksRaycasts = false;
        }
    }

    private IEnumerator ShowCanvas(CanvasGroup group, float target, float animTime)
    {
        if (group != null)
        {
            float startAlpha = group.alpha;
            float t = 0.0f;

            group.interactable = !group.interactable;
            group.blocksRaycasts = !group.blocksRaycasts;
            
            while (t < animTime)
            {
                t = Mathf.Clamp(t + Time.unscaledDeltaTime, 0.0f, animTime);
                group.alpha = Mathf.SmoothStep(startAlpha, target, t / animTime);
                yield return null;
            }

        }
    }
}
