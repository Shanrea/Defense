using UnityEngine;

public class EdgeDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Knight knight = collision.gameObject.GetComponentInParent<Knight>();
        if (knight != null)
        {
            knight.FlipDirection();
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
