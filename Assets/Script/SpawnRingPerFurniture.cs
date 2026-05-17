using UnityEngine;
using Meta.XR.MRUtilityKit;
using System.Collections.Generic;

public class SpawnRingPerFurniture : MonoBehaviour
{
    [Header("光环预制体（支持多个随机选择）")]
    public GameObject[] lightRingPrefabs;


    [Header("生成位置偏移")]
    public float couchHeightOffset = 0.6f;      
    public float bedHeightOffset = 0.8f;        
    public float storageSideHeight = 0.3f;     
    public float storageSideOffset = 0.4f;      

    private List<MRUKAnchor> processedAnchors = new List<MRUKAnchor>();

    [SerializeField]
    RingCollector ringCollector;

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
        yield return null; 

        MRUKRoom currentRoom = MRUK.Instance.GetCurrentRoom();
        if (currentRoom == null) yield break;

        List<MRUKAnchor> allAnchors = currentRoom.Anchors;
        int spawnedCount = 0;

        foreach (MRUKAnchor anchor in allAnchors)
        {
            MRUKAnchor.SceneLabels label = anchor.Label; 
            Vector3? spawnPos = null;

            
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
        ringCollector.GetTotalRings();
    }

   
    private Vector3 GetPositionAboveAnchor(MRUKAnchor anchor, float heightOffset)
    {
       
        if (!anchor.VolumeBounds.HasValue)
        {
            
            return anchor.transform.position + Vector3.up * heightOffset;
        }

        Bounds localBounds = anchor.VolumeBounds.Value;


        Vector3 localTopCenter = new Vector3(localBounds.center.x, localBounds.max.y, localBounds.center.z);

        Vector3 worldTopCenter = anchor.transform.TransformPoint(localTopCenter);

        return worldTopCenter + Vector3.up * heightOffset;
    }

   
    private Vector3 GetPositionOnStorageSide(MRUKAnchor anchor, float heightFromFloor, float outwardDist)
    {
        if (!anchor.VolumeBounds.HasValue)
        {
           
            return anchor.transform.position + new Vector3(0, heightFromFloor, outwardDist);
        }

        Bounds localBounds = anchor.VolumeBounds.Value;

       
        Vector3 localSidePos = new Vector3(localBounds.center.x, localBounds.min.y + heightFromFloor, localBounds.max.z + outwardDist);

       
        Vector3 worldSidePos = anchor.transform.TransformPoint(localSidePos);

        return worldSidePos;
    }

    private void SpawnRingAtPosition(Vector3 position, MRUKAnchor furnitureAnchor)
    {
        if (lightRingPrefabs == null || lightRingPrefabs.Length == 0)
        {
            
            return;
        }

        GameObject prefab = lightRingPrefabs[Random.Range(0, lightRingPrefabs.Length)];
        GameObject ring = Instantiate(prefab, position, Quaternion.identity);

      
        LightRing ringScript = ring.GetComponent<LightRing>();
        if (ringScript != null)
        {
            ringScript.Initialize(furnitureAnchor);
        }

    }
}