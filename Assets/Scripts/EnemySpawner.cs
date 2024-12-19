using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Knight m_enemyPrefab;
    [SerializeField] private Vector2 m_spawnRate;
    [SerializeField] private BoxCollider2D m_spawnArea;
    [SerializeField] private AnimationCurve m_spawnRateMultiplier;

    private float m_timer = 0;
    private float m_cooldown = 0;

    private void Start()
    {
        SetCooldown();
    }

    private void Update()
    {
        m_timer += Time.deltaTime;

        if (m_timer >= m_cooldown)
        {
            SpawnEnemy();
            m_timer = 0;
            SetCooldown();
        }
    }

    private void SpawnEnemy()
    {
        Knight newEnemy = Instantiate(m_enemyPrefab, transform);
        newEnemy.transform.position = RandomPointInBounds(m_spawnArea.bounds);
    }

    private void SetCooldown()
    {
        // Suppression de l'utilisation de m_timerController
        float multiplier = m_spawnRateMultiplier.Evaluate(m_timer); // Utilisation de m_timer au lieu de m_timerController
        m_cooldown = Random.Range(m_spawnRate.x, m_spawnRate.y) * multiplier;
    }

    public static Vector2 RandomPointInBounds(Bounds bounds)
    {
        return new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
        );
    }
}
