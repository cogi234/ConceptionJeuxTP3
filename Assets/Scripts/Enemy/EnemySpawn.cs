using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float distanceToSpawn = 10;
    [SerializeField] GameObject spawnText;
    Transform player;

    bool firstSpawn = true;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(SpawnEnemy());
    }

    public IEnumerator SpawnEnemy()
    {
        //We only spawn the enemy if the player is far enough
        while (true)
        {
            if (Vector3.Distance(player.position, transform.position) >= distanceToSpawn)
            {
                Instantiate(enemyPrefab, transform.position, transform.rotation, transform.parent);

                if (firstSpawn)
                {
                    spawnText.SetActive(true);
                    firstSpawn = false;
                }
                break;
            }

            yield return null;
        }
    }
}
