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
    public static event Action OnTowerStabilized;

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
    [SerializeField] private float basePlatformY = 0.0f;

    [Tooltip("Max active blocks (lock older blocks in place)")]
    [SerializeField] private int maxActivePhysicsBlocks = 5;

    [Tooltip("How close (in X) a block needs to be to count as perfect")]
    [SerializeField] private float perfectPlacementTolerance = 0.15f;

    // List of placed blocks
    private List<BlockController> activeTowerBlocks = new List<BlockController>();
    public BlockController currentFallingBlock { get; private set; }

    private const string MAIN_MENU_SCENE_NAME = "MainMenuScene";

    private bool isCheckingStability = false;
    private float stabilityTimer = 0f;
    private const float STABILITY_DELAY = 0.1f;

    private bool hasLostLifeThisDrop = false;
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

    private void Update()
    {
        if (isCheckingStability && !IsGameOver && !IsPaused)
        {
            stabilityTimer -= Time.deltaTime;

            if (stabilityTimer <= 0f)
            {
                if (IsTowerStable())
                {
                    isCheckingStability = false;
                    RecalculateTowerHeight();
                    OnTowerStabilized?.Invoke();
                }
            }
        }
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
    public void RegisterNewBlockDrop(BlockController block)
    {
        currentFallingBlock = block;
        hasLostLifeThisDrop = false;
    }

    public void OnBlockHitBase(BlockController block)
    {
        if (IsGameOver) return;

        if (activeTowerBlocks.Count == 0)
        {
            if (currentFallingBlock == block)
            {
                AcceptValidBlock(block, true);
            }
        }
        else
        {
            if (activeTowerBlocks.Count > 0 && activeTowerBlocks[0] == block) return;

            OnBlockHitDeathZone(block);
        }
    }

    public void OnBlockLanded(BlockController block, string hitTag)
    {
        if (IsGameOver) return;

        if (hitTag == "Block")
        {
            if (block == currentFallingBlock)
            {
                bool isPerfect = CheckPerfectPlacement(block);
                AcceptValidBlock(block, isPerfect);
            }
        }
    }

    public void OnBlockHitDeathZone(BlockController block)
    {
        if (IsGameOver) return;

        bool isCurrentOrTop = (block == currentFallingBlock) ||
                              (activeTowerBlocks.Count > 0 && block == activeTowerBlocks[activeTowerBlocks.Count - 1]);

        if (activeTowerBlocks.Contains(block))
        {
            activeTowerBlocks.Remove(block);
        }

        if (block != null) Destroy(block.gameObject);

        if (isCurrentOrTop)
        {
            if (!hasLostLifeThisDrop)
            {
                LoseLife();
                hasLostLifeThisDrop = true;
            }

            if (block == currentFallingBlock)
            {
                currentFallingBlock = null;
                stabilityTimer = STABILITY_DELAY;
                isCheckingStability = true;
            }
        }

        RecalculateTowerHeight();
    }

    private bool CheckPerfectPlacement(BlockController newBlock)
    {
        if (activeTowerBlocks.Count == 0) return true;

        BlockController topBlock = activeTowerBlocks[activeTowerBlocks.Count - 1];
        float differenceX = Mathf.Abs(newBlock.transform.position.x - topBlock.transform.position.x);

        if (differenceX <= perfectPlacementTolerance)
        {
            newBlock.transform.position = new Vector3(topBlock.transform.position.x, newBlock.transform.position.y, newBlock.transform.position.z);
            newBlock.rb.linearVelocity = Vector3.zero;
            newBlock.rb.angularVelocity = Vector3.zero;
            return true;
        }

        return false;
    }

    private void AcceptValidBlock(BlockController block, bool isPerfectPlacement)
    {
        currentFallingBlock = null;
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

        if (activeTowerBlocks.Count > maxActivePhysicsBlocks)
        {
            BlockController oldestBlock = activeTowerBlocks[activeTowerBlocks.Count - maxActivePhysicsBlocks - 1];
            if (oldestBlock != null && oldestBlock.rb != null)
            {
                oldestBlock.rb.isKinematic = true;
            }
        }

        stabilityTimer = STABILITY_DELAY;
        isCheckingStability = true;
    }

    private bool IsTowerStable()
    {
        int startIndex = Mathf.Max(0, activeTowerBlocks.Count - maxActivePhysicsBlocks);

        for (int i = startIndex; i < activeTowerBlocks.Count; i++)
        {
            BlockController block = activeTowerBlocks[i];
            if (block != null && block.rb != null && !block.rb.isKinematic)
            {
                if (block.rb.linearVelocity.magnitude > 0.1f || block.rb.angularVelocity.magnitude > 0.1f)
                {
                    return false;
                }
            }
        }

        return true;
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
                if (block.transform.position.y > highestY)
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