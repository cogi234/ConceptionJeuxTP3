using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    [SerializeField] float timeVisible = 2;
    [SerializeField] float timeToFade = 2;

    TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        Color color = text.color;
        color.a = 1;
        text.color = color;
        StartCoroutine(StartFading());
    }

    private IEnumerator StartFading()
    {
        yield return new WaitForSeconds(timeVisible);

        float timer = timeToFade;
        Color color = text.color;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, timer / timeToFade);
            text.color = color;

            yield return null;
        }

        gameObject.SetActive(false);
    }
}
