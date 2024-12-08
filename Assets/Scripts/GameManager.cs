using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int score = 0;
    public Transform planeTransform;
    public Rigidbody planeRigidbody;
    public TextMeshProUGUI timerText, scoreText, heightText, speedText, finalTimerText, finalScoreText;

    public GameObject gameOverPanel, gamePanel,pausePanel;
    public float elapsedTime = 0f;
    private bool isGameRunning = true;
    private bool isGamePaused = false;
    private const float metersToFeet = 3.28084f, mpsToKnots = 1.94384f;
    public AirplaneController AirplaneController;

    void Start()
    {

        gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
        isGameRunning = true;
        score = 0;
        elapsedTime = 0f;
    }

    void Update()
    {
        if (isGameRunning)
        {
            UpdateTimer();
            updateFlightData();
            scoreText.text = score.ToString();
        }
        else
        {
            finalTimerText.text = timerText.text;
            finalScoreText.text = scoreText.text;
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                Time.timeScale = 1f;
                isGamePaused = false;
                pausePanel.SetActive(false);
            }
            else
            {
                Time.timeScale = 0f;
                isGamePaused = true;
                pausePanel.SetActive(true);
            }
        }
    }

    private void UpdateTimer()
    {
        elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.SetText(string.Format("{0:00}:{1:00}", minutes, seconds));
    }
    private void updateFlightData()
    {
        float altitude = planeTransform.position.y * metersToFeet;

        float speed = AirplaneController.speed * mpsToKnots * 10;

        Debug.Log("Plane Velocity Magnitude: " + planeRigidbody.velocity.magnitude);
        Debug.Log("Calculated Speed in Knots: " + speed);

        heightText.text = altitude.ToString("F0");
        speedText.text = speed.ToString("F0");
    }

    public void EndGame()
    {
        isGameRunning = false;
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void TryAgain()
    {
        Debug.Log("Try Again button clicked");
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");

    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
        pausePanel.SetActive(false);
    }

}
