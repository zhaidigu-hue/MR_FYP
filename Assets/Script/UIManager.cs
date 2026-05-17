using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("临时提示")]
    public GameObject floatingTextPrefab;   // 一个 World Space Canvas 预制体，包含 TextMeshPro
    public float displayDuration = 2f;
    public float distanceFromCamera = 1.2f;  // 距离相机前方距离
    public Vector3 offset = new Vector3(0, 0.2f, 0); // 相对于相机中心的偏移（垂直方向）

    private Camera mainCamera;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    // 在玩家视野前方显示一段文字
    public void ShowFloatingMessage(string message, Color color)
    {
        if (floatingTextPrefab == null || mainCamera == null) return;

        // 计算生成位置：相机前方 distanceFromCamera 处，加上偏移
        Vector3 spawnPos = mainCamera.transform.position + mainCamera.transform.forward * distanceFromCamera + offset;
        GameObject msgObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);

        TextMeshProUGUI text = msgObj.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = message;
            text.color = color;
        }

        // 让 UI 面向相机（使其始终正对玩家）
        msgObj.transform.LookAt(mainCamera.transform);
        msgObj.transform.Rotate(0, 180, 0); // 因为 LookAt 使正面朝向相机，需要翻转

        // 自动销毁
        Destroy(msgObj, displayDuration);
    }
}