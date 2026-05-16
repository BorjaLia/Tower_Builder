using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : Singleton<GameplayManager>
{
    // Events
    public static event Action<int> OnScoreUpdated;
    public static event Action<int> OnStrikeUpdated;
    public static event Action<float> OnHeightUpdated;
    public static event Action<int> OnLivesUpdated;
    public static event Action OnGameOver;
    public static event Action<bool> OnPauseToggled;

    [Header("Game State")]
    public bool IsGameOver { get; private set; }
    public bool IsPaused { get; private set; }

    private int currentScore;
    private int currentStrikes;
    private float currentHeight;
    private int currentLives;


    [Header("Game settings")]

    [Tooltip("Starting Lives")]
    [SerializeField] private int startingLives = 3;

    [Tooltip("Base starting position")]
    [SerializeField] private float basePlatformY = -3f;

    [Tooltip("Max active blocks (lock older blocks in place)")]
    [SerializeField] private int maxActivePhysicsBlocks = 5;

    // List of placed blocks
    private List<BlockController> activeTowerBlocks = new List<BlockController>();

    private const string MAIN_MENU_SCENE_NAME = "MainMenuScene";

    protected override void Awake()
    {
        base.Awake();
        currentScore = 0;
        currentStrikes = 0;
        currentLives = startingLives;
        currentHeight = basePlatformY;
        IsGameOver = false;
        IsPaused = false;

        Time.timeScale = 1.0f;
    }

    private void Start()
    {
        OnScoreUpdated?.Invoke(currentScore);
        OnStrikeUpdated?.Invoke(currentStrikes);
        OnHeightUpdated?.Invoke(currentHeight);
        OnLivesUpdated?.Invoke(currentLives);
    }

    public void LoseLife()
    {
        if (IsGameOver) return;

        currentLives--;
        currentStrikes = 0;

        OnLivesUpdated?.Invoke(currentLives);
        OnStrikeUpdated?.Invoke(currentStrikes);

        if (currentLives <= 0)
        {
            EndGame();
        }
    }

    public void TogglePause()
    {
        if (IsGameOver) return;

        IsPaused = !IsPaused;

        Time.timeScale = IsPaused ? 0.0f : 1.0f;

        OnPauseToggled?.Invoke(IsPaused);
    }

    public void BlockLanded(BlockController block, bool isPerfectPlacement)
    {
        if (IsGameOver) return;

        activeTowerBlocks.Add(block);

        int points = block.currentData.scoreValue;
        if (isPerfectPlacement)
        {
            points = Mathf.RoundToInt(points * block.currentData.perfectMultiplier);
            currentStrikes++;
        }
        else
        {
            currentStrikes = 0;
        }

        currentScore += points;
        OnScoreUpdated?.Invoke(currentScore);
        OnStrikeUpdated?.Invoke(currentStrikes);

        RecalculateTowerHeight();

        // Freze any block older than the limit
        if (activeTowerBlocks.Count > maxActivePhysicsBlocks)
        {
            int oldestIndex = activeTowerBlocks.Count - maxActivePhysicsBlocks - 1;
            BlockController oldestBlock = activeTowerBlocks[oldestIndex];

            if (oldestBlock != null)
            {
                Rigidbody rb = oldestBlock.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;
            }
        }
    }

    public void BlockFellFromTower(BlockController fallenBlock)
    {
        if (IsGameOver) return;

        bool isLastBlock = (BlockController.lastPlacedBlock == fallenBlock.transform);

        if (activeTowerBlocks.Contains(fallenBlock))
        {
            activeTowerBlocks.Remove(fallenBlock);

            if (isLastBlock)
            {
                if (activeTowerBlocks.Count > 0)
                {
                    BlockController.lastPlacedBlock = activeTowerBlocks[activeTowerBlocks.Count - 1].transform;
                }
                else
                {
                    BlockController.lastPlacedBlock = null;
                    isLastBlock = false;
                }
            }
        }

        if (isLastBlock)
        {
            LoseLife();
        }

        RecalculateTowerHeight();
    }


    private void RecalculateTowerHeight()
    {
        activeTowerBlocks.RemoveAll(block => block == null);

        if (activeTowerBlocks.Count == 0)
        {
            currentHeight = basePlatformY;
        }
        else
        {
            float highestY = basePlatformY;
            foreach (var block in activeTowerBlocks)
            {
                if (block != null && block.transform.position.y > highestY)
                {
                    highestY = block.transform.position.y;
                }
            }
            currentHeight = highestY;
        }

        OnHeightUpdated?.Invoke(currentHeight);
    }

    public void EndGame()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        print($"Game Ended! Score: {currentScore}");

        DataManager.s_instance.SaveHighScore(currentScore);

        OnGameOver?.Invoke();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE_NAME);
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}