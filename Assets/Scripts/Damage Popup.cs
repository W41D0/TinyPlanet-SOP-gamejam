using UnityEngine;
using TMPro; 

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private float floatSpeed = 3f;
    [SerializeField] private float disappearTimerMax = 0.5f;
    
    [Header("Colors")]
    [SerializeField] private Color textColor = Color.red;
    [SerializeField] private Color critColor = Color.red;

    private float disappearTimer;
    private Color fadeColor;
    private Vector3 moveVector;

    public void Setup(float damageAmount, bool isCrit = false)
    {
        ApplyColor(damageAmount, isCrit ? critColor : textColor);
    }

    private void ApplyColor(float damageAmount, Color finalColor)
    {
        textMesh.SetText(damageAmount.ToString("F0")); 
        
        textMesh.color = finalColor;
        fadeColor = finalColor;
        disappearTimer = disappearTimerMax;
        
        moveVector = new Vector3(Random.Range(-0.7f, 0.7f), 1f, 0f).normalized * floatSpeed;
    }

    void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 3f * Time.deltaTime; 

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float fadeAmount = 5f;
            fadeColor.a -= fadeAmount * Time.deltaTime;
            textMesh.color = fadeColor;
            
            if (fadeColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
