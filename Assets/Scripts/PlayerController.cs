using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Force;
    public ParticleSystem click;
    public ParticleSystem boom;
    public Animator animator;

    private Rigidbody2D rb;
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
        //rb = GetComponent<Rigidbody2D>();
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        animator = GetComponent<Animator>();
        //animator.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Flap();
        }
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Flap();
            }
        }
        direction.y += gravity * Time.deltaTime;
        transform.position += direction * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Game Over");
            gc.GameOver();
            //boom.transform.position = this.transform.position;
            //boom.Play();
        }
    }

    public void Flap()
    {
        //animator.enabled = true;
        click.transform.position = this.transform.position;
        click.Play();
        direction = Vector3.up * strength;
        //animator.Play("Base Layer.rotation", 0, 0);
        //Debug.Log(direction);

    }
}
