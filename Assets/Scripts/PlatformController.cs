using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] private float m_minYPos = -20;
    [SerializeField] private float weightFactor = 0.1f; // Facteur pour déterminer combien la plateforme descend par unité de poids
    [SerializeField] private float m_upSpeed = 5; // Vitesse de remontée lorsque la plateforme n'est pas chargée

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

    private void FixedUpdate()
    {
        float totalWeight = GetTotalWeight();

        float targetYPos = baseHeight - totalWeight * weightFactor;

        // Clamp la position pour s'assurer qu'elle reste dans les limites spécifiées
        targetYPos = Mathf.Clamp(targetYPos, m_minYPos, m_maxYPos);

        // Interpoler en douceur vers la nouvelle position, augmenter le facteur pour une remontée plus rapide
        float newYPos = Mathf.Lerp(transform.position.y, targetYPos, 0.5f * Time.fixedDeltaTime);
        rb.MovePosition(new Vector2(transform.position.x, newYPos));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            return; // Exclure le joueur
        }

        Knight knight = collision.gameObject.GetComponentInParent<Knight>();
        if (knight != null)
        {
            AddEnemyInContact(knight);

        }
        else
        {

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            return; // Exclure le joueur
        }

        Knight knight = collision.gameObject.GetComponentInParent<Knight>();
        if (knight != null)
        {
            RemoveEnemyInContact(knight);

        }
        else
        {

        }
    }

    private void AddEnemyInContact(Knight knight)
    {
        if (!m_currentEnemiesInContact.Contains(knight))
        {
            m_currentEnemiesInContact.Add(knight);
            Debug.Log($"Knight added to list: {knight.Weight}");
        }
        else
        {
            Debug.Log("Knight already in the list.");
        }
    }

    private void RemoveEnemyInContact(Knight knight)
    {
        if (m_currentEnemiesInContact.Contains(knight))
        {
            m_currentEnemiesInContact.Remove(knight);
            Debug.Log($"Knight removed from list: {knight.Weight}");
        }
        else
        {
            Debug.Log("Knight not found in the list.");
        }
    }

    private float GetTotalWeight()
    {
        float totalWeight = 0;
        foreach (Knight knight in m_currentEnemiesInContact)
        {
            totalWeight += knight.Weight;
        }
        return totalWeight;
    }
}
