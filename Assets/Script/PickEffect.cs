using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class PickEffect : MonoBehaviour
{
    public float destroyDelay = 4f;
    public ItemType itemType;

    public enum ItemType
    {
        PearlGrass,       // 珍珠草
        LongevityIncense, // 长寿线香
        DottingBrush      // 点睛朱笔
    }

    public UnityEvent<ItemType> onPickedUp;
    private bool pickedUp = false;
    [SerializeField]
    private RingCollector ringCollector;
    [SerializeField]
    private bool testPick=false;
    [Header("Pickup Effects")]
    public ParticleSystem[] pickupParticleSystems;
    [Header("Audio")]
    public AudioClip pickupSound;
    private AudioSource audioSource;
    [Header("Pickup Message UI")]
    public PickUPUI pickupMessageUI;

    private void Start()
    {
        
        ringCollector = FindObjectOfType<RingCollector>();
        if (ringCollector == null)
        {
            Debug.LogError("场景中没有找到 RingCollector 组件！道具效果将无法触发。");
        }
        pickupMessageUI = FindObjectOfType<PickUPUI>();
        if (pickupMessageUI == null)
        {
            Debug.LogError("场景中没有找到 RingCollector 组件！道具效果将无法触发。");
        }
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // 配置 AudioSource 属性
        audioSource.spatialBlend = 1f; // 设置为 3D 音效，更具沉浸感
        audioSource.playOnAwake = false;
    }
    private void Update()
    {
        if (testPick)
        {
            OnGrabbed();
        }
    }
    public void OnGrabbed()
    {
        if (pickedUp) return;
        pickedUp = true;
        PlayPickupEffects();

        if (pickupMessageUI != null)
        {
            pickupMessageUI.Show(itemType);
        }

        if (ringCollector != null)
        {
            switch (itemType)
            {
                case ItemType.PearlGrass:
                    ringCollector.ReduceRequiredHoops(1);
                    Debug.Log("Pearl Grass effect: Required hoops reduced by 1");
                    break;
                case ItemType.LongevityIncense:
                    ringCollector.IncreaseRemainingTime(60f);
                    Debug.Log("Longevity Incense effect: +60 seconds");
                    break;
                case ItemType.DottingBrush:
                    ringCollector.InstantVictory();
                    Debug.Log("Dotting Brush effect: Instant victory");
                    break;
            }
        }

        StartCoroutine(DestroyAfterDelay());
    }

    private void PlayPickupEffects()
    {

        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
        if (pickupParticleSystems == null) return;
        foreach (var ps in pickupParticleSystems)
        {
            if (ps != null)
            {
               
                //ps.transform.SetParent(null);
                ps.Play();
             
                //Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
            }
        }
    }
    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
