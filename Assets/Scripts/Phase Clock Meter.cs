using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class PhaseClockMeter : Graphic
{
    [Header("Phase Colors")]
    [SerializeField] private Color solidColor = new Color(0.6f, 0.3f, 0.2f, 1f); // Earthy red/brown
    [SerializeField] private Color liquidColor = new Color(0.2f, 0.3f, 0.5f, 1f); // Deep blue
    [SerializeField] private Color gasColor = new Color(0.9f, 0.8f, 0.3f, 1f); // Yellowish
    [SerializeField] private Color plasmaColor = new Color(0.6f, 0f, 1f, 1f); // Glowing Purple

    [Header("UI References")]
    [Tooltip("Assign the RectTransform of the Needle image here.")]
    [SerializeField] private RectTransform clockNeedle;
    [Tooltip("Assign a Text component to display the time remaining.")]
    [SerializeField] private TextMeshProUGUI timeLeftText;

    [Header("Clock Settings")]
    [Range(12, 120)]
    [Tooltip("How smooth the circle is. Higher is smoother.")]
    [SerializeField] private int circleResolution = 60;
    
    [SerializeField] private string timeFormat = "F1"; // "F1" means 1 decimal place (e.g., 2.5)

    // Cached values from the GunScript
    private float currentTotal;
    private float maxSolid, maxLiquid, maxGas, currentPlasma, maxPlasma;
    private string currentState;

    /// <summary>
    /// Updates the clock dynamically based on current phase and values.
    /// </summary>
    public void UpdateClock(float currentTotal, float maxSolid, float maxLiquid, float maxGas, float currentPlasma, float maxPlasma, string currentState)
    {
        this.currentTotal = currentTotal;
        this.maxSolid = maxSolid;
        this.maxLiquid = maxLiquid;
        this.maxGas = maxGas;
        this.currentPlasma = currentPlasma;
        this.maxPlasma = maxPlasma;
        this.currentState = currentState;

        UpdateNeedleAndText();
        
        // Trigger the procedural mesh to redraw the clock face
        SetVerticesDirty(); 
    }

    private void UpdateNeedleAndText()
    {
        float totalStandardMax = maxSolid + maxLiquid + maxGas;
        float needleAngle = 0f;
        float timeLeft = 0f;

        if (currentState == "p") // Plasma State
        {
            // Hand moves backwards from 360 down to 0
            float plasmaPercent = Mathf.Clamp01(currentPlasma / maxPlasma);
            needleAngle = Mathf.Lerp(360f, 0f, plasmaPercent);
            
            timeLeft = maxPlasma - currentPlasma;
        }
        else // Normal States
        {
            // Hand moves forward from 0 to 360
            float totalPercent = Mathf.Clamp01(currentTotal / totalStandardMax);
            needleAngle = Mathf.Lerp(0f, 360f, totalPercent);

            // Calculate time left based on the current specific state
            if (currentState == "s")
            {
                timeLeft = maxSolid - currentTotal;
            }
            else if (currentState == "l")
            {
                timeLeft = (maxSolid + maxLiquid) - currentTotal;
            }
            else if (currentState == "g")
            {
                timeLeft = totalStandardMax - currentTotal;
            }
        }

        // Apply Needle Rotation (Negative Z rotates clockwise in Unity UI)
        if (clockNeedle != null)
        {
            clockNeedle.localRotation = Quaternion.Euler(0f, 0f, -needleAngle);
        }

        // Apply Text
        if (timeLeftText != null)
        {
            timeLeftText.text = Mathf.Max(0f, timeLeft).ToString(timeFormat);
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect r = GetPixelAdjustedRect();
        float radius = Mathf.Min(r.width, r.height) / 2f;
        Vector2 center = r.center;

        float totalStandardMax = maxSolid + maxLiquid + maxGas;
        
        // Prevent division by zero errors on the first frame if max values are 0
        if (totalStandardMax <= 0) return; 

        // Calculate thresholds in degrees (0 to 360) for the pie slices
        float solidEndAngle = (maxSolid / totalStandardMax) * 360f;
        float liquidEndAngle = solidEndAngle + ((maxLiquid / totalStandardMax) * 360f);

        float angleStep = 360f / circleResolution;

        for (int i = 0; i < circleResolution; i++)
        {
            float startAngleDeg = i * angleStep;
            float endAngleDeg = (i + 1) * angleStep;

            Color segmentColor;

            // If in plasma, override the whole clock face
            if (currentState == "p")
            {
                segmentColor = plasmaColor;
            }
            else
            {
                // Determine slice color based on where this triangle starts
                if (startAngleDeg < solidEndAngle) segmentColor = solidColor;
                else if (startAngleDeg < liquidEndAngle) segmentColor = liquidColor;
                else segmentColor = gasColor;
            }

            DrawPieTriangle(vh, center, radius, startAngleDeg, endAngleDeg, segmentColor);
        }
    }

    private void DrawPieTriangle(VertexHelper vh, Vector2 center, float radius, float startAngleDeg, float endAngleDeg, Color color)
    {
        // Convert degrees to radians. 
        // We use Math.Sin for X and Math.Cos for Y so that 0 degrees is Top (12 o'clock) and rotates clockwise.
        float startRad = startAngleDeg * Mathf.Deg2Rad;
        float endRad = endAngleDeg * Mathf.Deg2Rad;

        Vector2 pos1 = center + new Vector2(Mathf.Sin(startRad), Mathf.Cos(startRad)) * radius;
        Vector2 pos2 = center + new Vector2(Mathf.Sin(endRad), Mathf.Cos(endRad)) * radius;

        int vertCount = vh.currentVertCount;

        // Add Center Vertex
        UIVertex vCenter = UIVertex.simpleVert;
        vCenter.position = center;
        vCenter.color = color;
        vh.AddVert(vCenter);

        // Add Start Vertex
        UIVertex vStart = UIVertex.simpleVert;
        vStart.position = pos1;
        vStart.color = color;
        vh.AddVert(vStart);

        // Add End Vertex
        UIVertex vEnd = UIVertex.simpleVert;
        vEnd.position = pos2;
        vEnd.color = color;
        vh.AddVert(vEnd);

        // Connect the triangle
        vh.AddTriangle(vertCount, vertCount + 1, vertCount + 2);
    }
}