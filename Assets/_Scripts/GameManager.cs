using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : SingletonMonoBehavior<GameManager>
{
    [SerializeField] private int maxLives = 3;
    [SerializeField] private Ball ball;
    [SerializeField] private Transform bricksContainer;
    [SerializeField] private ScoreCounterUI scoreCounter;
    [SerializeField] private TextMeshProUGUI current;

    private int currentBrickCount;
    private int totalBrickCount;
    private int score = 0;
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
        // implement particle effect here
        // add camera shake here
        currentBrickCount--;
        Debug.Log($"Destroyed Brick at {position}, {currentBrickCount}/{totalBrickCount} remaining");
        score++;
        scoreCounter.UpdateScore(score);
        if (currentBrickCount == 0)
        {
            SceneHandler.Instance.LoadNextScene();
        }
    }

    public void KillBall()
    {
        maxLives--;
        // update lives on HUD here
        current.text = $"{maxLives}";
        // game over UI if maxLives < 0, then exit to main menu after delay
        if (maxLives < 0) {
            // Freeze time and show game over UI
            Time.timeScale = 0;
            // ShowGameOverUI();
            // Start coroutine to transition to main menu
            StartCoroutine(GameOverSequence());
        }
        else {
            ball.ResetBall();
        }
    }

    private IEnumerator GameOverSequence() {
        yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 1;
        SceneHandler.Instance.LoadMenuScene();
    }
}
