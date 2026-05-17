using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeftHandUIManager : MonoBehaviour
{
    [Header("计时器组件")]
    public Image timerFillImage;      // 圆环填充图
    public TextMeshProUGUI timerText;     // 剩余时间文字

    [Header("光环计数组件")]
    public TextMeshProUGUI ringCounterText; // 显示 "剩余: X"

    private RingCollector ringCollector;
    private float maxTime;             // 记录总时间上限（用于计算比例）

    void Start()
    {
        ringCollector = FindObjectOfType<RingCollector>();
        if (ringCollector == null)
        {
            Debug.LogError("未找到 RingCollector 组件！UI 将无法更新。");
            return;
        }

        maxTime = ringCollector.TimeLimmit;

       
        UpdateTimerUI();
        UpdateRingUI();
    }

    void Update()
    {
       
        UpdateTimerUI();
    }

    public void UpdateTimerUI()
    {
        if (ringCollector == null) return;

      
        float fillAmount = ringCollector.remainingTime / maxTime;
        fillAmount = Mathf.Clamp01(fillAmount);
        if (timerFillImage != null)
            timerFillImage.fillAmount = fillAmount;

       
        int minutes = Mathf.FloorToInt(ringCollector.remainingTime / 60);
        int seconds = Mathf.FloorToInt(ringCollector.remainingTime % 60);
        if (timerText != null)
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateRingUI()
    {
        if (ringCollector == null) return;
        int remainingRings = ringCollector.totalRings - ringCollector.CollectedRings;
        if (ringCounterText != null)
            ringCounterText.text = $"Rings Left: {remainingRings}";
    }

    
    public void OnRingCollectedCallback()
    {
        UpdateRingUI();
    }
}
