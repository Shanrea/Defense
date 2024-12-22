using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] private float m_minYPos = -20;
    [SerializeField] private float weightFactor = 0.01f; // Facteur pour déterminer combien la plateforme descend par unité de poids
    [SerializeField] private float lerpSpeed = 0.05f; // Vitesse d'interpolation pour une descente plus douce
    [SerializeField] private float upSpeed = 0.1f; // Vitesse de remontée de la plateforme

    private float m_maxYPos = 0;
    private float baseHeight;
    private List<Knight> m_currentEnemiesInContact = new List<Knight>();
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic; // Assurez-vous que le Rigidbody est défini sur Dynamic
        rb.gravityScale = 0; // Désactive la gravité
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation; // Empêche la rotation et le mouvement sur l'axe X
    }

    private void Start()
    {
        m_maxYPos = transform.position.y;
        baseHeight = transform.position.y; // Initialise baseHeight à la position initiale de la plateforme
    }

    private void Update()
    {
        float totalWeight = GetTotalWeight();
        Debug.Log($"Total Weight: {totalWeight}");

        float targetYPos;
        if (totalWeight > 0)
        {
            targetYPos = baseHeight - (totalWeight * weightFactor);
        }
        else
        {
            targetYPos = m_maxYPos; // Remonte à la position initiale si aucun ennemi n'est présent
        }

        // Clamp la position pour s'assurer qu'elle reste dans les limites spécifiées
        targetYPos = Mathf.Clamp(targetYPos, m_minYPos, m_maxYPos);

        // Interpoler en douceur vers la nouvelle position
        float newYPos = Mathf.Lerp(transform.position.y, targetYPos, totalWeight > 0 ? lerpSpeed * Time.deltaTime : upSpeed * Time.deltaTime);
        rb.MovePosition(new Vector2(transform.position.x, newYPos));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            return; // Exclure le joueur
        }

        Knight knight = collision.gameObject.GetComponentInParent<Knight>();
        if (knight != null && !m_currentEnemiesInContact.Contains(knight))
        {
            AddEnemyInContact(knight);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            return; // Exclure le joueur
        }

        Knight knight = collision.gameObject.GetComponentInParent<Knight>();
        if (knight != null && m_currentEnemiesInContact.Contains(knight))
        {
            RemoveEnemyInContact(knight);
        }
    }

    private void AddEnemyInContact(Knight knight)
    {
        m_currentEnemiesInContact.Add(knight);
        Debug.Log($"Knight added to list: {knight.Weight}");
    }

    private void RemoveEnemyInContact(Knight knight)
    {
        m_currentEnemiesInContact.Remove(knight);
        Debug.Log($"Knight removed from list: {knight.Weight}");
    }

    private float GetTotalWeight()
    {
        float totalWeight = 0;
        foreach (Knight knight in m_currentEnemiesInContact)
        {
            totalWeight += knight.Weight;
            Debug.Log($"Knight Weight: {knight.Weight}");
        }
        Debug.Log($"Computed Total Weight: {totalWeight}");
        return totalWeight;
    }

    private void LateUpdate()
    {
        // Met à jour la liste des ennemis présents à la fin de chaque frame pour une meilleure détection
        m_currentEnemiesInContact.RemoveAll(knight => knight == null);
    }
}
