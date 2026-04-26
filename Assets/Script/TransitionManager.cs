using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [Header("UI过渡")]
   
 

    [Header("音效")]
    //香枝出现后播放的音效
    public AudioClip transitionSound;
    //用于播放音效的AudioSource组件
    public AudioSource audioSource;

    [Header("场景设置")]
    public string targetSceneName = "PlayTest";

    private bool isTransitioning = false;

    private void Awake()
    {
        // 实现单例模式，确保跨场景唯一
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 确保UI元素一开始是完全透明的
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 开始转场流程，由香炉交互脚本调用
    /// </summary>
    public void StartTransition()
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionCoroutine());
        }
    }

    private IEnumerator TransitionCoroutine()
    {
        isTransitioning = true;

        // 1. 播放音效
        if (audioSource != null && transitionSound != null)
        {
            audioSource.PlayOneShot(transitionSound);
        }

        // 3. 异步加载目标场景 (加载时保持黑屏)
        yield return StartCoroutine(LoadSceneAsync());



        isTransitioning = false;
    }

  

    /// <summary>
    /// 异步加载场景，期间保持黑屏直到加载完成
    /// </summary>
    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
        asyncLoad.allowSceneActivation = false;  // 禁止自动切换

        // 等待加载进度达到90% (相当于底层资源加载完成)
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // 可在此处增加最小等待时间，以避免加载过快导致视觉跳跃 (可选)
        // yield return new WaitForSeconds(0.5f);

        // 激活新场景
        asyncLoad.allowSceneActivation = true;

        // 等待场景激活完成，确保新场景的物体已经初始化（特别是新的Camera和系统）
        // 方式一：等待一个短暂时间（简易且可靠）
        yield return new WaitForSeconds(0.2f);
        // 方式二：等待场景激活（更严谨但需额外逻辑，此处不做强制）
        // while (!asyncLoad.isDone) yield return null;
    }

   
}
