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
    [SerializeField] private float backgroundOpacity;
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI notificationTextLabel;
    [SerializeField] private Image background;
    [SerializeField] private Image notificationCountBackground;
    [SerializeField] private TextMeshProUGUI notificationCountLabel;

    private static readonly Queue<string> Queue = new();
    private static bool _showingNotification;
    
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (int)EventTypes.SendNotificationEvent)
        {
            Queue.Enqueue(SendNotificationEvent.Deserialize(photonEvent.CustomData as object[]).Message);
        }
    }

    /// <summary>
    /// Raises <see cref="SendNotificationEvent"/> that queues notification for all other clients
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

    private void Start()
    {
        var color = Players.PlayersList.Find(player => player.Name == SharedVariables.GetUsername()).Color;
        background.color = new Color(color.r, color.g, color.b, backgroundOpacity);
        canvas.enabled = false;
        notificationCountLabel.color = color;
    }

    private void Update()
    {
        if (Queue.Count == 0) return;
        if (_showingNotification) return;
        _showingNotification = true;
        StartCoroutine(DisplayNotification(Queue.Count, Queue.Dequeue()));
    }

    private IEnumerator DisplayNotification(int notificationsLeft, string message)
    {
        notificationCountBackground.enabled = notificationsLeft > 1;
        notificationCountLabel.enabled = notificationsLeft > 1;
        notificationCountLabel.text = notificationsLeft.ToString();
        notificationTextLabel.text = message;
        
        notificationTextLabel.canvasRenderer.SetAlpha(0);
        background.canvasRenderer.SetAlpha(0);
        notificationCountBackground.canvasRenderer.SetAlpha(0);
        notificationCountLabel.canvasRenderer.SetAlpha(0);
        
        canvas.enabled = true;

        for (float alpha = 0; alpha < 1; alpha += 0.1f)
        {
            notificationCountBackground.canvasRenderer.SetAlpha(alpha);
            notificationCountLabel.canvasRenderer.SetAlpha(alpha);
            notificationTextLabel.canvasRenderer.SetAlpha(alpha);
            background.canvasRenderer.SetAlpha(alpha);
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(4);
        
        for (float alpha = 1; alpha > 0; alpha -= 0.01f)
        {
            notificationCountBackground.canvasRenderer.SetAlpha(alpha);
            notificationCountLabel.canvasRenderer.SetAlpha(alpha);
            notificationTextLabel.canvasRenderer.SetAlpha(alpha);
            background.canvasRenderer.SetAlpha(alpha);
            yield return new WaitForSeconds(0.01f);
        }
        
        canvas.enabled = false;
        _showingNotification = false;
    }
}
