using UnityEngine;

public class DestructionZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.gameObject.GetComponentInParent<Damageable>();
        if (damageable != null)
        {
            // V�rifie si l'objet en collision est le joueur
            PlayerController player = collision.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                if (player.IsAlive)
                {
                    player.OnDeath(); // Appelle la m�thode de mort dans le script du joueur
                    Debug.Log("Player death animation played.");
                }
            }
            else
            {
                Destroy(damageable.gameObject, damageable.delay); // D�lai pour la destruction de l'objet
                Debug.Log("Knight destroyed.");
            }
        }
    }
}
