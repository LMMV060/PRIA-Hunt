using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource music;
    public GameObject panel;

    void Update()
    {
        if (panel.activeSelf && music.isPlaying)
        {
            music.Pause();
        }
        else if (!panel.activeSelf && !music.isPlaying)
        {
            music.UnPause();
        }
    }
}