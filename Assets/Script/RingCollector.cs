using UnityEngine;
using UnityEngine.Events; // 用于 UnityEvent

public class RingCollector : MonoBehaviour
{
    [Header("计数")]
    public int ringsCollected = 0;          // 当前收集的光环数量
    public int totalRings = 5;               // 场景中总光环数（可手动设置，或动态获取）

    [Header("事件")]
    public UnityEvent onRingCollected;       // 每当收集到一个光环时触发（可挂载音效、特效等）
    public UnityEvent onAllRingsCollected;   // 当收集完所有光环时触发
    // 用于动态获取总光环数（可选）
    private void Start()
    {
        // 如果totalRings为0，尝试自动计算场景中Tag为"Ring"的对象数量
        if (totalRings == 0)
        {
            totalRings = GameObject.FindGameObjectsWithTag("Ring").Length;
            Debug.Log($"自动检测到 {totalRings} 个光环");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查碰撞对象是否有"Ring"标签
        if (other.CompareTag("Ring"))
        {
            CollectRing(other.gameObject);
        }
    }

    // 收集逻辑，抽出为单独方法便于扩展
    private void CollectRing(GameObject ringObject)
    {
        // 1. 光环消失（禁用，而不是销毁，方便后续重置）
        ringObject.SetActive(false);

        // 2. 增加计数
        ringsCollected++;
        Debug.Log($"收集光环 {ringsCollected}/{totalRings}");

        // 3. 触发自定义事件（可在Inspector中绑定特效、音效等）
        onRingCollected?.Invoke();

        // 4. 检查是否收集完所有光环
        if (ringsCollected >= totalRings)
        {
            Debug.Log("所有光环收集完毕！");
            onAllRingsCollected?.Invoke();
        }
    }

    // 可选：提供一个公共方法用于重置（例如重新开始游戏时）
    public void ResetRings()
    {
        ringsCollected = 0;
        // 如果需要重新激活所有光环，可以遍历并SetActive(true)
        GameObject[] rings = GameObject.FindGameObjectsWithTag("Ring");
        foreach (GameObject ring in rings)
        {
            ring.SetActive(true);
        }
    }
}