using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour, IInteractable
{
    GameManager gameManager;
    Light orbLight;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        orbLight = GetComponentInChildren<Light>();
        GetComponent<ParticleSystem>().Stop();
    }

    public void Interact()
    {
        transform.tag = "Untagged";

        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(4.5f);
        orbLight.enabled = false;
        gameManager.AugmentGameStage();
    }
}
