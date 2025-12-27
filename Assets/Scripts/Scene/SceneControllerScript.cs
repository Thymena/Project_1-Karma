using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneControllerScript : MonoBehaviour
{
    public static SceneControllerScript instance;

    [SerializeField] private Animator SceneTransition;

    private bool fadeInProgress = false;
    private bool isLoading = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void BeginTransition(float fadeDuration = 1f, float sceneLoadDelay = 0f)
    {
        if (fadeInProgress) return;
        fadeInProgress = true;
        StartCoroutine(TransitionCoroutine(fadeDuration, sceneLoadDelay));
    }

    private IEnumerator TransitionCoroutine(float fadeDuration, float sceneLoadDelay)
    {
        // Start scene transition animation
        if (SceneTransition != null)
        {
            SceneTransition.ResetTrigger("Start");
            SceneTransition.SetTrigger("End");
        }

        // delay before loading scene for elevator
        if (sceneLoadDelay > 0)
            yield return new WaitForSeconds(sceneLoadDelay);

        // Wait fade duration to be same with animation
        if (fadeDuration > 0)
            yield return new WaitForSeconds(fadeDuration);

        // Load next scene
        if (!isLoading)
        {
            isLoading = true;
            yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }

        fadeInProgress = false;
        isLoading = false;

        // Start transition in for the new scene
        GameObject transitionObj = GameObject.FindGameObjectWithTag("SceneTransition");
        if (transitionObj != null)
        {
            SceneTransition = transitionObj.GetComponent<Animator>();
            if (SceneTransition != null)
            {
                SceneTransition.ResetTrigger("End");
                SceneTransition.SetTrigger("Start");
            }
        }
    }
}
