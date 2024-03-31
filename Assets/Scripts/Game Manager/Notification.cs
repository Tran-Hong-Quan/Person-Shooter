using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Android;

public class Notification : MonoBehaviour
{
    void Start()
    {
        PushStartNotification();
    }

    private void RequestPermission()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
    }

    private void PushStartNotification()
    {
        RequestPermission();
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        var channel = new AndroidNotificationChannel()
        {
            Id = "Start Game Notification",
            Name = "Welcome back",
            Importance = Importance.Default,
            Description = "Let's kill zombies",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    private void FrequencyNotification()
    {
        var notification = new AndroidNotification();
        notification.Title = "Help us";
        notification.Text = "Zombies are atacking us, please help, we can't hold it for long";
        notification.FireTime = System.DateTime.Now.AddHours(1);

        var id = AndroidNotificationCenter.SendNotification(notification, "Frequency Notification");
        if (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Scheduled)
        {
            AndroidNotificationCenter.CancelAllScheduledNotifications();
            AndroidNotificationCenter.SendNotification(notification, "Frequency Notification");
        }
    }

    private void QuitGameNotification()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "Quit Game Notification",
            Name = "Goodbye",
            Importance = Importance.Default,
            Description = "Bye player, we hope you back soon",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    private void OnApplicationQuit()
    {
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        FrequencyNotification();
        QuitGameNotification();
    }

}
