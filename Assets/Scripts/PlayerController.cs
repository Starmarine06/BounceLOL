using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Force;
    public ParticleSystem click;
    public ParticleSystem boom;
    public Animator animator;

    private GameController gc;

    private Vector3 direction;
    public float gravity = -9.8f;
    public float strength = 5f;

    private void OnEnable()
    {
        Vector3 position = transform.position;
        position.y = 0;
        transform.position = position;
        direction = Vector3.zero;
    }
    void Awake()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (GameController.IsGameOver || GameController.IsGamePaused)
        {
            return;
        }

        if (TapPressed())
        {
            Flap();
        }
        direction.y += gravity * Time.deltaTime;
        transform.position += direction * Time.deltaTime;
    }

    // Touch and mouse are mutually exclusive: Unity also emulates a mouse click from a
    // touch on mobile, so checking both would flap twice per tap.
    private static bool TapPressed()
    {
        if (Input.touchCount > 0)
        {
            return Input.GetTouch(0).phase == TouchPhase.Began;
        }
        return Input.GetMouseButtonDown(0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Game Over");
            gc.GameOver();
        }
    }

    public void Flap()
    {
        click.transform.position = this.transform.position;
        click.Play();
        direction = Vector3.up * strength;
    }
}
