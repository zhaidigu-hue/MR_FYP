using UnityEngine;
using Meta.XR.MRUtilityKit;
using System.Collections.Generic;

public class SpawnRingPerFurniture : MonoBehaviour
{
    [Header("光环预制体（支持多个随机选择）")]
    public GameObject[] lightRingPrefabs;

    [Header("生成位置偏移")]
    public float couchHeightOffset = 0.6f;      // 沙发上方的偏移高度
    public float bedHeightOffset = 0.8f;        // 床上方的偏移高度
    public float storageSideHeight = 1.2f;      // 储物柜侧面的生成高度（从地面算起）
    public float storageSideOffset = 0.4f;      // 储物柜侧面外扩距离

    private List<MRUKAnchor> processedAnchors = new List<MRUKAnchor>();

    void Start()
    {
        MRUK.Instance.SceneLoadedEvent.AddListener(OnSceneLoaded);
    }

    private void OnSceneLoaded()
    {
        StartCoroutine(SpawnRingsCoroutine());
    }

    private System.Collections.IEnumerator SpawnRingsCoroutine()
    {
        yield return null; // 等待一帧，确保房间数据就绪

        MRUKRoom currentRoom = MRUK.Instance.GetCurrentRoom();
        if (currentRoom == null) yield break;

        List<MRUKAnchor> allAnchors = currentRoom.Anchors;
        int spawnedCount = 0;

        foreach (MRUKAnchor anchor in allAnchors)
        {
            MRUKAnchor.SceneLabels label = anchor.Label; // 现在 label 是 SceneLabels 枚举
            Vector3? spawnPos = null;

            // 使用枚举比较，不再用字符串
            if (label == MRUKAnchor.SceneLabels.COUCH)
            {
                spawnPos = GetPositionAboveAnchor(anchor, couchHeightOffset);
            }
            else if (label == MRUKAnchor.SceneLabels.BED)
            {
                spawnPos = GetPositionAboveAnchor(anchor, bedHeightOffset);
            }
            else if (label == MRUKAnchor.SceneLabels.STORAGE)
            {
                spawnPos = GetPositionOnStorageSide(anchor, storageSideHeight, storageSideOffset);
            }

            if (spawnPos.HasValue)
            {
                SpawnRingAtPosition(spawnPos.Value, anchor);
                spawnedCount++;
                processedAnchors.Add(anchor);
            }
        }

        Debug.Log($"成功为 {spawnedCount} 个家具生成了光环 (COUCH/BED/STORAGE)");
    }

    // 在锚点正上方偏移处生成（适用于沙发、床等体积型物体）
    private Vector3 GetPositionAboveAnchor(MRUKAnchor anchor, float heightOffset)
    {
        // 1. 检查体积边界是否有效
        if (!anchor.VolumeBounds.HasValue)
        {
            Debug.LogWarning($"锚点 {anchor.Label} 没有 VolumeBounds，使用锚点位置作为基准。");
            return anchor.transform.position + Vector3.up * heightOffset;
        }

        Bounds localBounds = anchor.VolumeBounds.Value;

        // 2. 关键：计算体积顶部中心的世界坐标
        //    先计算出局部坐标下的顶部中心点 (x=center.x, y=max.y, z=center.z)
        Vector3 localTopCenter = new Vector3(localBounds.center.x, localBounds.max.y, localBounds.center.z);

        // 3. 将局部坐标转换为世界坐标
        Vector3 worldTopCenter = anchor.transform.TransformPoint(localTopCenter);

        // 4. 在世界坐标基础上，向上偏移指定的高度
        return worldTopCenter + Vector3.up * heightOffset;
    }

    // 在储物柜侧面（正面）生成，高度从地面算起
    private Vector3 GetPositionOnStorageSide(MRUKAnchor anchor, float heightFromFloor, float outwardDist)
    {
        if (!anchor.VolumeBounds.HasValue)
        {
            Debug.LogWarning($"锚点 {anchor.Label} 没有 VolumeBounds，使用锚点位置作为基准。");
            return anchor.transform.position + new Vector3(0, heightFromFloor, outwardDist);
        }

        Bounds localBounds = anchor.VolumeBounds.Value;

        // 1. 计算侧面生成点的局部坐标
        //    以正面（Z轴正方向）为例，向外偏移
        Vector3 localSidePos = new Vector3(localBounds.center.x, localBounds.min.y + heightFromFloor, localBounds.max.z + outwardDist);

        // 2. 转换为世界坐标
        Vector3 worldSidePos = anchor.transform.TransformPoint(localSidePos);

        return worldSidePos;
    }

    private void SpawnRingAtPosition(Vector3 position, MRUKAnchor furnitureAnchor)
    {
        if (lightRingPrefabs == null || lightRingPrefabs.Length == 0)
        {
            Debug.LogError("未设置光环预制体！");
            return;
        }

        GameObject prefab = lightRingPrefabs[Random.Range(0, lightRingPrefabs.Length)];
        GameObject ring = Instantiate(prefab, position, Quaternion.identity);

        // 初始化光环，关联家具锚点（供后续答题 UI 使用）
        LightRing ringScript = ring.GetComponent<LightRing>();
        if (ringScript != null)
        {
            ringScript.Initialize(furnitureAnchor);
        }

        Debug.Log($"在 {position} 生成光环，关联家具: {furnitureAnchor.Label}");
    }
}