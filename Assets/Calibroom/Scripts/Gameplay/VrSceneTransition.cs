using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using MECM;
using UnityEditor;

public class VrSceneTransition : MonoBehaviour
{
    DashboardRefs dashboardRefs;

    private void Start()
    {
        dashboardRefs = Resources.Load<DashboardRefs>("ScriptableObjects/DashboardRefs");
        if (dashboardRefs == null)
        {
            dashboardRefs = ScriptableObject.CreateInstance<DashboardRefs>();
            AssetDatabase.CreateAsset(dashboardRefs, "Assets/MECM/Resources/ScriptableObjects/DashboardRefs.asset");
            EditorApplication.delayCall += AssetDatabase.SaveAssets;
        }
        FindObjectOfType<LoadingOverlay>().FadeIn();
    }

    private void Update()
    {
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
            UIConfirmIDController ui_confirmID = FindObjectOfType<UIConfirmIDController>();
            if (ui_confirmID != null && dashboardRefs != null)
            {
                Debug.Log("SELECTED USER ID : " + ui_confirmID.getUserID());
                dashboardRefs.userID = ui_confirmID.getUserID();
            }
        }
        string path = SceneUtility.GetScenePathByBuildIndex(LevelToGo);
        string sceneName = path.Substring(0, path.Length - 6).Substring(path.LastIndexOf('/') + 1);
        LoadScene(sceneName);
    }

    public void LoadScene(string sceneName)
    {
        FindObjectOfType<LoadingOverlay>().FadeOut(() =>
        {
            Debug.Log("FadedOut");
            SceneManager.LoadScene(sceneName);
        });
    }
}
