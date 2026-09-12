using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

    public class SceneFader : MonoBehaviour
    {
        private static SceneFader s_instance;
        
        public static SceneFader Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = FindObjectOfType<SceneFader>();
                }
                
                if (s_instance == null)
                {
                    GameObject faderObject = new GameObject("SceneFader");
                    s_instance = faderObject.AddComponent<SceneFader>();
                    DontDestroyOnLoad(faderObject);
                }
                
                return s_instance;
            }
        }
        
        public static bool IsInstanceSet => s_instance != null;
        
        [Header("Fade Settings")]
        [SerializeField] private float defaultFadeDuration = 1f;
        [SerializeField] private Color fadeColor = Color.black;
        [SerializeField] private int sortOrder = 9999;
        [SerializeField] private bool debugMode = false;
        
        private Canvas canvas;
        private Image fadeImage;
        private CanvasGroup canvasGroup;
        private Coroutine currentFadeCoroutine;
        
        void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            s_instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (canvas == null)
            {
                Initialize();
            }
        }
        
        private void Initialize()
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortOrder;
            
            gameObject.AddComponent<GraphicRaycaster>();
            
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            
            GameObject imageObject = new GameObject("FadeImage");
            imageObject.transform.SetParent(transform, false);
            
            fadeImage = imageObject.AddComponent<Image>();
            fadeImage.color = fadeColor;
            fadeImage.raycastTarget = false;
            
            RectTransform rectTransform = fadeImage.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            
            fadeImage.enabled = false;
        }
        
        public void FadeOut(float duration = -1f, Action onComplete = null)
        {
            if (duration < 0) duration = defaultFadeDuration;
            
            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
            }
            
            currentFadeCoroutine = StartCoroutine(FadeCoroutine(0f, 1f, duration, onComplete));
        }
        
        public void FadeIn(float duration = -1f, Action onComplete = null)
        {
            if (duration < 0) duration = defaultFadeDuration;
            
            if (debugMode) Debug.Log($"[SceneFader] FadeIn called with duration: {duration}");
            
            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
                if (debugMode) Debug.Log($"[SceneFader] Stopped existing fade coroutine");
            }
            
            currentFadeCoroutine = StartCoroutine(FadeCoroutine(1f, 0f, duration, onComplete));
        }
        
        public void FadeTo(float targetAlpha, float duration = -1f, Action onComplete = null)
        {
            if (duration < 0) duration = defaultFadeDuration;
            
            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
            }
            
            float currentAlpha = fadeImage.enabled ? fadeImage.color.a : 0f;
            currentFadeCoroutine = StartCoroutine(FadeCoroutine(currentAlpha, targetAlpha, duration, onComplete));
        }
        
        public void SetAlpha(float alpha)
        {
            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
                currentFadeCoroutine = null;
            }
            
            Color color = fadeImage.color;
            color.a = Mathf.Clamp01(alpha);
            fadeImage.color = color;
            fadeImage.enabled = alpha > 0f;
            
            if (debugMode) Debug.Log($"[SceneFader] SetAlpha called: {alpha}, image enabled: {fadeImage.enabled}");
        }
        
        public void LoadSceneWithFade(string sceneName, float fadeOutDuration, float fadeInDuration, bool additive = false)
        {
            StartCoroutine(LoadSceneWithFadeCoroutine(sceneName, fadeOutDuration, fadeInDuration, additive));
        }
        
        public void LoadSceneWithFadeByIndex(int buildIndex, float fadeOutDuration, float fadeInDuration, bool additive = false)
        {
            StartCoroutine(LoadSceneWithFadeByIndexCoroutine(buildIndex, fadeOutDuration, fadeInDuration, additive));
        }
        
        private IEnumerator LoadSceneWithFadeCoroutine(string sceneName, float fadeOutDuration, float fadeInDuration, bool additive)
        {
            if (debugMode) Debug.Log($"[SceneFader] Starting fade out for scene: {sceneName}");
            
            if (fadeOutDuration > 0f)
            {
                FadeOut(fadeOutDuration);
                yield return new WaitForSecondsRealtime(fadeOutDuration);
            }
            else
            {
                SetAlpha(1f);
            }

            if (debugMode) Debug.Log($"[SceneFader] Loading scene: {sceneName}");
            
            LoadSceneMode mode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);
            
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            if (debugMode) Debug.Log($"[SceneFader] Scene loaded, waiting for initialization");
            
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            
            EnsureCanvasOnTop();

            if (debugMode) Debug.Log($"[SceneFader] Starting fade in, current alpha: {CurrentAlpha}");

            if (fadeInDuration > 0f)
            {
                FadeIn(fadeInDuration);
            }
            else
            {
                SetAlpha(0f);
            }
        }
        
        private IEnumerator LoadSceneWithFadeByIndexCoroutine(int buildIndex, float fadeOutDuration, float fadeInDuration, bool additive)
        {
            if (debugMode) Debug.Log($"[SceneFader] Starting fade out for scene index: {buildIndex}");
            
            if (fadeOutDuration > 0f)
            {
                FadeOut(fadeOutDuration);
                yield return new WaitForSecondsRealtime(fadeOutDuration);
            }
            else
            {
                SetAlpha(1f);
            }

            if (debugMode) Debug.Log($"[SceneFader] Loading scene index: {buildIndex}");
            
            LoadSceneMode mode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex, mode);
            
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            if (debugMode) Debug.Log($"[SceneFader] Scene loaded, waiting for initialization");
            
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            
            EnsureCanvasOnTop();

            if (debugMode) Debug.Log($"[SceneFader] Starting fade in, current alpha: {CurrentAlpha}");

            if (fadeInDuration > 0f)
            {
                FadeIn(fadeInDuration);
            }
            else
            {
                SetAlpha(0f);
            }
        }
        
        private void EnsureCanvasOnTop()
        {
            if (canvas != null)
            {
                canvas.sortingOrder = sortOrder;
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
        }
        
        private IEnumerator FadeCoroutine(float startAlpha, float endAlpha, float duration, Action onComplete)
        {
            if (debugMode) Debug.Log($"[SceneFader] FadeCoroutine started: {startAlpha} -> {endAlpha} over {duration}s");
            
            if (duration <= 0f)
            {
                Debug.LogWarning($"[SceneFader] Duration is {duration}, fading instantly!");
                fadeImage.enabled = endAlpha > 0f;
                Color instantColor = fadeImage.color;
                instantColor.a = endAlpha;
                fadeImage.color = instantColor;
                currentFadeCoroutine = null;
                onComplete?.Invoke();
                yield break;
            }
            
            fadeImage.enabled = true;
            
            float elapsed = 0f;
            Color color = fadeImage.color;
            color.a = startAlpha;
            fadeImage.color = color;
            
            if (debugMode) Debug.Log($"[SceneFader] Initial alpha set to: {startAlpha}, Starting fade loop");
            
            int frameCount = 0;
            while (elapsed < duration)
            {
                if (!fadeImage.enabled)
                {
                    if (debugMode) Debug.LogWarning($"[SceneFader] fadeImage was disabled during fade! Re-enabling.");
                    fadeImage.enabled = true;
                }
                
                float deltaTime = Time.unscaledDeltaTime;
                elapsed += deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                
                color.a = Mathf.Lerp(startAlpha, endAlpha, t);
                fadeImage.color = color;
                
                frameCount++;
                if (debugMode && frameCount <= 3)
                {
                    Debug.Log($"[SceneFader] Frame {frameCount}: elapsed={elapsed:F3}, deltaTime={deltaTime:F3}, t={t:F3}, alpha={color.a:F3}");
                }
                
                yield return null;
            }
            
            color.a = endAlpha;
            fadeImage.color = color;
            
            if (endAlpha <= 0f)
            {
                fadeImage.enabled = false;
            }
            
            currentFadeCoroutine = null;
            
            if (debugMode) Debug.Log($"[SceneFader] FadeCoroutine completed after {frameCount} frames, final alpha: {endAlpha}");
            
            onComplete?.Invoke();
        }
        
        public bool IsFading => currentFadeCoroutine != null;
        
        public float CurrentAlpha => fadeImage.enabled ? fadeImage.color.a : 0f;

        public void SetColor(Color color)
        {
            fadeImage.color = color;
            ;
        }
    }
