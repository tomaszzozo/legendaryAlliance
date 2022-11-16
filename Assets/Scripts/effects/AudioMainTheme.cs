using System.Collections;
using UnityEngine;

public class AudioMainTheme : MonoBehaviour
{
    public static AudioMainTheme Instance { get; private set; }
    [SerializeField] private AudioClip mainTheme;
    [SerializeField] private float volume;
    private const float SecondsToFadeOut = 5;
    private AudioSource _audioSource;
    
    public void Stop()
    {
        StartCoroutine (Instance.FadeOut());
    }
    
    private void Play()
    {
        if (_audioSource.isPlaying) return;
        _audioSource.Play();
    }
    
    private IEnumerator FadeOut() {
        var startVolume = _audioSource.volume;
        while (_audioSource.volume > 0) {
            _audioSource.volume -= startVolume * Time.deltaTime / SecondsToFadeOut;
 
            yield return null;
        }
        _audioSource.Stop();
        _audioSource.volume = startVolume;
    }
    
    private void Start()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.clip = mainTheme;
        _audioSource.volume = volume;
        _audioSource.loop = true;
        _audioSource.Play();
    }
    
    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Instance.Play();
            Debug.Log("An instance of main theme already exists, destroying this object...");
            Destroy(gameObject); 
        } 
        else 
        { 
            Instance = this; 
        } 
        DontDestroyOnLoad(transform.gameObject);
    }
}
