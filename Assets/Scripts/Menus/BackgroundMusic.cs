using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioSource bgmSource;
    public AudioClip menuMusic;

    void Start()
    {
        if (bgmSource != null && menuMusic != null)
        {
            bgmSource.clip = menuMusic;
            
            bgmSource.loop = true; 
            
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning("Missing AudioSource or AudioClip in BackgroundMusic script!");
        }
    }
}