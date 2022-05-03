using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BossHealthBar : UI_ChargingBar
{
    private MortalHealthComponent boss;
    private CanvasGroup canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<CanvasGroup>();
        if (canvas) canvas.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (boss)
        {
            UpdateProgBar(boss.GetHealthPercentage());
        }
        else
        {
            boss = GameObject.FindGameObjectWithTag("Boss")?.GetComponent<MortalHealthComponent>();

            canvas.alpha = boss ? 1f : 0f;
        }
    }
}
