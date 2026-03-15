using UnityEngine;
using Meta.XR.MRUtilityKit;

public class InitializeInRoom : MonoBehaviour
{
    public Transform objectToPlace; // 要放置的物体（比如龙珠）
    public float heightOffset = 10f; // 离地面的高度偏移（相对于房间中心）

    void Start()
    {
        // 等待一帧确保MRUK已经加载完成（有时需要）
        // 或者直接调用，因为MRUK通常在场景加载时初始化
        PlaceObject();
    }

    void PlaceObject()
    {
        if (MRUK.Instance == null)
        {
            Debug.LogError("MRUK Instance not found!");
            return;
        }

        MRUKRoom currentRoom = MRUK.Instance.GetCurrentRoom();
        if (currentRoom != null)
        {
            // 获取房间的包围盒
            Bounds roomBounds = currentRoom.GetRoomBounds();
            Vector3 roomCenter = roomBounds.center;

            // 调整Y轴高度，让物体悬浮在视野中央（但不要超出房间）
            float desiredY = roomCenter.y + heightOffset;
            // 确保不超出房间顶部
            desiredY = Mathf.Min(desiredY, roomBounds.max.y);

            Vector3 spawnPosition = new Vector3(roomCenter.x, desiredY, roomCenter.z);
            objectToPlace.position = spawnPosition;
            Debug.Log($"Object placed at: {spawnPosition} (room bounds: {roomBounds})");
        }
        else
        {
            Debug.LogWarning("No MRUK room found. Placing object at origin.");
            objectToPlace.position = Vector3.zero;
        }
    }
}