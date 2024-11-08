using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Android; // Required for runtime permission handling

public class GPSController : MonoBehaviour
{
    public Text statusText;
    private string apiUrl = "https://ipinfo.io/json";

    [System.Serializable]
    public class IPInfo
    {
        public string loc;
    }

    void Start()
    {
        if (Application.isEditor)
        {
            // If running in the Unity Editor, fetch simulated location
            StartCoroutine(FetchRealLocation());
        }
        else
        {
            // On mobile devices
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                StartCoroutine(StartLocationService());
            }
            else
            {
                // On PC or other non-mobile devices
                StartCoroutine(FetchRealLocation());
            }
        }
    }

    IEnumerator StartLocationService()
    {
        // Check if the user has granted permission for location services
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            // Request the location permission
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.FineLocation));
        }

        // Check if location services are enabled by the user
        if (!Input.location.isEnabledByUser)
        {
            statusText.text = "Location services are not enabled.";
            yield break;
        }

        // Start the location service
        Input.location.Start();

        int maxWait = 60; // Increase timeout for better location initialization
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            statusText.text = "Initializing location services...";
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait <= 0)
        {
            statusText.text = "Location services initialization timed out.";
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            statusText.text = "Unable to determine device location.";
            yield break;
        }
        else
        {
            // Successfully retrieved location data
            statusText.text = "Location: " +
                "Latitude: " + Input.location.lastData.latitude +
                " Longitude: " + Input.location.lastData.longitude +
                " Altitude: " + Input.location.lastData.altitude +
                " Accuracy: " + Input.location.lastData.horizontalAccuracy +
                " Timestamp: " + Input.location.lastData.timestamp;

            // Fetch place name based on GPS coordinates
            StartCoroutine(GetPlaceName(Input.location.lastData.latitude, Input.location.lastData.longitude));
        }

        // Do not stop the location service if you need continuous updates
        // Input.location.Stop(); 
    }

    IEnumerator FetchRealLocation()
    {
        // Get location based on IP (only for editor and non-mobile platforms)
        UnityWebRequest www = UnityWebRequest.Get(apiUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            statusText.text = "Error retrieving location.";
        }
        else
        {
            string jsonResult = www.downloadHandler.text;
            IPInfo ipInfo = JsonUtility.FromJson<IPInfo>(jsonResult);

            string[] coordinates = ipInfo.loc.Split(',');
            string latitude = coordinates[0];
            string longitude = coordinates[1];

            statusText.text = "Real Location: " +
                "Latitude: " + latitude + " Longitude: " + longitude;

            StartCoroutine(GetPlaceName(double.Parse(latitude), double.Parse(longitude)));
        }
    }

    IEnumerator GetPlaceName(double latitude, double longitude)
    {
        string reverseGeocodeUrl = $"https://nominatim.openstreetmap.org/reverse?lat={latitude}&lon={longitude}&format=json&accept-language=en";

        UnityWebRequest www = UnityWebRequest.Get(reverseGeocodeUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            statusText.text += "\nError retrieving place name.";
        }
        else
        {
            // Parse the JSON result
            string jsonResult = www.downloadHandler.text;
            NominatimResponse placeInfo = JsonUtility.FromJson<NominatimResponse>(jsonResult);

            if (placeInfo.address != null)
            {
                string city = placeInfo.address.city ?? "Unknown city";
                string region = placeInfo.address.state ?? "Unknown region";
                statusText.text += $"\nPlace: {city}, {region}";
            }
            else
            {
                statusText.text += "\nPlace name not found.";
            }
        }
    }

    [System.Serializable]
    public class NominatimResponse
    {
        public Address address;
    }

    [System.Serializable]
    public class Address
    {
        public string city;
        public string state;
    }
}
