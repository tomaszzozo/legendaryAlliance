using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    private static AudioSource _audioSourceFieldHover;
    private static AudioSource _audioSourceButtonClick;
    private static AudioSource _audioSourceYourTurn;

    public static void PlayFieldHover() { _audioSourceFieldHover.Play(); }
    public static void PlayButtonClick() { _audioSourceButtonClick.Play(); }
    public static void PlayYourTurn() { _audioSourceYourTurn.Play(); }

    private void Start()
    {
        _audioSourceFieldHover = gameObject.AddComponent<AudioSource>();
        _audioSourceFieldHover.clip = Resources.Load("fieldHover") as AudioClip;
        
        _audioSourceButtonClick = gameObject.AddComponent<AudioSource>();
        _audioSourceButtonClick.clip = Resources.Load("buttonClick") as AudioClip;
        
        _audioSourceYourTurn = gameObject.AddComponent<AudioSource>();
        _audioSourceYourTurn.clip = Resources.Load("yourTurn") as AudioClip;
    }
}
