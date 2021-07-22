using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenBehavior : MonoBehaviour
{
    public Transform Padlock;
    public GameObject CageDoor;
    public Transform SecondPadlock;
    public bool isFreed;
    private Animator chickenAC;
    public Sprite CageOpenSprite;
    private AudioSource audioSource;
    private AudioClip sound_free;
    

    void Start()
    {
        chickenAC = gameObject.GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        sound_free = Resources.Load("Audio/free") as AudioClip;
    }

    public GameObject GetPadlock()
    {
        if (Padlock != null)
        {
            return Padlock.gameObject;
        } else
        {
            return null;
        }
        
    }

    public void FreeChicken()
    {
        Padlock.GetComponent<Animator>().SetBool("IsOpen", true);
        isFreed = true;
        chickenAC.SetBool("IsFree", true);

        audioSource.clip = sound_free;
        audioSource.Play();

        Padlock.GetComponent<BoxCollider2D>().enabled = false;
        if (CageDoor != null)
        {
            CageDoor.GetComponent<SpriteRenderer>().sprite = CageOpenSprite;
        }


        Padlock = null;
        FindObjectOfType<LevelBehavior>().OnChickenSaved();

    }

  
}
