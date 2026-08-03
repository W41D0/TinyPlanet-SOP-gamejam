using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class ProceduralPhaseMeter : Graphic
{
    [Header("Phase Colors")]
    [SerializeField] private Color solidColor = Color.cyan;
    [SerializeField] private Color liquidColor = Color.blue;
    [SerializeField] private Color gasColor = Color.white;
    [SerializeField] private Color plasmaColor = new Color(0.6f, 0f, 1f, 1f); // Glowing Purple

    [Header("Glow / Pulse Settings")]
    [SerializeField] private float pulseSpeed = 5f;

    private float fillPercentage = 0f;
    private Color activeColor;

    /// <summary>
    /// Updates the bar dynamically based on current phase and values.
    /// </summary>
    public void UpdateMeter(float currentTotal, float maxSolid, float maxLiquid, float maxGas, float currentPlasma, float maxPlasma, bool isPlasma)
    {
        if (isPlasma)
        {
            // Plasma State: Starts FULL (1.0) and drains down to 0 over maxPlasmaMeter duration
            float plasmaRemaining = maxPlasma - currentPlasma;
            fillPercentage = Mathf.Clamp01(plasmaRemaining / maxPlasma);

            // Pulse glowing purple
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
            activeColor = Color.Lerp(plasmaColor, Color.white, pulse * 0.4f);
        }
        else if (currentTotal > maxSolid + maxLiquid)
        {
            // Gas Phase (fills from 0 to maxGas)
            float currentGas = currentTotal - (maxSolid + maxLiquid);
            fillPercentage = Mathf.Clamp01(currentGas / maxGas);
            activeColor = gasColor;
        }
        else if (currentTotal > maxSolid)
        {
            // Liquid Phase (fills from 0 to maxLiquid)
            float currentLiquid = currentTotal - maxSolid;
            fillPercentage = Mathf.Clamp01(currentLiquid / maxLiquid);
            activeColor = liquidColor;
        }
        else
        {
            // Solid Phase (fills from 0 to maxSolid)
            fillPercentage = Mathf.Clamp01(currentTotal / maxSolid);
            activeColor = solidColor;
        }

        // Trigger mesh redraw
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect r = GetPixelAdjustedRect();
        float currentWidth = r.width * fillPercentage;

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = activeColor;

        // Bottom-Left
        vertex.position = new Vector3(r.xMin, r.yMin);
        vh.AddVert(vertex);

        // Top-Left
        vertex.position = new Vector3(r.xMin, r.yMax);
        vh.AddVert(vertex);

        // Top-Right
        vertex.position = new Vector3(r.xMin + currentWidth, r.yMax);
        vh.AddVert(vertex);

        // Bottom-Right
        vertex.position = new Vector3(r.xMin + currentWidth, r.yMin);
        vh.AddVert(vertex);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }
}