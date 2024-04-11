using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    public Animator animator;
    public Footsteps drain;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool w = Input.GetKey(KeyCode.W);
        bool s = Input.GetKey(KeyCode.S);
        bool a = Input.GetKey(KeyCode.A);
        bool d = Input.GetKey(KeyCode.D);
        bool shift = Input.GetKey(KeyCode.LeftShift);

        if (w)
        {
            animator.SetBool("pressForward", true);
        }
        else
        {
            animator.SetBool("pressForward", false);
        }

        if (s)
        {
            animator.SetBool("pressBackward", true);
        }
        else
        {
            animator.SetBool("pressBackward", false);
        }

        if (w && s)
        {
            animator.SetBool("pressForward", false);
            animator.SetBool("pressBackward", false);
        }

        if (a)
        {
            animator.SetBool("pressLeft", true);
        }
        else
        {
            animator.SetBool("pressLeft", false);
        }

        if (d)
        {
            animator.SetBool("pressRight", true);
        }
        else
        {
            animator.SetBool("pressRight", false);
        }

        if (a && d) 
        {
            animator.SetBool("pressRight", false);
            animator.SetBool("pressLeft", false);
        }

        if (shift && !drain.isDrained)
        {
            animator.SetBool("sprint", true);
        }
        else
        {
            animator.SetBool("sprint", false);
        }
    }
}
