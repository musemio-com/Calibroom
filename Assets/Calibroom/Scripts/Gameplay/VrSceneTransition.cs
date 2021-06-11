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
    LoadingOverlay overlay;

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneLoaded;
    }
    private void Start()
    {
        overlay = GameObject.Find("LoadingOverlay").gameObject.GetComponent<LoadingOverlay>();
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
        if (Input.GetKeyDown(KeyCode.I))
            overlay.FadeIn(() =>
            {

            });
        if (Input.GetKeyDown(KeyCode.O))
            overlay.FadeOut(() =>
            {

            });
        if (Input.GetKeyDown(KeyCode.L))
            loadNextScene();
    }


    public void loadNextScene()
    {
        int LevelToGo = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.sceneCountInBuildSettings < 1)
        {
            Debug.Log("There are no other levels to load in the build settings, Reloading this Scene");
            LevelToGo = 0;
        }
            
        if (LevelToGo > SceneManager.sceneCountInBuildSettings - 1)
        {
            Debug.Log("Scene Level is Out of range of levels added..Loading First Scene");
            LevelToGo = 0;
        }
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            UISelectIDController ui_selectID = FindObjectOfType<UISelectIDController>();
            if (ui_selectID != null && dashboardRefs != null)
            {
                Debug.Log("SELECTED USER ID : " + ui_selectID.GetUserIDInt());
                dashboardRefs.userID = ui_selectID.GetUserIDInt();
            }
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
                overlay.FadeOut(() =>
                {
                    Debug.Log("FadedOut");
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
            overlay.FadeIn(() =>
            {
                Debug.Log("FadedIn");
                sceneLoading = false;
            });
        }
    }

    //private IEnumerator WaitAndFadeIn()
    //{
    //    // Wait a few frames to avoid freeze (caused by Awake calls?).
    //    yield return null;
    //    yield return null;
    //    yield return null;
    //    fadeScreen.FadeIn(() => {
    //        sceneLoading = false;
    //    });
    //}
}
