using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEndUI : MonoBehaviour
{
    [SerializeField] private GameObject gameEndPanel;
    [SerializeField] private TMP_Text resultTitle;
    [SerializeField] private TMP_Text finalScore;
    [SerializeField] private Button newGameButton;

    private void Awake()
    {
        Time.timeScale = 1f;
        gameEndPanel.SetActive(false);
        newGameButton.onClick.AddListener(RestartGame);
    }

    private void OnEnable()
    {
        GameEvents.LivesChanged += CheckGameOver;
        GameEvents.BrickDestroyed += CheckWin;
    }

    private void OnDisable()
    {
        GameEvents.LivesChanged -= CheckGameOver;
        GameEvents.BrickDestroyed -= CheckWin;
    }

    private void CheckGameOver(int lives)
    {
        if (lives <= 0)
        {
            ShowResult("GAME OVER");
        }
    }

    private void CheckWin(ImpactInfo info)
    {
        StartCoroutine(CheckRemainingBricks());
    }

    private IEnumerator CheckRemainingBricks()
    {
        yield return null;

        if (FindObjectsByType<Brick>(
            FindObjectsSortMode.None).Length == 0)
        {
            ShowResult("YOU WIN!");
        }
    }

    private void ShowResult(string title)
    {
        if (gameEndPanel.activeSelf) return;

        int currentScore = GameManager.Instance.Score;
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);

        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
        }

        Time.timeScale = 0f;

        resultTitle.text = title;
        finalScore.text =
            "SCORE: " + currentScore +
            "\nBEST SCORE: " + bestScore;

        gameEndPanel.SetActive(true);
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex);
    }
}
