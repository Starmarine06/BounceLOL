using UnityEngine;

public class Boundary : MonoBehaviour
{
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            GameObject obstacle = other.gameObject;
            // An obstacle with several colliders can exit more than once; only recycle it while
            // active so it is never enqueued twice.
            if (!obstacle.activeSelf) return;
            obstacle.SetActive(false);
            ObstacleGenerator.Obstacles.Enqueue(obstacle);
        }
        else if (other.CompareTag("Player"))
        {
            // Destroying the player here left the run going with nothing to lose. End the game.
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null) player.Kill();
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
