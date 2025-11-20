using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControllerScript : MonoBehaviour
{
    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
    
}
//public static SceneController instance;

//public void Awake()
//{
//if (instance == null)
//{ instance = this;
//DontDestroyOnLoad(gameObject); }
//else
//{ Destroy(gameObject); }
//}
//public void NextLevel()
//{ SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex+1); }