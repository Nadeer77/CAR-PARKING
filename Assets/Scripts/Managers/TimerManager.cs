using System;
using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [Header("Timer Config")]
    public LevelTimerConfig timerConfig;  // ScriptableObject holding the time limit

    [Header("UI Display")]
    public TextMeshProUGUI timerText;

    private float currentTime;
    private bool isRunning = false;

    // Events (optional, for other scripts to subscribe)
    public event Action OnTimerStart;
    public event Action<float> OnTimerUpdate;
    public event Action OnTimerEnd;

    void Start()
    {
        // Start the timer with a short delay (optional, ensures everything is loaded)
        StartCoroutine(DelayedStart());
    }

    private System.Collections.IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.3f);
        StartTimer();
    }

    public void StartTimer()
    {
        if (timerConfig == null)
        {
            Debug.LogError("Timer Config not assigned!");
            return;
        }

        currentTime = timerConfig.timeLimit;
        isRunning = true;
        OnTimerStart?.Invoke();
    }

    public void StopTimer()
    {
        // Safely stop the timer (used when game is won, lost, or loading)
        isRunning = false;
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        OnTimerUpdate?.Invoke(currentTime);

        if (timerText != null)
            timerText.text = "Time: " + Mathf.Ceil(currentTime).ToString();

        if (currentTime <= 0)
        {
            isRunning = false;
            OnTimerEnd?.Invoke();
            TimeOver();
        }
    }

    private void TimeOver()
    {
        Debug.Log("⏰ Time is over!");
        UIManager.Instance.GameLoss();  // Show your GameOverPanel
    }
}