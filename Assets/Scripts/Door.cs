using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Animator animator = null;

    [SerializeField] private bool openTrigger = false;
    [SerializeField] private bool closeTrigger = false;
    private bool hasKey = false;
    public AudioSource open, close;

    private void OnEnable()
    {
        EventManager.OnKeyCollected += UnlockDoor;
    }

    private void OnDisable()
    {
        EventManager.OnKeyCollected -= UnlockDoor;
    }

    private void UnlockDoor()
    {
        hasKey = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (openTrigger && hasKey)
            {
                animator.SetBool("isOpen", true);
                open.Play();
                gameObject.SetActive(false);
            }
            else if (closeTrigger)
            {
                animator.SetBool("isOpen", false);
                close.Play();
                gameObject.SetActive(false);
            }
        }
    }
}
