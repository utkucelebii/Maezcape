using UnityEngine;
using System.Collections.Generic;

public class Prefabs : MonoBehaviour
{
    public int prefabIndex;
    public bool roadToGlory, isWall; 
    public Material[] material;
    private Renderer rend;
    public List<int> names;
    private LevelManager levelManager;

    void Start()
    {
        levelManager = LevelManager.Instance;
        if (isWall == false)
        {
            rend = transform.GetComponent<Renderer>();
            rend.enabled = true;
            rend.sharedMaterial = material[0];
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (!names.Contains(transform.GetInstanceID()) && (gameObject.tag == "finishingPoint" || gameObject.tag == "startingPoint" || roadToGlory) && isWall == false)
            {
                rend.sharedMaterial = material[1];
                levelManager.solutionCounter++;
            }
            names.Add(transform.GetInstanceID());
            if (gameObject.tag == "finishingPoint")
                levelManager.isEnd = true;
        }
        else
        {
            rend.sharedMaterial = material[0];
        }
    }
}
