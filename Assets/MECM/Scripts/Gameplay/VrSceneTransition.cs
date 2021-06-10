using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using MECM;
using UnityEditor;

public class VrSceneTransition : MonoBehaviour
{

    [SerializeField]
    private FadeScreen fadeScreen;

    private bool sceneLoading;
    private Action callback;
    DashboardRefs dashboardRefs;
    [HideInInspector]
    public int currentLevelIndex;

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneLoaded;
    }

    private void Start()
    {
        dashboardRefs = Resources.Load<DashboardRefs>("ScriptableObjects/DashboardRefs");
        if (dashboardRefs == null)
        {
            dashboardRefs = ScriptableObject.CreateInstance<DashboardRefs>();
            AssetDatabase.CreateAsset(dashboardRefs, "Assets/MECM/Resources/ScriptableObjects/DashboardRefs.asset");
            EditorApplication.delayCall += AssetDatabase.SaveAssets;
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.L))
        //    loadNextScene();
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneLoaded;
    }
    //public void loadMECMScene()
    //{
    //    dashboardRefs.userID = FindObjectOfType<UISelectIDController>().GetUserIDInt();
    //    LoadScene("MECM Room");
    //}

    //public void loadEndScene()
    //{
    //    LoadScene("sceneEnd");
    //}
    public void loadNextScene()
    {
        int LevelToGo = SceneManager.GetActiveScene().buildIndex + 1;

        if (SceneManager.sceneCountInBuildSettings < 1)
        {
            Debug.Log("There are no other levels to load in the build settings, Reloading this Scene");
            LevelToGo = 0;
        }
            
        if (LevelToGo > SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("Scene Level is Out of range of levels added..Loading First Scene");
            LevelToGo = 0;
        }
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Debug.Log("SELECTED USER ID : " + FindObjectOfType<UISelectIDController>().GetUserIDInt());
            dashboardRefs.userID = FindObjectOfType<UISelectIDController>().GetUserIDInt();
        }
        string path = SceneUtility.GetScenePathByBuildIndex(LevelToGo);
        string sceneName = path.Substring(0, path.Length - 6).Substring(path.LastIndexOf('/') + 1);

        LoadScene(sceneName);
    }
    public void LoadScene(int sceneIndex, Action callback = null, bool noFadeOut = false)
    {
        Debug.Log(sceneIndex);
        Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
        if (!scene.IsValid())
        {
            Debug.LogError("Can't load scene: Invalid scene index = " + sceneIndex);
            return;
        }
        LoadScene(scene.name, callback, noFadeOut);
    }

    public void LoadScene(string sceneName, Action callback = null, bool noFadeOut = false)
    {
        if (!sceneLoading)
        {
            this.callback = callback;
            sceneLoading = true;
            if (noFadeOut)
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                fadeScreen.FadeOut(() => {
                    SceneManager.LoadScene(sceneName);
                });
            }
        }
    }

    private void OnSceneLoaded(Scene unused, Scene unused2)
    {
        if (sceneLoading)
        {
            if (callback != null)
            {
                callback();
                callback = null;
            }
            StartCoroutine(WaitAndFadeIn());
        }
    }

    private IEnumerator WaitAndFadeIn()
    {
        // Wait a few frames to avoid freeze (caused by Awake calls?).
        yield return null;
        yield return null;
        yield return null;
        fadeScreen.FadeIn(() => {
            sceneLoading = false;
        });
    }
}
