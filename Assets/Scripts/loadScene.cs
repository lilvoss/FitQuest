using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour

{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider slider;

    
    

    //public AudioMixer audioMixer;
    //public Slider volumeSlider;

    private void Start()
    {

        SetMusicVolume();
        //float volume = 0f;
        //audioMixer.GetFloat("Volume", out volume);
        //volumeSlider.value = Mathf.Pow(10, volume / 20); // Convert dB to linear scale

        //volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SceneLoader(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }

    public void SetMusicVolume()
    {

        float volume = slider.value;
        myMixer.SetFloat("music", Mathf.Log10(volume) * 20);






        // Prevent logarithmic error for zero
        //if (volume == 0)
        // {
        //  audioMixer.SetFloat("Volume", -80f); // Typically set to a very low value
        //  return;
        //  }

        // float db = Mathf.Log10(volume) * 20;
        // audioMixer.SetFloat("Volume", db);

        // Debug log to check the values
        // Debug.Log($"Slider Value: {volume}, dB Value: {db}");
    }
}
