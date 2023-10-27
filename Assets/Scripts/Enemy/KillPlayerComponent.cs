using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerComponent : MonoBehaviour
{
    GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
            gameManager.GameOver("Game Over!");
    }
}
