using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCellObject : MonoBehaviour
{
    [SerializeField] GameObject topWall;
    [SerializeField] GameObject bottomWall;
    [SerializeField] GameObject rightWall;
    [SerializeField] GameObject leftWall;

    public void Init(bool top, bool bottom, bool right, bool left)
    {
        topWall.SetActive(top);
        bottomWall.SetActive(bottom);
        rightWall.SetActive(right);
        leftWall.SetActive(left);
    }

    public void disableTop()
    {
        topWall.SetActive(false);
    }

    public void disableBottom()
    {
        bottomWall.SetActive(false);
    }

    public void disableLeft()
    {
        leftWall.SetActive(false);
    }

    public void disableRight()
    {
        rightWall.SetActive(false);
    }
}
