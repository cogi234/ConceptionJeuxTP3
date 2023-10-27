using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    int gameStage = 0;
    float gameTime = 0;

    //Gamestage stuff
    [SerializeField] Light skylight;
    [SerializeField] List<Material> skyboxesForGamestages;
    [SerializeField] List<float> lightLevelsForGamestages;
    [SerializeField] float lightRotation = 15;

    //Ui stuff
    [SerializeField] GameObject endGameCanvas, uiCanvas;
    [SerializeField] Text timerText, endgameTimerText, endgameText;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UpdateGameStage();
    }

    public void ToggleCursor()
    {
        if (Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void AugmentGameStage()
    {
        gameStage++;
        UpdateGameStage();

        if (gameStage >= skyboxesForGamestages.Count - 1)
        {
            GameOver("Victory!");
        }
    }

    private void UpdateGameStage()
    {
        skylight.intensity = lightLevelsForGamestages[gameStage];
        RenderSettings.skybox = skyboxesForGamestages[gameStage];

        skylight.transform.Rotate(lightRotation, 0, 0);
    }

    private void Update()
    {
        gameTime += Time.deltaTime;

        timerText.text = $"{Mathf.Floor(gameTime)} seconds";
    }

    public void GameOver(string endgameMessage)
    {
        Time.timeScale = 0;
        endGameCanvas.SetActive(true);
        uiCanvas.SetActive(false);

        endgameText.text = endgameMessage;
        endgameTimerText.text = $"You survived {Mathf.Floor(gameTime)} seconds";
    }
}
