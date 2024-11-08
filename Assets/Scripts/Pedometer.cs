using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Notifications.Android;
using UnityEngine.Android; // Add this to use Android notifications

public class Pedometer : MonoBehaviour
{
    [Header("UI")]
    public Text stepsText;
    public Text walkingText;
    public Text TimeElapsedWalkingText;
    public Text TimeElapsedStandingStillText;
    public Text distanceText;

    [Header("Pedometer")]
    public float lowLimit = 0.005F;
    public float highLimit = 0.1F;
    private bool stateHigh = false;

    public float filterHigh = 10.0F;
    public float filterLow = 0.1F;
    public float currentAcceleration = 0F;
    float averageAcceleration = 0F;

    public int steps = 0;
    private int oldSteps;
    public float waitCounter = 0F;
    public float timeElapsedWalking = 0F;
    public float timeElapsedStandingStill = 0F;
    public bool isWalking = false;
    private bool startWaitCounter = false;
    public float stepLength = 1.5F;

    private int stepsUntilNotification = 10; // Number of steps until next notification

    private void Start()
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
        notification.SmallIcon = "dumbells";  
        notification.LargeIcon = "dumbells";  


        //Send the notification
        var id = AndroidNotificationCenter.SendNotification(notification, "channel_id");
        //if the script is run and msg is already scheduled, cancel it and re-schedule another msg
        if (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Scheduled)
        {
            AndroidNotificationCenter.CancelAllNotifications();
            AndroidNotificationCenter.SendNotification(notification, "channel_id");
        }
    }
    void Awake()
    {
        averageAcceleration = Input.acceleration.magnitude;
        oldSteps = steps;
        SaveDistance();

        // Create Android notification channel
        CreateNotificationChannel();
    }

    void Update()
    {
        UpdateElapsedWalkingTime();
        WalkingCheck();
        UpdateDistance();

        if (isWalking)
        {
            walkingText.text = ("Good job! You're walking!");
        }
        else
        {
            walkingText.text = ("You are not walking!");
        }
    }

    void SaveDistance()
    {
        PlayerPrefs.SetFloat("TotalDistance", steps * stepLength);
        PlayerPrefs.SetFloat("StepLength", stepLength);
    }

    void UpdateDistance()
    {
        float distance = steps * stepLength;
        distanceText.text = "Distance: " + (distance / 1000).ToString("F2") + " km";
    }

    void FixedUpdate()
    {
        currentAcceleration = Mathf.Lerp(currentAcceleration, Input.acceleration.magnitude, Time.deltaTime * filterHigh);
        averageAcceleration = Mathf.Lerp(averageAcceleration, Input.acceleration.magnitude, Time.deltaTime * filterLow);

        float delta = currentAcceleration - averageAcceleration;

        if (!stateHigh)
        {
            if (delta > highLimit)
            {
                stateHigh = true;
                steps++;
                stepsText.text = "Steps: " + steps;

                // Check if steps reached a multiple of 10 to send notification
                if (steps % stepsUntilNotification == 0)
                {
                    SendStepNotification(steps);
                }
            }
        }
        else
        {
            if (delta < lowLimit)
            {
                stateHigh = false;
            }
        }
    }

    private void WalkingCheck()
    {
        if (steps != oldSteps)
        {
            startWaitCounter = true;
            waitCounter = 0F;
        }

        if (startWaitCounter)
        {
            waitCounter += Time.deltaTime;

            if (waitCounter != 0)
            {
                isWalking = true;
            }
            if (waitCounter > 2.5)
            {
                waitCounter = 0F;
                startWaitCounter = false;
            }
        }
        else if (!startWaitCounter)
        {
            isWalking = false;
        }
        oldSteps = steps;
    }

    private void UpdateElapsedWalkingTime()
    {
        int secondsWalk = (int)(timeElapsedWalking % 60);
        int minutesWalk = (int)(timeElapsedWalking / 60) % 60;
        int hourWalk = (int)(timeElapsedWalking / 3600) % 24;

        int secondsStill = (int)(timeElapsedStandingStill % 60);
        int minutesStill = (int)(timeElapsedStandingStill / 60) % 60;
        int hoursStill = (int)(timeElapsedStandingStill / 3600) % 24;

        string timeElapsedWalkingString = string.Format("{0:0}:{1:00}:{2:00}", hourWalk, minutesWalk, secondsWalk);
        string timeElapsedStandingStillString = string.Format("{0:0}:{1:00}:{2:00}", hoursStill, minutesStill, secondsStill);

        TimeElapsedWalkingText.text = "Time spent walking: " + timeElapsedWalkingString;
        TimeElapsedStandingStillText.text = "Time spent standing still: " + timeElapsedStandingStillString;

        if (isWalking)
        {
            timeElapsedWalking += Time.deltaTime;
        }
        else
        {
            timeElapsedStandingStill += Time.deltaTime;
        }
    }

    public void ResetPedometer()
    {
        steps = 0;
        timeElapsedWalking = 0f;
        timeElapsedStandingStill = 0f;
        waitCounter = 0f;
        isWalking = false;

        // Update UI elements
        stepsText.text = "Steps: 0";
        TimeElapsedWalkingText.text = "Time spent walking: 00:00:00";
        TimeElapsedStandingStillText.text = "Time spent standing still: 00:00:00";
        distanceText.text = "Distance: 0 km";
    }

    void CreateNotificationChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "step_channel",
            Name = "Step Notifications",
            Importance = Importance.Default,
            Description = "Notifies the user every 10 steps",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    void SendStepNotification(int steps)
    {
        var notification = new AndroidNotification
        {
            Title = "Step Milestone Reached!",
            Text = $"You've taken {steps} steps. Keep going!",
            FireTime = System.DateTime.Now
        };

        AndroidNotificationCenter.SendNotification(notification, "step_channel");
        Debug.Log("Notification sent for " + steps + " steps");
    }
}
