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
        FrequencyNotification();
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
        var channel = new AndroidNotificationChannel()
        {
            Id = "Start Game Notification",
            Name = "Welcome back",
            Importance = Importance.Default,
            Description = "Let's kill zombies",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        var notification = new AndroidNotification();
        notification.Title = "Welcome back";
        notification.Text = "Let's kill zombies";
        notification.LargeIcon = "icon_1";
        notification.SmallIcon = "icon_0";
        notification.FireTime = System.DateTime.Now;

        AndroidNotificationCenter.SendNotification(notification, channel.Id);
        Debug.Log("Notifaction Setup Success");
    }

    private void FrequencyNotification()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "Frequency Notification",
            Name = "Call player back",
            Importance = Importance.Default,
            Description = "Let's kill zombies",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        var notification = new AndroidNotification();
        notification.Title = "Help us";
        notification.Text = "Zombies are atacking us, please help, we can't hold it for long";
        notification.LargeIcon = "icon_1";
        notification.SmallIcon = "icon_0";
        notification.FireTime= System.DateTime.Now;
        notification.RepeatInterval = System.TimeSpan.FromMinutes(1);

        AndroidNotificationCenter.SendNotification(notification, channel.Id);
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

        var notification = new AndroidNotification();
        notification.Title = "Goodbye";
        notification.Text = "Bye player, we hope you back soon";
        notification.LargeIcon = "icon_1";
        notification.SmallIcon = "icon_0";
        notification.FireTime = System.DateTime.Now;

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        AndroidNotificationCenter.SendNotification(notification, channel.Id);
    }

    private void OnApplicationQuit()
    {
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        QuitGameNotification();
    }
}
