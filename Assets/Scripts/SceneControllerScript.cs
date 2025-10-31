using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControllerScript : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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