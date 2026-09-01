using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle Instance;

    [Header("Referencias de Iluminación")]
    public Light sunLight;

    [Header("Duración del Ciclo")]
    [Tooltip("Duración en segundos reales de 1 día completo de 24 horas")]
    public float dayDurationInSeconds = 120f;

    [Tooltip("Hora inicial del día (0 a 24)")]
    [Range(0f, 24f)]
    public float startingHour = 8f; // Empieza a las 8:00 AM

    [Header("Gradientes de Luz")]
    public Gradient lightColorGradient;
    public AnimationCurve lightIntensityCurve;
    public Gradient ambientSkyGradient;

    [Header("Estado de Tiempo (Lectura)")]
    [SerializeField] [Range(0f, 1f)] private float timeOfDay = 0.33f;
    public int currentDay = 1;

    public float CurrentHour => timeOfDay * 24f;
    public bool IsNight => timeOfDay < 0.22f || timeOfDay > 0.78f;
    public string TimeString => $"Día {currentDay} - {Mathf.FloorToInt(CurrentHour):00}:{Mathf.FloorToInt((CurrentHour % 1f) * 60f):00}";

    private void Awake()
    {
        Instance = this;
        SetupDefaultGradients();
    }

    private void Start()
    {
        if (sunLight == null)
        {
            sunLight = RenderSettings.sun;
            if (sunLight == null)
            {
                sunLight = FindFirstObjectByType<Light>();
            }
        }

        timeOfDay = (startingHour % 24f) / 24f;
        UpdateLighting();
    }

    private void Update()
    {
        if (dayDurationInSeconds <= 0f) return;

        // Avanzar el tiempo
        float timeDelta = (Time.deltaTime / dayDurationInSeconds);
        timeOfDay += timeDelta;

        if (timeOfDay >= 1f)
        {
            timeOfDay -= 1f;
            currentDay++;
        }

        UpdateLighting();
    }

    private void UpdateLighting()
    {
        if (sunLight == null) return;

        // Rotación del Sol (0% = Medianoche, 25% = 6:00 Amanecer, 50% = 12:00 Mediodía, 75% = 18:00 Atardecer)
        float sunAngle = (timeOfDay * 360f) - 90f;
        sunLight.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

        // Color e Intensidad del Sol
        sunLight.color = lightColorGradient.Evaluate(timeOfDay);
        sunLight.intensity = lightIntensityCurve.Evaluate(timeOfDay);

        // Luz Ambiental
        RenderSettings.ambientSkyColor = ambientSkyGradient.Evaluate(timeOfDay);
    }

    private void SetupDefaultGradients()
    {
        if (lightColorGradient == null || lightColorGradient.colorKeys.Length == 0)
        {
            lightColorGradient = new Gradient();
            lightColorGradient.colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.18f, 0.22f, 0.38f), 0.00f), // Medianoche (Azul noche)
                new GradientColorKey(new Color(0.20f, 0.25f, 0.40f), 0.20f), // Noche tardía
                new GradientColorKey(new Color(1.00f, 0.60f, 0.30f), 0.25f), // Amanecer (Dorado cálido)
                new GradientColorKey(new Color(1.00f, 0.96f, 0.88f), 0.50f), // Mediodía (Luz brillante)
                new GradientColorKey(new Color(1.00f, 0.50f, 0.25f), 0.75f), // Atardecer (Ámbar)
                new GradientColorKey(new Color(0.25f, 0.18f, 0.35f), 0.80f), // Anochecer (Púrpura/Azul)
                new GradientColorKey(new Color(0.18f, 0.22f, 0.38f), 1.00f)  // Medianoche
            };
        }

        if (lightIntensityCurve == null || lightIntensityCurve.length == 0)
        {
            lightIntensityCurve = new AnimationCurve();
            lightIntensityCurve.AddKey(0.00f, 0.10f); // Medianoche
            lightIntensityCurve.AddKey(0.20f, 0.10f); // Antes del amanecer
            lightIntensityCurve.AddKey(0.28f, 0.70f); // Amanecer
            lightIntensityCurve.AddKey(0.50f, 1.25f); // Mediodía
            lightIntensityCurve.AddKey(0.72f, 0.70f); // Atardecer
            lightIntensityCurve.AddKey(0.80f, 0.10f); // Noche
            lightIntensityCurve.AddKey(1.00f, 0.10f); // Medianoche
        }

        if (ambientSkyGradient == null || ambientSkyGradient.colorKeys.Length == 0)
        {
            ambientSkyGradient = new Gradient();
            ambientSkyGradient.colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.05f, 0.07f, 0.12f), 0.00f), // Medianoche
                new GradientColorKey(new Color(0.25f, 0.20f, 0.18f), 0.25f), // Amanecer
                new GradientColorKey(new Color(0.28f, 0.32f, 0.38f), 0.50f), // Mediodía
                new GradientColorKey(new Color(0.25f, 0.18f, 0.18f), 0.75f), // Atardecer
                new GradientColorKey(new Color(0.05f, 0.07f, 0.12f), 1.00f)  // Medianoche
            };
        }
    }
}

