using UnityEngine;
using System.Linq;
using System.Collections;

public class AttachUIToHandFacingCamera : MonoBehaviour
{
    [Header("手部设置")]
    public OVRHand leftHand; // 手动拖拽赋值

    [Header("附加位置")]
    public OVRSkeleton.BoneId anchorBone = OVRSkeleton.BoneId.Hand_WristRoot;
    public Vector3 worldOffset = new Vector3(0, 0.05f, 0.1f); // 相对于手腕骨骼的局部偏移

    private OVRSkeleton skeleton;
    private OVRBone targetBone;
    private Transform cameraTransform;
    private bool isReady = false;

    void Start()
    {
        if (leftHand == null)
        {
            Debug.LogError("左手 OVRHand 未赋值！请在 Inspector 中手动拖拽。");
            return;
        }

        cameraTransform = Camera.main?.transform;
        if (cameraTransform == null)
        {
            Debug.LogError("未找到 Main Camera！");
            return;
        }

        skeleton = leftHand.GetComponent<OVRSkeleton>();
        if (skeleton == null)
        {
            Debug.LogError("OVRHand上未找到OVRSkeleton组件！");
            return;
        }

        StartCoroutine(InitializeWhenReady());
    }

    IEnumerator InitializeWhenReady()
    {
        // 等待手部被追踪且骨骼数据加载完毕
        while (!leftHand.IsTracked || skeleton.Bones == null || skeleton.Bones.Count == 0)
        {
            yield return null;
        }

        // 查找指定的骨骼
        targetBone = skeleton.Bones.FirstOrDefault(b => b.Id == anchorBone);
        if (targetBone == null)
        {
            Debug.LogError($"未找到骨骼: {anchorBone}");
            yield break;
        }

        isReady = true;
        Debug.Log("手腕骨骼已就绪");
    }

    void LateUpdate()
    {
        if (!isReady || targetBone?.Transform == null || cameraTransform == null)
            return;

        Transform boneTransform = targetBone.Transform;

        // 世界偏移：手腕位置 + 固定世界偏移
        Vector3 worldPos = boneTransform.position + worldOffset;

        // 计算指向摄像机的方向
        Vector3 dirToCamera = cameraTransform.position - worldPos;
        if (dirToCamera.sqrMagnitude > 0.001f)
        {


            Quaternion targetRot = Quaternion.LookRotation(-dirToCamera, Vector3.up);

            transform.SetPositionAndRotation(worldPos, targetRot);
        }
    }
}