using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    private GameManager gameManager;
    private int music;
    public Slider volumeBar;
    public Text volumePercentage;
    public Toggle toogle;
    private void Start()
    {
        gameManager = GameManager.Instance;
        toogle.isOn = gameManager.analog;
        volumeBar.value = PlayerPrefs.GetFloat("MusicVolume") * 100;
        volumePercentage.text = (int)(PlayerPrefs.GetFloat("MusicVolume") * 100) + "%";

    }
    public void setVolume(float volume)
    {
        GameManager.Instance.SetVolume(volume);
        volumePercentage.text = ((int)(volume)).ToString() + "%";
    }

    public void changeScene(string sceneName)
    {
        gameManager.sceneChange(sceneName);
    }
    public void AnaLook(bool value)
    {
        if(value)
            gameManager.AnaLook(1);
        else
            gameManager.AnaLook(0);

        gameManager.analog = value;
    }
}
