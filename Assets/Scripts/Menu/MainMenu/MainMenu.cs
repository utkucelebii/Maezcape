using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelData
{
    public LevelData(string levelName)
    {
        string data = PlayerPrefs.GetString(levelName);
        if (data == "")
            return;

        //string[] allData = data.Split("&")[0];
        bestTime = float.Parse(data.Split('&')[0]);
        stars = int.Parse(data.Split('&')[1]);
    }
    public float bestTime { set; get; }
    public int stars { set; get; }
}

public class MainMenu : MonoBehaviour
{
    private GameManager gameManager;
    public TextMeshProUGUI currentCurreny;
    public GameObject levelMenu;
    private GameObject firstItem, g;
    public Transform[] levelScrollView;
    public ScrollRect[] scrollRect;
    private string selectedMode;

    private void Start()
    {
        gameManager = GameManager.Instance;
        modeMenu();
    }
    private void Update()
    {
        currentCurreny.text = gameManager.currency.ToString();
    }

    public void selectMode(string modeName)
    {
        gameManager.gameModeName = modeName;
    }

    private void modeMenu()
    {
        modeSelection("Normal", 0);
        modeSelection("Fun", 1);
        modeSelection("Time", 2);
    }

    public void changeScene(string sceneName)
    {
        gameManager.sceneChange(sceneName);
    }
    private void modeSelection(string mode, int modeNo)
    {
        if (levelScrollView[modeNo].childCount > 0) 
        {
            int a = 0;
            foreach (Transform child in levelScrollView[modeNo])
            {
                if(a != 0)
                    GameObject.Destroy(child.gameObject);
                a++;
            }

        }
        int lastActive = 0;
        bool nextLevelLocked = false;
        firstItem = levelScrollView[modeNo].GetChild(0).gameObject;
        int levelCount = Resources.LoadAll<TextAsset>("Maps/"+ mode).Length;
        int index = 1;
        for (int i = 1; i < levelCount + 1; i++)
        {
            GameObject test = Instantiate(firstItem, levelScrollView[modeNo]);
            g = test;
            g.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = i.ToString();
            LevelData level = new LevelData((i).ToString() + "_" + mode);
            g.transform.GetChild(1).gameObject.SetActive(nextLevelLocked);
            int levelIndex = index;
            g.GetComponent<Button>().onClick.AddListener(() => startGame(levelIndex));
            g.GetComponent<Button>().interactable = !nextLevelLocked;
            if (level.bestTime == 0.0f)
            {
                if (lastActive == 0)
                    lastActive = levelIndex - 1;
                nextLevelLocked = true;
            }
            else
            {

                g.transform.GetChild(0).GetChild(1).GetChild(level.stars - 1).gameObject.SetActive(true);
            }
            index++;
        }
        Destroy(firstItem);
        levelScrollView[modeNo].GetComponent<RectTransform>().localPosition = Vector3.zero;
        levelScrollView[modeNo].GetComponent<RectTransform>().localPosition = new Vector3(0, (lastActive / 3) * 130f, levelScrollView[modeNo].GetComponent<RectTransform>().localPosition.z);


    }


    private void startGame(int level)
    {
        gameManager.gameModeLevelIndex = level;
        LevelLoader.Instance.LoadLevel("Game");
    }
}
