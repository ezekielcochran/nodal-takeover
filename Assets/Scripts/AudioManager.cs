using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("-----------Audio Source-----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("-----------Audio Clip-----------")]
    public AudioClip enemyCapture;
    public AudioClip playerCapture;
    public AudioClip clickNode;
    public AudioClip initiateAttack;
    public AudioClip win;
    public AudioClip lose;
    public AudioClip backgroundMusic;
    public AudioClip menuMusic;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        musicSource.clip = menuMusic;
        musicSource.Play(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playMusic(AudioClip clip) {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void playSFX(AudioClip clip) {
        SFXSource.PlayOneShot(clip);
    }
}
