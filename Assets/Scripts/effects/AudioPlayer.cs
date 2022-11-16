using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class AudioPlayer : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private static AudioSource _audioSourceFieldHover;
    private static AudioSource _audioSourceButtonClick;
    private static AudioSource _audioSourceAttack;
    private static AudioSource _audioSourceRegroup;
    private static AudioSource _audioSourceBuyUnit;
    private static AudioSource _audioSourceNotification;

    public static void PlayFieldHover()
    {
        _audioSourceFieldHover.Play();
    }

    public static void PlayButtonClick()
    {
        _audioSourceButtonClick.Play();
    }

    public static void PlayYourTurn()
    {
        NotificationsBarManager.EnqueueNotification("Your turn!");
    }

    public static void PlayAttack()
    {
        _audioSourceAttack.Play();
        var eventData = new PlaySound("attack");
        PhotonNetwork.RaiseEvent(eventData.GetEventType(), eventData.Serialize(),
            new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
    }

    public static void PlayRegroup()
    {
        _audioSourceRegroup.Play();
        var eventData = new PlaySound("regroup");
        PhotonNetwork.RaiseEvent(eventData.GetEventType(), eventData.Serialize(),
            new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
    }

    public static void PlayBuyUnit()
    {
        _audioSourceBuyUnit.Play();
    }

    public static void PlayNotification()
    {
        _audioSourceNotification.Play();
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code != (int)EventTypes.PlaySound) return;
        switch (PlaySound.Deserialize(photonEvent.CustomData as object[]).SoundName)
        {
            case "attack":
                PlayAttack();
                break;
            case "regroup":
                PlayRegroup();
                break;
        }
    }

    private void Start()
    {
        _audioSourceFieldHover = gameObject.AddComponent<AudioSource>();
        _audioSourceFieldHover.clip = Resources.Load("fieldHover") as AudioClip;
        
        _audioSourceButtonClick = gameObject.AddComponent<AudioSource>();
        _audioSourceButtonClick.clip = Resources.Load("buttonClick") as AudioClip;
        
        _audioSourceAttack = gameObject.AddComponent<AudioSource>();
        _audioSourceAttack.clip = Resources.Load("attack") as AudioClip;
        _audioSourceAttack.volume = 0.5f;
        
        _audioSourceRegroup = gameObject.AddComponent<AudioSource>();
        _audioSourceRegroup.clip = Resources.Load("regroup") as AudioClip;
        _audioSourceRegroup.volume = 0.5f;
        
        _audioSourceBuyUnit = gameObject.AddComponent<AudioSource>();
        _audioSourceBuyUnit.clip = Resources.Load("buyUnit") as AudioClip;
        
        _audioSourceNotification = gameObject.AddComponent<AudioSource>();
        _audioSourceNotification.clip = Resources.Load("notification") as AudioClip;
        _audioSourceNotification.volume = 0.25f;
    }
}
