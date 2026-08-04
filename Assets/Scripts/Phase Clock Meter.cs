using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class PhaseClockMeter : Graphic
{
    [Header("Clock Background")]
    [Tooltip("The color of the clock face when no time has elapsed (time is 'used up'). Only applies during Plasma state.")]
    [SerializeField] private Color depletedBackground = new Color(0.15f, 0.15f, 0.15f, 1f); // Dark Grey

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

    // The current logical fill angle (0 to 360) corresponding to progress
    private float currentFillAngle;

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

        UpdateNeedleAndTextAndCalculateFill();
        
        // Trigger the procedural mesh to redraw the clock face
        SetVerticesDirty(); 
    }

    private void UpdateNeedleAndTextAndCalculateFill()
    {
        float totalStandardMax = maxSolid + maxLiquid + maxGas;
        float needleAngle = 0f;
        float timeLeft = 0f;

        if (currentState == "p") // Plasma State
        {
            // The GunScript depletes PlasmaMeter over maxPlasma duration.
            float plasmaPercentRemaining = Mathf.Clamp01((maxPlasma - currentPlasma) / maxPlasma);

            // Hand moves backwards from 360 down to 0
            needleAngle = Mathf.Lerp(360f, 0f, 1f - plasmaPercentRemaining); 
            
            timeLeft = maxPlasma - currentPlasma;
            
            // The visual fill for Plasma is tied to the needle
            currentFillAngle = needleAngle; 
        }
        else // Normal States (S, L, G or cooldown)
        {
            if (totalStandardMax <= 0) return; 

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

            // We track this for the needle, but we won't restrict the mesh generation below
            currentFillAngle = needleAngle;
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

        if (currentState == "p")
        {
            // PLASMA MODE: Draw the grey background, then draw Plasma color up to the needle
            DrawPieSlicePass(vh, center, radius, 360f, true);
            DrawPieSlicePass(vh, center, radius, currentFillAngle, false);
        }
        else
        {
            // NORMAL MODE: Always draw the full 360 degree painted pie chart (no greying out)
            DrawPieSlicePass(vh, center, radius, 360f, false);
        }
    }

    private void DrawPieSlicePass(VertexHelper vh, Vector2 center, float radius, float totalDrawAngleDeg, bool isBackgroundLayer)
    {
        float totalStandardMax = maxSolid + maxLiquid + maxGas;
        float solidEndAngle = 0;
        float liquidEndAngle = 0;

        if (totalStandardMax > 0)
        {
            solidEndAngle = (maxSolid / totalStandardMax) * 360f;
            liquidEndAngle = solidEndAngle + ((maxLiquid / totalStandardMax) * 360f);
        }

        float angleStep = 360f / circleResolution;

        for (int i = 0; i < circleResolution; i++)
        {
            float startAngleDeg = i * angleStep;
            float endAngleDeg = (i + 1) * angleStep;

            // Don't draw vertices past the fill amount for this layer
            if (startAngleDeg > totalDrawAngleDeg) break;

            Color segmentColor;

            // Determine what color to paint this specific triangle
            if (isBackgroundLayer)
            {
                segmentColor = depletedBackground;
            }
            else
            {
                if (currentState == "p") 
                {
                    segmentColor = plasmaColor;
                }
                else 
                {
                    // Draw standard proportional states
                    if (startAngleDeg < solidEndAngle) segmentColor = solidColor;
                    else if (startAngleDeg < liquidEndAngle) segmentColor = liquidColor;
                    else segmentColor = gasColor;
                }
            }

            // Correct for a very small mismatch at the final vertex to keep the line sharp with the needle
            float drawAngleForTriangle = Mathf.Min(endAngleDeg, totalDrawAngleDeg);

            DrawPieTriangle(vh, center, radius, startAngleDeg, drawAngleForTriangle, segmentColor);
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