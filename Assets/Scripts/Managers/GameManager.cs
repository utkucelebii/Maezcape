using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{

    public int currency = 0;
    public string currentCharacter = "Top";

    public int gameModeLevelIndex;
    public string gameModeName;

    public bool analog = false;

    public int totalGamePlayed = 0;

    private void Awake()
    {
        this.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
        analog = (PlayerPrefs.GetInt("AnaLook") == 1)? true : false;
        DontDestroyOnLoad(gameObject);
        if (PlayerPrefs.HasKey("current_selected_character"))
        {
            currentCharacter = PlayerPrefs.GetString("current_selected_character");
            currency = PlayerPrefs.GetInt("Currency");
        }
        else
        {
            PlayerPrefs.SetString("own_characters", currentCharacter);
            Save();
        }
    }

    public void Save()
    {
        PlayerPrefs.SetString("current_selected_character", currentCharacter);
        PlayerPrefs.SetInt("Currency", currency);
    }

    public void sceneChange(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void SetVolume(float volume)
    {
        this.GetComponent<AudioSource>().volume = volume / 100;
        PlayerPrefs.SetFloat("MusicVolume", volume / 100);
    }

    public void AnaLook(int value)
    {
        PlayerPrefs.SetInt("AnaLook", value);
    }


}
