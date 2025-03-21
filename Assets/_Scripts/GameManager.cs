﻿using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : SingletonMonoBehavior<GameManager>
{
    [SerializeField] private static int maxLives = 3;
    public static int GetLives => maxLives;
    public static void SetLives(int l) => maxLives = l;
    [SerializeField] private Ball ball;
    [SerializeField] private Transform bricksContainer;
    [SerializeField] private ScoreCounterUI scoreCounter;
    public GameObject vfx_explosion;
    [SerializeField] private TextMeshProUGUI current;

    private int currentBrickCount;
    private int totalBrickCount;
    private static int score = 0;
    public static int GetScore => score;
    public static void SetScore(int s) => score = s;
    public GameObject gameOverUI;

    private void OnEnable()
    {
        InputHandler.Instance.OnFire.AddListener(FireBall);
        ball.ResetBall();
        totalBrickCount = bricksContainer.childCount;
        currentBrickCount = bricksContainer.childCount;
        current.SetText($"{maxLives}");
    }

    private void OnDisable()
    {
        InputHandler.Instance.OnFire.RemoveListener(FireBall);
    }

    private void FireBall()
    {
        ball.FireBall();
    }

    public void OnBrickDestroyed(Vector3 position)
    {
        // fire audio here
        // implemented particle effect here
        if (vfx_explosion != null)
        {
            GameObject effect = Instantiate(vfx_explosion, position, Quaternion.identity);
            Destroy(effect, 1.5f); // Destroy particle effect after 1.5 seconds
        }
        // add camera shake here
        CameraShake.Shake(0.5f, 0.24f);

        currentBrickCount--;
        Debug.Log($"Destroyed Brick at {position}, {currentBrickCount}/{totalBrickCount} remaining");
        score++;
        scoreCounter.UpdateScore(score);
        
        // update counters so that new game is correctly loaded
        if (score == 21)
        {
            score = 0;
            maxLives = 3;
        }
        if (currentBrickCount == 0)
        {
            SceneHandler.Instance.LoadNextScene();
        }
    }

    public void KillBall()
    {
        maxLives--;
        // update lives on HUD here
        // current.text = $"{maxLives}";
        current.SetText($"{GameManager.GetLives}");
        // game over UI if maxLives = 0, then exit to main menu after delay
        if (maxLives <= 0) {
            Destroy(ball.gameObject);
            Time.timeScale = 0;
            gameOverUI.SetActive(true);
            StartCoroutine(MainMenuTransition());
        }
        else {
            ball.ResetBall();
        }
    }

    private IEnumerator MainMenuTransition() {
        yield return new WaitForSecondsRealtime(1.5f);
        SceneHandler.Instance.LoadMenuScene();
        gameOverUI.SetActive(false);
        Time.timeScale = 1;
        maxLives = 3;
        score = 0;
    }
}
