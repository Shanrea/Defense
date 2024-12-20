using UnityEngine;

public class DestructionZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {

        Damageable knight = collision.gameObject.GetComponentInParent<Damageable>();
        if (knight != null)
        {
            knight.damageableHit.Invoke(1000, Vector2.up);
            //Destroy(knight.gameObject);

            //Debug.Log("Knight Destroyed");
        }
    }

}
