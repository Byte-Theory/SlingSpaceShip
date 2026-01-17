using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : MonoBehaviour
{
    [Header("Chromatic Aberration Data")] 
    [SerializeField] private float minChromaticAbIntensity;
    [SerializeField] private float maxChromaticAbIntensity;
    [SerializeField] private float chromaticAbChangeSpeed;
    private float curChromaticAbIntensity;
    private float targetChromaticAbIntensity;
    
    [Header("Vignette Data")] 
    [SerializeField] private float minVignetteIntensity;
    [SerializeField] private float maxVignetteIntensity;
    [SerializeField] private float vignetteChangeSpeed;
    private float curVignetteIntensity;
    private float targetVignetteIntensity;
    
    [Header("LensDistortion Data")] 
    [SerializeField] private AnimationCurve lensDistortionCurve;
    [SerializeField] private float lensDistortionChangeDur;
    private float lensDistortionChangeTimeElapsed;
    private bool animLensDistortion;
    
    // Volume
    private Volume volume;
    
    // Postprocessing Items
    private ChromaticAberration chromaticAberration;
    private Vignette vignette;
    private LensDistortion lensDistortion;
    
    #region Singlton

    public static PostProcessingManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    #endregion
    
    void Start()
    {
        volume = GetComponent<Volume>();

        volume.profile.TryGet(out chromaticAberration);
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out lensDistortion);

        curChromaticAbIntensity = minChromaticAbIntensity;
        targetChromaticAbIntensity = minChromaticAbIntensity;
        chromaticAberration.intensity.value = curChromaticAbIntensity;

        curVignetteIntensity = minVignetteIntensity;
        targetVignetteIntensity = minVignetteIntensity;
        vignette.intensity.value = minVignetteIntensity;

        lensDistortionChangeTimeElapsed = 0.0f;
        lensDistortion.intensity.value = 0.0f;
        animLensDistortion = false;

        SetNormalValues();
    }

    void Update()
    {
        if (!Mathf.Approximately(curChromaticAbIntensity, targetChromaticAbIntensity))
        {
            bool increment = curChromaticAbIntensity < targetChromaticAbIntensity;
            curChromaticAbIntensity += Time.deltaTime * chromaticAbChangeSpeed * (increment ? 1 : -1);

            if ((curChromaticAbIntensity > targetChromaticAbIntensity && increment) ||
                (curChromaticAbIntensity < targetChromaticAbIntensity && !increment))
            {
                curChromaticAbIntensity = targetChromaticAbIntensity;
            }
            
            chromaticAberration.intensity.value = curChromaticAbIntensity;
        }
        
        if (!Mathf.Approximately(curVignetteIntensity, targetVignetteIntensity))
        {
            bool increment = curVignetteIntensity < targetVignetteIntensity;
            curVignetteIntensity += Time.deltaTime * vignetteChangeSpeed * (increment ? 1 : -1);

            if ((curVignetteIntensity > targetVignetteIntensity && increment) ||
                (curVignetteIntensity < targetVignetteIntensity && !increment))
            {
                curVignetteIntensity = targetVignetteIntensity;
            }
            
            vignette.intensity.value = curVignetteIntensity;
        }
        
        if (animLensDistortion)
        {
            lensDistortionChangeTimeElapsed += Time.deltaTime;
            float fac = lensDistortionChangeTimeElapsed / lensDistortionChangeDur;
            float lensDistortionIntensity = lensDistortionCurve.Evaluate(fac);
            
            lensDistortion.intensity.value = lensDistortionIntensity;

            if (fac >= 1.0f)
            {
                animLensDistortion = false;
            }
        }
    }

    #region Animation Triggers

    public void AnimateTimeSlowDown()
    {
        targetChromaticAbIntensity = maxChromaticAbIntensity;
        targetVignetteIntensity = maxVignetteIntensity;
        animLensDistortion = false;
    }

    public void AnimateTimeBackToNormal()
    {
        targetChromaticAbIntensity = minChromaticAbIntensity;
        targetVignetteIntensity = minVignetteIntensity;
        
        lensDistortionChangeTimeElapsed = 0.0f;
        lensDistortion.intensity.value = 0.0f;
        animLensDistortion = true;
    }
    
    private void SetNormalValues()
    {
        targetChromaticAbIntensity = minChromaticAbIntensity;
        targetVignetteIntensity = minVignetteIntensity;
        
        lensDistortionChangeTimeElapsed = 0.0f;
        lensDistortion.intensity.value = 0.0f;
        animLensDistortion = false;
    }
    
    #endregion
}
