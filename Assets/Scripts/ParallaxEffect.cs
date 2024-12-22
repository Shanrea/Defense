using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Camera cam;
    public Transform followTarget;

    Vector2 startingPosition;
    float startingZ;
    bool warningShown = false; // Variable pour vérifier si le warning a déjà été affiché

    Vector2 camMoveSinceStart => (Vector2)cam.transform.position - startingPosition;

    float zDistanceFromTarget => followTarget != null ? transform.position.z - followTarget.transform.position.z : 0f;

    float clippingPlane => cam.transform.position.z + (zDistanceFromTarget > 0 ? cam.farClipPlane : cam.nearClipPlane);

    float parallaxFactor => followTarget != null ? Mathf.Abs(zDistanceFromTarget) / clippingPlane : 0f;

    void Start()
    {
        startingPosition = transform.position;
        startingZ = transform.position.z;
    }

    void Update()
    {
        if (followTarget == null)
        {
            if (!warningShown)
            {
                Debug.LogWarning("Follow target is missing or has been destroyed.");
                warningShown = true; // Marquer que le warning a été affiché
            }
            return;
        }

        Vector2 newPosition = startingPosition + camMoveSinceStart * parallaxFactor;

        transform.position = new Vector3(newPosition.x, newPosition.y, startingZ);
    }
}
