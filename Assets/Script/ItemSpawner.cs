using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using Oculus.Interaction.HandGrab;
using UnityEngine;


public class ItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnConfig
    {
        [Tooltip("要生成道具的家具标签（仅当 spawnOneRandomly = false 时生效）")]
        public MRUKAnchor.SceneLabels furnitureLabel = MRUKAnchor.SceneLabels.OTHER;

        [Tooltip("道具预制体")]
        public GameObject itemPrefab;

        [Tooltip("每个家具上生成的数量（仅当 spawnOneRandomly = false 时生效）")]
        public int spawnCount = 1;

        [Tooltip("生成高度偏移（家具顶部上方）")]
        public float heightOffset = 0.2f;

        [Tooltip("是否使用 LabelFilter 辅助（保持 false 即可）")]
        public bool useLabelFilter = false;

        [Tooltip("是否启用全局随机单一生成模式（点睛笔专用）")]
        public bool spawnOneRandomly = false;

        [Tooltip("随机生成时，候选家具标签列表（仅当 spawnOneRandomly = true 时生效）")]
        public List<MRUKAnchor.SceneLabels> randomLabels = new List<MRUKAnchor.SceneLabels>();
    }

    public List<SpawnConfig> itemsToSpawn;

    private void Start()
    {
        if (MRUK.Instance != null)
            MRUK.Instance.SceneLoadedEvent.AddListener(OnSceneLoaded);
        else
            Debug.LogError("MRUK Instance not found!");
    }

    private void OnSceneLoaded()
    {
        MRUKRoom currentRoom = MRUK.Instance.GetCurrentRoom();
        if (currentRoom == null) return;

        foreach (var config in itemsToSpawn)
        {
            if (config.spawnOneRandomly)
            {
               
                SpawnOneRandomly(currentRoom, config);
            }
            else
            {
                
                List<MRUKAnchor> targetAnchors = GetAnchorsByLabel(currentRoom, config.furnitureLabel, config.useLabelFilter);
                foreach (var anchor in targetAnchors)
                {
                    Vector3 spawnPos = GetSpawnPosition(anchor, config.heightOffset);
                    for (int i = 0; i < config.spawnCount; i++)
                    {
                        GameObject item = Instantiate(config.itemPrefab, spawnPos, Quaternion.identity);
                        SetItemActive(item, false);
                    }
                }
            }
        }
    }

   
    private void SpawnOneRandomly(MRUKRoom room, SpawnConfig config)
    {
        if (config.randomLabels == null || config.randomLabels.Count == 0)
        {
            Debug.LogWarning("随机生成模式未指定候选标签列表，跳过");
            return;
        }

       
        List<MRUKAnchor> candidates = new List<MRUKAnchor>();
        foreach (var anchor in room.Anchors)
        {
            if (config.randomLabels.Contains(anchor.Label))
            {
                candidates.Add(anchor);
            }
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning("未找到任何符合条件的家具，点睛笔将不会生成");
            return;
        }

      
        MRUKAnchor selectedAnchor = candidates[Random.Range(0, candidates.Count)];
        Vector3 spawnPos = GetSpawnPosition(selectedAnchor, config.heightOffset);
        GameObject item = Instantiate(config.itemPrefab, spawnPos, Quaternion.identity);
        SetItemActive(item, false);
        Debug.Log($"点睛笔已生成在 {selectedAnchor.Label} 上");
    }

    private List<MRUKAnchor> GetAnchorsByLabel(MRUKRoom room, MRUKAnchor.SceneLabels targetLabel, bool useFilter = false)
    {
        List<MRUKAnchor> result = new List<MRUKAnchor>();
        if (!useFilter)
        {
            foreach (var anchor in room.Anchors)
            {
                if (anchor.Label == targetLabel)
                    result.Add(anchor);
            }
        }
        else
        {
            var filter = new LabelFilter(targetLabel);
            foreach (var anchor in room.Anchors)
            {
                if (filter.PassesFilter(anchor.Label))
                    result.Add(anchor);
            }
        }
        return result;
    }

    private Vector3 GetSpawnPosition(MRUKAnchor anchor, float yOffset)
    {
        if (anchor.VolumeBounds.HasValue)
        {
            Bounds bounds = anchor.VolumeBounds.Value;
            Vector3 topCenter = anchor.transform.TransformPoint(new Vector3(bounds.center.x, bounds.max.y, bounds.center.z));
            return topCenter + Vector3.up * yOffset;
        }
        else
        {
           
            return anchor.transform.position + Vector3.up * 0.5f;
        }
    }

    private void SetItemActive(GameObject item, bool active)
    {
        var renderers = item.GetComponentsInChildren<MeshRenderer>();
        foreach (var r in renderers) r.enabled = active;
        var grab = item.GetComponent<DistanceHandGrabInteractable>();
        if (grab != null) grab.enabled = active;
    }
}
