
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;
using UnityEngine.UI;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    public List<WaveDataSO> waves;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 5f;

    private int currentWaveIndex = 0;
    private bool spawning = false;
    public TextMeshProUGUI waveText;

    void Start()
    {
        spawnPoints = GetComponentsInChildren<Transform>();
        StartCoroutine(StartNextWave());
    }

    IEnumerator StartNextWave()
    {
        UpdateWaveText();
        yield return new WaitForSeconds(timeBetweenWaves);

        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("Todas las oleadas completadas");
            yield break;
        }

        WaveDataSO wave = waves[currentWaveIndex];
        spawning = true;

        foreach (var enemyData in wave.enemiesInWave)
        {
            StartCoroutine(SpawnEnemies(enemyData));
        }

        yield return new WaitForSeconds(wave.duration);

        spawning = false;
        currentWaveIndex++;

        StartCoroutine(StartNextWave());
    }

    IEnumerator SpawnEnemies(WaveDataSO.EnemySpawnData spawnData)
    {
        int spawned = 0;
        while (spawned < spawnData.quantity)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy = PoolManager.Instance.Get(spawnData.enemyData.enemyPrefab, spawnPoint.position, Quaternion.identity);

            var health = enemy.GetComponent<Health>();
            if (health != null)
                health.SetHealth(spawnData.enemyData.health);

            // Aquí puedes setear speed, daño, AI si quieres

            spawned++;
            yield return new WaitForSeconds(1f / spawnData.spawnRate);
        }
    }
    void UpdateWaveText()
    {
        if (waveText != null)
        {
            waveText.text = "Wave " + (currentWaveIndex + 1);
        }
    }
}
