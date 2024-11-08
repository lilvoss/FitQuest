using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneNavigator : MonoBehaviour
{
    public void LoadMapScene()
    {
        SceneManager.LoadScene("MapScene"); 
    }
}
