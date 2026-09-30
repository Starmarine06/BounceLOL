using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    public static Queue<GameObject> Obstacles;

    public int PoolSize;
    public float Speed;
    public float SpeedIncrement;
    public static float SpeedChange;
    public float Smooth;
    public Vector2 WidthRange;
    public Vector2 HeightRange;
    public float Delta;
    public int D;
    public Transform ObstaclesContainer;
    public GameObject Obstacle;

    private float time;
    private Vector3 startPos;
    private GameObject top;
    private GameObject bottom;
    private float topHeight;
    private float topWidth;
    private float bottomHeight;
    private float bottomWidth;

    private float topInterval
    {
        get => (topWidth - Smooth / Speed) / Speed;
    }

    private float bottomInterval
    {
        get => (bottomWidth - Smooth / Speed) / Speed;
    }

    private Vector3 topScale
    {
        get => new Vector3(topWidth, topHeight, 1);
    }

    private Vector3 bottomScale
    {
        get => new Vector3(bottomWidth, bottomHeight, 1);
    }

    void Awake()
    {
        startPos = new Vector3(15f, 0f, 0f);
        FillPool();
        InitializeLevelSettings();
    }

    private void InitializeLevelSettings()
    {
        LevelManager.LevelConfig levelConfig = LevelManager.GetCurrentLevelConfig();
        
        // Only speed increment and smooth change per level
        SpeedIncrement = levelConfig.speedIncrement;
        Smooth = Mathf.Max(levelConfig.smooth, 2.0f); // Ensure minimum 2
        
        // Keep default values for other parameters
        Speed = 9f;
        Delta = 0.5f;
        D = 1;
    }

    void Start()
    {
        StartCoroutine(topRandGen());
        StartCoroutine(bottomRandGen());
    }

    private void FillPool()
    {
        Obstacles = new Queue<GameObject>();
        for (int i = 0; i < PoolSize; i++)
        {
            GameObject clone = Instantiate(Obstacle, startPos, Quaternion.identity, ObstaclesContainer);
            clone.SetActive(false);
            Obstacles.Enqueue(clone);
        }
    }

    private GameObject GetObstacle()
    {
        if (Obstacles == null)
        {
            Obstacles = new Queue<GameObject>();
        }

        if (Obstacles.Count == 0)
        {
            Debug.LogWarning("Obstacle pool empty! Instantiating a new obstacle dynamically.");
            GameObject newClone = Instantiate(Obstacle, startPos, Quaternion.identity, ObstaclesContainer);
            newClone.SetActive(false);
            updateSpeed();
            return newClone;
        }

        GameObject clone = Obstacles.Dequeue();
        clone.transform.position = startPos;
        updateSpeed();
        return clone;
    }

    private void updateSpeed()
    {
        Mover.Speed = Speed;
    }

    private void updateTopTransform()
    {
        top.transform.localScale = topScale;
        top.transform.position = new Vector3(top.transform.position.x, 5 - top.transform.localScale.y / 2, 0f);
    }

    private void updateBottomTransform()
    {
        bottom.transform.localScale = bottomScale;
        bottom.transform.position = new Vector3(bottom.transform.position.x, -5 + bottom.transform.localScale.y / 2, 0f);
    }

    private IEnumerator topRandGen()
    {
        topWidth = WidthRange.x;

        while (true)
        {
            top = GetObstacle();
            topHeight = Random.Range(HeightRange.x, HeightRange.y);
            updateTopTransform();
            yield return new WaitForSeconds(topInterval);
            top.SetActive(true);
        }
    }

    private IEnumerator bottomRandGen()
    {
        bottomWidth = WidthRange.x;

        while (true)
        {
            bottom = GetObstacle();
            bottomHeight = Random.Range(HeightRange.x, HeightRange.y);
            updateBottomTransform();
            yield return new WaitForSeconds(bottomInterval);
            bottom.SetActive(true);
        }
    }

    public void FixedUpdate()
    {
        Speed += SpeedIncrement;
    }
}
