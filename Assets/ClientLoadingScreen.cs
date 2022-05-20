using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientLoadingScreen : MonoBehaviour
{
    [SerializeField]
    CanvasGroup m_CanvasGroup;

    [SerializeField]
    float m_DelayBeforeFadeOut = 0.5f;

    [SerializeField]
    float m_FadeOutDuration = 0.1f;

    bool m_LoadingScreenRunning;

    AsyncOperation m_LoadOperation;

    Coroutine m_FadeOutCoroutine;


    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        m_CanvasGroup.alpha = 0;
    }

    public void StopLoadingScreen()
    {
        if (m_LoadingScreenRunning)
        {
            if (m_FadeOutCoroutine != null)
            {
                StopCoroutine(m_FadeOutCoroutine);
            }
            m_FadeOutCoroutine = StartCoroutine(FadeOutCoroutine());
        }
    }

    public void StartLoadingScreen(string sceneName, AsyncOperation loadOperation)
    {
        m_CanvasGroup.alpha = 1;
        m_LoadingScreenRunning = true;
        UpdateLoadingScreen(sceneName, loadOperation);
    }

    public void UpdateLoadingScreen(string sceneName, AsyncOperation loadOperation)
    {
        if (m_LoadingScreenRunning)
        {
            m_LoadOperation = loadOperation;
        }
    }


    IEnumerator FadeOutCoroutine()
    {
        yield return new WaitForSeconds(m_DelayBeforeFadeOut);
        m_LoadingScreenRunning = false;

        float currentTime = 0;
        while (currentTime < m_FadeOutDuration)
        {
            m_CanvasGroup.alpha = Mathf.Lerp(1, 0, currentTime / m_FadeOutDuration);
            yield return null;
            currentTime += Time.deltaTime;
        }

        m_CanvasGroup.alpha = 0;
    }
}
