using UnityEngine;

[CreateAssetMenu(fileName = "NewWave", menuName = "Waves/WaveData")]
public class WaveDataSO : ScriptableObject
{
    [System.Serializable]
    public class EnemySpawnData
    {
        public EnemyDataSO enemyData;
        public int quantity;
        public float spawnRate; // cuántos por segundo
    }

    public EnemySpawnData[] enemiesInWave;
    public float duration; // cuánto dura la oleada
}
