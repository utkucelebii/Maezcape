using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;


public class MapManager : MonoBehaviour
{
    [SerializeField] private List<Prefabs> prefabs;

    private GameManager gameManager;
    private LevelManager levelManager;
    //private int gameModeIndex, gameModeLevelIndex;
    private Transform walls, grounds;
    [SerializeField] private float wallSize = 25f, resizeMap = 3f;
    public List<Vector2> positions = new List<Vector2>();
    public List<Vector2> solutions = new List<Vector2>();
    public List<Vector3> solutionPoints = new List<Vector3>();
    private int wCells, hCells;

    private void Start()
    {
        gameManager = GameManager.Instance;
        levelManager = LevelManager.Instance;
        walls = transform.GetChild(0);
        grounds = transform.GetChild(1);
        createMap(gameManager.gameModeName, gameManager.gameModeLevelIndex);

        Vector3 startingPoint = new Vector3(solutionPoints.OrderByDescending(p => p.y).ToList()[solutionPoints.Count - 1].x, 5f, solutionPoints.OrderByDescending(p => p.y).ToList()[solutionPoints.Count - 1].z);
        Vector3 finishingPoint = new Vector3(solutionPoints.OrderByDescending(p => p.y).ToList()[0].x, 0f, solutionPoints.OrderByDescending(p => p.y).ToList()[0].z);
        GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(startingPoint.x * resizeMap, 5f, startingPoint.z * resizeMap);
        this.transform.localScale = new Vector3(resizeMap, 1f, resizeMap);
        GameObject.Find(startingPoint.x + "/" + startingPoint.z).gameObject.tag = "startingPoint";
        GameObject.Find(finishingPoint.x + "/" + finishingPoint.z).gameObject.tag = "finishingPoint";
    }

    private void createMap(string modeName, int gameModeLevelIndex)
    {
        string levelNo = gameModeLevelIndex.ToString();
        string mapType = "";
        string textFile = Resources.Load<TextAsset>("Maps/" + modeName + "/" + levelNo).ToString();
        string[] lines = textFile.Split(
            new[] { Environment.NewLine },
            StringSplitOptions.None
        );

        foreach (string line in lines)
        {
            if (line.Contains("<polyline") && !line.Contains("<polyline fill"))
            {
                var polyline = between("points=\"", "\"", line);
                string[] points = polyline.Split(' ');
                for (int i = 0; i < points.Length - 1; i++)
                {
                    float x1 = convertToFloat(points[i].Split(',')[0]);
                    float y1 = convertToFloat(points[i].Split(',')[1]);
                    float x2 = convertToFloat(points[i + 1].Split(',')[0]);
                    float y2 = convertToFloat(points[i + 1].Split(',')[1]);
                    //Making them a vector
                    Vector3 first = new Vector3(x1, 0, y1);
                    Vector3 second = new Vector3(x2, 0, y2);
                    //Finding angle of between two vector, midpoint and the distance
                    float xDiff = x1 - x2;
                    float yDiff = y1 - y2;
                    float angle = (float)Math.Atan2(yDiff, xDiff) * (float)(180 / Math.PI);
                    Vector3 between2 = second - first;
                    float distance = Vector3.Distance(second, first);
                    //Create wall
                    GameObject newObject = Instantiate(prefabs.Find(x => x.prefabIndex == 3).gameObject, first + (between2 / 2), Quaternion.identity) as GameObject;
                    newObject.transform.parent = walls.transform;
                    newObject.transform.localScale = new Vector3(distance, wallSize, 2f);
                    newObject.transform.position = new Vector3(newObject.transform.position.x, wallSize / 2f, newObject.transform.position.z);
                    newObject.transform.Rotate(0, angle * -1, 0);
                    newObject.name = (x1 + (x2 - x1) / 2f).ToString() + "/" + (y1 + (y2 - y1) / 2f).ToString();
                    //Save the position
                    positions.Add(new Vector2(x1 + (x2 - x1) / 2f, y1 + (y2 - y1) / 2f));

                }
            }
            else if (line.Contains("<line"))
            {
                float x1 = convertToFloat(between("x1=\"", "\"", line));
                float y1 = convertToFloat(between("y1=\"", "\"", line));
                float x2 = convertToFloat(between("x2=\"", "\"", line));
                float y2 = convertToFloat(between("y2=\"", "\"", line));
                //Making them a vector
                Vector3 first = new Vector3(x1, 0, y1);
                Vector3 second = new Vector3(x2, 0, y2);
                //Finding angle of between two vector, midpoint and the distance
                float xDiff = x1 - x2;
                float yDiff = y1 - y2;
                float angle = (float)Math.Atan2(yDiff, xDiff) * (float)(180 / Math.PI);
                Vector3 between2 = second - first;
                float distance = Vector3.Distance(second, first);
                //Create wall
                GameObject newObject = Instantiate(prefabs.Find(x => x.prefabIndex == 3).gameObject, first + (between2 / 2), Quaternion.identity) as GameObject;
                newObject.transform.parent = walls.transform;
                newObject.transform.localScale = new Vector3(distance, wallSize, 2f);
                newObject.transform.position = new Vector3(newObject.transform.position.x, wallSize / 2f, newObject.transform.position.z);
                newObject.transform.Rotate(0, angle * -1, 0);
                newObject.name = (x1 + (x2 - x1) / 2f).ToString() + "/" + (y1 + (y2 - y1) / 2f).ToString();
                //Save the position
                positions.Add(new Vector2(x1 + (x2 - x1) / 2f, y1 + (y2 - y1) / 2f));
            }
            else if (line.Contains("<title>"))
            {
                var grids = between("<title>", "</title>", line);
                if (!(grids.Contains("maze with")))
                {
                    string[] gridSize = grids.Split(' ');
                    wCells = int.Parse(gridSize[0]);
                    hCells = int.Parse(gridSize[2]);
                }
                var mapTypeName = between("<title>", " maze", line);
                string theMap = mapTypeName;
                if (theMap.Contains("by"))
                {
                    mapType = theMap.Replace(wCells + " by " + hCells + " ", "");
                }
                else
                {
                    mapType = theMap;
                }
            }
            else if (line.Contains("<polyline fill="))
            {
                var finishLine = between("points=\"", "\"", line);
                string[] points = finishLine.Split(' ');
                for (int i = 0; i < points.Length; i++)
                {
                    if (i == 0)
                        positions.Add(new Vector2(convertToFloat(points[i].Split(',')[0]), convertToFloat(points[i].Split(',')[1])));

                    solutions.Add(new Vector2(convertToFloat(points[i].Split(',')[0]), convertToFloat(points[i].Split(',')[1])));
                    if (i != points.Length - 1)
                    {
                        if (Math.Abs(convertToFloat(points[i].Split(',')[0]) - convertToFloat(points[i + 1].Split(',')[0])) >= 11)
                        {
                            float diffX = convertToFloat(points[i + 1].Split(',')[0]) - convertToFloat(points[i].Split(',')[0]);
                            float diffY = convertToFloat(points[i + 1].Split(',')[1]) - convertToFloat(points[i].Split(',')[1]);
                            float pointNum = Math.Abs(convertToFloat(points[i].Split(',')[0]) - convertToFloat(points[i + 1].Split(',')[0])) / 11f;
                            float interval_X = diffX / (pointNum + 1);
                            float interval_Y = diffY / (pointNum + 1);
                            for (int q = 1; q <= pointNum; q++)
                            {
                                solutions.Add(new Vector2((convertToFloat(points[i].Split(',')[0]) + interval_X * q), (convertToFloat(points[i].Split(',')[1]) + interval_Y * q)));
                            }
                        }
                        if (Math.Abs(convertToFloat(points[i].Split(',')[1]) - convertToFloat(points[i + 1].Split(',')[1])) >= 11)
                        {
                            float diffX = convertToFloat(points[i + 1].Split(',')[0]) - convertToFloat(points[i].Split(',')[0]);
                            float diffY = convertToFloat(points[i + 1].Split(',')[1]) - convertToFloat(points[i].Split(',')[1]);
                            float pointNum = Math.Abs(convertToFloat(points[i].Split(',')[1]) - convertToFloat(points[i + 1].Split(',')[1])) / 11f;
                            float interval_X = diffX / (pointNum + 1);
                            float interval_Y = diffY / (pointNum + 1);
                            for (int q = 1; q <= pointNum; q++)
                            {
                                solutions.Add(new Vector2((convertToFloat(points[i].Split(',')[0]) + interval_X * q), (convertToFloat(points[i].Split(',')[1]) + interval_Y * q)));
                            }
                        }
                    }
                }
            }
            else if (line.Contains("<svg width="))
            {
                var widt = between("width=\"", "\"", line);
                var heigh = between("height=\"", "\"", line);
            }
        }
        Vector2 invisibleWallPos = positions[positions.Count - 1];
        Vector3 pos = new Vector3(invisibleWallPos.x, 0, invisibleWallPos.y);
        GameObject newWallObject = (GameObject)Instantiate(prefabs.Find(x => x.prefabIndex == 3).gameObject);
        newWallObject.transform.parent = walls.transform;
        if (!mapType.Contains("Triangular delta"))
            newWallObject.transform.Rotate(0, 180, 0);
        else
            newWallObject.transform.Rotate(0, 60, 0);
        newWallObject.transform.localScale = new Vector3(30f, wallSize, 2f);
        newWallObject.transform.position = pos;
        newWallObject.name = "invisible Wall";
        newWallObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;

        createGrounds(modeName, mapType);
    }

    private void createGrounds(string gameModeName, string mapType)
    {
        if (gameModeName == "Normal")
        {
            normalModeGenerator();
        }
        else
        {
            if (mapType.Contains("Triangular delta"))
            {
                triangleGround();
            }
            else if (mapType.Contains("sigma"))
            {
                sigmaGroundGenerator();
            }
            else if (mapType.Contains("delta") && !mapType.Contains("triangular"))
            {
                deltaGroundGenerator();
            }
            else if (mapType.Contains("orthogonal"))
            {
                squareGroundGenerator();
            }
        }
        levelManager.solutions = solutionPoints.Count;
    }




    private void squareGroundGenerator()
    {
        int fixer = 0;
        for (int i = 0; i < wCells; i++)
        {
            for (int x = 0; x < hCells; x++)
            {
                Vector3 pos = Vector3.zero;
                if (fixer % 2 == 0)
                    pos = new Vector3(((16f * i) + 2) + (16f / 2), 0f, ((16f * x) + 2) + (16f / 2));
                else
                    pos = new Vector3(((16f * i) + 2) + (16f / 2), 0.001f, ((16f * x) + 2) + (16f / 2));

                GameObject newObject = Instantiate(prefabs.Find(x => x.prefabIndex == 0).gameObject, pos, Quaternion.identity) as GameObject;
                newObject.transform.parent = grounds.transform;
                newObject.transform.localScale = new Vector3(16f, 1, 16f);
                newObject.name = (pos.x + "/" + pos.z).ToString();
                if (checkSolution(pos) == true)
                    newObject.GetComponent<Prefabs>().roadToGlory = true;

                fixer++;
            }
        }
    }

    private void normalModeGenerator()
    {
        for (int i = 0; i < wCells; i++)
        {
            for (int x = 0; x < hCells; x++)
            {
                Vector3 pos = new Vector3(((16f * i) + 2) + (16f / 2), 0f, ((16f * x) + 2) + (16f / 2));
                Vector3 pos2 = new Vector3(((16f * i) + 2) + (16f / 2), wallSize / 2f, ((16f * x) + 2) + (16f / 2));

                if (checkSolution(pos) == true)
                {
                    GameObject newObject = (GameObject)Instantiate(prefabs.Find(x => x.prefabIndex == 0).gameObject);
                    newObject.transform.parent = grounds.transform;
                    newObject.transform.localScale = new Vector3(16f, 1f, 16f);
                    newObject.transform.position = pos;
                    newObject.name = (pos.x + "/" + pos.z).ToString();
                    newObject.GetComponent<Prefabs>().roadToGlory = true;
                }
                else
                {
                    GameObject newObject = (GameObject)Instantiate(prefabs.Find(x => x.prefabIndex == 3).gameObject);
                    newObject.transform.parent = grounds.transform;
                    newObject.transform.localScale = new Vector3(18f, wallSize, 18f);
                    newObject.transform.position = pos2;
                    newObject.name = (pos.x + "/" + pos.z).ToString();
                }
            }
        }
    }
    private void sigmaGroundGenerator()
    {
        int fixer = 0;
        for (int i = 0; i < hCells; i++)
        {
            for (int q = 0; q < wCells; q++)
            {
                if (q % 2 == 0)
                {
                    Vector3 pos = Vector3.zero;
                    if (fixer % 2 == 0)
                        pos = new Vector3(16f + q * 21f, 0, 26.12436f + i * 24.24871f);
                    else
                        pos = new Vector3(16f + q * 21f, 0.001f, 26.12436f + i * 24.24871f);

                    GameObject newObject = (GameObject)Instantiate(prefabs.Find(x => x.prefabIndex == 1).gameObject);
                    newObject.transform.parent = grounds.transform;
                    newObject.transform.localScale = new Vector3(14f, 1f, 14f);
                    newObject.transform.position = pos;
                    newObject.name = (pos.x + "/" + pos.z).ToString();
                    if (checkSolution(pos) == true)
                        newObject.GetComponent<Prefabs>().roadToGlory = true;
                }
                else
                {
                    Vector3 pos = new Vector3(16f + q * 21f, 0, 14f + i * 24.24871f);
                    GameObject newObject = (GameObject)Instantiate(prefabs.Find(x => x.prefabIndex == 1).gameObject);
                    newObject.transform.parent = grounds.transform;
                    newObject.transform.localScale = new Vector3(14f, 1f, 14f);
                    newObject.transform.position = pos;
                    newObject.name = (pos.x + "/" + pos.z).ToString();
                    if (checkSolution(pos) == true)
                        newObject.GetComponent<Prefabs>().roadToGlory = true;
                }
                fixer++;
            }
        }
    }
    private void deltaGroundGenerator()
    {
        for (int i = 0; i < positions.Count - 1; i++)
        {
            Vector2 first = positions.OrderByDescending(p => p.y).ThenBy(p => p.x).Reverse().ToList()[i];
            Vector2 second = positions.OrderByDescending(p => p.y).ThenBy(p => p.x).Reverse().ToList()[i + 1];
            float angleOfObject;
            if (second.y == positions[positions.Count - 1].y && second.x == positions[positions.Count - 1].x)
                angleOfObject = 180;
            else
                angleOfObject = GameObject.Find(second.x + "/" + second.y).transform.rotation.eulerAngles.y;
            if (first.y == second.y && angleOfObject != 180 && angleOfObject != -180 && angleOfObject != 0)
            {
                float distanceC = Math.Abs(second.x - first.x) / 11f;
                for (int counter = 0; counter < distanceC; counter++)
                {
                    if ((angleOfObject > 59 && angleOfObject < 61) || (angleOfObject == 120) || (angleOfObject > 239 && angleOfObject < 241))
                    {
                        if (counter % 2 == 0)
                        {
                            Vector3 pos = new Vector3(second.x + (counter * 11.25f) + 5.5f, 0.001f, first.y + 3.5f);
                            GameObject newObject = Instantiate(prefabs.Find(x => x.prefabIndex == 2).gameObject, pos, Quaternion.identity) as GameObject;
                            newObject.transform.parent = grounds.transform;
                            newObject.transform.localScale = new Vector3(13f, 1f, 13f);
                            newObject.transform.Rotate(0, 0, 0);
                            newObject.name = (pos.x + "/" + pos.z).ToString();
                            if (checkSolution(pos) == true)
                                newObject.GetComponent<Prefabs>().roadToGlory = true;
                        }
                        else
                        {
                            Vector3 pos = new Vector3(second.x + (counter * 11.25f) - 11f + 16.5f, 0f, first.y - 2.57f);
                            GameObject newObject = Instantiate(prefabs.Find(x => x.prefabIndex == 2).gameObject, pos, Quaternion.identity) as GameObject;
                            newObject.transform.parent = grounds.transform;
                            newObject.transform.localScale = new Vector3(13f, 1f, 13f);
                            newObject.transform.Rotate(0, 180f, 0);
                            newObject.name = (pos.x + "/" + pos.z).ToString();
                            if (checkSolution(pos) == true)
                                newObject.GetComponent<Prefabs>().roadToGlory = true;
                        }
                    }
                    else
                    {
                        if (counter % 2 == 0)
                        {
                            Vector3 pos = new Vector3(second.x + (counter * 11.25f) - 11f + 16.5f, 0f, first.y - 2.57f);
                            GameObject newObject = Instantiate(prefabs.Find(x => x.prefabIndex == 2).gameObject, pos, Quaternion.identity) as GameObject;
                            newObject.transform.parent = grounds.transform;
                            newObject.transform.localScale = new Vector3(13f, 1f, 13f);
                            newObject.transform.Rotate(0, 180f, 0);
                            newObject.name = (pos.x + "/" + pos.z).ToString();
                            if (checkSolution(pos) == true)
                                newObject.GetComponent<Prefabs>().roadToGlory = true;
                        }
                        else
                        {
                            Vector3 pos = new Vector3(second.x + (counter * 11.25f) + 5.5f, 0.001f, first.y + 3.5f);
                            GameObject newObject = Instantiate(prefabs.Find(x => x.prefabIndex == 2).gameObject, pos, Quaternion.identity) as GameObject;
                            newObject.transform.parent = grounds.transform;
                            newObject.transform.localScale = new Vector3(13f, 1f, 13f);
                            newObject.transform.Rotate(0, 0, 0);
                            newObject.name = (pos.x + "/" + pos.z).ToString();
                            if (checkSolution(pos) == true)
                                newObject.GetComponent<Prefabs>().roadToGlory = true;
                        }
                    }
                }
            }
        }
    }

    private void triangleGround()
    {
        for (int i = 0; i < positions.Count - 1; i++)
        {
            Vector2 first = positions.OrderByDescending(p => Math.Round(p.y)).ThenBy(p => p.x).Reverse().ToList()[i];
            Vector2 second = positions.OrderByDescending(p => Math.Round(p.y)).ThenBy(p => p.x).Reverse().ToList()[i + 1];

            //Debug.Log(first + " ----------- " + second);
            float angleOfObject;
            if (second.y == positions[positions.Count - 1].y && second.x == positions[positions.Count - 1].x)
                angleOfObject = 60;
            else
                angleOfObject = GameObject.Find(second.x + "/" + second.y).transform.rotation.eulerAngles.y;
            if (Math.Round(first.y, 4) == Math.Round(second.y, 4) && angleOfObject != 180 && angleOfObject != -180 && angleOfObject != 0)
            {
                float distanceC = Math.Abs(second.x - first.x) / 11f;
                for (int counter = 0; counter < distanceC; counter++)
                {
                    if ((angleOfObject > 59 && angleOfObject < 61) || (angleOfObject == 120) || (angleOfObject > 239 && angleOfObject < 241))
                    {
                        if (counter % 2 == 0)
                        {
                            Vector3 pos = new Vector3(second.x + (counter * 11.25f) + 5.5f, 0.001f, first.y + 3.5f);
                            GameObject newObject = Instantiate(prefabs.Find(x => x.prefabIndex == 2).gameObject, pos, Quaternion.identity) as GameObject;
                            newObject.transform.parent = grounds.transform;
                            newObject.transform.localScale = new Vector3(13f, 1f, 13f);
                            newObject.transform.Rotate(0, 0, 0);
                            newObject.name = (pos.x + "/" + pos.z).ToString();
                            if (checkSolution(pos) == true)
                                newObject.GetComponent<Prefabs>().roadToGlory = true;
                        }
                        else
                        {
                            Vector3 pos = new Vector3(second.x + (counter * 11.25f) - 11f + 16.5f, 0f, first.y - 2.57f);
                            GameObject newObject = Instantiate(prefabs.Find(x => x.prefabIndex == 2).gameObject, pos, Quaternion.identity) as GameObject;
                            newObject.transform.parent = grounds.transform;
                            newObject.transform.localScale = new Vector3(13f, 1f, 13f);
                            newObject.transform.Rotate(0, 180f, 0);
                            newObject.name = (pos.x + "/" + pos.z).ToString();
                            if (checkSolution(pos) == true)
                                newObject.GetComponent<Prefabs>().roadToGlory = true;
                        }
                    }
                    else
                    {
                        if (counter % 2 == 0)
                        {
                            Vector3 pos = new Vector3(second.x + (counter * 11.25f) - 11f + 16.5f, 0f, first.y - 2.57f);
                            GameObject newObject = Instantiate(prefabs.Find(x => x.prefabIndex == 2).gameObject, pos, Quaternion.identity) as GameObject;
                            newObject.transform.parent = grounds.transform;
                            newObject.transform.localScale = new Vector3(13f, 1f, 13f);
                            newObject.transform.Rotate(0, 180f, 0);
                            newObject.name = (pos.x + "/" + pos.z).ToString();
                            if (checkSolution(pos) == true)
                                newObject.GetComponent<Prefabs>().roadToGlory = true;
                        }
                        else
                        {
                            Vector3 pos = new Vector3(second.x + (counter * 11.25f) + 5.5f, 0.001f, first.y + 3.5f);
                            GameObject newObject = Instantiate(prefabs.Find(x => x.prefabIndex == 2).gameObject, pos, Quaternion.identity) as GameObject;
                            newObject.transform.parent = grounds.transform;
                            newObject.transform.localScale = new Vector3(13f, 1f, 13f);
                            newObject.transform.Rotate(0, 0, 0);
                            newObject.name = (pos.x + "/" + pos.z).ToString();
                            if (checkSolution(pos) == true)
                                newObject.GetComponent<Prefabs>().roadToGlory = true;
                        }
                    }

                }
            }
        }
    }
    
    Boolean checkSolution(Vector3 pos)
    {
        for (int i = 0; i < solutions.Count; i++)
        {
            Vector3 solutionPos = new Vector3(solutions[i].x, 0f, solutions[i].y);
            float distance = Vector3.Distance(pos, solutionPos);
            if (distance < 10f)
            {
                if (pos.x != 0 && pos.z != 0)
                {
                    Vector3 leggo = new Vector3(pos.x, i, pos.z);
                    solutionPoints.Add(leggo);
                }
                return true;
            }
        }
        return false;
    }

    
    float convertToFloat(string number)
    {
        return float.Parse(number, CultureInfo.InvariantCulture.NumberFormat);
    }
    string between(string first, string second, string text)
    {
        var respond = Regex.Matches(text, @"(?<=" + first + ")(.+?)(?=" + second + ")");
        return respond[0].Value;
    }
}
