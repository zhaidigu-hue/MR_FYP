using UnityEngine;
using Meta.XR.MRUtilityKit;

public class RingSpawnController : MonoBehaviour
{
    public FindSpawnPositions spawner;

    void Start()
    {
        
        if (MRUK.Instance != null)
        {
            MRUK.Instance.SceneLoadedEvent.AddListener(OnSceneLoaded);
        }
        else
        {
            Debug.LogError("MRUK Instance not found!");
        }
    }

    private void OnSceneLoaded()
    {
       
        if (spawner != null)
        {
            spawner.StartSpawn();
            Debug.Log("Room loaded. Rings spawned!");
        }

        
        MRUK.Instance.SceneLoadedEvent.RemoveListener(OnSceneLoaded);
    }
}
