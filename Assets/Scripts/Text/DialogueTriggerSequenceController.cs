using UnityEngine;

public class DialogueTriggerSequenceController : MonoBehaviour
{
    [SerializeField] private OneTimeText[] dialogueTriggers;

    private int _currentIndex;

    private void Awake()
    {
        if (dialogueTriggers == null || dialogueTriggers.Length == 0)
        {
            Debug.LogWarning($"{nameof(DialogueTriggerSequenceController)} on {name}: dialogueTriggers is empty.");
            return;
        }

        for (int i = 0; i < dialogueTriggers.Length; i++)
        {
            OneTimeText trigger = dialogueTriggers[i];
            if (trigger == null)
            {
                Debug.LogWarning($"{nameof(DialogueTriggerSequenceController)} on {name}: dialogueTriggers[{i}] is null.");
                continue;
            }

            trigger.OnDialogueCompleted += HandleDialogueCompleted;
            trigger.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        _currentIndex = 0;
        ActivateCurrentTrigger();
    }

    private void OnDestroy()
    {
        if (dialogueTriggers == null)
        {
            return;
        }

        for (int i = 0; i < dialogueTriggers.Length; i++)
        {
            OneTimeText trigger = dialogueTriggers[i];
            if (trigger != null)
            {
                trigger.OnDialogueCompleted -= HandleDialogueCompleted;
            }
        }
    }

    private void HandleDialogueCompleted(OneTimeText completedTrigger)
    {
        if (dialogueTriggers == null || _currentIndex >= dialogueTriggers.Length)
        {
            return;
        }

        if (dialogueTriggers[_currentIndex] != completedTrigger)
        {
            return;
        }

        completedTrigger.gameObject.SetActive(false);
        _currentIndex++;
        ActivateCurrentTrigger();
    }

    private void ActivateCurrentTrigger()
    {
        if (dialogueTriggers == null || _currentIndex >= dialogueTriggers.Length)
        {
            return;
        }

        OneTimeText trigger = dialogueTriggers[_currentIndex];
        if (trigger == null)
        {
            Debug.LogWarning($"{nameof(DialogueTriggerSequenceController)} on {name}: dialogueTriggers[{_currentIndex}] is null.");
            _currentIndex++;
            ActivateCurrentTrigger();
            return;
        }

        trigger.gameObject.SetActive(true);
    }
}
