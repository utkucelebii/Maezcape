using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : Singleton<LevelManager>
{
    private GameManager gameManager;
    public int solutions = 0, solutionCounter = 0;
    private float startTime, duration, calculatedTime;
    private bool isPaused = false;
    public bool isEnd;
    public GameObject pauseMenu, winLoseBox, starHolder;
    private Character character;
    public Text winBoxTotalTime, timer;
    public VirtualJoystick[] cameraJoystick;
    public AdsManager adsManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        character = Resources.LoadAll<Character>("Characters").ToList<Character>().Find(x => x.characterName == gameManager.currentCharacter);
        Instantiate(character.gameObject, GameObject.FindGameObjectWithTag("Player").transform);
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        startTime = Time.time;
    }

    private void FixedUpdate()
    {
        //Total solution
        if (calculatedTime == 0.0f)
            calculatedTime = solutions * 2.0f;

        if (gameManager.gameModeName != "Time")
        {
            duration = Time.time - startTime;
            timer.text = TimeSpan.FromSeconds(duration).ToString(@"hh\:mm\:ss");
        }
        else
        {
            duration = Time.time - startTime;
            calculatedTime -= Time.deltaTime;
            timer.text = TimeSpan.FromSeconds(calculatedTime).ToString(@"hh\:mm\:ss");
            Debug.Log(TimeSpan.FromSeconds(calculatedTime).ToString(@"hh\:mm\:ss"));
        }
        Rules(gameManager.gameModeName);
        if (gameManager.totalGamePlayed % 5 == 0)
            adsManager.ShowInterstitialAd();
    }

    private void Win(string gameModeName)
    {
        LevelData level = new LevelData(gameManager.gameModeLevelIndex.ToString() + "_" + gameModeName);
        string saveString = "";
        saveString += (level.bestTime > duration || level.bestTime == 0.0f) ? duration.ToString("R") : level.bestTime.ToString();
        if (duration < calculatedTime)
        {
            saveString += "&" + "3";
            gameManager.currency += 250;
            for (var i = 0; i < 3; i++)
            {
                starHolder.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else if (duration > calculatedTime && duration < calculatedTime * 2f)
        {
            saveString += "&" + "2";
            gameManager.currency += 250;
            for (var i = 0; i < 2; i++)
            {
                starHolder.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else
        {
            saveString += "&" + "1";
            gameManager.currency += 250;
            for (var i = 0; i < 1; i++)
            {
                starHolder.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        gameManager.Save();
        winBoxTotalTime.text = "Süre: " +TimeSpan.FromSeconds(duration).ToString(@"hh\:mm\:ss");
        PlayerPrefs.SetString(gameManager.gameModeLevelIndex.ToString() + "_" + gameModeName, saveString);
    }
    public void Rules(string gameModeName)
    {
        if (gameModeName == "Normal")
        {
            if (solutionCounter != 0 && solutions != 0 && (solutions == solutionCounter))
            {
                if (!winLoseBox.activeSelf)
                {
                    winLoseBox.SetActive(!winLoseBox.activeSelf);
                    winLoseBox.transform.GetChild(0).gameObject.SetActive(!winLoseBox.transform.GetChild(0).gameObject.activeSelf);
                    Time.timeScale = 0;
                    isPaused = true;
                    Win(gameModeName);
                    gameManager.totalGamePlayed++;
                }
            }
        }
        else if (gameModeName == "Fun")
        {
            if (isEnd)
                if (!winLoseBox.transform.GetChild(0).gameObject.activeSelf)
                {
                    winLoseBox.SetActive(!winLoseBox.activeSelf);
                    winLoseBox.transform.GetChild(0).gameObject.SetActive(!winLoseBox.transform.GetChild(0).gameObject.activeSelf);
                    Time.timeScale = 0;
                    isPaused = true;
                    Win(gameModeName);
                    gameManager.totalGamePlayed++;
                }
        }
        else
        {
            if (!(calculatedTime > 0))
            {
                if (!winLoseBox.transform.GetChild(1).gameObject.activeSelf)
                {
                    winLoseBox.SetActive(!winLoseBox.activeSelf);
                    winLoseBox.transform.GetChild(1).gameObject.SetActive(!winLoseBox.transform.GetChild(1).gameObject.activeSelf);
                    Time.timeScale = 0;
                    isPaused = true;
                    gameManager.totalGamePlayed++;

                }
            }
            else
            {
                if (isEnd)
                    if (!winLoseBox.transform.GetChild(0).gameObject.activeSelf)
                    {
                        winLoseBox.SetActive(!winLoseBox.activeSelf);
                        winLoseBox.transform.GetChild(0).gameObject.SetActive(!winLoseBox.transform.GetChild(0).gameObject.activeSelf);
                        Time.timeScale = 0;
                        isPaused = true;
                        Win(gameModeName);
                        gameManager.totalGamePlayed++;
                    }
            }

        }
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);

        if (isPaused)
        {
            Time.timeScale = 1;
            isPaused = false;
        }
        else
        {
            Time.timeScale = 0;
            isPaused = true;
        }
    }
    public void changeScene(string sceneName)
    {
        LevelLoader.Instance.LoadLevel(sceneName);
    }

    public void nextLevel()
    {
        gameManager.gameModeLevelIndex++;
        LevelLoader.Instance.LoadLevel("Game");
    }
}
