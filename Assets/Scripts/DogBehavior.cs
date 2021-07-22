using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DogBehavior : MonoBehaviour
{
    public float speed;
    private float MoveSpeed = 2f;
    private float DetectionRange = 2f;
    private Rigidbody2D DogRB;
    private SpriteRenderer DogSR;
    private Vector2 movement;
    private LayerMask layerMask;
    private Vector2 originalPosition;
    private bool isChasing;
    private bool stopDetection;
    private GameObject tooltip_alert;
    private GameObject tooltip_question;
    private GameObject tooltip_success;

    private Animator tooltipAnimator;
    private Animator dogLegsAnimator;

    private AudioClip sound_alert;
    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        DogRB = GetComponent<Rigidbody2D>();
        DogSR = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        layerMask = LayerMask.GetMask("Default", "Player", "Exit", "Ladder");
        originalPosition = transform.position;
        tooltip_alert = gameObject.transform.GetChild(0).gameObject;
        dogLegsAnimator = gameObject.transform.Find("DogLegs").gameObject.GetComponent<Animator>();
        tooltipAnimator = tooltip_alert.GetComponent<Animator>();
        sound_alert = Resources.Load("Audio/alert") as AudioClip;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            stopDetection = true;
        }
    }

    private void StartChasing()
    {
        if (isChasing) return;

        audioSource.clip = sound_alert;
        audioSource.Play();
     

        isChasing = true;
        dogLegsAnimator.SetBool("IsWalking", true);

        tooltipAnimator.SetBool("IsAlert", true);
    }

    private void StopChasing()
    {
        if (isChasing == false) return;
        isChasing = false;
        dogLegsAnimator.SetBool("IsWalking", false);
        tooltipAnimator.SetBool("IsAlert", false);

    }


    private void FixedUpdate()
    {

      
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, new Vector2(-1, 0), DetectionRange, layerMask);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, new Vector2(1, 0), DetectionRange, layerMask);

        if (hitLeft.collider != null && hitLeft.collider.tag == "Player" )
        {
            //Debug.Log("Player spotted left");
            movement.x = -0.4f;
            StartChasing();
          

        } else if (hitRight.collider != null &&  hitRight.collider.tag == "Player")
        {
            //Debug.Log("Player spotted right");
            movement.x = 0.4f;
            StartChasing();

        } else
        {
            movement.x = 0;
            StopChasing();
        }

        // flip sprite
        if (movement.x > 0)
        {
            DogSR.flipX = false;
        } else if (movement.x < 0)
        {
            DogSR.flipX = true;
        }

        DogRB.MovePosition(DogRB.position + movement * MoveSpeed * Time.fixedDeltaTime);
    }
}
