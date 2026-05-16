using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BlockSpawner : MonoBehaviour
{
    [Header("Spawner config")]

    [Tooltip("Base prefab")]
    [SerializeField] private GameObject blockPrefab;

    [Tooltip("Block spawn location")]
    [SerializeField] private Transform spawnPoint;

    [Tooltip("Block container on hierarchy")]
    [SerializeField] private Transform dynamicElementsContainer;


    [Header("Block types")]

    [Tooltip("Available ScriptableObjects to spawn")]
    [SerializeField] private List<BlockData> availableBlocks;


    [Header("Movement")]

    [Tooltip("Base movement speed")]
    [SerializeField] private float startMoveSpeed = 2.0f;
    [Tooltip("Maximum movement speed")]
    [SerializeField] private float maxMoveSpeed = 10.0f;

    [Tooltip("Vertical smoothing")]
    [SerializeField] private float smoothSpeed = 2.0f;

    [Tooltip("Max movement range")]
    [SerializeField] private float moveRangeX = 5.0f;

    [Tooltip("Height over last placed block")]
    [SerializeField] private float heightOffset = 6f;

    private BlockController currentBlock;
    private Vector3 initialPosition;
    private float currentSpeed;
    private float offsetX;
    private float targetY;

    private void Start()
    {
        initialPosition = transform.position;
        targetY = initialPosition.y;
        GameplayManager.OnScoreUpdated += CalculateDifficulty;
        GameplayManager.OnHeightUpdated += UpdateSpawnerHeight;

        CalculateDifficulty(0);
        SpawnNewBlock();
    }

    private void OnDestroy()
    {
        GameplayManager.OnScoreUpdated -= CalculateDifficulty;
        GameplayManager.OnHeightUpdated -= UpdateSpawnerHeight;
    }

    private void CalculateDifficulty(int score)
    {
        int lowThreshold = 50; // Minimum necesary highscore for dynamic difficlty
        int highThreshold = 1000; // Maximum highscore for dynamic difficlty

        if (score < lowThreshold)
        {
            currentSpeed = startMoveSpeed;
        }

        int highScore = DataManager.s_instance.highScore;

        currentSpeed = Mathf.Lerp(startMoveSpeed, maxMoveSpeed, (float)((float)score / (float)Mathf.Min((2 * highScore), highThreshold)));

        print($"HighScore: {highScore}. New speed: {currentSpeed}. Threshold: {(float)((float)score / (float)Mathf.Min(2 * highScore, highThreshold))}");
    }

    private void Update()
    {
        float lastDir = offsetX;
        MoveSpawner();

        if (currentBlock != null && !GameplayManager.s_instance.IsPaused && !GameplayManager.s_instance.IsGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                ReleaseBlock(new Vector3(10 * (offsetX - lastDir), 0.0f,0.0f));
            }
        }
    }

    private void MoveSpawner()
    {
        offsetX = Mathf.Sin(Time.time * currentSpeed) * moveRangeX;
        float currentY = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * smoothSpeed);

        transform.position = new Vector3(initialPosition.x + offsetX, currentY, transform.position.z);
    }

    private void UpdateSpawnerHeight(float newTowerHeight)
    {
        targetY = newTowerHeight + heightOffset;
    }

    private void SpawnNewBlock()
    {
        if (availableBlocks == null || availableBlocks.Count == 0)
        {
            Debug.LogError("No blocks available to spawn");
            return;
        }

        BlockData randomData = availableBlocks[Random.Range(0, availableBlocks.Count)];

        BlockData chosenData = ChooseBlockBasedOnWeight();

        GameObject newBlockObj = Instantiate(blockPrefab, spawnPoint.position, Quaternion.identity, transform);
        currentBlock = newBlockObj.GetComponent<BlockController>();
        currentBlock.Initialize(chosenData);
    }

    private BlockData ChooseBlockBasedOnWeight()
    {
        int totalWeight = 0;
        foreach (var block in availableBlocks)
        {
            totalWeight += block.spawnWeight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int currentSum = 0;

        foreach (var block in availableBlocks)
        {
            currentSum += block.spawnWeight;
            if (randomValue < currentSum)
            {
                return block;
            }
        }
        return availableBlocks[0];
    }

    private void ReleaseBlock(Vector3 dir)
    {
        currentBlock.transform.SetParent(dynamicElementsContainer);

        currentBlock.DropBlock(dir);

        currentBlock = null;

        Invoke(nameof(SpawnNewBlock), 1.0f);
    }
}