using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // learning if the startingHole has been activated.
    public void PuzzleActive(bool activated)
    {
    //    // if puzzleactive is true then puzzle is active
        if (activated == true)
    //        Debug.Log("puzzle is activated");
            gameObject.SetActive(true);
    }
}