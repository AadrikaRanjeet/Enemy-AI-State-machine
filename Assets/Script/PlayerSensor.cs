using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    public Transform player;

    public float detectionRange = 10f;

    public bool useLineOfSight = true;

    public LayerMask obstacleMask;

    public bool PlayerDetected { get; private set; }

    private void Update()
    {
        PlayerDetected = DetectPlayer();
    }

    private bool DetectPlayer()
    {
        if (player == null)
        {
            return false;
        }

        // 1. Check distance
        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance > detectionRange)
        {
            return false;
        }

        // 2. If we don't want line-of-sight,
        // distance alone is enough.
        if (!useLineOfSight)
        {
            return true;
        }

        // 3. Check if a wall is between enemy and player
        Vector3 direction =
            (player.position - transform.position).normalized;

        if (Physics.Raycast(
            transform.position,
            direction,
            out RaycastHit hit,
            detectionRange,
            obstacleMask))
        {
            // Something on the obstacle layer was hit.
            // Therefore the player is hidden.
            return false;
        }

        return true;
    }
}