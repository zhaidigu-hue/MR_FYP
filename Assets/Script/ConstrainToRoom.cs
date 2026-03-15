using UnityEngine;
using Meta.XR.MRUtilityKit;

public class ConstrainToRoom : MonoBehaviour
{
    void LateUpdate()
    {
        if (MRUK.Instance == null) return;

        MRUKRoom currentRoom = MRUK.Instance.GetCurrentRoom();
        if (currentRoom == null) return;

        // 获取房间包围盒
        Bounds roomBounds = currentRoom.GetRoomBounds();

        // 将当前位置限制在包围盒内
        Vector3 constrainedPos = transform.position;
        constrainedPos.x = Mathf.Clamp(constrainedPos.x, roomBounds.min.x, roomBounds.max.x);
        constrainedPos.y = Mathf.Clamp(constrainedPos.y, roomBounds.min.y, roomBounds.max.y);
        constrainedPos.z = Mathf.Clamp(constrainedPos.z, roomBounds.min.z, roomBounds.max.z);

        transform.position = constrainedPos;
    }
}