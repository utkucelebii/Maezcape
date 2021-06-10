using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    GameObject firstItem, g;
    [SerializeField] Transform shopScroolView;

    private GameManager gameManager;
    public TextMeshProUGUI currentCurreny;
    public List<Character> characters;
    private string selectedCharacter;
    public string[] ownCharacters;
    private GameObject selectedObject;

    private void Start()
    {
        gameManager = GameManager.Instance;
        ownCharacters = PlayerPrefs.GetString("own_characters").Split(',');
        selectedCharacter = PlayerPrefs.GetString("current_selected_character");
        characters = Resources.LoadAll<Character>("Characters").ToList<Character>();
        firstItem = shopScroolView.GetChild(0).gameObject;
        int index = 0;
        for (int i = 0; i < characters.Count; i++)
        {
            int itemIndex = index;
            GameObject currentG = Instantiate(firstItem, shopScroolView);
            g = currentG;
            g.transform.GetChild(0).GetComponent<Image>().sprite = characters[i].icon;
            g.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = characters[i].price.ToString();
            g.GetComponent<Button>().onClick.AddListener(() => buyCharacter(characters[itemIndex], currentG));
            if (ownCharacters.Any(characters[i].characterName.Contains))
            {
                g.transform.GetChild(2).GetComponent<Button>().gameObject.SetActive(true);
                if (characters[i].characterName == selectedCharacter)
                {
                    selectedObject = g;
                    g.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "SEÇÝLÝ";
                    g.GetComponent<Button>().interactable = false;
                }
                else
                {
                    g.GetComponent<Button>().onClick.RemoveAllListeners();
                    g.GetComponent<Button>().onClick.AddListener(() => selectCharacter(characters[itemIndex], currentG));
                }
            }
            index++;
        }
        Destroy(firstItem);
    }

    private void Update()
    {
        currentCurreny.text = gameManager.currency.ToString();
    }

    private void buyCharacter(Character character, GameObject newCharacter)
    {
        if (gameManager.currency >= character.price)
        {
            gameManager.currency -= character.price;
            fixShopMenu(character, newCharacter, true);
            PlayerPrefs.SetString("own_characters", PlayerPrefs.GetString("own_characters") + "," + character.characterName);
        }
        else
        {
            Debug.Log("Not enough money");
        }
    }

    private void selectCharacter(Character character, GameObject newCharacter)
    {
        fixShopMenu(character, newCharacter, false);
        PlayerPrefs.SetString("current_selected_character", character.characterName);
        gameManager.currentCharacter = character.characterName;
    }

    private void fixShopMenu(Character character, GameObject newCharacter, bool isBuy)
    {
        if (isBuy)
        {
            newCharacter.GetComponent<Button>().interactable = true;
            newCharacter.transform.GetChild(2).GetComponent<Button>().gameObject.SetActive(true);
            newCharacter.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "SEÇ";
            newCharacter.GetComponent<Button>().onClick.RemoveAllListeners();
            newCharacter.GetComponent<Button>().onClick.AddListener(() => selectCharacter(character, newCharacter));
        }
        else
        {
            fixShopMenu(character, selectedObject, true);
            newCharacter.GetComponent<Button>().onClick.RemoveAllListeners();
            selectedObject = newCharacter;
            newCharacter.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "SEÇÝLÝ";
            newCharacter.GetComponent<Button>().interactable = false;
        }
        gameManager.Save();
    }

    public void OnPurchaseComplete(Product product)
    {
        if (product.definition.id == "com.utkucelebi.mazecape.coin1000")
            gameManager.currency += 1000;
        else if(product.definition.id == "com.utkucelebi.mazecape.coin5000")
            gameManager.currency += 5000;
        else if(product.definition.id == "com.utkucelebi.mazecape.coin10000")
            gameManager.currency += 10000;
        else if(product.definition.id == "com.utkucelebi.mazecape.coin20000")
            gameManager.currency += 20000;

        gameManager.Save();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.Log(reason);
    }

    public void changeScene(string sceneName)
    {
        gameManager.sceneChange(sceneName);
    }
}
