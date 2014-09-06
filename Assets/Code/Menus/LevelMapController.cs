using UnityEngine;
using System.Collections.Generic;
using Assets.Plugins.SmartLevelsMap.Scripts;

public class LevelMapController : MonoBehaviour
{
    public static LevelMapController Instance;
    
    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        if (CharacterSelectionMenu.Instance != null &&
            CharacterSelectionMenu.Instance.SelectedPlayerInfo != null)
        {
            var sr = transform.FindChild("LevelsMap").FindChild("Character").GetComponent<SpriteRenderer>();
            sr.sprite = CharacterSelectionMenu.Instance.SelectedPlayerInfo.IdleHeads [0];
        }


        LevelsMap.LevelReached += (object sender, LevelReachedEventArgs e) => {
            SubjectController.Instance.ChangeLevel(e.Number - 1);
            MenuController.Instance.ReturnFromMenu(MenuState.LevelSelection);
        };
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLevelStars(int level, int stars)
    {
        LevelsMap.CompleteLevel(level + 1, stars);
    }

    private void __CreateLevels()
    {
        var levelCount = Mathf.CeilToInt(SubjectController.Instance._subject._entries.Length / 10);

        var levelsMap = GetComponentInChildren<Assets.Plugins.SmartLevelsMap.Scripts.LevelsMap>();

        var levels = GenerateLevels(levelCount);
        Path path = GeneratePath(levels);

        var _levelsMap = GetComponentInChildren<LevelsMap>();
        _levelsMap.WaypointsMover.transform.position = levels [0].transform.position;
    }

    public float _width;
    public float _height;

    private List<MapLevel> GenerateLevels(int count)
    {
        var _levelsMap = GetComponentInChildren<LevelsMap>();
        _levelsMap.Count = count;

        // Remove old levels
        var oldLevels = _levelsMap.transform.Find("Levels");

        if (oldLevels != null)
        {
            oldLevels.transform.parent = null;
            Destroy(oldLevels.gameObject);
        }

        var oldPath = _levelsMap.transform.Find("Path");
        
        if (oldPath != null)
        {
            oldPath.transform.parent = null;
            Destroy(oldPath.gameObject);
        }

        GameObject goLevels = new GameObject("Levels");
        goLevels.transform.parent = _levelsMap.transform;
        float[] points = DivideLineToPoints(_levelsMap.Count);

        List<MapLevel> levels = new List<MapLevel>();
        for (int i = 0; i < _levelsMap.Count; i++)
        {
            MapLevel mapLevel = CreateMapLevel(points [i], i + 1);
            mapLevel.transform.parent = goLevels.transform;
            mapLevel.IsClickEnabled = _levelsMap.IsClickEnabled;
            levels.Add(mapLevel);
        }
        return levels;
    }
    
    private MapLevel CreateMapLevel(float point, int number)
    {
        var levelPrefab = transform.FindChild("LevelPrefab").gameObject;
        MapLevel mapLevel = (Instantiate(levelPrefab.transform) as Transform).GetComponent< MapLevel>();
        mapLevel.gameObject.SetActive(true);

        mapLevel.name = string.Format("Level{0:00}", number);
        mapLevel.Number = number;
        if (point < 1f / 3f)

            mapLevel.transform.position = GetPosition(point * 3f, _width, 0, _height / 3f, 0);
        else if (point < 2f / 3f)
            mapLevel.transform.position = GetPosition((point - 1f / 3f) * 3f, -_width, _width, _height / 3f, _height / 3f);
        else
            mapLevel.transform.position = GetPosition((point - 2f / 3f) * 3f, _width, 0, _height / 3f, _height * 2f / 3f);
        return mapLevel;
    }

    private Vector3 GetPosition(float p, float width, float xOffset, float height, float yOffset)
    {
        return new Vector3(
            xOffset + p * width - _width / 2f,
            yOffset + p * height - _height / 2f,
            0f);
    }
    
    private Path GeneratePath(List<MapLevel> levels)
    {
        var _levelsMap = GetComponentInChildren<LevelsMap>();


        Path path = new GameObject("Path").AddComponent<Path>();
        path.IsCurved = false;
        path.GizmosRadius = Camera.main.orthographicSize / 40f;
        
        path.transform.parent = _levelsMap.transform;
        foreach (MapLevel mapLevel in levels)
            path.Waypoints.Add(mapLevel.PathPivot);
        return path;
    }

    /// <summary>
    /// Devide [0,1] line to array of points.
    /// If count = 1, ret {0}
    /// If count = 2, ret {0, 1}
    /// If count = 3, ret {0, 0.5, 1}
    /// If count = 4, ret {0, 0.25, 0.25, 1}
    /// </summary>
    private float[] DivideLineToPoints(int pointsCount)
    {
        if (pointsCount <= 0)
            return new float[0];
        
        float[] points = new float[pointsCount];
        for (int i = 0; i < pointsCount; i++)
            points [i] = i * 1f / (pointsCount - 1);
        
        return points;
    }
}
