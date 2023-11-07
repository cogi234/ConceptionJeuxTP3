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
    MoveToPoint skylightMovement;
    [SerializeField] List<float> lightLevelsForGamestages;
    [SerializeField] List<Vector3> lightPositionsForGamestages;

    //Ui stuff
    [SerializeField] GameObject endGameCanvas, uiCanvas;
    [SerializeField] Text timerText, endgameTimerText, endgameText;

    private void Awake()
    {
        skylightMovement = skylight.transform.parent.GetComponent<MoveToPoint>();
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

        if (gameStage >= lightLevelsForGamestages.Count - 1)
        {
            GameOver("Victory!");
        }
    }

    private void UpdateGameStage()
    {
        skylight.intensity = lightLevelsForGamestages[gameStage];
        skylightMovement.target = lightPositionsForGamestages[gameStage];
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
