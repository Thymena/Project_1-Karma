using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalSceneTransition : MonoBehaviour
{
    public float fadeDuration = 1f;     // must match FadeOut animation length
    public float sceneLoadDelay = 0f;   // usually 0 for instant trigger

    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (triggered) return;

        triggered = true;
        StartCoroutine(TriggerRoutine());
    }

    private IEnumerator TriggerRoutine()
    {
        // IMPORTANT: wait one frame so Animator can initialize properly
        yield return null;

        // Now safely start the transition
        SceneControllerScript.instance.BeginTransition(
            fadeDuration,
            sceneLoadDelay
        );
    }
}

