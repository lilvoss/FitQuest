using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Android;

public class mobilenotifs : MonoBehaviour
{

    

    // Start is called before the first frame update
    void Start()
    {

     



        //Remove notifications that have already been displayed
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }


        //Create the android Notification Chanel to send messages through
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Notifications Channel",
            Importance = Importance.Default,
            Description = "Reminder notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);


        //Create the notification that is going to be sent
        var notification = new AndroidNotification();
        notification.Title = "Hey! time for training ";
        notification.Text = "Do Some Workouts";
        notification.FireTime = System.DateTime.Now.AddSeconds(15);


        //Send the notification
        var id = AndroidNotificationCenter.SendNotification(notification, "channel_id");
        //if the script is run and msg is already scheduled, cancel it and re-schedule another msg
        if(AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Scheduled) 
        {
            AndroidNotificationCenter.CancelAllNotifications();
            AndroidNotificationCenter.SendNotification(notification, "channel_id");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
