using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnNewScene : MonoBehaviour
{
    public static DontDestroyOnNewScene Instance;

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
