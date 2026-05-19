using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;
using UnityEngine.Serialization;

public class DayNightCycle : MonoBehaviour
{
    #region Variables
    [Header("References")]
    [SerializeField] private Light2D globalLight;
    //Curve And Gradient
    private AnimationCurve dayNightCurve;
    private Gradient lightColorGradient;
    
    [Header("Time Settings")]
    [SerializeField, Range(1f, 1200f)] private float cycleDuration = 60f; //duration in seconds
    [SerializeField] private float timePassed;      //start time
    
    [Header("Light Settings")]
    [SerializeField] private float nightLightIntensity = 0.05f;
    [SerializeField] private float dayLightIntensity = 1f;

    [Header("Light Gradient Colors")]
    [SerializeField] private Color32 nightColor = new Color32(10, 16, 32, 255);
    [SerializeField] private Color32 dayColor = new Color32(255, 255, 255, 255);
    [SerializeField] private Color32 sunsetColor = new Color32(255, 229, 191, 255);
    
    [Header("Custom Day settings")]
    [SerializeField, Range(0, 24f)] private float sunriseStart = 6f;
    [SerializeField, Range(0, 24f)] private float sunriseEnd = 7f;
    [SerializeField, Range(0, 24f)] private float noon = 12f;
    [SerializeField, Range(0, 24f)] private float sunsetStart = 18f;
    [SerializeField, Range(0, 24f)] private float sunsetEnd = 19f;
    [SerializeField, Range(0, 24f)] private float midNight = 1f;
    private float sunriseStartPercent;
    private float sunriseEndPercent;
    private float noonPercent;
    private float sunsetStartPercent;
    private float sunsetEndPercent;
    private float midNightPercent;

    public bool IsNight {get; private set;}
    public float DayPercent => timePassed / cycleDuration; //returns value between 0 and 1 
    public float CurrentTimePassed => timePassed;
    public int CurrentHour => Mathf.FloorToInt(DayPercent * 24f);

    [SerializeField] private bool useDefaultStartTime = true;
    [SerializeField, Range(0f, 24f)] private float startHour = 6f;
    
    public event Action OnTimeOfDayChanged; //event for sunrise/sunset time changes
    #endregion

    private void Start() //Calculate sunrise/noon/sunset values and apply them to light Gradient and AnimationCurve
    {
        if(useDefaultStartTime)
        {
            timePassed = cycleDuration * startHour / 24f;
        }
        SetNormalizedTimeValues();
        BuildDayNightCurve();
        BuildDayNightGradient();
    }
    private void Update()
    {
        UpdateTime();

        float dp = DayPercent;
        SetNightBool(dp);
        ApplyLighting(dp);
    }
    private void UpdateTime()
    {
        timePassed += Time.deltaTime;
        if (timePassed >= cycleDuration) timePassed = 0f; //reset timer if time > cycle duration
    }
    private void ApplyLighting(float dayPercent)
    {
        float lightIntensityValue = dayNightCurve.Evaluate(dayPercent);
        globalLight.intensity = lightIntensityValue;  //evaluate value in animation curve and assign to light intensity
        
        Color lightColorValue = lightColorGradient.Evaluate(dayPercent);
        globalLight.color = lightColorValue;  //evaluate dayPercent value in color gradient and apply to light color
    }
    private void SetNightBool(float dayPercent)
    {
        float sunsetP = (sunsetStart + 1f) / 24f;
        float sunriseP = (sunriseStart + 1f) / 24f;
        
        bool previousState = IsNight;
        IsNight = dayPercent >= sunsetP || dayPercent <= sunriseP; //its night if after sunset or before sunrise
        if(previousState != IsNight)
            OnTimeOfDayChanged?.Invoke();
    }
    #region Helpers
    private void SetNormalizedTimeValues()
    {
        sunriseStartPercent = sunriseStart / 24f;
        sunriseEndPercent = sunriseEnd / 24f;
        noonPercent = noon / 24f;
        sunsetStartPercent = sunsetStart / 24f;
        sunsetEndPercent = sunsetEnd / 24f;
        midNightPercent = midNight / 24f;
    }
    private void BuildDayNightCurve()
    {
        dayNightCurve = new AnimationCurve(
        new Keyframe(midNightPercent, nightLightIntensity),
        new Keyframe(sunriseStartPercent, nightLightIntensity),
        new Keyframe(sunriseEndPercent, dayLightIntensity),
        new Keyframe(sunsetStartPercent, dayLightIntensity),
        new Keyframe(sunsetEndPercent, nightLightIntensity));
    }
    private void BuildDayNightGradient()
    {
        GradientColorKey[] colorKeys = new GradientColorKey[]
        {
            new GradientColorKey(nightColor, midNightPercent), // midnight (dark blue)
            new GradientColorKey(nightColor, sunriseStartPercent), // sunrise start
            new GradientColorKey(sunsetColor, sunriseEndPercent), //sunrise end (warm)
            new GradientColorKey(dayColor, noonPercent), // noon
            new GradientColorKey(sunsetColor, sunsetStartPercent), // sunset
            new GradientColorKey(nightColor, sunsetEndPercent), //sunset end (dark)
        };
        GradientAlphaKey[] alphaKeys =
        {
            new GradientAlphaKey(1, 0f),
            new GradientAlphaKey(1, 1f),
        };
        lightColorGradient = new Gradient();
        lightColorGradient.SetKeys(colorKeys, alphaKeys);
    }
    #endregion
    #region public methods
    public string GetFormattedTime()
    {
        int hours = Mathf.FloorToInt(DayPercent * 24f);
        int minutes = Mathf.FloorToInt((DayPercent * 24f - hours) * 60f);

        return $"{hours:00}:{minutes:00}";
    }
    public void SetTimePassed(float timePassed)
    {
        this.timePassed = Mathf.Clamp(timePassed, 0, cycleDuration);
    }
    #endregion
}