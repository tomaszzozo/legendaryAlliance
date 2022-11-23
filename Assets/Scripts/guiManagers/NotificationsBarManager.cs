using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationsBarManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private static readonly Queue<string> Queue = new();
    private static bool _showingNotification;
    private static bool _skip;
    [SerializeField] private float backgroundOpacity;
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI notificationTextLabel;
    [SerializeField] private Image background;
    [SerializeField] private Image notificationCountBackground;
    [SerializeField] private TextMeshProUGUI notificationCountLabel;

    private void Start()
    {
        var color = Players.PlayersList.Find(player => player.Name == SharedVariables.GetUsername()).Color;
        background.color = new Color(color.r, color.g, color.b, backgroundOpacity);
        canvas.enabled = false;
    }

    private void Update()
    {
        if (Queue.Count == 0) return;
        if (_showingNotification) return;
        _showingNotification = true;
        StartCoroutine(DisplayNotification(Queue.Count, Queue.Dequeue()));
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code != (int)EventTypes.SendNotificationEvent) return;
        var message = SendNotificationEvent.Deserialize(photonEvent.CustomData as object[]).Message;
        Queue.Enqueue(message);
    }

    public static void SkipNotification()
    {
        _skip = true;
    }

    /// <summary>
    ///     Raises <see cref="SendNotificationEvent" /> that queues notification for all other clients
    /// </summary>
    /// <param name="message">message to display in notification</param>
    public static void SendNotification(string message)
    {
        PhotonNetwork.RaiseEvent(
            (byte)EventTypes.SendNotificationEvent,
            new SendNotificationEvent(message).Serialize(),
            new RaiseEventOptions { Receivers = ReceiverGroup.Others },
            SendOptions.SendReliable);
    }

    /// <summary>
    ///     Enqueues a notification that only the user will see, for example "your turn" notification.
    /// </summary>
    /// <param name="message">Notification to queue</param>
    public static void EnqueueNotification(string message)
    {
        Queue.Enqueue(message);
    }

    private IEnumerator DisplayNotification(int notificationsLeft, string message)
    {
        AudioPlayer.PlayNotification();
        notificationCountBackground.enabled = notificationsLeft > 1;
        notificationCountLabel.enabled = notificationsLeft > 1;
        notificationCountLabel.text = notificationsLeft > 99 ? "99+" : notificationsLeft.ToString();
        notificationTextLabel.text = message;

        notificationCountBackground.canvasRenderer.SetAlpha(1);
        notificationCountLabel.canvasRenderer.SetAlpha(1);
        notificationTextLabel.canvasRenderer.SetAlpha(1);
        background.canvasRenderer.SetAlpha(1);
        canvas.enabled = true;

        for (float i = 0; i < 1; i += 0.01f)
        {
            if (_skip) break;
            yield return new WaitForSeconds(0.01f);
        }

        for (var alpha = background.canvasRenderer.GetAlpha(); alpha >= 0; alpha -= 0.01f)
        {
            if (_skip) break;
            notificationCountBackground.canvasRenderer.SetAlpha(alpha);
            notificationCountLabel.canvasRenderer.SetAlpha(alpha);
            notificationTextLabel.canvasRenderer.SetAlpha(alpha);
            background.canvasRenderer.SetAlpha(alpha);
            yield return new WaitForSeconds(0.01f);
        }

        _skip = false;
        canvas.enabled = false;
        _showingNotification = false;
    }
}