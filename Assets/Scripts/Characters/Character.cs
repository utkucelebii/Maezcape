using UnityEngine;

public enum CharacterInfo
{
    top = 0,
    kutuk = 1,
    yarasa = 2,
    bomba = 3,
    balkabagi = 4,
    robot = 5,
    slime = 6,
    tascanavar = 7,
    kaplumbaga = 8,
}
public class Character : MonoBehaviour
{
    public CharacterInfo characterType;
    public string characterName;
    public int price;
    public Sprite icon;
}
