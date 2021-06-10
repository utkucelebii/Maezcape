using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoader : Singleton<LevelLoader>
{
    public GameObject loadingScreen;
    public Image circle;
    public Slider slider;
    public Text text;


    void Start()
    {
        var scene = SceneManager.GetActiveScene();
        if (scene.buildIndex == 0)
            LoadLevel("MainMenu");
    }

    public void LoadLevel(string sceneName)
    {
        StartCoroutine(LoadAsynchronously(sceneName));

    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            circle.fillAmount = progress;

            slider.value = progress;

            text.text = ((int)(progress * 100f)).ToString();

            yield return null;
        }
    }

}
