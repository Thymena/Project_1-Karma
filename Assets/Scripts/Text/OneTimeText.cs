using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeText : MonoBehaviour
{
    [Header("Dialogue Content")]
    [SerializeField] private List<string> dialogueLines = new List<string>();

    [Header("Timing")]
    [SerializeField] private float delayBeforeShow = DialogueSystemDefaults.DelayBeforeShow;

    [Header("Trigger")]
    [SerializeField] private string playerTag = DialogueSystemDefaults.PlayerTag;

    [Header("One-Time")]
    [SerializeField] private bool allowRepeat = true;
    [SerializeField] private bool clearSaveOnAwake = true;
    [SerializeField] private string triggerId;

    private const string SaveKeyPrefix = "OneTimeText_";
    private const int SaveValueCompleted = 2;

    public event Action<OneTimeText> OnDialogueCompleted;

    private bool _completedThisSession;
    private Coroutine _triggerCoroutine;

    private void Awake()
    {
        if (string.IsNullOrWhiteSpace(triggerId))
        {
            triggerId = Guid.NewGuid().ToString("N");
        }

        if (clearSaveOnAwake)
        {
            PlayerPrefs.DeleteKey(GetSaveKey());
            PlayerPrefs.Save();
        }
    }

    private void OnDisable()
    {
        if (_triggerCoroutine != null)
        {
            StopCoroutine(_triggerCoroutine);
            _triggerCoroutine = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsBlockedByPersistentSave() || _completedThisSession)
        {
            return;
        }

        if (!other.CompareTag(playerTag))
        {
            return;
        }

        if (DialogueUIController.Instance == null)
        {
            Debug.LogWarning($"{nameof(OneTimeText)} on {name}: DialogueUIController not found in scene.");
            return;
        }

        if (DialogueUIController.Instance.IsPlaying)
        {
            return;
        }

        if (_triggerCoroutine != null)
        {
            return;
        }

        _triggerCoroutine = StartCoroutine(TriggerSequence());
    }

    private IEnumerator TriggerSequence()
    {
        yield return new WaitForSeconds(delayBeforeShow);

        _triggerCoroutine = null;

        if (DialogueUIController.Instance == null || DialogueUIController.Instance.IsPlaying)
        {
            yield break;
        }

        if (dialogueLines == null || dialogueLines.Count == 0)
        {
            yield break;
        }

        if (!DialogueUIController.Instance.TryPlayDialogue(dialogueLines, this))
        {
            yield break;
        }
    }

    public void NotifyDialogueFinished()
    {
        if (!allowRepeat)
        {
            _completedThisSession = true;
            PlayerPrefs.SetInt(GetSaveKey(), SaveValueCompleted);
            PlayerPrefs.Save();
        }

        OnDialogueCompleted?.Invoke(this);
    }

#if UNITY_EDITOR
    [ContextMenu("Clear One-Time Save")]
    private void ClearOneTimeSave()
    {
        PlayerPrefs.DeleteKey(GetSaveKey());
        _completedThisSession = false;
    }
#endif

    private bool IsBlockedByPersistentSave()
    {
        if (allowRepeat)
        {
            return false;
        }

        return GetSavedValue() >= SaveValueCompleted;
    }

    private int GetSavedValue()
    {
        int saved = PlayerPrefs.GetInt(GetSaveKey(), 0);
        if (saved == 1)
        {
            PlayerPrefs.DeleteKey(GetSaveKey());
            PlayerPrefs.Save();
            saved = 0;
        }

        return saved;
    }

    private string GetSaveKey()
    {
        return SaveKeyPrefix + triggerId;
    }

    private void Reset()
    {
        delayBeforeShow = DialogueSystemDefaults.DelayBeforeShow;
        playerTag = DialogueSystemDefaults.PlayerTag;
        allowRepeat = false;
        clearSaveOnAwake = false;
    }

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(triggerId))
        {
            triggerId = Guid.NewGuid().ToString("N");
        }
    }
}
