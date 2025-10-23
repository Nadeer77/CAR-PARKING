using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panels")]
    public GameObject gameWinPanel;
    public GameObject gameOverPanel;
    public GameObject loadingPanel;
    public GameObject levelSelectionPanel;

    [Header("Loading UI")]
    public Slider loadingBar;
    public TextMeshProUGUI progressText;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Hide UI panels at start
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (gameWinPanel) gameWinPanel.SetActive(false);
        if (loadingPanel) loadingPanel.SetActive(false);
    }

    // ------------------------
    // BASIC SCENE CONTROLS
    // ------------------------
    public void Play() => LoadLevel(1);
    public void Exit() => Application.Quit();

    public void Back() => LoadLevel(1);
    public void MainMenu() => LoadLevel(0);

    public void Retry()
    {
        Time.timeScale = 1f;

        // Stop timer when retrying
        TimerManager timer = FindObjectOfType<TimerManager>();
        if (timer != null)
            timer.StopTimer();

        gameOverPanel?.SetActive(false);
        string currentLevel = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentLevel);
    }

    public void Next()
    {
        Time.timeScale = 1f;
        gameWinPanel?.SetActive(false);

        int current = SceneManager.GetActiveScene().buildIndex;
        int next = current + 1;

        if (next < SceneManager.sceneCountInBuildSettings)
            LoadLevel(next);
        else
        {
            Debug.Log("No more levels. Returning to Main Menu.");
            LoadLevel(0);
        }
    }

    // ------------------------
    // LEVEL SELECTION BUTTONS
    // ------------------------
    public void LevelOne() => LoadLevel(2);
    public void LevelTwo() => LoadLevel(3);
    public void LevelThree() => LoadLevel(4);
    public void LevelFour() => LoadLevel(5);

    // ------------------------
    // GAME STATE HANDLERS
    // ------------------------
    public void GameWon()
    {
        Time.timeScale = 0f;

        // Stop timer when game is won
        TimerManager timer = FindObjectOfType<TimerManager>();
        if (timer != null)
            timer.StopTimer();

        gameWinPanel?.SetActive(true);
    }

    public void GameLoss()
    {
        Time.timeScale = 0f;

        // Stop timer when game is lost
        TimerManager timer = FindObjectOfType<TimerManager>();
        if (timer != null)
            timer.StopTimer();

        gameOverPanel?.SetActive(true);
    }

    // ------------------------
    // ASYNC LOADING SYSTEM
    // ------------------------
    public void LoadLevel(int sceneIndex)
    {
        Time.timeScale = 1f;

        // Stop timer before loading new level
        TimerManager timer = FindObjectOfType<TimerManager>();
        if (timer != null)
            timer.StopTimer();

        // Hide level selection panel
        if (levelSelectionPanel != null)
            levelSelectionPanel.SetActive(false);

        // Show loading screen
        if (loadingPanel == null || loadingBar == null || progressText == null)
        {
            Debug.LogWarning("Loading UI references missing! Assign them in the Inspector.");
            SceneManager.LoadScene(sceneIndex);
            return;
        }

        loadingPanel.SetActive(true);
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    private IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.value = progress;
            progressText.text = $"{(progress * 100f):F0}%";

            if (operation.progress >= 0.9f)
            {
                progressText.text = "Press any key to continue...";
                if (Input.anyKeyDown)
                    operation.allowSceneActivation = true;
            }

            yield return null;
        }

        // Hide loading screen after scene loads
        loadingPanel.SetActive(false);
    }
}