using UnityEngine;
using Meta.XR.MRUtilityKit;

public class LightRing : MonoBehaviour
{
    [SerializeField]
    public MRUKAnchor attachedFurniture; // 这个光环对应的家具

    public void Initialize(MRUKAnchor furniture)
    {
        attachedFurniture = furniture;
    }
}
