using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassthroughDebug : MonoBehaviour
{
    void Start()
    {
        // 延迟一点确保 OVRManager 已经初始化完成，给系统一点准备时间
        Invoke(nameof(EnablePassthroughManually), 0.5f);
    }

    void EnablePassthroughManually()
    {
        if (OVRManager.instance != null)
        {
            // 直接开启 Passthrough，跳过已废弃的 Capability 检查
            OVRManager.instance.isInsightPassthroughEnabled = true;
            Debug.Log("Passthrough manually enabled");
        }
    }
}
