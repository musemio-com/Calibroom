using UnityEngine;
using System.Collections;
using ReusableMethods;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

/// <summary>
/// Controls all logic related to the game levels, how to transit them and store them
/// </summary>
[AddComponentMenu("CarlosFramework/GameLevelController")]
public class GameLevelController : MonoBehaviour
{

    public bool m_AllowDebug;

    /// <summary>
    /// Definition of enum of option to Load GameLevels
    /// </summary>
    public enum GameLevelLoadOptionEnum
    {
        EnablingObjects,
        LoadingScenes,
        Mixed
    }
    /// <summary>
    /// (Field) Current Option to load a GameLevel
    /// </summary>
    [SerializeField]
    private GameLevelLoadOptionEnum m_GameLevelLoadOption;

    /// <summary>
    /// Definition of enum of option to Load Scenes
    /// </summary>
    public enum SceneLoadOptionEnum
    {
        Synchronous,
        Async
    }
    /// <summary>
    /// (Field) Current option to load a Scene
    /// </summary>
    [SerializeField]
    private SceneLoadOptionEnum m_SceneLoadOption;

    /// <summary>
    /// (Field) The array of levels in the game
    /// </summary>
    [SerializeField]
    private GameObject[] m_GameLevels;
    /// <summary>
    /// (Property) The array of levels in the game
    /// </summary>
    public GameObject[] GameLevels { get { return this.m_GameLevels; } set { this.m_GameLevels = value; } }

    [SerializeField]
    private int m_IndexCurrentLevel;
    /// <summary>
    /// (Property) The current level we are in
    /// </summary>
    public int IndexCurrentLevel { get { return this.m_IndexCurrentLevel; } set { this.m_IndexCurrentLevel = value; } }

    /// <summary>
    /// (Field) The object containing the loading scene
    /// </summary>
    [SerializeField, Header("Loading Screen Setup")]
    private GameObject m_LoadingScreen;
    /// <summary>
    /// (Field) The loading bar in the loading screen
    /// </summary>
    [SerializeField]
    private GameObject m_LoadingBar;


    // Use this for initialization
    void Start()
    {
        // We initialize the component
        Initialize();
        // We start on the first level
        //LoadGameLevel(0);
    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// Initializes the component to its default values
    /// </summary>
    private void Initialize()
    {
        // If the array of levels of the current component is empty or null...
        if (m_GameLevels == null || m_GameLevels.Length < 1)
        {
            // We find all the enemies children to the current object
            FindLevelsAsChildren(this.gameObject);
        }

        // If the gameLevels array is define and contain any levels...
        if (m_GameLevels != null && m_GameLevels.Length > 0)
        {
            // We set all the levels to false
            Arrays.SetActiveAllArray(ref m_GameLevels, false);

        }
    }

    /// <summary>
    /// Finds levels as children of the parent passed in, adding them to the level array
    /// </summary>
    /// <param name="parent"> The parent containing the children</param>
    private void FindLevelsAsChildren(GameObject parent)
    {
        //// We initialize a new temporary list of levels
        //List<GameLevelLogic> LevelList = new List<GameLevelLogic>();
        //// We fill the list from the children of the current object
        //this.GetComponentsInChildren<GameLevelLogic>(true, LevelList);
        //// If we found any...
        //if (LevelList.Count > 0)
        //{
        //    // We then copy this list to our component field 
        //    // We resize the array 
        //    Array.Resize<GameObject>(ref m_GameLevels, LevelList.Count);
        //    // We copy each result as a gameObject to the component array
        //    for (int i = 0; i < LevelList.Count; i++)
        //    {
        //        m_GameLevels[i] = LevelList[i].gameObject;
        //    }
        //}

        //Arrays.FindComponentAsChildren<GameLevelLogic>(ref m_GameLevels, this.gameObject);
    }

    /// <summary>
    /// Load the next level in the array of game levels
    /// </summary>
    public void LoadNextLevel()
    {
        // We load current level +1
        LoadGameLevel(IndexCurrentLevel + 1);
    }

    /// <summary>
    /// Loads the previous level in the array of game levels
    /// </summary>
    public void LoadPreviousLevel()
    {
        // We load current level -1
        LoadGameLevel(IndexCurrentLevel - 1);
    }

    /// <summary>
    /// Loads the level specified by an index, overriding the loading method
    /// </summary>
    /// <param name="indexLevelToGo"> The index of the level we want to load </param>
    /// <param name="loadOption"> The loading method we want to use</param>
    public void LoadGameLevel(int indexLevelToGo, GameLevelLoadOptionEnum loadOption)
    {
        // We set the load option in the current component
        m_GameLevelLoadOption = loadOption;
        // We load the level specified by the index passed in
        LoadGameLevel(indexLevelToGo);
    }

    /// <summary>
    /// Loads the level specified by an index
    /// </summary>
    /// <param name="indexLevelToGo"> The index of the level we want to load </param>
    public void LoadGameLevel(int indexLevelToGo)
    {
        if (m_AllowDebug)
        {
            Debug.Log("LOADING LEVEL: " + indexLevelToGo);
        }

        // We check if there are any levels added in the build settings
        if (SceneManager.sceneCountInBuildSettings < 1)
        {
            // We show an error instead of an exception to not stop the execution of this frame
            if (m_AllowDebug)
                Debug.LogError("There are no levels to load in the build settings!");
            // We end the method
            return;
        }

        // We check if the indexToGo is not out of the range of levels
        if (indexLevelToGo >= GameLevels.Length || indexLevelToGo < 0)
        {
            //Debug.LogError("AAAAA");
            // If it is, we throw an exception
            //throw new UnityException("The level " + indexLevelToGo.ToString() + " is out of the range of GameLevels!");
            // We show an error instead of an exception to not stop the execution of this frame
            if (m_AllowDebug)
                Debug.LogError("The level " + indexLevelToGo.ToString() + " is out of the range of GameLevels!");
            // We end the method
            return;
        }

        // We check that the level is added to the build settings (only if the load options is set to load scenes or mixed)
        if (indexLevelToGo > SceneManager.sceneCountInBuildSettings && (m_GameLevelLoadOption == GameLevelLoadOptionEnum.LoadingScenes || m_GameLevelLoadOption == GameLevelLoadOptionEnum.Mixed))
        {
            // We show an error instead of an exception to not stop the execution of this frame
            Debug.LogError("The level " + indexLevelToGo.ToString() + " is out of the range of Levels added in the Build Settings!");
            // We end the method
            return;
        }

        // SUPER DIRTY CODE!! FOR THIS PROJECT I AM PUTTING THE MAIN MENU AS LEVEL 0!!! THAT IS WHY I REMOVE 1 HERE
        //indexLevelToGo -= 1;

        // We check that the levelToLoad is not the same one we are
        if (indexLevelToGo != m_IndexCurrentLevel)
        {
            // We load depending on the option specified in the component
            switch (m_GameLevelLoadOption)
            {
                case GameLevelLoadOptionEnum.EnablingObjects:
                    // We activate the next level
                    GameLevels[indexLevelToGo].SetActive(true);
                    // We deactivate the current level
                    GameLevels[m_IndexCurrentLevel].SetActive(false);
                    break;
                case GameLevelLoadOptionEnum.LoadingScenes:
                    // We load the next scene
                    LoadScene(indexLevelToGo);
                    break;
                case GameLevelLoadOptionEnum.Mixed:
                    // We activate the next level
                    GameLevels[indexLevelToGo].SetActive(true);
                    // We deactivate the current level
                    GameLevels[m_IndexCurrentLevel].SetActive(false);
                    // We load the next scene
                    LoadScene(indexLevelToGo);
                    break;
                default:
                    break;
            }
            // Current level is now levelToGo
            IndexCurrentLevel = indexLevelToGo;
        }
        else
        {
            // If it is, we warn it in the editor
            if (m_AllowDebug)
                Debug.LogWarning("You are loading the same level you are in with the GameLevelController!");
            // We load depending on the option specified in the component
            switch (m_GameLevelLoadOption)
            {
                case GameLevelLoadOptionEnum.EnablingObjects:
                    // We deactivate the current level
                    GameLevels[m_IndexCurrentLevel].SetActive(false);
                    // We activate the current level
                    GameLevels[IndexCurrentLevel].SetActive(true);
                    break;
                case GameLevelLoadOptionEnum.LoadingScenes:
                    // We load the current scene
                    LoadScene(m_IndexCurrentLevel);
                    break;
                case GameLevelLoadOptionEnum.Mixed:
                    // We deactivate the current level
                    GameLevels[m_IndexCurrentLevel].SetActive(false);
                    // We activate the current level
                    GameLevels[IndexCurrentLevel].SetActive(true);
                    // We load the current scene
                    LoadScene(m_IndexCurrentLevel);
                    break;
                default:
                    break;
            }
        }



    }

    /// <summary>
    /// Loads the scene depending on the index passed in
    /// </summary>
    /// <param name="index"> The index of the level to load</param>
    public void LoadScene(int index)
    {
        switch (m_SceneLoadOption)
        {
            case SceneLoadOptionEnum.Synchronous:
                //ShowLoadingScreen(true);
                SceneManager.LoadScene(index);
                //ShowLoadingScreen(false);
                break;
            case SceneLoadOptionEnum.Async:
                StartCoroutine(AsynchronousLoad(index));
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Loads Asynchronouly a level
    /// </summary>
    /// <param name="scene"> The index of the scene to load</param>
    /// <returns>An IEnumerator</returns>
    IEnumerator AsynchronousLoad(int scene)
    {
        yield return null;

        bool stopCoroutineFlag = false;

        AsyncOperation ao = SceneManager.LoadSceneAsync(scene);

        // Try catch to avoid nullreference exceptionss if the loading failed
        try
        {
            ao.allowSceneActivation = false;
        }
        catch (Exception)
        {

            Debug.LogError("Cannot load asynchronously next scene, stopping scene loading");
            stopCoroutineFlag = true;
        }

        // If the loading failed, we stop the coroutine
        if (stopCoroutineFlag)
        {
            yield break;
        }

        // If it didn't failed, we proceed with the loading
        while (!ao.isDone)
        {
            // [0, 0.9] > [0, 1]
            //float progress = Mathf.Clamp01(ao.progress / 0.9f);
            //Debug.Log("Loading progress: " + (ao.progress));

            // We show a loading screen showing the progress
            ShowLoadingScreen(true, ao.progress);

            // Loading completed
            if (ao.progress == 0.9f)
            {
                //Debug.Log("Press ANY key to start");
                //if (Input.anyKey)
                //    ao.allowSceneActivation = true;
                ao.allowSceneActivation = true;
            }

            yield return null;
        }

        // When the loading is complete, we show the loading screen one more frame, completed
        ShowLoadingScreen(true, ao.progress);
        // We wait one frame
        yield return null;

        // We then hide the loading screen and set the bar to 0
        ShowLoadingScreen(false, 0f);
    }

    /// <summary>
    /// Shows the loading screen depending on the value passed in
    /// </summary>
    /// <param name="value"> The value to show the loading screen. True for showing</param>
    public void ShowLoadingScreen(bool value)
    {
        // If the value is true...
        if (value)
        {
            // If the loading screen is not active...
            if (!m_LoadingScreen.activeSelf)
            {
                // We activate it
                m_LoadingScreen.SetActive(true);
            }
        }
        // If it is false...
        else
        {
            // ... We deactivate the loading screen
            m_LoadingScreen.SetActive(false);
        }
    }

    /// <summary>
    /// Shows the loading screen depending on the value passed in
    /// </summary>
    /// <param name="value"> The value to show the loading screen. True for showing</param>
    /// <param name="loadBarProgress"> The porgress of the loading bar</param>
    public void ShowLoadingScreen(bool value, float loadBarProgress)
    {
        if (m_LoadingScreen == null || m_LoadingBar == null )
        {
            return;
        }

        // If the value is true...
        if (value)
        {
            // If the loading screen is not active...
            if (!m_LoadingScreen.activeSelf)
            {
                // We activate it
                m_LoadingScreen.SetActive(true);
            }
            // We update the loading bar with the progress
            UpdateLoadBar(loadBarProgress);
        }
        // If it is false...
        else
        {
            // ... We deactivate the loading screen
            m_LoadingScreen.SetActive(false);
            // ... We make sure the the loading bar is set to 0
            UpdateLoadBar(0f);
        }
    }

    /// <summary>
    /// Updates the progress of the loading bar
    /// </summary>
    /// <param name="value"> The progress to update in the bar</param>
    private void UpdateLoadBar(float value)
    {
        if (m_LoadingBar != null)
        {
            // We make a local copy of the m_LoadingBar values
            Vector3 loadBarTransform;
            loadBarTransform = m_LoadingBar.transform.localScale;
            // We change the local copy
            loadBarTransform.x = value;
            // We update the UILife scaleValues from the local copy
            m_LoadingBar.transform.localScale = loadBarTransform;
            //Debug.Log(Vector3.right * NormalizedLife);
        }
    }

}
