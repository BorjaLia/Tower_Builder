using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [Header("Spawner config")]

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

    [Tooltip("Percentage of applied speed while dropping")]
    [SerializeField] private float dropSpeedCushioning = 1.0f;
    [Tooltip("Max movement range")]
    [SerializeField] private float moveRangeX = 5.0f;

    private BlockController currentBlock;
    private float currentSpeed;
    private float offsetX;
    private float initialLocalX;

    private void Start()
    {
        initialLocalX = transform.localPosition.x;

        GameplayManager.OnScoreUpdated += CalculateDifficulty;
        GameplayManager.OnTowerStabilized += SpawnNewBlock;

        CalculateDifficulty(0);
        SpawnNewBlock();
    }

    private void OnDestroy()
    {
        GameplayManager.OnScoreUpdated -= CalculateDifficulty;
        GameplayManager.OnTowerStabilized -= SpawnNewBlock;
    }

    private void CalculateDifficulty(int score)
    {
        // Sugestion: estos umbrales son numeros magicos; conviene exponerlos como [SerializeField].
        int lowThreshold = 50; // Minimum necesary highscore for dynamic difficlty
        int highThreshold = 1000; // Maximum highscore for dynamic difficlty

        if (score < lowThreshold)
        {
            currentSpeed = startMoveSpeed;
            return;
        }

        int highScore = DataManager.s_instance.highScore;

        highScore = Mathf.Max(20, highScore);

        float difficultyPercentage = (float)score / Mathf.Min((2 * highScore), highThreshold);

        currentSpeed = Mathf.Lerp(startMoveSpeed, maxMoveSpeed, difficultyPercentage);

        print($"HighScore: {highScore}. New speed: {currentSpeed}. Threshold: {(float)((float)score / (float)Mathf.Min(2 * highScore, highThreshold))}");
    }

    private void Update()
    {
        float previousX = transform.position.x;

        MoveSpawner();

        float currentX = transform.position.x;

        // Warning: lectura de Input mezclada con la logica del spawner en Update; convendria separar el input en su propia clase/InputSystem para desacoplar.
        if (currentBlock != null && !GameplayManager.s_instance.IsPaused && !GameplayManager.s_instance.IsGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                float currentVelocityX = (currentX - previousX) / Time.deltaTime;
                Vector3 inheritedVelocity = new Vector3(currentVelocityX * dropSpeedCushioning, 0.0f, 0.0f);

                ReleaseBlock(inheritedVelocity);
            }
        }
    }

    private void MoveSpawner()
    {
        offsetX = Mathf.Sin(Time.time * currentSpeed) * moveRangeX;

        transform.localPosition = new Vector3(initialLocalX + offsetX, transform.localPosition.y, transform.localPosition.z);
    }
    private void SpawnNewBlock()
    {
        if (availableBlocks == null || availableBlocks.Count == 0) return;

        BlockData chosenData = ChooseBlockBasedOnWeight();

        GameObject newBlockObj = Instantiate(chosenData.blockModel, spawnPoint.position, Quaternion.identity, transform);
        // Sugestion: el modelo del BlockData deberia traer ya el BlockController en el prefab; el AddComponent en runtime indica un prefab incompleto.
        currentBlock = newBlockObj.GetComponent<BlockController>();

        if (currentBlock == null)
        {
            currentBlock = newBlockObj.AddComponent<BlockController>();
        }

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

    private void ReleaseBlock(Vector3 inheritedVelocity)
    {
        GameplayManager.s_instance.RegisterNewBlockDrop(currentBlock);

        currentBlock.transform.SetParent(dynamicElementsContainer);
        currentBlock.DropBlock(inheritedVelocity);

        currentBlock = null;
    }
}