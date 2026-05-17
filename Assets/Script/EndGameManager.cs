using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndGameManager : MonoBehaviour
{
    public enum EndType
    {
        VictoryByHoops,   
        VictoryByItem,    
        FailureTimeout    
    }

    [Header("UI 组件")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;

    [Header("显示设置")]
    public float distanceFromCamera = 0.8f;
    public float fadeInDuration = 0.3f;

    private Camera playerCamera;
    public GameObject Panel;

    private void Start()
    {
        playerCamera = Camera.main;
        Panel.SetActive(false);
    }

    public void Show(EndType type)
    {
        // 更新文字
        switch (type)
        {
            case EndType.VictoryByHoops:
                titleText.text = "Victory!";
                messageText.text = "Collected enough incense hoops. The fire dragon blesses your home!";
                titleText.color = Color.yellow;
                break;
            case EndType.VictoryByItem:
                titleText.text = "Victory!";
                messageText.text = "The eye-dotting brush awakens the dragon. Divine blessing!";
                titleText.color = new Color(1f, 0.8f, 0f); // 金色
                break;
            case EndType.FailureTimeout:
                titleText.text = "Defeat";
                messageText.text = "Failed to collect enough hoops within the time limit… Try again";
                titleText.color = Color.gray;
                break;
        }

        // 将 UI 放置在玩家面前固定距离
        PositionInFrontOfCamera();

        Panel.SetActive(true);
       
    }

    private void PositionInFrontOfCamera()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        transform.position = playerCamera.transform.position + forward.normalized * distanceFromCamera;
    
        transform.LookAt(playerCamera.transform);
        Vector3 euler = transform.eulerAngles;
        euler.x = 0;
        euler.z = 0;
        euler.y = 0;
        transform.eulerAngles = euler;
    }

   
}
