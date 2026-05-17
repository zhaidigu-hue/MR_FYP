using Meta.XR.MRUtilityKit;
using UnityEngine;
using UnityEngine.Events;

public class RingCollector : MonoBehaviour
{
    public int ringsCollected { get; private set; }
    [SerializeField]
    public int totalRings = 10;  // 添加 totalRings 字段，默认值可修改
    public int CollectedRings { get; private set; }
    public float TimeLimmit = 100f;//假设100秒
    public float remainingTime;
    private bool isGameActive = false;

    public EndGameManager EndGameManager;

   

    [Header("Event")]
    public UnityEvent onRingCollected;
    public UnityEvent onTimeOut;
    public UnityEvent onVictory;

    private float startTime;
    private LeftHandUIManager LeftHandUIManager;
    private bool victoryByItem=false;

    private void Start()
    {
        remainingTime = TimeLimmit;
        LeftHandUIManager = FindObjectOfType<LeftHandUIManager>();
        if (LeftHandUIManager == null)
        {
            Debug.LogError("未找到 LeftHandUIManager 组件！UI 将无法更新。");
            return;
        }
    }

    public void ReduceRequiredHoops(int amount)
    {
        totalRings = Mathf.Max(1, totalRings - amount);
    }

    public void IncreaseRemainingTime(float extraSeconds)
    {
        if (!isGameActive) return;
        remainingTime += extraSeconds;
        Debug.Log($"剩余时间增加 {extraSeconds} 秒，当前剩余 {remainingTime:F1} 秒");
    }

    public void InstantVictory()
    {
       
        if (!isGameActive) return;
        victoryByItem=true;
        OnVictory();
        Debug.Log("直接胜利！");
        // 可调用一个公共方法显示胜利界面
    }
    public void GetTotalRings()
    {
        if (totalRings == 0)
        {
            totalRings = GameObject.FindGameObjectsWithTag("Ring").Length/2;
            Debug.Log($"total:{totalRings}");
            LeftHandUIManager.OnRingCollectedCallback();
            isGameActive = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isGameActive) { return; }
        if (other.tag == "Ring")
        {
            LightRing ring = other.GetComponent<LightRing>();
            
            other.gameObject.SetActive(false);
            onRingCollected?.Invoke();
            CollectedRings++;

        }
       
    }
    public void Update()
    {
        if (isGameActive)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                if (remainingTime <= 0)
                {
                    remainingTime = 0;
                    OnTimeOut();
                }
                if (totalRings <= CollectedRings)
                {
                    //ShowVictorUI
                    OnVictory();
                }
            }
            else { return; }

        }
    }
    private void OnTimeOut()
    {
        if (!isGameActive) return;
        isGameActive = false;
        EndGameManager?.Show(EndGameManager.EndType.FailureTimeout);

        onTimeOut?.Invoke();
        Debug.Log("倒计时结束，游戏失败！");
    }
    private void OnVictory()
    {
        if (!isGameActive) return;
        isGameActive = false;

        if (victoryByItem)   // 需新增 bool victoryByItem，默认 false
            EndGameManager?.Show(EndGameManager.EndType.VictoryByItem);
        else
            EndGameManager?.Show(EndGameManager.EndType.VictoryByHoops);
        onVictory?.Invoke();
        Debug.Log("游戏胜利！");
    }

    public void ResetRings()
    {
        ringsCollected = 0;
        GameObject[] rings = GameObject.FindGameObjectsWithTag("Ring");
        foreach (GameObject ring in rings)
        {
            ring.SetActive(true);
        }
    }
}