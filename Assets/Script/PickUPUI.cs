using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickUPUI : MonoBehaviour
{
    public TextMeshProUGUI feedbackText;   
    public TextMeshProUGUI cultureText;    
    public float displayDuration = 4f;
    public float distanceFromCamera = 1.2f;   

    private Camera playerCamera;
    private float disappearTime;

    public GameObject Panel;

    private void Start()
    {
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            Debug.LogError("No Main Camera found!");
            return;
        }
        Panel.SetActive(false);   
    }

    public void Show(PickEffect.ItemType type)
    {
        SetTextByType(type);

      
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;                 
        transform.position = playerCamera.transform.position + forward.normalized * distanceFromCamera;

       
        transform.LookAt(playerCamera.transform);
        Vector3 euler = transform.eulerAngles;
        euler.x = 0;
        euler.z = 0;
        euler.y = 0;
        transform.eulerAngles = euler;

        Panel.SetActive(true);
        disappearTime = Time.time + displayDuration;
    }

    private void Update()
    {
        if (Panel.activeSelf && Time.time >= disappearTime)
        {
            Panel.SetActive(false);
        }
    }

    private void SetTextByType(PickEffect.ItemType type)
    {
        switch (type)
        {
            case PickEffect.ItemType.PearlGrass:
                feedbackText.text = "Required Rings -1";
                cultureText.text = "\"Pearl Grass ¡¤ Endless Incense";
                break;
            case PickEffect.ItemType.LongevityIncense:
                feedbackText.text = "Time +60s";
                cultureText.text = "Longevity Incense ¡¤ Blessings of Long Life";
                break;
            case PickEffect.ItemType.DottingBrush:
                feedbackText.text = "Victory!";
                cultureText.text = "Eye-Dotting ¡¤ The Dragon Awakens";
                break;
        }
    }
}
