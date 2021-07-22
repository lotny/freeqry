using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class PlayerBehavior : MonoBehaviour
{
    public PlayerInput playerInput;

    private AudioClip sound_goDown;
    private AudioClip sound_goUp;
    private AudioClip sound_padlock;
    private AudioClip sound_free;
    private AudioClip sound_win;
    private AudioClip sound_death;

    private AudioSource audioSource;

    private int nextLevel;

    private Gamepad gamepad;
    private Keyboard keyboard;
    private Vector2 movement;
    private bool isNearLadder;
    private bool isNearPadlock;
    private bool IsNearExit;
    private bool isBelowGround;

    private bool isDownPressed;
    private bool isUpPressed;

    private bool isAboveLadder;
    private bool isBelowLadder;

    private Transform ProgressBar;

    private GameObject ActiveLadder;
    private GameObject ActivePadlock;

    private Animator playerAC;
    private Animator playerLegsAC;
    private Rigidbody2D playerRB;
    private float MoveSpeed = 2f;
    private SpriteRenderer playerSR;
    private bool isGrounded;
    private bool jumpPressed;
    private bool interactionPressed;
    private bool isInteracting;
    private bool isDead;
    private float LockPickProgress;

    void Start()
    {
        ResetProgress();
        isGrounded = true;
        isInteracting = false;
        Application.targetFrameRate = 60;
        playerRB = GetComponent<Rigidbody2D>();
        playerSR = GetComponentInChildren<SpriteRenderer>();
        playerAC = GetComponentInChildren<Animator>();
        playerLegsAC = transform.Find("FoxSprite/FoxLegs").gameObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        sound_goUp = (AudioClip)Resources.Load("Audio/go_up");
        sound_goDown = Resources.Load("Audio/go_down") as AudioClip;
        sound_padlock = Resources.Load("Audio/padlock") as AudioClip;
        sound_win = Resources.Load("Audio/win") as AudioClip;
        sound_death = Resources.Load("Audio/death_4") as AudioClip;

        if (SceneManager.GetActiveScene().name == "End")
        {
            audioSource.clip = sound_win;
            audioSource.volume = 1f;
            audioSource.Play();
        } else if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            audioSource.clip = sound_goUp;
            audioSource.volume = 1f;
            audioSource.Play();
        }
    }


    public void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>();
        if (inputMovement.x > 0)
        {
            inputMovement.x = 1;
        } else if (inputMovement.x < 0)
        {
            inputMovement.x = -1;
        } else
        {
            inputMovement.x = 0;
        }

        movement.x = inputMovement.x;
       
        if (inputMovement.y > 0)
        {
            isUpPressed = true;
            isDownPressed = false;
        } else if (inputMovement.y < 0)
        {
            isUpPressed = false;
            isDownPressed = true;
        } else
        {
            isUpPressed = false;
            isDownPressed = false;
        }
    }

    void Update()
    {
        //old input
        //movement.x = Input.GetAxisRaw("Horizontal");


        // pressed down near ladder
        //if (Input.GetAxisRaw("Vertical") < 0 && isNearLadder && isAboveLadder)
        if (isDownPressed && isNearLadder && isAboveLadder)
        {
            GoDownLadder();


        }

        // pressed up near exit
        //if ((Input.GetAxisRaw("Vertical") > 0 || Input.GetKeyDown(KeyCode.Space)) && IsNearExit  )
        if ((isUpPressed || interactionPressed) && IsNearExit)
        {
            GoToNextLevel();

        }

        // pressed up near ladder
        if (isUpPressed && isNearLadder && isBelowLadder)
           // if (Input.GetAxisRaw("Vertical") > 0 && isNearLadder && isBelowLadder)
            {
            GoUpLadder();
     
        }
     
    }

    public void OnInteraction(InputAction.CallbackContext value)
    {
        if (value.canceled)
        {
            interactionPressed = false;
            if (audioSource.clip == sound_padlock) audioSource.Stop();
        } else if (value.performed)
        {
            interactionPressed = true;
        }
    }

    private void LockPickPadlock()
    {
        if (LockPickProgress == 0)
        {
            audioSource.clip = sound_padlock;
            audioSource.volume = 1f;
            audioSource.Play();
        }
        if (LockPickProgress <= 40)
        {
            LockPickProgress += 0.5f; // 0.12

            var newScale = new Vector3(LockPickProgress / 10, 1f);
            ProgressBar.localScale = newScale;

            
        } else {
            Debug.Log("Lock was picked!");
            ProgressBar.localScale = new Vector3(0f, 1f);

            // free chicken connected with padlock
            var chickens = FindObjectsOfType<ChickenBehavior>();
            foreach(var chicken in chickens)
            {
                if (ActivePadlock == chicken.GetPadlock())
                {
                    chicken.FreeChicken();
                 

                }
            }

            
        }
    }

    private void ResetProgress()
    {
        if (ProgressBar != null)
        {
            ProgressBar.localScale = new Vector3(0, 0);
            LockPickProgress = 0f;
        }
     
    }

    private void Die()
    {
        interactionPressed = false;
        isDead = true;
        playerAC.SetBool("IsDead", true);
        GetComponent<Collider2D>().enabled = false;
        playerRB.Sleep();
        ResetProgress();
        audioSource.clip = sound_death;
        audioSource.volume = 1f;
        audioSource.Play();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ladder")
        {
            isNearLadder = true;
            //Debug.Log(collision.gameObject);
            ActiveLadder = collision.gameObject;
            ActiveLadder.GetComponent<LadderBehavior>().ShowTooltip();
            var ladderPosition = ActiveLadder.transform.position;

            if (ladderPosition.y > transform.position.y)
            {
                isBelowLadder = true;
                isAboveLadder = false;
                ActiveLadder.GetComponent<LadderBehavior>().FlipToolTipUp();
            } else
            {
                isAboveLadder = true;
                isBelowLadder = false;
                ActiveLadder.GetComponent<LadderBehavior>().FlipToolTipDown();
            }
        }

        if (collision.tag == "Padlock")
        {
            Debug.Log("Padlock");
            ProgressBar = collision.transform.Find("ProgressBar");
            ProgressBar.localScale = new Vector3(0, 1);
            isNearPadlock = true;
            ActivePadlock = collision.gameObject;

        }

        if (collision.tag == "Exit")
        {
            if (collision.gameObject.GetComponent<ExitBehavior>().IsExitOpen)
            {
                IsNearExit = true;
            }
        }

        if (collision.tag == "Dog")
        {
            Die();
        }

    }


    private void GoToNextLevel()
    {
        nextLevel = 0;
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        //audioSource.clip = sound_goUp;
        //audioSource.Play();

        if (SceneManager.sceneCountInBuildSettings > currentSceneIndex + 1)
        {
            nextLevel = currentSceneIndex + 1;
        }
        //Debug.Log("next level:" + nextLevel);
        Invoke("LoadScene", 0f);
    }
  
    private void LoadScene()
    {
       
        SceneManager.LoadScene(nextLevel);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ladder")
        {
            isNearLadder = false;
            ActiveLadder.GetComponent<LadderBehavior>().HideTooltip();
            ActiveLadder = null;
            isBelowLadder = false;
            isAboveLadder = false;
        }

        if (collision.tag == "Padlock")
        {
            isNearPadlock = false;

        }

        if (collision.tag == "Exit")
        {
            IsNearExit = false;

        }

    }

    private void GoDownLadder()
    {
        var currentPosition = transform.position;
        currentPosition.y = -0.74f;
        transform.SetPositionAndRotation(currentPosition, new Quaternion());

        ActiveLadder.GetComponent<LadderBehavior>().FlipToolTipUp();
        isAboveLadder = false;
        isBelowLadder = true;

        audioSource.clip = sound_goDown;
        audioSource.volume = 0.5f;
        audioSource.Play();
    }

    private void GoUpLadder()
    {
        var currentPosition = transform.position;
        currentPosition.y += 0.74f;
        transform.SetPositionAndRotation(currentPosition, new Quaternion());

        ActiveLadder.GetComponent<LadderBehavior>().FlipToolTipDown();

        isBelowLadder = false;
        isAboveLadder = true;
        audioSource.clip = sound_goUp;
        audioSource.volume = 0.5f;
        audioSource.Play();
    }




    private void GoUp()
    {
        isBelowGround = false;
        var currentPosition = transform.position;
        currentPosition.y = 0f;
        transform.SetPositionAndRotation(currentPosition, new Quaternion());
        ActiveLadder.GetComponent<LadderBehavior>().FlipToolTipDown();
    }

    private void FixedUpdate()
    {
        if (jumpPressed && isGrounded)
        {
            jumpPressed = false;
            
        }
        //if (Input.GetKeyDown(KeyCode.Space))
        if (interactionPressed && isDead)
        {
    
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        if (interactionPressed && isNearPadlock)
        {
            isInteracting = true;
            LockPickPadlock();
        } else
        {
            isInteracting = false;
            ResetProgress();
        }


        if (movement.x > 0)
        {
            playerSR.flipX = false;
            playerLegsAC.SetBool("IsWalking", true);
        } else if (movement.x < 0)
        {
            playerSR.flipX = true;
            playerLegsAC.SetBool("IsWalking", true);
        } else
        {
            playerLegsAC.SetBool("IsWalking", false);
        }

        if (!isInteracting && !isDead)
        {
            playerRB.MovePosition(playerRB.position + movement * MoveSpeed * Time.fixedDeltaTime);
        }
    }

  
}
