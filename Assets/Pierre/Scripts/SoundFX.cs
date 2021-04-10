using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFX : MonoBehaviour
{
    public AudioSource playSound;



    void OnTriggerEnter(Collider other)
    {
        playSound.Play();
        Debug.Log("sound played");
    }
}
